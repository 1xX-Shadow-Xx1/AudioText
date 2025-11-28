using System.Diagnostics;

namespace AudioText.Helpers
{
    /// <summary>
    /// Clase auxiliar para manejar la conversión de formatos de audio (MP3 a WAV).
    /// Esta es una responsabilidad secundaria, separada del procesamiento principal (Cumple SRP).
    /// </summary>
    public static class AudioConverterHelper
    {
        /// <summary>
        /// Convierte el archivo de audio subido (MP3, etc.) a formato WAV, si es necesario.
        /// </summary>
        /// <param name="rutaArchivoOriginal">La ruta del archivo subido por el usuario.</param>
        /// <param name="rutaDestinoTemp">La ruta donde se guardará el archivo WAV temporal.</param>
        /// <returns>La ruta del archivo WAV listo para ser procesado.</returns>
        public static string PrepararAudioParaProcesamiento(string rutaArchivoOriginal, string rutaDestinoTemp)
        {
            string extension = Path.GetExtension(rutaArchivoOriginal).ToLower();

            // ... validaciones iniciales ...

            if (extension == ".mp3")
            {
                // ** IMPLEMENTACIÓN REAL CON NAUDIO **
                try
                {
                    // Usamos Mp3FileReader de NAudio para convertir.
                    using (var reader = new NAudio.Wave.Mp3FileReader(rutaArchivoOriginal))
                    {
                        NAudio.Wave.WaveFileWriter.CreateWaveFile(rutaDestinoTemp, reader);
                    }
                    // Retornamos la ruta del nuevo archivo WAV temporal
                    return rutaDestinoTemp;
                }
                catch (Exception ex)
                {
                    // Manejo de errores de conversión real
                    throw new InvalidOperationException($"Fallo la conversión de MP3 a WAV: {ex.Message}");
                }
            }

            // Criterio 1.1: Controlar y convertir formatos no deseados (como .MP3).
            if (extension == ".mp3")
            {
                // ** Lógica de conversión de MP3 a WAV **
                // NOTA: Para implementar la conversión real, necesitarías una librería como NAudio.
                // Por ejemplo: new Mp3FileReader(rutaArchivoOriginal).Save(rutaDestinoTemp);

                // --- INICIO DE IMPLEMENTACIÓN SIMULADA ---
                Debug.WriteLine("Simulando conversión de MP3 a WAV. Se requiere NAudio para la conversión real.");
                throw new NotImplementedException(
                    "Conversión de MP3 a WAV no implementada. " +
                    "Para continuar, use un archivo '.wav' o agregue la librería 'NAudio' para la conversión interna."
                );
                // --- FIN DE IMPLEMENTACIÓN SIMULADA ---
            }

            // Si es otro formato, se rechaza.
            throw new NotSupportedException($"El formato '{extension}' no es soportado. Use .MP3 o .WAV.");
        }
    }
}
