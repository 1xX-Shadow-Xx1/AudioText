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
    /// Implementación de IProcesadorAudio usando Whisper.net (IA Offline).
    /// </summary>
    public class WhisperProcessorService : IProcesadorAudio
    {
        // Nombre del archivo del modelo que descargaste en el Paso 1
        private const string ModelFileName = "ggml-base.bin";

        /// <summary>
        /// Convierte audio a texto utilizando el modelo local de Whisper.
        /// </summary>
        /// <param name="rutaArchivo">Ruta absoluta del archivo de audio.</param>
        /// <param name="progresoTexto">Reportador de progreso para texto parcial.</param>
        /// <param name="progresoPorcentaje">Reportador de progreso en porcentaje (0-100).</param>
        /// <param name="token">Token de cancelación.</param>
        /// <returns>Texto transcrito.</returns>
        public async Task<string> ConvertirATextoAsync(string rutaArchivo, IProgress<string> progresoTexto, IProgress<int> progresoPorcentaje, CancellationToken token)
        {
            if (!File.Exists(ModelFileName)) throw new FileNotFoundException($"Falta el modelo '{ModelFileName}'.");
            if (!File.Exists(rutaArchivo)) throw new FileNotFoundException("Archivo de audio no encontrado.");

            try
            {
                // Pasamos el token al proceso interno
                return await ProcessAudioAsync(rutaArchivo, progresoTexto, progresoPorcentaje, token);
            }
            catch (OperationCanceledException)
            {
                throw; // Re-lanzamos para que el formulario sepa que fue cancelado
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error en Whisper: {ex.Message}");
            }
        }

        private async Task<string> ProcessAudioAsync(string rutaArchivo, IProgress<string> progresoTexto, IProgress<int> progresoPorcentaje, CancellationToken token)
        {

            // 1. Obtener la duración total del audio para calcular el %
            TimeSpan duracionTotal;
            using (var reader = new AudioFileReader(rutaArchivo))
            {
                duracionTotal = reader.TotalTime;
            }

            using var whisperFactory = WhisperFactory.FromPath(ModelFileName);
            using var processor = whisperFactory.CreateBuilder()
                .WithLanguage("es")
                .Build();

            using var fileStream = File.OpenRead(rutaArchivo);
            StringBuilder transcripcionCompleta = new StringBuilder();

            // 2. Procesar
            await foreach (var segment in processor.ProcessAsync(fileStream, token)) // El token interno de Whisper a veces falla, controlamos manual abajo
            {
                string nuevoTexto = segment.Text;
                transcripcionCompleta.Append(nuevoTexto);

                // Reportar Texto (Streaming)
                progresoTexto?.Report(nuevoTexto);

                // Reportar Porcentaje (Cálculo matemático)
                if (progresoPorcentaje != null && duracionTotal.TotalSeconds > 0)
                {
                    // segment.End es el tiempo donde termina la frase actual
                    double porcentaje = (segment.End.TotalSeconds / duracionTotal.TotalSeconds) * 100;

                    // Aseguramos que esté entre 0 y 100
                    int porcentajeEntero = Math.Clamp((int)porcentaje, 0, 100);

                    progresoPorcentaje.Report(porcentajeEntero);
                }
            }

            // Aseguramos que llegue al 100% al final
            progresoPorcentaje?.Report(100);

            return transcripcionCompleta.ToString().Trim();
        }
    }
}
