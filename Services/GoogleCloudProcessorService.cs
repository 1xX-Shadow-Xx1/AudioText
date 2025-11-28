using AudioText.Interfaces;
using Google.Cloud.Speech.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioText.Services
{
    /// <summary>
    /// Implementación del contrato IProcesadorAudio utilizando Google Cloud Speech-to-Text API.
    /// Esto resuelve los problemas de calidad y texto incompleto del motor local.
    /// </summary>
    public class GoogleCloudProcessorService : IProcesadorAudio
    {
        /// <inheritdoc />
        public string ConvertirATexto(string rutaArchivo)
        {
            if (!File.Exists(rutaArchivo))
            {
                throw new FileNotFoundException("El archivo de audio no fue encontrado.");
            }

            // Nota: Aquí el AudioConverterHelper ya debería haber asegurado que el archivo 
            // esté en un formato aceptable para la API (ej. WAV, FLAC, o MP3/M4A si la API lo soporta).

            try
            {
                // 1. Inicializar el cliente (asume que las credenciales están configuradas en el entorno)
                // En un proyecto real, se deben manejar las credenciales de forma segura.
                SpeechClient speech = SpeechClient.Create();

                // 2. Leer el archivo de audio a bytes
                byte[] audioBytes = File.ReadAllBytes(rutaArchivo);
                var audio = RecognitionAudio.FromBytes(audioBytes);

                // 3. Configurar las opciones de la API
                var config = new RecognitionConfig
                {
                    Encoding = RecognitionConfig.Types.AudioEncoding.Linear16, // PCM (para WAV)
                    SampleRateHertz = 16000, // Requerimiento común
                    LanguageCode = "es-ES", // O el idioma que necesites
                };

                // 4. Realizar la llamada a la API (sincrónica o asíncrona)
                RecognitionResponse response = speech.Recognize(config, audio);

                // 5. Procesar la respuesta
                StringBuilder transcription = new StringBuilder();
                foreach (var result in response.Results)
                {
                    // Obtiene la transcripción más probable
                    transcription.AppendLine(result.Alternatives[0].Transcript);
                }

                return transcription.ToString();
            }
            catch (Exception ex)
            {
                // Manejo de errores de red, autenticación o formato.
                throw new InvalidOperationException($"Error al usar la API de Google Cloud: {ex.Message}");
            }
        }
    }
}
