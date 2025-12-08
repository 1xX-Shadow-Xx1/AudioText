using AudioText.Interfaces;
using System.Text;

namespace AudioText.Services
{
    /// <summary>
    /// Servicio encargado de la manipulación y conversión de archivos a representaciones binarias.
    /// </summary>
    public class BinarioService : IBinarioHexadecimalService
    {
        /// <summary>
        /// Lee un archivo de forma asíncrona y convierte su contenido a una cadena de texto en formato binario.
        /// </summary>
        /// <param name="rutaArchivo">Ruta absoluta del archivo a procesar.</param>
        /// <param name="progreso">Interfaz para reportar el progreso de la conversión (0-100%).</param>
        /// <returns>Una cadena con la representación binaria del archivo, donde cada byte está separado por un espacio.</returns>
        /// <exception cref="FileNotFoundException">Se lanza si el archivo especificado no existe.</exception>
        public async Task<string> ObtenerConvercionDelArchivoAsync(string rutaArchivo, IProgress<int> progreso)
        {
            if (!File.Exists(rutaArchivo))
                throw new FileNotFoundException("El archivo especificado no fue encontrado en la ruta indicada.");

            return await Task.Run(async () =>
            {
                // Leemos la totalidad de los bytes del archivo para procesarlos en memoria.
                byte[] bytes = await File.ReadAllBytesAsync(rutaArchivo);

                // Pre-asignamos la capacidad del StringBuilder para optimizar el uso de memoria.
                // Cada byte se convierte en 8 bits + 1 espacio separador = 9 caracteres.
                StringBuilder sb = new StringBuilder(bytes.Length * 9);

                int totalBytes = bytes.Length;
                // Calculamos el intervalo de reporte para notificar el progreso aproximadamente cada 1%.
                // Usamos Math.Max para evitar división por cero o intervalos de 0 en archivos muy pequeños.
                int reporteCada = Math.Max(1, totalBytes / 100);

                for (int i = 0; i < totalBytes; i++)
                {
                    // Convertimos el byte a su representación binaria (base 2) y aseguramos el formato de 8 bits con ceros a la izquierda.
                    string binario = Convert.ToString(bytes[i], 2).PadLeft(8, '0');

                    sb.Append(binario);
                    sb.Append(" ");

                    // Optimizamos el reporte de progreso para no saturar el hilo de la UI o el llamador,
                    // notificando solo cuando se completa un bloque significativo (cada 1% aprox).
                    if (i % reporteCada == 0)
                    {
                        progreso?.Report((i * 100) / totalBytes);
                    }
                }

                // Aseguramos que el progreso llegue al 100% al finalizar.
                progreso?.Report(100);

                return sb.ToString();
            });
        }
    }
}
