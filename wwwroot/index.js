const baseURL = '/api';
const imgGris = 'data:image/svg+xml;charset=UTF-8,%3Csvg xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22 width%3D%22150%22 height%3D%22150%22 viewBox%3D%220 0 150 150%22%3E%3Crect width%3D%22150%22 height%3D%22150%22 fill%3D%22%23eeeeee%22%2F%3E%3Ctext x%3D%2250%25%22 y%3D%2250%25%22 dominant-baseline%3D%22middle%22 text-anchor%3D%22middle%22 fill%3D%22%23999999%22 font-family%3D%22sans-serif%22 font-size%3D%2214%22 font-weight%3D%22bold%22%3ESin Foto%3C%2Ftext%3E%3C%2Fsvg%3E';
const fetchOptions = { headers: { 'ngrok-skip-browser-warning': 'true' } };

let costoEnvioFijo = 0;
let carrito = JSON.parse(localStorage.getItem('carritoLaDolce')) || {};
window.todosLosProductos = [];
let numeroWhatsAppDinamico = '';
let aliasDinamico = '';
let pedidoEnEdicion = null;
let prodSeleccionadoDetalle = null;
let cantidadDetalle = 1;

// --- VARIABLES GLOBALES DEL SLIDER ---
let sliderActualIndex = 0;
let sliderTotalFrames = 0;
let galeriaImagenes = [];

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

function guardarCarritoEnStorage() { localStorage.setItem('carritoLaDolce', JSON.stringify(carrito)); }

function guardarDatosCliente() {
    const datos = {
        nombre: document.getElementById('cli-nombre').value,
        direccion: document.getElementById('cli-direccion').value,
        telefono: document.getElementById('cli-telefono').value,
        pago: document.getElementById('cli-pago').value,
        abonaCon: document.getElementById('cli-abona-con').value
    };
    localStorage.setItem('datosClienteLaDolce', JSON.stringify(datos));
}

function cargarDatosClienteFormulario() {
    const datosGuardados = JSON.parse(localStorage.getItem('datosClienteLaDolce'));
    if (datosGuardados) {
        document.getElementById('cli-nombre').value = datosGuardados.nombre || '';
        document.getElementById('cli-direccion').value = datosGuardados.direccion || '';
        document.getElementById('cli-telefono').value = datosGuardados.telefono || '';
        document.getElementById('cli-pago').value = datosGuardados.pago || 'Efectivo';
        document.getElementById('cli-abona-con').value = datosGuardados.abonaCon || '';
        verificarMetodoPago();
    }
}

async function inicializarApp() {
    try {
        const antiCache = '?_t=' + new Date().getTime();
        const resEnvio = await fetch(baseURL + '/TarifaEnvio' + antiCache, fetchOptions);
        if (resEnvio.ok) { costoEnvioFijo = await resEnvio.json(); }

        const resConfig = await fetch(baseURL + '/Configuracion' + antiCache, fetchOptions);
        if (resConfig.ok) {
            const config = await resConfig.json();
            const wppRecibido = config.whatsApp || config.WhatsApp;
            if (wppRecibido) numeroWhatsAppDinamico = wppRecibido;
            const aliasRecibido = config.alias || config.Alias;
            if (aliasRecibido) {
                aliasDinamico = aliasRecibido;
                const aliasDisplay = document.getElementById('alias-display');
                if (aliasDisplay) aliasDisplay.innerText = aliasDinamico;
            } else {
                document.getElementById('alias-display').innerText = "Aún no configurado";
            }
        }
    } catch (e) { console.error("Error de config", e); }

    await cargarMenu();
    iniciarCarruselCategorias();

    const urlParams = new URLSearchParams(window.location.search);
    const recoverData = urlParams.get('recover');
    const recoverId = urlParams.get('id');
    const itemCompartido = urlParams.get('item');

    // --- LÓGICA DE RECUPERACIÓN CORREGIDA CON BÚSQUEDA BLINDADA ---
    if (recoverData && recoverId) {
        pedidoEnEdicion = recoverId;
        let carritoLiviano = {};

        try {
            carritoLiviano = JSON.parse(recoverData);
        } catch (e) {
            carritoLiviano = JSON.parse(decodeURIComponent(recoverData));
        }

        carrito = {};
        let hayProductos = false;

        for (let nombreProd in carritoLiviano) {
            const prodOriginal = window.todosLosProductos.find(p => {
                const nomLim = p.nombre.replace(/'/g, "\\'");
                return nombreProd === p.nombre ||
                    nombreProd === nomLim ||
                    nombreProd.startsWith(p.nombre + " (") ||
                    nombreProd.startsWith(nomLim + " (");
            });

            if (prodOriginal) {
                carrito[nombreProd] = {
                    precio: prodOriginal.precio,
                    cantidad: carritoLiviano[nombreProd],
                    img: prodOriginal.img
                };
                hayProductos = true;
            } else {
                carrito[nombreProd] = {
                    precio: 0,
                    cantidad: carritoLiviano[nombreProd],
                    img: imgGris
                };
                hayProductos = true;
            }
        }

        if (hayProductos) {
            guardarCarritoEnStorage();
            localStorage.setItem('modalPasoLaDolce', '1');
            setTimeout(() => {
                document.getElementById('modal-cart').style.display = 'flex';
                renderizarItemsModal();
                irPaso(1);
            }, 500);
        }

        window.history.replaceState({}, document.title, window.location.pathname);
    }

    if (itemCompartido) {
        const nombreItem = decodeURIComponent(itemCompartido);
        setTimeout(() => { abrirModalProducto(nombreItem); }, 500);
    }

    cargarDatosClienteFormulario();
    actualizarUIFlotante();
    const pasoGuardado = localStorage.getItem('modalPasoLaDolce');
    if (pasoGuardado && Object.keys(carrito).length > 0 && !itemCompartido && !recoverData) {
        document.getElementById('modal-cart').style.display = 'flex';
        renderizarItemsModal();
        irPaso(parseInt(pasoGuardado));
    }
}

// ==========================================
// --- LÓGICA DE GALERÍA Y DETALLE VIP ---
// ==========================================
async function abrirModalProducto(nombre) {
    const prod = window.todosLosProductos.find(p => p.nombre === nombre || p.nombre.replace(/'/g, "\\'") === nombre);
    if (!prod) return;

    prodSeleccionadoDetalle = prod;
    cantidadDetalle = 1;

    document.getElementById('det-nombre').innerText = prod.nombre;
    document.getElementById('det-precio').innerText = '$' + prod.precio;

    galeriaImagenes = [prod.img];
    if (prod.imagenesExtras && prod.imagenesExtras.length > 0) {
        galeriaImagenes = galeriaImagenes.concat(prod.imagenesExtras);
    }

    renderizarSlider();

    const contGustos = document.getElementById('contenedor-gustos');
    const listaGustos = document.getElementById('lista-gustos');
    const contenedorCantidad = document.querySelector('.det-qty-controls');

    if (prod.maxGustos && prod.maxGustos > 0) {
        contenedorCantidad.style.display = 'none';
        contGustos.style.display = 'block';
        document.getElementById('badge-gustos').innerText = `Elegí hasta ${prod.maxGustos} sabores`;
        listaGustos.innerHTML = '<p style="grid-column: span 2; padding: 15px; color: var(--text-muted); text-align: center;"><i class="fas fa-spinner fa-spin"></i> Cargando sabores...</p>';

        document.getElementById('modal-producto').style.display = 'flex';

        try {
            const res = await fetch(baseURL + '/Gustos', fetchOptions);
            if (res.ok) {
                const gustosDB = await res.json();
                let htmlGustos = '';

                if (gustosDB.length === 0) {
                    htmlGustos = '<p style="grid-column: span 2; padding: 15px; color: var(--text-muted); text-align: center;">No hay sabores cargados aún.</p>';
                } else {
                    gustosDB.forEach(g => {
                        const nombreGusto = g.nombre !== undefined ? g.nombre : g.Nombre;
                        const hayStock = g.hayStock !== undefined ? g.hayStock : g.HayStock;
                        const disabled = hayStock ? '' : 'disabled';
                        const itemClass = hayStock ? 'gusto-item' : 'gusto-item disabled';
                        const dataStock = hayStock ? 'true' : 'false';
                        const pointerEvents = hayStock ? 'auto' : 'none';
                        const rightElement = hayStock ? `<div class="check-custom"></div>` : `<span class="badge-agotado">Agotado</span>`;
                        const textStyle = hayStock ? '' : 'text-decoration: line-through; opacity: 0.6;';
                        const iconStyle = hayStock ? '' : 'filter: grayscale(100%); opacity: 0.5;';

                        htmlGustos += `
                            <label class="${itemClass}" style="pointer-events: ${pointerEvents};">
                                <div class="gusto-icon-box" style="${iconStyle}"><i class="fas fa-ice-cream"></i></div>
                                <span class="gusto-text" style="${textStyle}">${nombreGusto}</span>
                                <input type="checkbox" class="check-gusto" value="${nombreGusto}" data-stock="${dataStock}" ${disabled} onchange="validarLimitesGusto(${prod.maxGustos})">
                                ${rightElement}
                            </label>
                        `;
                    });
                }
                listaGustos.innerHTML = htmlGustos;
            } else {
                listaGustos.innerHTML = '<p style="grid-column: span 2; color: var(--primary); text-align: center;">Error al cargar sabores.</p>';
            }
        } catch (e) {
            listaGustos.innerHTML = '<p style="grid-column: span 2; color: var(--primary); text-align: center;">Error de conexión.</p>';
        }
    } else {
        contGustos.style.display = 'none';
        contenedorCantidad.style.display = 'flex';
        document.getElementById('modal-producto').style.display = 'flex';
    }

    actualizarBotonDetalle();
    const nombreSeguro = encodeURIComponent(prod.nombre);
    window.history.pushState({}, '', '?item=' + nombreSeguro);
}

function renderizarSlider() {
    const cont = document.getElementById('slider-producto');
    sliderTotalFrames = galeriaImagenes.length;
    sliderActualIndex = 0;

    let trackHtml = '<div class="slider-track" id="slider-track">';
    let dotsHtml = '<div class="slider-dots" id="slider-dots">';

    galeriaImagenes.forEach((url, i) => {
        trackHtml += `<div class="slider-slide"><img src="${url}" loading="lazy" decoding="async"></div>`;
        const activeClass = i === 0 ? 'active' : '';
        dotsHtml += `<div class="slider-dot ${activeClass}" onclick="moverSlider(${i})"></div>`;
    });

    trackHtml += '</div>';
    dotsHtml += '</div>';

    let navButtons = '';
    if (sliderTotalFrames > 1) {
        navButtons = `
            <button class="slider-btn slider-btn-prev" onclick="moverSlider(sliderActualIndex - 1)"><i class="fas fa-chevron-left"></i></button>
            <button class="slider-btn slider-btn-next" onclick="moverSlider(sliderActualIndex + 1)"><i class="fas fa-chevron-right"></i></button>
            ${dotsHtml}
        `;
    }

    cont.innerHTML = trackHtml + navButtons;
}

function moverSlider(index) {
    if (index < 0) index = sliderTotalFrames - 1;
    if (index >= sliderTotalFrames) index = 0;

    sliderActualIndex = index;
    const track = document.getElementById('slider-track');
    if (track) {
        track.style.transform = `translateX(-${index * 100}%)`;
    }

    const dots = document.querySelectorAll('.slider-dot');
    dots.forEach((d, i) => {
        if (i === index) d.classList.add('active');
        else d.classList.remove('active');
    });
}

function validarLimitesGusto(maximo) {
    const checks = document.querySelectorAll('.check-gusto');
    const seleccionados = document.querySelectorAll('.check-gusto:checked').length;
    const limite = parseInt(maximo);

    checks.forEach(chk => {
        if (chk.getAttribute('data-stock') === 'true') {
            if (!chk.checked && seleccionados >= limite) {
                chk.disabled = true;
                chk.parentElement.style.opacity = '0.5';
                chk.parentElement.style.pointerEvents = 'none';
            } else {
                chk.disabled = false;
                chk.parentElement.style.opacity = '1';
                chk.parentElement.style.pointerEvents = 'auto';
            }
        }
    });
}

function cerrarModalProducto() {
    document.getElementById('modal-producto').style.display = 'none';
    prodSeleccionadoDetalle = null;
    galeriaImagenes = [];
    window.history.pushState({}, '', window.location.pathname);
}

function cambiarCantDetalle(delta) {
    cantidadDetalle += delta;
    if (cantidadDetalle < 1) cantidadDetalle = 1;
    actualizarBotonDetalle();
}

function actualizarBotonDetalle() {
    document.getElementById('det-cant').innerText = cantidadDetalle;
    const total = prodSeleccionadoDetalle.precio * cantidadDetalle;
    document.getElementById('btn-agregar-total').innerText = `$${total}`;
}

function agregarDesdeDetalle() {
    if (!prodSeleccionadoDetalle) return;
    let nombreFinalEnCarrito = prodSeleccionadoDetalle.nombre;

    if (prodSeleccionadoDetalle.maxGustos && prodSeleccionadoDetalle.maxGustos > 0) {
        const checksSeleccionados = document.querySelectorAll('.check-gusto:checked');
        if (checksSeleccionados.length === 0) {
            mostrarNotificacion("⚠️ Por favor, elegí al menos 1 gusto de helado para continuar.");
            return;
        }
        const gustosElegidos = Array.from(checksSeleccionados).map(c => c.value).join(", ");
        nombreFinalEnCarrito += ` (${gustosElegidos})`;
    }

    if (!carrito[nombreFinalEnCarrito]) {
        carrito[nombreFinalEnCarrito] = { precio: prodSeleccionadoDetalle.precio, cantidad: cantidadDetalle, img: prodSeleccionadoDetalle.img };
    } else {
        carrito[nombreFinalEnCarrito].cantidad += cantidadDetalle;
    }

    guardarCarritoEnStorage();
    actualizarUIFlotante();
    renderizarItemsModal();
    cerrarModalProducto();
}

function compartirProducto() {
    const url = window.location.href;
    if (navigator.share) { navigator.share({ title: prodSeleccionadoDetalle.nombre, text: '¡Mirá este producto en La Dolce!', url: url }).catch(console.error); }
    else { navigator.clipboard.writeText(url); mostrarNotificacion("¡Link copiado al portapapeles!", "success"); }
}

function iniciarCarruselCategorias() {
    const caja = document.getElementById('caja-categorias');
    if (!caja) return;
    let pausa = false;
    function moverCarrusel() {
        if (!pausa) { caja.scrollLeft += 1; if (caja.scrollLeft + caja.clientWidth >= caja.scrollWidth - 1) { caja.scrollLeft = 0; } }
        requestAnimationFrame(moverCarrusel);
    }
    requestAnimationFrame(moverCarrusel);
    caja.addEventListener('touchstart', function () { pausa = true; });
    caja.addEventListener('touchend', function () { setTimeout(function () { pausa = false; }, 2000); });
    caja.addEventListener('mouseenter', function () { pausa = true; });
    caja.addEventListener('mouseleave', function () { pausa = false; });
}

function separarEmojiDeTexto(textoCrudo) {
    if (!textoCrudo) return { emoji: '', texto: '' };
    const indexEspacio = textoCrudo.indexOf(' ');
    if (indexEspacio !== -1 && indexEspacio <= 5) {
        const primeraParte = textoCrudo.substring(0, indexEspacio);
        if (!/[a-zA-Z]/.test(primeraParte)) { return { emoji: primeraParte, texto: textoCrudo.substring(indexEspacio + 1) }; }
    }
    return { emoji: '', texto: textoCrudo };
}

function irAInicio() { document.getElementById('input-busqueda').value = ''; buscarGlobal(''); const btnPromos = document.getElementById('btn-cat-promos'); if (btnPromos) filtrarCategoria('seccion-promos', btnPromos); window.scrollTo(0, 0); }
function abrirMenuLateral() { const menu = document.getElementById('menu-lateral'); menu.style.display = 'block'; setTimeout(function () { menu.classList.add('activo'); }, 10); }
function cerrarMenuLateral() { const menu = document.getElementById('menu-lateral'); menu.classList.remove('activo'); setTimeout(function () { menu.style.display = 'none'; }, 300); }

function toggleRubro(idRubro) {
    const submenu = document.getElementById('submenu-rubro-' + idRubro);
    const icono = document.getElementById('icono-rubro-' + idRubro);
    if (submenu.classList.contains('abierto')) { submenu.classList.remove('abierto'); icono.classList.remove('rotado'); }
    else { document.querySelectorAll('.categorias-submenu').forEach(function (el) { el.classList.remove('abierto'); }); document.querySelectorAll('.icono-acordeon').forEach(function (el) { el.classList.remove('rotado'); }); submenu.classList.add('abierto'); icono.classList.add('rotado'); }
}

function seleccionarCategoriaDesdeMenu(idSeccion, idBotonCatSuperior) {
    cerrarMenuLateral();
    document.getElementById('input-busqueda').value = '';
    buscarGlobal('');
    const botonFisico = document.getElementById(idBotonCatSuperior);
    if (botonFisico) filtrarCategoria(idSeccion, botonFisico);
}

function filtrarCategoria(idSeccion, elementoBoton) {
    document.querySelectorAll('#contenedor-vistas .seccion-vista').forEach(function (sec) { sec.classList.remove('activa'); });
    document.getElementById(idSeccion).classList.add('activa');
    document.querySelectorAll('.cat-item').forEach(function (btn) { btn.classList.remove('active'); });
    if (elementoBoton) elementoBoton.classList.add('active');
    window.scrollTo(0, 0);
}

function buscarGlobal(texto) {
    if (typeof texto !== 'string') return;
    const quitarAcentos = (str) => str ? str.normalize("NFD").replace(/[\u0300-\u036f]/g, "") : "";
    const txtCrudo = texto.toLowerCase().trim();
    const txtBusqueda = quitarAcentos(txtCrudo);

    const vistaBusqueda = document.getElementById('vista-busqueda');
    const cajaCategorias = document.getElementById('caja-categorias');
    const contenedorVistas = document.getElementById('contenedor-vistas');
    const listaBusqueda = document.getElementById('lista-busqueda-contenido');
    const textoResultados = document.getElementById('texto-resultados');

    if (txtBusqueda === '') {
        vistaBusqueda.style.display = 'none';
        cajaCategorias.style.display = 'flex';
        contenedorVistas.style.display = 'block';
        return;
    }

    vistaBusqueda.style.display = 'block';
    cajaCategorias.style.display = 'none';
    contenedorVistas.style.display = 'none';

    const filtrados = window.todosLosProductos.filter(function (p) {
        const nombreLimpio = quitarAcentos((p.nombre || "").toLowerCase());
        const descLimpia = quitarAcentos((p.desc || "").toLowerCase());
        const catLimpia = quitarAcentos((p.categoria || "").toLowerCase());
        return nombreLimpio.includes(txtBusqueda) || descLimpia.includes(txtBusqueda) || catLimpia.includes(txtBusqueda);
    });

    filtrados.sort((a, b) => {
        const nomA = quitarAcentos((a.nombre || "").toLowerCase());
        const nomB = quitarAcentos((b.nombre || "").toLowerCase());
        const catA = quitarAcentos((a.categoria || "").toLowerCase());
        const catB = quitarAcentos((b.categoria || "").toLowerCase());

        const aEmpieza = nomA.startsWith(txtBusqueda);
        const bEmpieza = nomB.startsWith(txtBusqueda);
        if (aEmpieza && !bEmpieza) return -1;
        if (!aEmpieza && bEmpieza) return 1;

        const aPalabra = nomA.split(' ').some(p => p.startsWith(txtBusqueda));
        const bPalabra = nomB.split(' ').some(p => p.startsWith(txtBusqueda));
        if (aPalabra && !bPalabra) return -1;
        if (!aPalabra && bPalabra) return 1;

        const aCat = catA.includes(txtBusqueda);
        const bCat = catB.includes(txtBusqueda);
        if (aCat && !bCat) return -1;
        if (!aCat && bCat) return 1;

        return 0;
    });

    textoResultados.innerText = filtrados.length > 0 ? `Resultados para "${texto}"` : `No encontramos "${texto}"`;

    let htmlResultados = '';
    filtrados.forEach(function (item) {
        const descLimpio = (item.nombre || "").replace(/'/g, "\\'");
        const promoHtml = item.esPromo ? '<p class="desc" style="color: var(--primary); font-weight: bold; margin-bottom: 2px;">¡COMBO ESPECIAL!</p>' : '';
        let cardClick = ''; let btnHtml = ''; let badgeStyle = ''; let imgStyle = ''; let cardStyle = '';

        if (item.activo) {
            const funcionBotonAdd = (item.maxGustos > 0 || item.imagenesExtras.length > 0) ? `abrirModalProducto('${descLimpio}')` : `agregarAlCarrito('${descLimpio}', ${item.precio}, '${item.img}')`;
            cardClick = `onclick="abrirModalProducto('${descLimpio}')"`;
            btnHtml = `<button class="btn-add" onclick="event.stopPropagation(); ${funcionBotonAdd}">+</button>`;
        } else {
            cardClick = `onclick="mostrarNotificacion('Este producto se encuentra temporalmente agotado.')"`;
            cardStyle = `opacity: 0.6; background: #f3f4f6; cursor: not-allowed;`;
            imgStyle = `filter: grayscale(100%);`;
            badgeStyle = `<span class="badge-agotado" style="display:inline-block; margin-bottom:8px; margin-left:0; border: 1px solid #d1d5db; width: fit-content;">Sin Stock</span>`;
        }

        htmlResultados += `<div class="producto" style="${cardStyle}" ${cardClick}>
            <div class="producto-info">
                ${promoHtml}
                <h3>${item.nombre}</h3>
                <p class="desc">${item.desc}</p>
                ${badgeStyle}
                <div class="precio">$${item.precio}</div>
            </div>
            <div class="producto-img-container">
                <img src="${item.img}" style="${imgStyle}" loading="lazy" decoding="async" alt="${item.nombre}">
                ${btnHtml}
            </div>
        </div>`;
    });
    listaBusqueda.innerHTML = htmlResultados;
}

async function cargarMenu() {
    try {
        window.todosLosProductos = [];
        const cajaCategorias = document.getElementById('caja-categorias');
        const contenedorVistas = document.getElementById('contenedor-vistas');
        const listaMenuLateral = document.getElementById('lista-menu-lateral');

        let cajaCatHTML = '<div class="cat-item active" id="btn-cat-promos" onclick="filtrarCategoria(\'seccion-promos\', this)"><span class="cat-icon">⭐</span><span>Promos</span></div>';
        let menuLateralHTML = '<div class="menu-item-lateral" onclick="seleccionarCategoriaDesdeMenu(\'seccion-promos\', \'btn-cat-promos\')"><span class="cat-icon" style="margin-right:10px;">⭐</span> Promociones</div>';
        let vistasHTML = '<div id="seccion-promos" class="seccion-vista activa"><div class="seccion-header-flex"><span class="cat-icon" style="color:var(--primary); font-size:1.3rem;">⭐</span><h2 class="seccion-titulo">Promociones</h2></div><p class="seccion-subtitulo">Combos especiales para vos</p><div id="lista-promos-contenido"></div></div>';

        const resPromo = await fetch(baseURL + '/Promos', fetchOptions);
        let promosDOMHTML = '';
        if (resPromo.ok) {
            const promos = await resPromo.json();
            promos.forEach(function (item) {
                const idPromo = item.codPromo !== undefined ? item.codPromo : item.CodPromo;
                const img = item.urlImagen || item.UrlImagen || imgGris;
                const nombrePromo = item.nombre || item.Nombre;
                const precioPromo = item.precioVenta || item.PrecioVenta;
                const nombreLimpio = nombrePromo.replace(/'/g, "\\'");
                const desc = "Todo lo que necesitás para armar el mejor plan.";

                window.todosLosProductos.push({ id: idPromo, nombre: nombrePromo, desc: desc, precio: precioPromo, img: img, esPromo: true, maxGustos: 0, activo: true, categoria: "Promos", imagenesExtras: [] });

                promosDOMHTML += `<div class="producto" onclick="abrirModalProducto('${nombreLimpio}')">
                    <div class="producto-info"><p class="desc" style="color: var(--primary); font-weight: bold; margin-bottom: 2px;">¡COMBO ESPECIAL!</p><h3>${nombrePromo}</h3><p class="desc">${desc}</p><div class="precio">$${precioPromo}</div></div>
                    <div class="producto-img-container"><img src="${img}" loading="lazy" decoding="async" alt="${nombrePromo}"><button class="btn-add" onclick="event.stopPropagation(); abrirModalProducto('${nombreLimpio}')">+</button></div></div>`;
            });
        }

        const resRubros = await fetch(baseURL + '/Rubros', fetchOptions);
        let rubros = [];
        if (resRubros.ok) { rubros = await resRubros.json(); }

        rubros.sort((a, b) => {
            const nomA = (a.nombre || a.Nombre || "").toLowerCase();
            const nomB = (b.nombre || b.Nombre || "").toLowerCase();
            if (nomA.includes('helad')) return -1;
            if (nomB.includes('helad')) return 1;
            return 0;
        });

        const resCat = await fetch(baseURL + '/Categorias', fetchOptions);
        let categorias = [];
        if (resCat.ok) { categorias = await resCat.json(); }

        rubros.forEach(rubro => {
            const idRubro = rubro.codRubro !== undefined ? rubro.codRubro : rubro.CodRubro;
            const catsDelRubro = categorias.filter(c => (c.codRubro !== undefined ? c.codRubro : c.CodRubro) == idRubro);

            let submenuHTML = '<div class="categorias-submenu" id="submenu-rubro-' + idRubro + '">';
            if (catsDelRubro.length > 0) {
                catsDelRubro.forEach(cat => {
                    const idCat = cat.codCategoria !== undefined ? cat.codCategoria : cat.CodCategoria;
                    const nombreCat = cat.nombre !== undefined ? cat.nombre : cat.Nombre;
                    const format = separarEmojiDeTexto(nombreCat);
                    const btnId = 'btn-cat-' + idCat;
                    const iconoHTML = format.emoji !== '' ? `<span class="cat-icon">${format.emoji}</span>` : '';
                    const iconoInline = format.emoji !== '' ? `<span class="cat-icon" style="color:var(--primary); font-size:1.3rem;">${format.emoji}</span>` : '';
                    const iconoItem = format.emoji !== '' ? format.emoji + ' ' : '';

                    cajaCatHTML += `<div class="cat-item" id="${btnId}" onclick="filtrarCategoria('seccion-cat-${idCat}', this)">${iconoHTML}<span>${format.texto}</span></div>`;
                    vistasHTML += `<div id="seccion-cat-${idCat}" class="seccion-vista"><div class="seccion-header-flex">${iconoInline}<h2 class="seccion-titulo">${format.texto}</h2></div><p class="seccion-subtitulo">${rubro.nombre || rubro.Nombre}</p><div id="lista-cat-${idCat}"></div></div>`;
                    submenuHTML += `<div class="submenu-item" onclick="seleccionarCategoriaDesdeMenu('seccion-cat-${idCat}', '${btnId}')">${iconoItem}<span style="font-weight:600;">${format.texto}</span></div>`;
                });
            } else {
                submenuHTML += '<div class="submenu-item" style="color:var(--text-muted); font-style:italic;">Sin categorías todavía</div>';
            }
            submenuHTML += '</div>';

            const nombreRubro = rubro.nombre !== undefined ? rubro.nombre : rubro.Nombre;
            menuLateralHTML += `<div class="rubro-acordeon"><div class="rubro-header" onclick="toggleRubro(${idRubro})"><span>${nombreRubro}</span><i class="fas fa-chevron-down icono-acordeon" id="icono-rubro-${idRubro}"></i></div>${submenuHTML}</div>`;
        });

        cajaCategorias.innerHTML = cajaCatHTML;
        listaMenuLateral.innerHTML = menuLateralHTML;
        contenedorVistas.innerHTML = vistasHTML;

        if (promosDOMHTML !== '') {
            document.getElementById('lista-promos-contenido').innerHTML = promosDOMHTML;
        }

        const resArt = await fetch(baseURL + '/Articulos', fetchOptions);
        if (resArt.ok) {
            const articulos = await resArt.json();
            let articulosHTMLPorCat = {};

            articulos.forEach(function (item) {
                const idArticulo = item.codArticulo !== undefined ? item.codArticulo : item.CodArticulo;
                const esActivo = item.activo !== undefined ? item.activo : (item.Activo !== undefined ? item.Activo : true);
                const img = item.urlImagen || item.UrlImagen || imgGris;
                const precio = item.costo || item.Costo || 0;
                const desc = item.descripcion || item.Descripcion || "Producto";
                const catId = item.categoriaId !== undefined ? item.categoriaId : item.CategoriaId;
                const limitGustos = item.maxGustos || item.MaxGustos || 0;

                const extras = item.imagenesExtras ? item.imagenesExtras : (item.ImagenesExtras ? item.ImagenesExtras : []);

                const subtituloDesc = limitGustos > 0 ? "Helado artesanal" : "Sabor original";

                const catObj = categorias.find(c => (c.codCategoria !== undefined ? c.codCategoria : c.CodCategoria) == catId);
                const nombreCat = catObj ? (catObj.nombre !== undefined ? catObj.nombre : catObj.Nombre) : "";

                window.todosLosProductos.push({
                    id: idArticulo,
                    nombre: desc,
                    desc: subtituloDesc,
                    precio: precio,
                    img: img,
                    esPromo: false,
                    maxGustos: limitGustos,
                    activo: esActivo,
                    categoria: nombreCat,
                    imagenesExtras: extras
                });

                const descLimpio = desc.replace(/'/g, "\\'");
                let cardClick = ''; let btnHtml = ''; let badgeStyle = ''; let imgStyle = ''; let cardStyle = '';

                if (esActivo) {
                    const funcionBotonAdd = (limitGustos > 0 || extras.length > 0) ? `abrirModalProducto('${descLimpio}')` : `agregarAlCarrito('${descLimpio}', ${precio}, '${img}')`;
                    cardClick = `onclick="abrirModalProducto('${descLimpio}')"`;
                    btnHtml = `<button class="btn-add" onclick="event.stopPropagation(); ${funcionBotonAdd}">+</button>`;
                } else {
                    cardClick = `onclick="mostrarNotificacion('Este producto se encuentra temporalmente agotado.')"`;
                    cardStyle = `opacity: 0.6; background: #f3f4f6; cursor: not-allowed;`;
                    imgStyle = `filter: grayscale(100%);`;
                    badgeStyle = `<span class="badge-agotado" style="display:inline-block; margin-bottom:8px; margin-left:0; border: 1px solid #d1d5db; width: fit-content;">Sin Stock</span>`;
                }

                if (!articulosHTMLPorCat[catId]) articulosHTMLPorCat[catId] = '';

                articulosHTMLPorCat[catId] += `<div class="producto" style="${cardStyle}" ${cardClick}>
                    <div class="producto-info">
                        <h3>${desc}</h3>
                        <p class="desc">${subtituloDesc}</p>
                        ${badgeStyle}
                        <div class="precio">$${precio}</div>
                    </div>
                    <div class="producto-img-container">
                        <img src="${img}" style="${imgStyle}" loading="lazy" decoding="async" alt="${desc}">
                        ${btnHtml}
                    </div>
                </div>`;
            });

            for (let cId in articulosHTMLPorCat) {
                const div = document.getElementById('lista-cat-' + cId);
                if (div) div.innerHTML = articulosHTMLPorCat[cId];
            }
        }
    } catch (e) { console.error(e); }
}

function agregarAlCarrito(nombre, precio, imgUrl) {
    if (!carrito[nombre]) {
        carrito[nombre] = { precio: precio, cantidad: 1, img: imgUrl };
    } else {
        carrito[nombre].cantidad++;
    }
    guardarCarritoEnStorage(); actualizarUIFlotante(); renderizarItemsModal();
}

function restarDelCarrito(nombre) { if (carrito[nombre]) { carrito[nombre].cantidad--; if (carrito[nombre].cantidad <= 0) { delete carrito[nombre]; } } guardarCarritoEnStorage(); actualizarUIFlotante(); renderizarItemsModal(); }
function eliminarDelCarrito(nombre) { delete carrito[nombre]; guardarCarritoEnStorage(); actualizarUIFlotante(); renderizarItemsModal(); if (Object.keys(carrito).length === 0) { cerrarModalCarrito(); } }

function calcularTotales() {
    let cantItems = 0; let subtotal = 0;
    for (let n in carrito) { cantItems += carrito[n].cantidad; subtotal += (carrito[n].precio * carrito[n].cantidad); }
    let totalG = cantItems > 0 ? subtotal + costoEnvioFijo : 0;
    return { cantItems: cantItems, subtotal: subtotal, totalGeneral: totalG };
}

function actualizarUIFlotante() {
    const t = calcularTotales();
    document.getElementById('nav-cart-badge').innerText = t.cantItems;
    document.getElementById('cant-flotante').innerText = t.cantItems;
    document.getElementById('total-flotante').innerText = '$' + t.subtotal;
    document.querySelector('.carrito-flotante').style.display = t.cantItems > 0 ? 'flex' : 'none';
}

function renderizarItemsModal() {
    const lista = document.getElementById('lista-carrito-items');
    let cartHTML = '';
    for (let n in carrito) {
        let i = carrito[n];
        const nLimpio = n.replace(/'/g, "\\'");
        cartHTML += `<div class="cart-item"><img src="${i.img}" class="cart-item-img" loading="lazy" decoding="async" alt="${n}"><div class="cart-item-info"><h4>${n}</h4><div class="precio">$${i.precio}</div><div class="qty-controls"><button class="qty-btn" onclick="restarDelCarrito('${nLimpio}')">−</button><span class="qty-val">${i.cantidad}</span><button class="qty-btn" onclick="agregarAlCarrito('${nLimpio}', ${i.precio}, '${i.img}')">+</button></div></div><button class="btn-trash" onclick="eliminarDelCarrito('${nLimpio}')"><i class="fas fa-trash-alt"></i></button></div>`;
    }
    lista.innerHTML = cartHTML;

    const t = calcularTotales();
    document.getElementById('txt-subtotal-cant').innerText = 'Subtotal (' + t.cantItems + ')';
    document.getElementById('txt-subtotal-monto').innerText = '$' + t.subtotal;
    document.getElementById('txt-envio-monto').innerText = '$' + costoEnvioFijo;
    document.getElementById('txt-total-monto').innerText = '$' + t.totalGeneral;
    document.getElementById('txt2-subtotal-cant').innerText = t.cantItems + ' productos';
    document.getElementById('txt2-subtotal-monto').innerText = '$' + t.subtotal;
    document.getElementById('txt2-envio-monto').innerText = '$' + costoEnvioFijo;
    document.getElementById('txt2-total-monto').innerText = '$' + t.totalGeneral;
}

function abrirModalCarrito() {
    if (Object.keys(carrito).length === 0) return mostrarNotificacion("Agregá productos primero.");
    renderizarItemsModal();
    document.getElementById('modal-cart').style.display = 'flex';
    irPaso(1);
}

function cerrarModalCarrito() {
    document.getElementById('modal-cart').style.display = 'none';
    localStorage.removeItem('modalPasoLaDolce');
}

function irPaso(paso) {
    const steps = document.querySelectorAll('#modal-cart .modal-step');
    for (let i = 0; i < steps.length; i++) { steps[i].classList.remove('activa'); }
    const btnAccion = document.getElementById('btn-accion-modal'), titulo = document.getElementById('modal-titulo'), btnVolver = document.getElementById('btn-volver'), btnSeguir = document.getElementById('btn-seguir-comprando');
    const spacerVolver = document.getElementById('spacer-volver');
    localStorage.setItem('modalPasoLaDolce', paso);

    if (paso === 1) {
        document.getElementById('paso-1').classList.add('activa');
        btnVolver.style.display = 'none';
        spacerVolver.style.display = 'block';
        btnSeguir.style.display = 'flex';
        titulo.innerText = 'Tu Carrito';

        btnAccion.classList.remove('disabled');
        btnAccion.innerHTML = 'Continuar';
        btnAccion.setAttribute('onclick', 'irPaso(2)');
    } else if (paso === 2) {
        if (Object.keys(carrito).length === 0) { cerrarModalCarrito(); return; }

        const MINIMO_COMPRA = 5000;
        const t = calcularTotales();
        if (t.subtotal < MINIMO_COMPRA) {
            mostrarNotificacion(`El monto mínimo de compra es de $${MINIMO_COMPRA} (sin contar el envío).\nTe faltan $${MINIMO_COMPRA - t.subtotal} para poder avanzar.`);
            irPaso(1);
            return;
        }

        cargarDatosClienteFormulario();

        document.getElementById('paso-2').classList.add('activa');
        btnVolver.style.display = 'block';
        spacerVolver.style.display = 'none';
        btnSeguir.style.display = 'none';
        titulo.innerText = 'Tus Datos de Envío';

        btnAccion.classList.remove('disabled');
        btnAccion.innerHTML = 'Enviar pedido';
        btnAccion.setAttribute('onclick', 'enviarPedidoWhatsApp()');
    }
}

function verificarMetodoPago() {
    const metodo = document.getElementById('cli-pago').value;
    document.getElementById('caja-efectivo').style.display = (metodo === 'Efectivo') ? 'block' : 'none';
    document.getElementById('caja-transferencia').style.display = (metodo === 'Mercado Pago / Transferencia') ? 'block' : 'none';
}

async function enviarPedidoWhatsApp() {
    const nombre = document.getElementById('cli-nombre').value.trim();
    const direccion = document.getElementById('cli-direccion').value.trim();
    const telefono = document.getElementById('cli-telefono').value.trim();
    const pago = document.getElementById('cli-pago').value;
    const abonaCon = document.getElementById('cli-abona-con').value.trim();

    if (!nombre || !direccion || !telefono) {
        return mostrarNotificacion("Por favor, completá todos tus datos (Nombre, Dirección y Teléfono).");
    }

    // FIX: Validación de número mínimo de caracteres
    if (telefono.length < 8) {
        return mostrarNotificacion("⚠️ Por favor, ingresá un número de teléfono válido (mínimo 8 dígitos).");
    }

    const t = calcularTotales();
    if (pago === 'Efectivo') {
        if (!abonaCon) return mostrarNotificacion("Por favor, indicá con cuánto vas a pagar para poder enviarte el vuelto correcto.");
        if (parseFloat(abonaCon) < t.totalGeneral) return mostrarNotificacion('El monto a abonar ($' + abonaCon + ') debe ser mayor o igual al total de tu compra ($' + t.totalGeneral + ').');
    }

    const btnAccion = document.getElementById('btn-accion-modal');
    btnAccion.classList.add('disabled');
    btnAccion.innerHTML = '<i class="fas fa-circle-notch fa-spin"></i> Procesando...';
    btnAccion.removeAttribute('onclick');

    const datosClienteBD = nombre + ' (' + direccion + ') | Tel: ' + telefono; let idRealPedido = pedidoEnEdicion;
    const urlFetch = pedidoEnEdicion ? `${baseURL}/Pedidos/${pedidoEnEdicion}` : `${baseURL}/Pedidos/Nuevo`; const metodoFetch = pedidoEnEdicion ? 'PUT' : 'POST';

    const detallesEnvio = [];
    for (let nombreEnCarrito in carrito) {
        const itemCar = carrito[nombreEnCarrito];

        // --- BÚSQUEDA BLINDADA PARA EVITAR EL ERROR DEL CHOCOLATE ---
        const prodRef = window.todosLosProductos.find(p => {
            const nomLim = p.nombre.replace(/'/g, "\\'");
            return nombreEnCarrito === p.nombre ||
                nombreEnCarrito === nomLim ||
                nombreEnCarrito.startsWith(p.nombre + " (") ||
                nombreEnCarrito.startsWith(nomLim + " (");
        });

        if (prodRef) {
            detallesEnvio.push({
                CodArticulo: prodRef.esPromo ? null : prodRef.id,
                CodPromo: prodRef.esPromo ? prodRef.id : null,
                Cantidad: itemCar.cantidad,
                Precio: itemCar.precio
            });
        }
    }

    // --- PROTECCIÓN EXTRA: Si el carrito se bugeó y está vacío, frenamos el envío ---
    if (detallesEnvio.length === 0) {
        btnAccion.classList.remove('disabled');
        btnAccion.innerHTML = 'Enviar pedido';
        btnAccion.setAttribute('onclick', 'enviarPedidoWhatsApp()');
        return mostrarNotificacion("⚠️ Error leyendo los productos. Vaciá el carrito y volvé a agregarlos.");
    }

    try {
        const payload = {
            cliente: datosClienteBD,
            total: t.totalGeneral,
            detalles: detallesEnvio
        };

        const res = await fetch(urlFetch, {
            method: metodoFetch,
            headers: { 'Content-Type': 'application/json', 'ngrok-skip-browser-warning': 'true' },
            body: JSON.stringify(payload)
        });

        if (res.ok) { const data = await res.json(); idRealPedido = data.id; }
        else {
            mostrarNotificacion("Hubo un error al comunicar con el servidor.");
            btnAccion.classList.remove('disabled');
            btnAccion.innerHTML = 'Enviar pedido';
            btnAccion.setAttribute('onclick', 'enviarPedidoWhatsApp()');
            return;
        }
    } catch (error) {
        console.error("Error de conexión", error);
        mostrarNotificacion("No se pudo conectar con el servidor.");
        btnAccion.classList.remove('disabled');
        btnAccion.innerHTML = 'Enviar pedido';
        btnAccion.setAttribute('onclick', 'enviarPedidoWhatsApp()');
        return;
    }

    const carritoLiviano = {}; for (let p in carrito) { carritoLiviano[p] = carrito[p].cantidad; }
    const urlActual = window.location.origin + window.location.pathname; const datosRecuperacion = encodeURIComponent(JSON.stringify(carritoLiviano)); const linkRecuperar = urlActual + '?recover=' + datosRecuperacion + '&id=' + idRealPedido;

    let texto = '';
    if (pedidoEnEdicion) { texto = '¡Hola *La Dolce*! Necesito *MODIFICAR* mi pedido:\n\n*NÚMERO DE PEDIDO: #' + idRealPedido + '*\n\n*NUEVO DETALLE:*\n'; }
    else { texto = '¡Hola *La Dolce*! Quisiera hacer el siguiente pedido:\n\n*NÚMERO DE PEDIDO: #' + idRealPedido + '*\n\n*MI PEDIDO:*\n'; }

    for (let p in carrito) { texto += '- ' + carrito[p].cantidad + 'x ' + p + ' ($' + (carrito[p].precio * carrito[p].cantidad) + ')\n'; }

    texto += 'Envío: $' + costoEnvioFijo + '\n*Total a pagar:$' + t.totalGeneral + '*\n\n*MIS DATOS:*\nNombre: ' + nombre + '\nDirección: ' + direccion + '\nTeléfono: ' + telefono + '\n';
    if (pago === 'Efectivo') { texto += 'Pago: Efectivo (Abona con $' + abonaCon + ')\n'; } else { texto += 'Pago: Transferencia\n⏳ *El pedido será confirmado cuando envíes el comprobante por este medio.*\n'; }
    texto += '\n\n🔄 *¿Querés cambiar tu pedido?* Entrá a este link:\n' + linkRecuperar;

    const numeroDestino = numeroWhatsAppDinamico !== "" ? numeroWhatsAppDinamico : "5491100000000";
    window.open('https://wa.me/' + numeroDestino + '?text=' + encodeURIComponent(texto), '_blank');

    // --- ACÁ ESTÁ EL SECRETO: Ya NO borramos la memoria del cliente ---
    localStorage.removeItem('carritoLaDolce');
    localStorage.removeItem('modalPasoLaDolce');
    pedidoEnEdicion = null;
    window.location.reload();
}

window.onload = inicializarApp;


// ==========================================
// --- BLOQUEO DEFINITIVO DE ZOOM (IOS / ANDROID) ---
// ==========================================

// 1. Bloquea el "pellizco" (pinch-to-zoom) exclusivo de los iPhone/iPad
document.addEventListener('gesturestart', function (e) {
    e.preventDefault();
});

// 2. Bloquea el "doble toque" rápido que hace zoom en Android y iPhone
let lastTouchEnd = 0;
document.addEventListener('touchend', function (event) {
    const now = (new Date()).getTime();
    if (now - lastTouchEnd <= 300) {
        event.preventDefault();
    }
    lastTouchEnd = now;
}, false);

// 3. Bloquea el pellizco general con dos dedos en la pantalla (touchmove)
document.addEventListener('touchmove', function (event) {
    if (event.scale !== 1 && event.scale !== undefined) {
        event.preventDefault();
    }
}, { passive: false });