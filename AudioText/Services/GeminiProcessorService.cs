using AudioText.Interfaces;
using Mscc.GenerativeAI;
using AudioText.Interfaces;
using Mscc.GenerativeAI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Text.Json;

namespace AudioText.Services
{
    /// <summary>
    /// Servicio de integración con la API de Google Gemini para procesamiento de audio.
    /// Implementa la interfaz IProcesadorAudio para proporcionar capacidades de transcripción en la nube.
    /// </summary>
    public class GeminiProcessorService : IProcesadorAudio
    {
        private const string SecretsFileName = "secrets.json";
        private string? _apiKey;

        /// <summary>
        /// Envía un archivo de audio a la API de Gemini para su transcripción a texto.
        /// Maneja la autenticación, preparación del payload y procesamiento de la respuesta.
        /// </summary>
        /// <param name="rutaArchivo">Ruta absoluta del archivo de audio a procesar.</param>
        /// <param name="progresoTexto">Interfaz para reportar el estado del proceso (mensajes de log).</param>
        /// <param name="progresoPorcentaje">Interfaz para reportar el avance porcentual estimado.</param>
        /// <returns>El texto transcrito devuelto por la IA.</returns>
        /// <exception cref="FileNotFoundException">Si el archivo de audio o el archivo de secretos no existe.</exception>
        /// <exception cref="InvalidOperationException">Si ocurre un error en la comunicación con la API o la configuración es inválida.</exception>
        public async Task<string> ConvertirATextoAsync(string rutaArchivo, IProgress<string> progresoTexto, IProgress<int> progresoPorcentaje)
        {
            if (!File.Exists(rutaArchivo)) throw new FileNotFoundException("El archivo de audio especificado no fue encontrado.");

            return await Task.Run(async () =>
            {
                try
                {
                    progresoTexto?.Report("Iniciando conexión con el modelo Gemini 1.5 Flash...\r\n");
                    progresoPorcentaje?.Report(10);

                    // 1. Inicialización Lazy de la API Key para evitar lecturas de disco innecesarias en el constructor.
                    if (string.IsNullOrEmpty(_apiKey))
                    {
                         _apiKey = ObtenerApiKey();
                    }
                    var googleAI = new GoogleAI(_apiKey);

                    // Seleccionamos el modelo "flash" por su balance entre velocidad y costo/eficiencia.
                    var model = googleAI.GenerativeModel("gemini-2.5-flash");

                    // 2. Preparación del Audio: Codificación a Base64 requerida por la API.
                    byte[] audioBytes = File.ReadAllBytes(rutaArchivo);
                    string base64Audio = Convert.ToBase64String(audioBytes);
                    string mimeType = ObtenerMimeType(rutaArchivo);

                    progresoTexto?.Report($"Subiendo archivo de audio ({mimeType}) a la nube...\r\n");
                    progresoPorcentaje?.Report(30);

                    // 3. Construcción del Request: Prompt + Datos Binarios.
                    var request = new GenerateContentRequest
                    {
                        Contents = new List<Content>
                        {
                            new Content
                            {
                                Role = Role.User,
                                Parts = new List<IPart>
                                {
                                    new TextData
                                    {
                                        Text = "Por favor, transcribe este audio a texto en español. Solo dame la transcripción exacta, sin comentarios adicionales."
                                    },
                                    new InlineData
                                    {
                                        MimeType = mimeType,
                                        Data = base64Audio
                                    }
                                }
                            }
                        }
                    };

                    progresoTexto?.Report("Gemini está analizando y transcribiendo el audio...\r\n");
                    progresoPorcentaje?.Report(50);

                    // 4. Envío y Recepción.
                    var response = await model.GenerateContent(request, null ,CancellationToken.None);

                    progresoPorcentaje?.Report(90);

                    if (response.Text != null)
                    {
                        progresoTexto?.Report("¡Transcripción completada exitosamente!\r\n");
                        progresoPorcentaje?.Report(100);
                        return response.Text;
                    }
                    else
                    {
                        return "La IA no devolvió ningún texto. Es posible que el audio esté vacío o sea inaudible.";
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Error en el servicio Gemini: {ex.Message}", ex);
                }
            });
        }

        /// <summary>
        /// Determina el tipo MIME del archivo basado en su extensión.
        /// Necesario para que la API sepa cómo decodificar el audio.
        /// </summary>
        private string ObtenerMimeType(string ruta)
        {
            string ext = Path.GetExtension(ruta).ToLower();
            return ext switch
            {
                ".mp3" => "audio/mp3",
                ".wav" => "audio/wav",
                ".m4a" => "audio/mp4",
                ".ogg" => "audio/ogg",
                _ => "audio/mp3" // Default seguro
            };
        }

        /// <summary>
        /// Recupera la API Key de forma segura desde un archivo de configuración local 'secrets.json'.
        /// </summary>
        /// <returns>La cadena de la API Key.</returns>
        private string ObtenerApiKey()
        {
            if (!File.Exists(SecretsFileName))
            {
                throw new FileNotFoundException($"No se encontró el archivo de configuración '{SecretsFileName}'. Asegúrese de crearlo con su API Key.");
            }

            try
            {
                string json = File.ReadAllText(SecretsFileName);
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    if (doc.RootElement.TryGetProperty("GeminiApiKey", out JsonElement keyElement))
                    {
                        string? key = keyElement.GetString();
                        if (!string.IsNullOrWhiteSpace(key))
                        {
                            return key;
                        }
                    }
                }
                throw new InvalidOperationException("La propiedad 'GeminiApiKey' no existe o está vacía en el archivo secrets.json.");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error crítico al leer la configuración de seguridad: {ex.Message}", ex);
            }
        }
    }
}