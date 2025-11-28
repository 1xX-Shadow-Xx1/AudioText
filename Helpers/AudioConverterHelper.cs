using System.IO;
using System;
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

            // 1. Si ya es WAV, no se necesita conversión.
            if (extension == ".wav")
            {
                Debug.WriteLine("Formato WAV detectado. Sin necesidad de conversión.");
                return rutaArchivoOriginal;
            }

            // 2. Si es MP3, realizamos la conversión interna real con NAudio.
            if (extension == ".mp3")
            {
                try
                {
                    Debug.WriteLine($"Iniciando conversión de MP3 a WAV: {rutaArchivoOriginal}");

                    // Usamos Mp3FileReader de NAudio para leer el MP3
                    using (var reader = new Mp3FileReader(rutaArchivoOriginal))
                    {
                        // Usamos WaveFileWriter para escribir el audio descomprimido al archivo WAV temporal
                        WaveFileWriter.CreateWaveFile(rutaDestinoTemp, reader);
                    }

                    Debug.WriteLine($"Conversión exitosa. Archivo WAV guardado en: {rutaDestinoTemp}");
                    // Retornamos la ruta del nuevo archivo WAV temporal
                    return rutaDestinoTemp;
                }
                catch (Exception ex)
                {
                    // Si el archivo MP3 está corrupto o la conversión falla por algún motivo de códec.
                    throw new InvalidOperationException($"Fallo la conversión de MP3 a WAV. Verifique el archivo o el códec de audio: {ex.Message}");
                }
            }

            // 3. Si es otro formato no admitido. (Control de formatos no deseados)
            throw new NotSupportedException($"El formato '{extension}' no es soportado. Use .MP3 (con conversión interna) o .WAV.");
        }
    }
}