// --- INTERCEPTOR DE SEGURIDAD GLOBAL ---
const originalFetch = window.fetch;
window.fetch = async (url, options = {}) => {
    const token = localStorage.getItem('adminToken');
    if (token) {
        options.headers = options.headers || {};
        options.headers['Authorization'] = `Bearer ${token}`;
    }
    const response = await originalFetch(url, options);
    if (response.status === 401) {
        localStorage.removeItem('adminToken');
        window.location.href = '/login.html';
    }
    return response;
};

const baseUrl = '/api';
let chartInstancia = null;

let idRubroEditando = 0;
let idCategoriaEditando = 0;
let idArticuloEditando = 0;
let idPromoEditando = 0;
let idSaborEditando = 0;

// Constante para productos sin imagen (el SVG en línea que hicimos el otro día)
const imgGris = 'data:image/svg+xml;charset=UTF-8,%3Csvg xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22 width%3D%22150%22 height%3D%22150%22 viewBox%3D%220 0 150 150%22%3E%3Crect width%3D%22150%22 height%3D%22150%22 fill%3D%22%23eeeeee%22%2F%3E%3Ctext x%3D%2250%25%22 y%3D%2250%25%22 dominant-baseline%3D%22middle%22 text-anchor%3D%22middle%22 fill%3D%22%23999999%22 font-family%3D%22sans-serif%22 font-size%3D%2214%22 font-weight%3D%22bold%22%3ESin Foto%3C%2Ftext%3E%3C%2Fsvg%3E';

// --- SISTEMA DE NOTIFICACIONES TOAST ---
function mostrarNotificacion(mensaje, tipo = 'error') {
    const container = document.getElementById('toast-container');
    if (!container) return;
    const toast = document.createElement('div');
    toast.className = `toast ${tipo}`;
    toast.innerText = mensaje;
    container.appendChild(toast);
    setTimeout(() => {
        toast.style.opacity = '0';
        toast.style.transition = 'opacity 0.3s ease';
        setTimeout(() => toast.remove(), 300);
    }, 3000);
}

function cambiarVista(idVista, elementoMenu) {
    document.querySelectorAll('.vista-seccion').forEach(vista => vista.classList.remove('activa'));
    document.getElementById(idVista).classList.add('activa');
    document.querySelectorAll('.nav-item').forEach(btn => btn.classList.remove('active'));
    elementoMenu.classList.add('active');
}

function leerFoto(input) {
    return new Promise((resolve) => {
        if (!input.files || input.files.length === 0) { resolve(""); return; }
        const reader = new FileReader();
        reader.onload = (e) => resolve(e.target.result);
        reader.readAsDataURL(input.files[0]);
    });
}

function filtrarTablaProductos() {
    const textoBuscado = document.getElementById('inputBuscarProducto').value.toLowerCase();
    const filas = document.querySelectorAll('#tabla-html-productos tbody tr');
    filas.forEach(fila => {
        const celdaNombre = fila.querySelector('td:nth-child(2)'); // Ahora la descripción está en la 2da columna
        if (celdaNombre) {
            const nombreProducto = celdaNombre.textContent.toLowerCase();
            if (nombreProducto.includes(textoBuscado)) {
                fila.style.display = '';
            } else {
                fila.style.display = 'none';
            }
        }
    });
}

document.getElementById('categoriaArticulo').addEventListener('change', function () {
    const indexSeleccionado = this.selectedIndex;
    if (indexSeleccionado === -1) return;
    const categoriaTexto = this.options[indexSeleccionado].text.toLowerCase();
    const divGustos = document.getElementById('contenedor-limite-gustos');
    const inputGustos = document.getElementById('maxGustos');
    if (categoriaTexto.includes('helad') || categoriaTexto.includes('pote') || categoriaTexto.includes('kilo')) {
        divGustos.style.display = 'block';
    } else {
        divGustos.style.display = 'none';
        inputGustos.value = 0;
    }
});

function agregarInputImagen(url = '') {
    const container = document.getElementById('contenedor-imagenes-extra');
    const div = document.createElement('div');
    div.className = 'input-rapido';
    div.style.marginBottom = '10px';
    div.style.maxWidth = '100%';
    div.style.alignItems = 'center';
    if (url !== '') {
        div.innerHTML = `<img src="${url}" style="height: 40px; width: 40px; object-fit: cover; border-radius: 4px; border: 1px solid #ccc;"><input type="hidden" class="imagen-extra-existente" value="${url}"><button class="btn-borrar" type="button" onclick="this.parentElement.remove()"><i class="fas fa-times"></i></button>`;
    } else {
        div.innerHTML = `<input type="file" class="form-control imagen-extra-file" accept="image/*"><button class="btn-borrar" type="button" onclick="this.parentElement.remove()"><i class="fas fa-times"></i></button>`;
    }
    container.appendChild(div);
}

async function toggleEstadoArticulo(id, checkbox) {
    const estadoNuevo = checkbox.checked;
    const accion = estadoNuevo ? "activar" : "pausar";

    if (!confirm(`¿Seguro que querés ${accion} este producto en el catálogo?`)) { checkbox.checked = !estadoNuevo; return; }

    try {
        const res = await fetch(`${baseUrl}/Articulos/${id}/Estado/${estadoNuevo}`, { method: 'PUT' });
        if (!res.ok) throw new Error();
        mostrarNotificacion(`Producto ${estadoNuevo ? 'activado' : 'pausado'} con éxito`, 'success');
    } catch (e) { mostrarNotificacion("Hubo un error al comunicar con el servidor."); checkbox.checked = !estadoNuevo; }
}

async function toggleEstadoSabor(id, checkbox) {
    const estadoNuevo = checkbox.checked;
    const accion = estadoNuevo ? "poner como Disponible" : "marcar como Agotado";

    if (!confirm(`¿Seguro que querés ${accion} este sabor?`)) { checkbox.checked = !estadoNuevo; return; }

    try {
        const res = await fetch(`${baseUrl}/Gustos/${id}/Estado/${estadoNuevo}`, { method: 'PUT' });
        if (!res.ok) throw new Error();
        mostrarNotificacion(`Sabor actualizado con éxito`, 'success');
    } catch (e) { mostrarNotificacion("Hubo un error al comunicar con el servidor."); checkbox.checked = !estadoNuevo; }
}

async function cargarArticulos() {
    try {
        const res = await fetch(`${baseUrl}/Articulos?_t=${new Date().getTime()}`);
        const data = await res.json();
        document.getElementById('metric-prod').innerText = data.length;
        const tbody = document.getElementById('tabla-articulos');
        const cajaPromos = document.getElementById('lista-articulos-promo');
        tbody.innerHTML = ''; cajaPromos.innerHTML = '';
        data.forEach(item => {
            const id = item.codArticulo !== undefined ? item.codArticulo : item.CodArticulo;
            const esActivo = item.activo !== undefined ? item.activo : (item.Activo !== undefined ? item.Activo : true);
            const isChecked = esActivo ? 'checked' : '';
            const imgSrc = item.urlImagen ? item.urlImagen : (item.UrlImagen ? item.UrlImagen : imgGris);

            tbody.innerHTML += `<tr>
                <td><img src="${imgSrc}" style="width: 45px; height: 45px; object-fit: cover; border-radius: 6px; border: 1px solid var(--border);"></td>
                <td><strong>${item.descripcion}</strong></td>
                <td>$${item.costo}</td>
                <td><label class="switch"><input type="checkbox" onchange="toggleEstadoArticulo(${id}, this)" ${isChecked}><span class="slider"></span></label></td>
                <td><button class="btn-editar" onclick="prepararEdicionArticulo(${id})"><i class="fas fa-pen"></i></button><button class="btn-borrar" onclick="eliminarArticulo(${id})"><i class="fas fa-trash"></i></button></td>
            </tr>`;
            cajaPromos.innerHTML += `<div class="fila-articulo-promo"><span>${item.descripcion}</span><input type="number" min="0" value="0" data-id="${id}" class="cant-articulo-promo"></div>`;
        });
    } catch (e) { }
}

async function cargarSabores() {
    try {
        const res = await fetch(`${baseUrl}/Gustos?_t=${new Date().getTime()}`);
        const data = await res.json();
        const tbody = document.getElementById('tabla-sabores');
        tbody.innerHTML = '';
        data.forEach(item => {
            const id = item.id !== undefined ? item.id : item.Id;
            const nombre = item.nombre !== undefined ? item.nombre : item.Nombre;
            const hayStock = item.hayStock !== undefined ? item.hayStock : item.HayStock;
            const nombreSeguro = nombre.replace(/'/g, "\\'");
            const isChecked = hayStock ? 'checked' : '';
            tbody.innerHTML += `<tr><td><strong>${nombre}</strong></td><td><label class="switch"><input type="checkbox" onchange="toggleEstadoSabor(${id}, this)" ${isChecked}><span class="slider"></span></label></td><td><button class="btn-editar" onclick="prepararEdicionSabor(${id}, '${nombreSeguro}', ${hayStock})"><i class="fas fa-pen"></i></button><button class="btn-borrar" onclick="eliminarSabor(${id})"><i class="fas fa-trash"></i></button></td></tr>`;
        });
    } catch (e) { }
}

async function eliminarSabor(id) {
    if (confirm("¿Seguro que querés eliminar definitivamente este sabor?")) {
        try {
            const res = await fetch(`${baseUrl}/Gustos/${id}`, { method: 'DELETE' });
            if (res.ok) { mostrarNotificacion("Sabor eliminado", "success"); cargarSabores(); }
            else { mostrarNotificacion("Hubo un error al intentar borrar el sabor."); }
        } catch (e) { mostrarNotificacion("Error de conexión con el servidor."); }
    }
}

async function prepararEdicionArticulo(id) {
    try {
        const res = await fetch(`${baseUrl}/Articulos/${id}`);
        const articulo = await res.json();
        idArticuloEditando = id;
        document.getElementById('descArticulo').value = articulo.descripcion;
        document.getElementById('costoArticulo').value = articulo.costo;
        document.getElementById('categoriaArticulo').value = articulo.categoriaId;
        const evt = new Event('change'); document.getElementById('categoriaArticulo').dispatchEvent(evt);
        document.getElementById('maxGustos').value = articulo.maxGustos || 0;
        const container = document.getElementById('contenedor-imagenes-extra'); container.innerHTML = '';
        if (articulo.imagenesExtras && articulo.imagenesExtras.length > 0) { articulo.imagenesExtras.forEach(url => agregarInputImagen(url)); }
        document.getElementById('titulo-form-articulo').innerHTML = '<i class="fas fa-pen"></i> Editando Producto';
        document.getElementById('btn-submit-articulo').innerText = 'Actualizar Producto';
        document.getElementById('btn-cancelar-articulo').style.display = 'block';
        window.scrollTo(0, 0);
    } catch (e) { mostrarNotificacion("Error al cargar detalles del producto"); }
}

function cancelarEdicionArticulo() { idArticuloEditando = 0; document.getElementById('descArticulo').value = ''; document.getElementById('costoArticulo').value = ''; document.getElementById('imgArticulo').value = ''; document.getElementById('categoriaArticulo').value = ''; document.getElementById('contenedor-limite-gustos').style.display = 'none'; document.getElementById('maxGustos').value = '0'; document.getElementById('contenedor-imagenes-extra').innerHTML = ''; document.getElementById('titulo-form-articulo').innerHTML = '<i class="fas fa-plus-circle"></i> Nuevo Producto'; document.getElementById('btn-submit-articulo').innerText = 'Guardar Producto'; document.getElementById('btn-cancelar-articulo').style.display = 'none'; }

async function guardarArticulo() {
    const desc = document.getElementById('descArticulo').value; const costo = document.getElementById('costoArticulo').value; const inputImg = document.getElementById('imgArticulo'); const catId = document.getElementById('categoriaArticulo').value;
    if (!desc || !costo || !catId) { return mostrarNotificacion("⚠️ Completá los datos requeridos"); }
    const gustosLimit = parseInt(document.getElementById('maxGustos').value) || 0; const galeriaUrls = [];
    document.querySelectorAll('.imagen-extra-existente').forEach(hiddenInput => { galeriaUrls.push(hiddenInput.value); });
    const inputsNuevos = document.querySelectorAll('.imagen-extra-file');
    for (const input of inputsNuevos) { if (input.files && input.files.length > 0) { const fotoExtraBase64 = await leerFoto(input); if (fotoExtraBase64) galeriaUrls.push(fotoExtraBase64); } }
    const fotoBase64 = await leerFoto(inputImg);
    const payload = { descripcion: desc, urlImagen: fotoBase64, costo: parseFloat(costo), stock: 0, stockMinimo: 0, categoriaId: parseInt(catId), maxGustos: gustosLimit, imagenesExtras: galeriaUrls };
    const url = idArticuloEditando > 0 ? `${baseUrl}/Articulos/${idArticuloEditando}` : `${baseUrl}/Articulos`; const metodo = idArticuloEditando > 0 ? 'PUT' : 'POST';
    try {
        const res = await fetch(url, { method: metodo, headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(payload) });
        if (res.ok) { mostrarNotificacion("✅ Producto guardado con éxito", "success"); cancelarEdicionArticulo(); cargarArticulos(); }
        else { mostrarNotificacion("❌ Error al guardar el producto"); }
    } catch (e) { mostrarNotificacion("❌ Error de conexión al servidor"); }
}

async function eliminarArticulo(id) {
    if (confirm("¿Seguro que querés eliminar este producto?")) {
        try {
            await fetch(`${baseUrl}/Articulos/${id}`, { method: 'DELETE' });
            mostrarNotificacion("Producto eliminado", "success");
            cargarArticulos();
        } catch (e) { mostrarNotificacion("Error al eliminar"); }
    }
}

function prepararEdicionSabor(id, nombre, hayStock) { idSaborEditando = id; document.getElementById('nombreSabor').value = nombre; document.getElementById('titulo-form-sabor').innerHTML = '<i class="fas fa-pen"></i> Editando Sabor'; document.getElementById('btn-submit-sabor').innerText = 'Actualizar Sabor'; document.getElementById('btn-cancelar-sabor').style.display = 'block'; window.scrollTo(0, 0); }
function cancelarEdicionSabor() { idSaborEditando = 0; document.getElementById('nombreSabor').value = ''; document.getElementById('titulo-form-sabor').innerHTML = '<i class="fas fa-ice-cream"></i> Nuevo Sabor'; document.getElementById('btn-submit-sabor').innerText = 'Guardar Sabor'; document.getElementById('btn-cancelar-sabor').style.display = 'none'; }

async function guardarSabor() {
    const nombre = document.getElementById('nombreSabor').value;
    if (!nombre) return mostrarNotificacion("⚠️ Ingresá un nombre para el sabor");
    const payload = { id: idSaborEditando, nombre: nombre, hayStock: true };
    const url = idSaborEditando > 0 ? `${baseUrl}/Gustos/${idSaborEditando}` : `${baseUrl}/Gustos`; const metodo = idSaborEditando > 0 ? 'PUT' : 'POST';
    try {
        const res = await fetch(url, { method: metodo, headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(payload) });
        if (res.ok) { mostrarNotificacion("✅ Sabor guardado con éxito", "success"); cancelarEdicionSabor(); cargarSabores(); }
        else { mostrarNotificacion("❌ Error al guardar el sabor"); }
    } catch (e) { mostrarNotificacion("❌ Error de conexión al servidor"); }
}

// --- LÓGICA DE IMPRESIÓN ---
async function imprimirTicket(idPedido) {
    try {
        const res = await fetch(`${baseUrl}/Pedidos/${idPedido}`);
        if (!res.ok) throw new Error("Error al obtener pedido");
        const pedido = await res.json();

        const infoCliente = document.getElementById('ticket-info-cliente');
        if (infoCliente) {
            infoCliente.innerHTML = `
                Fecha: ${pedido.fecha}<br>
                Comprobante: 0001-${pedido.id.toString().padStart(8, '0')}<br>
                Cliente: ${pedido.cliente}<br>
                Condición de Venta: CONTADO<br>
                Atendido por: MOSTRADOR ADM
            `;
        }

        const tbodyItems = document.getElementById('ticket-items');
        if (tbodyItems) {
            tbodyItems.innerHTML = '';
            if (pedido.detalles && pedido.detalles.length > 0) {
                pedido.detalles.forEach(det => {
                    let desc = "Producto";
                    if (det.articulo && det.articulo.descripcion) desc = det.articulo.descripcion;
                    else if (det.promo && det.promo.nombre) desc = det.promo.nombre;

                    const subtotal = det.cantidad * det.precioUnitario;
                    tbodyItems.innerHTML += `
                        <tr>
                            <td style="padding: 3px 0; text-transform: uppercase;">${desc}</td>
                            <td style="text-align: center; padding: 3px 0;">${det.cantidad}</td>
                            <td style="text-align: right; padding: 3px 0;">$${subtotal}</td>
                        </tr>
                    `;
                });
            } else {
                tbodyItems.innerHTML = `<tr><td colspan="3" style="text-align:center; padding:5px;">Detalle no disponible (Pedido Antiguo)</td></tr>`;
            }
        }

        const ticketTotal = document.getElementById('ticket-total');
        if (ticketTotal) {
            ticketTotal.innerText = `Total: $${pedido.total}`;
        }

        setTimeout(() => { window.print(); }, 200);

    } catch (error) {
        mostrarNotificacion("No se pudo cargar el detalle para imprimir el ticket.");
        console.error(error);
    }
}

async function cargarPedidos() {
    try {
        const res = await fetch(`${baseUrl}/Pedidos/Pendientes`);
        if (!res.ok) throw new Error("");
        const data = await res.json();
        const tbody = document.getElementById('tabla-pedidos');
        const msjSinPedidos = document.getElementById('mensaje-sin-pedidos');
        tbody.innerHTML = '';
        if (data.length === 0) { msjSinPedidos.style.display = 'block'; } else {
            msjSinPedidos.style.display = 'none';
            data.forEach(item => {
                tbody.innerHTML += `<tr>
                    <td>${item.fecha}</td>
                    <td><strong>${item.cliente}</strong></td>
                    <td style="font-weight: bold; color: var(--text-main);">$${item.total}</td>
                    <td id="acciones-pedido-${item.id}">
                        <button class="btn-editar" style="width: auto; padding: 6px 12px; font-size: 0.85rem; margin-right: 5px; margin-bottom: 5px; color: #4b5563; border-color: #d1d5db;" onclick="imprimirTicket(${item.id})" title="Imprimir Ticket"><i class="fas fa-print"></i></button>
                        <button class="btn-guardar" style="width: auto; padding: 6px 12px; font-size: 0.85rem; margin-right: 5px; margin-bottom: 5px;" onclick="cambiarEstadoPedido(${item.id}, 'Completado')"><i class="fas fa-check"></i></button>
                        <button class="btn-borrar" style="margin-bottom: 5px;" onclick="cambiarEstadoPedido(${item.id}, 'Cancelado')"><i class="fas fa-times"></i></button>
                    </td>
                </tr>`;
            });
        }
    } catch (e) { }
}

async function cambiarEstadoPedido(idPedido, nuevoEstado) {
    if (confirm(`¿Marcar este pedido como ${nuevoEstado}?`)) {
        try {
            await fetch(`${baseUrl}/Pedidos/${idPedido}/Estado`, { method: 'PUT', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(nuevoEstado) });
            mostrarNotificacion(`Pedido ${nuevoEstado}`, 'success');
            cargarEstadisticas(); cargarPedidos(); cargarHistorialPedidos();
        } catch (e) { mostrarNotificacion("Error al cambiar estado del pedido"); }
    }
}

function inicializarFiltrosHistorial() { const selectAnio = document.getElementById('select-anio-historial'); const selectMes = document.getElementById('select-mes-historial'); const anioActual = new Date().getFullYear(); selectAnio.innerHTML = ''; for (let i = anioActual; i >= 2024; i--) { selectAnio.innerHTML += `<option value="${i}">${i}</option>`; } const mesActual = new Date().getMonth() + 1; selectMes.value = mesActual; }

async function cargarHistorialPedidos() {
    const anio = document.getElementById('select-anio-historial').value;
    const mes = document.getElementById('select-mes-historial').value;
    try {
        const res = await fetch(`${baseUrl}/Pedidos/Historial?anio=${anio}&mes=${mes}`);
        if (!res.ok) throw new Error("Error al traer historial");
        const data = await res.json();
        const tbody = document.getElementById('tabla-historial-pedidos');
        const msjSinPedidos = document.getElementById('mensaje-sin-historial');
        tbody.innerHTML = '';
        if (data.length === 0) { msjSinPedidos.style.display = 'block'; } else {
            msjSinPedidos.style.display = 'none';
            data.forEach(item => {
                const colorEstado = item.estado === 'Completado' ? 'var(--success)' : 'var(--danger)';
                const iconoEstado = item.estado === 'Completado' ? '<i class="fas fa-check-circle"></i>' : '<i class="fas fa-times-circle"></i>';
                tbody.innerHTML += `<tr>
                    <td>${item.fecha}</td>
                    <td><strong>${item.cliente}</strong></td>
                    <td style="color: ${colorEstado}; font-weight: bold;">${iconoEstado} ${item.estado}</td>
                    <td style="font-weight: bold; color: var(--text-main);">$${item.total} <button class="btn-editar" style="padding: 2px 8px; margin-left: 10px; font-size: 0.85rem;" onclick="imprimirTicket(${item.id})" title="Imprimir Ticket"><i class="fas fa-print"></i></button></td>
                </tr>`;
            });
        }
    } catch (e) { console.error(e); }
}

async function cargarEstadisticas() { const select = document.getElementById('selectYear'); if (select.options.length === 0) { const anioActual = new Date().getFullYear(); for (let i = anioActual; i >= 2026; i--) { const option = document.createElement('option'); option.value = i; option.text = i; select.appendChild(option); } } const anioSeleccionado = select.value; try { const res = await fetch(`${baseUrl}/Estadisticas/Ventas?anio=${anioSeleccionado}`); if (!res.ok) throw new Error("Error"); const data = await res.json(); const nombresMeses = ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic']; const labels = data.map(d => nombresMeses[d.mes - 1] || d.mes); const totales = data.map(d => d.total); const ctx = document.getElementById('graficoVentas').getContext('2d'); if (chartInstancia) { chartInstancia.destroy(); } chartInstancia = new Chart(ctx, { type: 'bar', data: { labels: labels, datasets: [{ label: `Facturación ${anioSeleccionado} ($)`, data: totales, backgroundColor: '#cc0000', borderRadius: 6 }] }, options: { responsive: true, maintainAspectRatio: false, scales: { y: { beginAtZero: true } } } }); } catch (e) { } }

async function cargarConfigLocal() {
    try {
        const res = await fetch(`${baseUrl}/Configuracion`);
        if (res.ok) {
            const data = await res.json();
            document.getElementById('config-wpp').value = data.whatsApp || data.WhatsApp || '';
            document.getElementById('config-fb').value = data.facebook || data.Facebook || '';
            document.getElementById('config-alias').value = data.alias || data.Alias || '';
        }
    } catch (e) { }
}

async function guardarConfigLocal() {
    const wpp = document.getElementById('config-wpp').value;
    const fb = document.getElementById('config-fb').value;
    const alias = document.getElementById('config-alias').value;

    const payload = {
        whatsApp: wpp, WhatsApp: wpp,
        facebook: fb, Facebook: fb,
        alias: alias, Alias: alias,
        instagram: "", googleMaps: ""
    };
    try {
        const res = await fetch(`${baseUrl}/Configuracion`, { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(payload) });
        if (res.ok) { mostrarNotificacion("✅ Datos del local actualizados", "success"); }
        else { mostrarNotificacion("❌ Error al guardar datos"); }
    } catch (e) { mostrarNotificacion("❌ Error de red"); }
}

async function cargarEnvio() { try { const res = await fetch(`${baseUrl}/TarifaEnvio`); document.getElementById('input-envio').value = await res.json(); } catch (e) { } }

async function guardarEnvio() {
    const costo = document.getElementById('input-envio').value;
    if (!costo) return mostrarNotificacion("⚠️ Ingresá un valor para el envío");
    try {
        const res = await fetch(`${baseUrl}/TarifaEnvio`, { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: costo });
        if (res.ok) { mostrarNotificacion("✅ Tarifa de envío actualizada", "success"); }
        else { mostrarNotificacion("❌ Error al actualizar tarifa"); }
    } catch (e) { mostrarNotificacion("❌ Error de red"); }
}

async function cargarRubros() { try { const res = await fetch(`${baseUrl}/Rubros`); const data = await res.json(); const tbody = document.getElementById('tabla-rubros'); const selectRubroCategoria = document.getElementById('rubroCategoria'); tbody.innerHTML = ''; selectRubroCategoria.innerHTML = '<option value="">Seleccioná...</option>'; data.forEach(item => { const id = item.codRubro !== undefined ? item.codRubro : item.CodRubro; const nombre = item.nombre !== undefined ? item.nombre : item.Nombre; const nombreSeguro = nombre.replace(/'/g, "\\'"); tbody.innerHTML += `<tr><td><strong>${nombre}</strong></td><td><button class="btn-editar" onclick="prepararEdicionRubro(${id}, '${nombreSeguro}')"><i class="fas fa-pen"></i></button><button class="btn-borrar" onclick="eliminarRubro(${id})"><i class="fas fa-trash"></i></button></td></tr>`; selectRubroCategoria.innerHTML += `<option value="${id}">${nombre}</option>`; }); } catch (e) { } }
function prepararEdicionRubro(id, nombre) { idRubroEditando = id; document.getElementById('nombreRubro').value = nombre; document.getElementById('titulo-form-rubro').innerHTML = '<i class="fas fa-pen"></i> Editando Rubro'; document.getElementById('btn-submit-rubro').innerText = 'Actualizar Rubro'; document.getElementById('btn-cancelar-rubro').style.display = 'block'; window.scrollTo(0, 0); }
function cancelarEdicionRubro() { idRubroEditando = 0; document.getElementById('nombreRubro').value = ''; document.getElementById('titulo-form-rubro').innerHTML = '<i class="fas fa-layer-group"></i> Nuevo Rubro'; document.getElementById('btn-submit-rubro').innerText = 'Guardar'; document.getElementById('btn-cancelar-rubro').style.display = 'none'; }

async function guardarRubro() {
    const nombre = document.getElementById('nombreRubro').value;
    if (!nombre) return mostrarNotificacion("⚠️ Ingresá el nombre del rubro");
    const payload = { codRubro: idRubroEditando, nombre: nombre };
    const url = idRubroEditando > 0 ? `${baseUrl}/Rubros/${idRubroEditando}` : `${baseUrl}/Rubros`; const metodo = idRubroEditando > 0 ? 'PUT' : 'POST';
    try {
        const res = await fetch(url, { method: metodo, headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(payload) });
        if (res.ok) { mostrarNotificacion("✅ Rubro guardado con éxito", "success"); cancelarEdicionRubro(); cargarRubros(); }
        else { mostrarNotificacion("❌ Error al guardar rubro"); }
    } catch (e) { mostrarNotificacion("❌ Error de red"); }
}

async function eliminarRubro(id) {
    if (confirm("¿Borrar rubro? Se borrarán las categorías relacionadas.")) {
        try {
            await fetch(`${baseUrl}/Rubros/${id}`, { method: 'DELETE' });
            mostrarNotificacion("Rubro eliminado", "success");
            cargarRubros();
        } catch (e) { mostrarNotificacion("Error al eliminar"); }
    }
}

async function cargarCategorias() {
    try {
        const res = await fetch(`${baseUrl}/Categorias`);
        const data = await res.json();
        document.getElementById('metric-cat').innerText = data.length;
        const tbody = document.getElementById('tabla-categorias');
        const selectArticulo = document.getElementById('categoriaArticulo');
        tbody.innerHTML = '';
        selectArticulo.innerHTML = '<option value="">Seleccioná...</option>';
        data.forEach(item => {
            const id = item.codCategoria !== undefined ? item.codCategoria : item.CodCategoria;
            const nombre = item.nombre !== undefined ? item.nombre : item.Nombre;
            const codRubro = item.codRubro !== undefined ? item.codRubro : item.CodRubro;
            const nombreSeguro = nombre.replace(/'/g, "\\'");
            tbody.innerHTML += `<tr><td><strong>${nombre}</strong></td><td><button class="btn-editar" onclick="prepararEdicionCategoria(${id}, '${nombreSeguro}', ${codRubro})"><i class="fas fa-pen"></i></button><button class="btn-borrar" onclick="eliminarCategoria(${id})"><i class="fas fa-trash"></i></button></td></tr>`;
            selectArticulo.innerHTML += `<option value="${id}">${nombre}</option>`;
        });
    } catch (e) { }
}

function prepararEdicionCategoria(id, nombre, codRubro) { idCategoriaEditando = id; document.getElementById('nombreCategoria').value = nombre; document.getElementById('rubroCategoria').value = codRubro || ""; document.getElementById('titulo-form-categoria').innerHTML = '<i class="fas fa-pen"></i> Editando Categoría'; document.getElementById('btn-submit-categoria').innerText = 'Actualizar Categoría'; document.getElementById('btn-cancelar-categoria').style.display = 'block'; window.scrollTo(0, 0); }
function cancelarEdicionCategoria() { idCategoriaEditando = 0; document.getElementById('nombreCategoria').value = ''; document.getElementById('rubroCategoria').value = ''; document.getElementById('imgCategoria').value = ''; document.getElementById('titulo-form-categoria').innerHTML = '<i class="fas fa-tag"></i> Nueva Categoría'; document.getElementById('btn-submit-categoria').innerText = 'Guardar Categoría'; document.getElementById('btn-cancelar-categoria').style.display = 'none'; }

async function guardarCategoria() {
    const nombre = document.getElementById('nombreCategoria').value; const rubroId = document.getElementById('rubroCategoria').value; const inputImg = document.getElementById('imgCategoria');
    if (!nombre || !rubroId || rubroId === "undefined" || rubroId === "") { return mostrarNotificacion("⚠️ Completá el nombre y seleccioná un rubro"); }
    const formData = new FormData(); formData.append("CodCategoria", idCategoriaEditando); formData.append("Nombre", nombre); formData.append("CodRubro", parseInt(rubroId));
    if (inputImg && inputImg.files && inputImg.files.length > 0) { formData.append("ArchivoImagen", inputImg.files[0]); }
    const url = idCategoriaEditando > 0 ? `${baseUrl}/Categorias/${idCategoriaEditando}` : `${baseUrl}/Categorias`; const metodo = idCategoriaEditando > 0 ? 'PUT' : 'POST';
    try {
        const res = await fetch(url, { method: metodo, body: formData });
        if (res.ok) { mostrarNotificacion("✅ Categoría guardada", "success"); cancelarEdicionCategoria(); cargarCategorias(); }
        else { mostrarNotificacion("❌ Error al guardar categoría"); }
    } catch (e) { mostrarNotificacion("❌ Error de conexión al servidor"); }
}

async function eliminarCategoria(id) {
    if (confirm("¿Borrar categoría?")) {
        try {
            await fetch(`${baseUrl}/Categorias/${id}`, { method: 'DELETE' });
            mostrarNotificacion("Categoría eliminada", "success");
            cargarCategorias();
        } catch (e) { mostrarNotificacion("Error al eliminar"); }
    }
}

async function cargarPromociones() {
    try {
        const res = await fetch(`${baseUrl}/Promos`);
        const data = await res.json();
        document.getElementById('metric-promo').innerText = data.length;
        const tbody = document.getElementById('tabla-promos');
        tbody.innerHTML = '';
        data.forEach(item => {
            const id = item.codPromo !== undefined ? item.codPromo : item.CodPromo;
            const imgSrc = item.urlImagen ? item.urlImagen : (item.UrlImagen ? item.UrlImagen : imgGris);
            tbody.innerHTML += `<tr>
                <td><img src="${imgSrc}" style="width: 45px; height: 45px; object-fit: cover; border-radius: 6px; border: 1px solid var(--border);"></td>
                <td><strong>${item.nombre}</strong></td>
                <td>$${item.precioVenta}</td>
                <td><button class="btn-editar" onclick="prepararEdicionPromo(${id})"><i class="fas fa-pen"></i></button><button class="btn-borrar" onclick="eliminarPromocion(${id})"><i class="fas fa-trash"></i></button></td>
            </tr>`;
        });
    } catch (e) { }
}

async function prepararEdicionPromo(id) {
    try {
        const res = await fetch(`${baseUrl}/Promos/${id}`);
        const promo = await res.json();
        idPromoEditando = id;
        document.getElementById('nombrePromo').value = promo.nombre;
        document.getElementById('costoPromo').value = promo.precioVenta;
        document.querySelectorAll('.cant-articulo-promo').forEach(input => input.value = 0);
        if (promo.detallePromos && promo.detallePromos.length > 0) {
            promo.detallePromos.forEach(det => {
                const input = document.querySelector(`.cant-articulo-promo[data-id='${det.codArticulo}']`);
                if (input) input.value = det.cantidad;
            });
        }
        document.getElementById('titulo-form-promo').innerHTML = '<i class="fas fa-pen"></i> Editando Promoción';
        document.getElementById('btn-submit-promo').innerText = 'Actualizar Promoción';
        document.getElementById('btn-cancelar-promo').style.display = 'block';
        window.scrollTo(0, 0);
    } catch (e) { mostrarNotificacion("Error al cargar detalles de la promoción"); }
}

function cancelarEdicionPromo() { idPromoEditando = 0; document.getElementById('nombrePromo').value = ''; document.getElementById('costoPromo').value = ''; document.getElementById('imgPromo').value = ''; document.querySelectorAll('.cant-articulo-promo').forEach(input => input.value = 0); document.getElementById('titulo-form-promo').innerHTML = '<i class="fas fa-star"></i> Nuevo Combo / Promo'; document.getElementById('btn-submit-promo').innerText = 'Guardar Promoción'; document.getElementById('btn-cancelar-promo').style.display = 'none'; }

async function guardarPromocion() {
    const nombre = document.getElementById('nombrePromo').value; const costo = document.getElementById('costoPromo').value; const inputImg = document.getElementById('imgPromo');
    if (!nombre || !costo) { return mostrarNotificacion("⚠️ Faltan datos en la promoción"); }
    const articulosSel = [];
    document.querySelectorAll('.cant-articulo-promo').forEach(input => { const cant = parseInt(input.value); if (cant > 0) articulosSel.push({ codArticulo: parseInt(input.getAttribute('data-id')), cantidad: cant }); });
    if (articulosSel.length === 0) { return mostrarNotificacion("⚠️ Seleccioná artículos para el combo"); }
    const fotoBase64 = await leerFoto(inputImg);
    const payload = { nombre: nombre, descripcion: "", categoria: "Promos", urlImagen: fotoBase64, precioVenta: parseFloat(costo), articulos: articulosSel };
    const url = idPromoEditando > 0 ? `${baseUrl}/Promos/${idPromoEditando}` : `${baseUrl}/Promos`; const metodo = idPromoEditando > 0 ? 'PUT' : 'POST';
    try {
        const res = await fetch(url, { method: metodo, headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(payload) });
        if (res.ok) { mostrarNotificacion("✅ Promoción guardada", "success"); cancelarEdicionPromo(); cargarPromociones(); }
        else { mostrarNotificacion("❌ Error al guardar promoción"); }
    } catch (e) { mostrarNotificacion("❌ Error de conexión al servidor"); }
}

async function eliminarPromocion(id) {
    if (confirm("¿Borrar promoción?")) {
        try {
            await fetch(`${baseUrl}/Promos/${id}`, { method: 'DELETE' });
            mostrarNotificacion("Promoción eliminada", "success");
            cargarPromociones();
        } catch (e) { mostrarNotificacion("Error al eliminar"); }
    }
}

window.onload = () => {
    cargarConfigLocal();
    cargarPedidos();
    cargarEstadisticas();
    cargarEnvio();
    cargarSabores();
    cargarRubros();
    cargarCategorias();
    cargarArticulos();
    cargarPromociones();

    inicializarFiltrosHistorial();
    cargarHistorialPedidos();

    const btnCerrarSesion = document.getElementById('btnCerrarSesion');
    if (btnCerrarSesion) {
        btnCerrarSesion.addEventListener('click', () => {
            localStorage.removeItem('adminToken');
            window.location.href = '/login.html';
        });
    }
};