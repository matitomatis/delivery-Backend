using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace delivery.Helpers; // Cambiá esto por el namespace de tu proyecto

public static class ImageOptimizer
{
    public static string OptimizeToBase64Webp(string base64Image)
    {
        // Si viene vacío o es una URL externa (o el placeholder), no tocamos nada
        if (string.IsNullOrEmpty(base64Image) || base64Image.StartsWith("http"))
            return base64Image;

        try
        {
            // 1. Limpiamos la cabecera (ej: "data:image/jpeg;base64,...")
            var match = Regex.Match(base64Image, @"data:image/(?<type>.+?);base64,(?<data>.+)");
            string base64Data = match.Success ? match.Groups["data"].Value : base64Image;

            // 2. Convertimos el texto a bytes
            byte[] imageBytes = Convert.FromBase64String(base64Data);

            // 3. Cargamos la imagen con ImageSharp
            using var image = Image.Load(imageBytes);

            // 4. (Opcional pero RECOMENDADO): Achicamos si la foto es gigante. 
            // Si mide más de 800px de ancho, la achica manteniendo la proporción.
            if (image.Width > 800)
            {
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(800, 0),
                    Mode = ResizeMode.Max
                }));
            }

            // 5. La guardamos en memoria con formato WebP al 80% de calidad
            using var ms = new MemoryStream();
            image.SaveAsWebp(ms, new WebpEncoder { Quality = 80 });

            // 6. Devolvemos el nuevo texto Base64 listo para guardar en la BD
            byte[] webpBytes = ms.ToArray();
            return "data:image/webp;base64," + Convert.ToBase64String(webpBytes);
        }
        catch
        {
            // Si la imagen está corrupta o algo falla, devolvemos la original para que no se rompa la app
            return base64Image;
        }
    }
}