using AudioText.Interfaces;

namespace AudioText.Services
{
    /// <summary>
    /// Servicio encargado de convertir el contenido de un archivo a su representación hexadecimal.
    /// Útil para inspección de bajo nivel o depuración de archivos binarios.
    /// </summary>
    public class HexadecimalConverterService : IBinarioHexadecimalService
    {
        /// <summary>
        /// Lee un archivo y convierte cada byte a su equivalente hexadecimal de dos dígitos.
        /// </summary>
        /// <param name="rutaArchivo">Ruta absoluta del archivo a leer.</param>
        /// <param name="progreso">Interfaz para reportar el progreso (0-100%).</param>
        /// <returns>Una cadena con la representación hexadecimal (ej: "4A 0F 2C...").</returns>
        /// <exception cref="FileNotFoundException">Si el archivo no existe.</exception>
        public async Task<string> ObtenerConvercionDelArchivoAsync(string rutaArchivo, IProgress<int> progreso)
        {
            if (!File.Exists(rutaArchivo))
                throw new FileNotFoundException("El archivo especificado no existe.");

            return await Task.Run(async () =>
            {
                // 1. Lectura completa del archivo a memoria.
                byte[] bytes = await File.ReadAllBytesAsync(rutaArchivo);

                // 2. Conversión eficiente a Hexadecimal.
                // BitConverter genera una cadena con el formato "AA-BB-CC".
                // Reemplazamos los guiones por espacios para mejorar la legibilidad visual.
                // Esta aproximación es significativamente más rápida que iterar y convertir byte por byte manualmente.

                progreso?.Report(50); // Notificamos que la lectura y conversión inicial están completas.

                string hex = BitConverter.ToString(bytes).Replace("-", " ");

                progreso?.Report(100); // Proceso finalizado.

                return hex;
            });
        }
    }
}
