using NAudio.Wave; // Necesario para Mp3FileReader y WaveFileWriter
using System.Diagnostics; // Para Debug.WriteLine

namespace AudioText.Helpers
{
    /// <summary>
    /// Clase auxiliar estática encargada de la normalización y conversión de archivos de audio.
    /// Su responsabilidad principal es asegurar que el audio tenga el formato requerido (WAV 16kHz Mono) antes de ser procesado.
    /// </summary>
    public static class AudioConverterHelper
    {
        /// <summary>
        /// Prepara un archivo de audio para su procesamiento, convirtiéndolo a formato WAV con una frecuencia de muestreo de 16kHz y canal mono.
        /// Este formato es el estándar requerido por modelos como Whisper para una transcripción óptima.
        /// </summary>
        /// <param name="rutaArchivoOriginal">Ruta absoluta del archivo de audio original (ej: .mp3, .wav, .m4a).</param>
        /// <param name="rutaDestinoTemp">Ruta absoluta donde se guardará el archivo temporal convertido.</param>
        /// <returns>La ruta del archivo convertido listo para ser procesado.</returns>
        /// <exception cref="InvalidOperationException">Se lanza si ocurre un error durante la conversión o lectura del archivo de audio.</exception>
        public static string PrepararAudioParaProcesamiento(string rutaArchivoOriginal, string rutaDestinoTemp)
        {
            string extension = Path.GetExtension(rutaArchivoOriginal).ToLower();

            try
            {
                Debug.WriteLine($"Iniciando conversión de audio: {rutaArchivoOriginal}");

                // Utilizamos AudioFileReader de NAudio, que es capaz de leer múltiples formatos (MP3, WAV, AIFF, etc.) de forma transparente.
                using (var reader = new AudioFileReader(rutaArchivoOriginal))
                {
                    // Definimos el formato de salida objetivo: 16000 Hz, 1 canal (Mono).
                    // Whisper funciona mejor con audio a 16kHz.
                    var outFormat = new WaveFormat(16000, 1);

                    // Utilizamos MediaFoundationResampler para realizar el re-muestreo (resampling) de alta calidad.
                    using (var resampler = new MediaFoundationResampler(reader, outFormat))
                    {
                        // Establecemos la calidad del resampler. 60 es un valor equilibrado entre calidad y velocidad.
                        resampler.ResamplerQuality = 60;

                        // Escribimos el flujo de audio re-muestreado al archivo de destino.
                        WaveFileWriter.CreateWaveFile(rutaDestinoTemp, resampler);
                    }
                }

                Debug.WriteLine($"Conversión completada exitosamente: {rutaDestinoTemp}");
                return rutaDestinoTemp;
            }
            catch (Exception ex)
            {
                // Envolvemos la excepción original para proporcionar un contexto más claro sobre el error.
                throw new InvalidOperationException($"Error crítico al convertir el audio. Asegúrese de que el archivo no esté corrupto. Detalle: {ex.Message}", ex);
            }
        }
    }
}