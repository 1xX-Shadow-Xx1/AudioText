
using NAudio.Wave; // Necesario para Mp3FileReader y WaveFileWriter
using System.Diagnostics; // Ya lo tienes, para Debug.WriteLine

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
        /// (Criterio 1.1: Controlar formatos no deseados, convirtiéndolos internamente).
        /// </summary>
        /// <param name="rutaArchivoOriginal">La ruta del archivo subido por el usuario.</param>
        /// <param name="rutaDestinoTemp">La ruta donde se guardará el archivo WAV temporal.</param>
        /// <returns>La ruta del archivo WAV listo para ser procesado.</returns>
        public static string PrepararAudioParaProcesamiento(string rutaArchivoOriginal, string rutaDestinoTemp)
        {
            string extension = Path.GetExtension(rutaArchivoOriginal).ToLower();

            try
            {
                Debug.WriteLine($"Procesando audio: {rutaArchivoOriginal}");

                // Usamos AudioFileReader de NAudio que maneja MP3, WAV, AIFF, etc.
                using (var reader = new AudioFileReader(rutaArchivoOriginal))
                {
                    // Configuración deseada para Whisper: 16000 Hz, 1 canal (Mono)
                    var outFormat = new WaveFormat(16000, 1);

                    // Usamos MediaFoundationResampler para cambiar la calidad a 16kHz
                    using (var resampler = new MediaFoundationResampler(reader, outFormat))
                    {
                        resampler.ResamplerQuality = 60; // Calidad decente

                        // Guardamos el archivo procesado
                        WaveFileWriter.CreateWaveFile(rutaDestinoTemp, resampler);
                    }
                }

                Debug.WriteLine($"Audio convertido a 16kHz exitosamente: {rutaDestinoTemp}");
                return rutaDestinoTemp;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al convertir audio para Whisper (Se requiere WAV 16kHz): {ex.Message}");
            }
        }
    }
}