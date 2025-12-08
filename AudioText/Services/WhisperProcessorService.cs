using AudioText.Interfaces;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whisper.net;

namespace AudioText.Services
{
    /// <summary>
    /// Implementación del servicio de transcripción utilizando Whisper.net.
    /// Permite realizar el reconocimiento de voz a texto de manera local (Offline), sin necesidad de conexión a internet.
    /// </summary>
    public class WhisperProcessorService : IProcesadorAudio
    {
        // Nombre del archivo del modelo cuantizado (GGML) requerido por Whisper.
        private const string ModelFileName = "ggml-base.bin";

        /// <summary>
        /// Procesa el archivo de audio utilizando el modelo local de Whisper para generar la transcripción.
        /// </summary>
        /// <param name="rutaArchivo">Ruta absoluta del archivo de audio (debe ser compatible, preferiblemente WAV 16kHz).</param>
        /// <param name="progresoTexto">Interfaz para reportar fragmentos de texto a medida que se generan (Streaming).</param>
        /// <param name="progresoPorcentaje">Interfaz para reportar el progreso estimado basado en la duración del audio.</param>
        /// <returns>El texto completo transcrito.</returns>
        /// <exception cref="FileNotFoundException">Si no se encuentra el modelo o el archivo de audio.</exception>
        /// <exception cref="InvalidOperationException">Si ocurre un error interno en el motor de Whisper.</exception>
        public async Task<string> ConvertirATextoAsync(string rutaArchivo, IProgress<string> progresoTexto, IProgress<int> progresoPorcentaje)
        {
            if (!File.Exists(ModelFileName)) throw new FileNotFoundException($"No se encontró el modelo de IA '{ModelFileName}' en el directorio de ejecución.");
            if (!File.Exists(rutaArchivo)) throw new FileNotFoundException("El archivo de audio a procesar no existe.");

            try
            {
                // Delegamos la lógica compleja al método privado para mantener este método limpio.
                return await ProcessAudioAsync(rutaArchivo, progresoTexto, progresoPorcentaje);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error crítico en el motor Whisper: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lógica interna de procesamiento con WhisperFactory.
        /// </summary>
        private async Task<string> ProcessAudioAsync(string rutaArchivo, IProgress<string> progresoTexto, IProgress<int> progresoPorcentaje)
        {
            // 1. Calculamos la duración total del audio para poder estimar el porcentaje de avance.
            TimeSpan duracionTotal;
            using (var reader = new AudioFileReader(rutaArchivo))
            {
                duracionTotal = reader.TotalTime;
            }

            // Inicializamos la fábrica y el procesador de Whisper.
            // 'WithLanguage("es")' fuerza el modelo a esperar español, mejorando la precisión.
            using var whisperFactory = WhisperFactory.FromPath(ModelFileName);
            using var processor = whisperFactory.CreateBuilder()
                .WithLanguage("es")
                .Build();

            using var fileStream = File.OpenRead(rutaArchivo);
            StringBuilder transcripcionCompleta = new StringBuilder();

            // 2. Procesamiento por segmentos.
            // Whisper procesa el audio en trozos. Iteramos asíncronamente sobre cada segmento reconocido.
            await foreach (var segment in processor.ProcessAsync(fileStream, CancellationToken.None))
            {
                string nuevoTexto = segment.Text;
                transcripcionCompleta.Append(nuevoTexto);

                // Reportamos el texto parcial inmediatamente para dar feedback visual al usuario.
                progresoTexto?.Report(nuevoTexto);

                // Calculamos y reportamos el progreso numérico.
                if (progresoPorcentaje != null && duracionTotal.TotalSeconds > 0)
                {
                    // segment.End indica la marca de tiempo final del segmento actual.
                    double porcentaje = (segment.End.TotalSeconds / duracionTotal.TotalSeconds) * 100;

                    // Clampeamos el valor entre 0 y 100 para evitar inconsistencias visuales.
                    int porcentajeEntero = Math.Clamp((int)porcentaje, 0, 100);

                    progresoPorcentaje.Report(porcentajeEntero);
                }
            }

            // Aseguramos que la barra de progreso llegue al final al terminar el bucle.
            progresoPorcentaje?.Report(100);

            return transcripcionCompleta.ToString().Trim();
        }
    }
}
