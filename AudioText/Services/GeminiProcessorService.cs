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
    /// Servicio para procesar audio utilizando la API de Google Gemini.
    /// </summary>
    public class GeminiProcessorService : IProcesadorAudio
    {
        private const string SecretsFileName = "secrets.json";
        private string? _apiKey;

        /// <summary>
        /// Convierte audio a texto utilizando la API de Google Gemini.
        /// </summary>
        /// <param name="rutaArchivo">Ruta absoluta del archivo de audio.</param>
        /// <param name="progresoTexto">Reportador de progreso para texto parcial (no soportado por todos los modelos).</param>
        /// <param name="progresoPorcentaje">Reportador de progreso en porcentaje (0-100).</param>
        /// <param name="token">Token de cancelación.</param>
        /// <returns>Texto transcrito.</returns>
        public async Task<string> ConvertirATextoAsync(string rutaArchivo, IProgress<string> progresoTexto, IProgress<int> progresoPorcentaje, CancellationToken token)
        {
            if (!File.Exists(rutaArchivo)) throw new FileNotFoundException("Archivo no encontrado.");

            return await Task.Run(async () =>
            {
                try
                {

                    // Chequeo de cancelación inicial
                    token.ThrowIfCancellationRequested();

                    progresoTexto?.Report("Conectando con Gemini 1.5 Flash...\r\n");
                    progresoPorcentaje?.Report(10);

                    // 1. Inicializar el cliente
                    if (string.IsNullOrEmpty(_apiKey))
                    {
                         _apiKey = ObtenerApiKey();
                    }
                    var googleAI = new GoogleAI(_apiKey);

                    // CORRECCIÓN 1: Usamos el string directo en lugar del Enum para evitar errores
                    var model = googleAI.GenerativeModel("gemini-2.5-flash");

                    // 2. Preparar audio
                    byte[] audioBytes = File.ReadAllBytes(rutaArchivo);
                    string base64Audio = Convert.ToBase64String(audioBytes);
                    string mimeType = ObtenerMimeType(rutaArchivo);

                    progresoTexto?.Report($"Subiendo audio ({mimeType}) a la IA...\r\n");
                    progresoPorcentaje?.Report(30);

                    // Chequeo de cancelación antes de subir
                    token.ThrowIfCancellationRequested();

                    // 3. Crear la solicitud (CORRECCIÓN DE TIPOS AQUÍ)
                    var request = new GenerateContentRequest
                    {
                        Contents = new List<Content>
                        {
                            new Content
                            {
                                Role = Role.User,
                                // CORRECCIÓN 2: Declaramos explícitamente List<IPart>
                                Parts = new List<IPart>
                                {
                                    new TextData
                                    {
                                        Text = "Por favor, transcribe este audio a texto en español. Solo dame la transcripción exacta."
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

                    progresoTexto?.Report("Gemini está procesando el audio...\r\n");
                    progresoPorcentaje?.Report(50);

                    // 4. Enviar
                    var response = await model.GenerateContent(request, null ,token);

                    progresoPorcentaje?.Report(90);

                    if (response.Text != null)
                    {
                        progresoTexto?.Report("¡Transcripción recibida con éxito!\r\n");
                        progresoPorcentaje?.Report(100);
                        return response.Text;
                    }
                    else
                    {
                        return "Gemini no devolvió texto (posible audio vacío).";
                    }
                }
                catch (OperationCanceledException)
                {
                    // Si se cancela, lanzamos la excepción para que el Form lo sepa
                    throw;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Error Gemini: {ex.Message}");
                }
            });
        }

        private string ObtenerMimeType(string ruta)
        {
            string ext = Path.GetExtension(ruta).ToLower();
            return ext switch
            {
                ".mp3" => "audio/mp3",
                ".wav" => "audio/wav",
                ".m4a" => "audio/mp4",
                ".ogg" => "audio/ogg",
                _ => "audio/mp3"
            };
        }

        /// <summary>
        /// Lee la API Key desde el archivo secrets.json.
        /// </summary>
        /// <returns>La API Key de Gemini.</returns>
        private string ObtenerApiKey()
        {
            if (!File.Exists(SecretsFileName))
            {
                throw new FileNotFoundException($"No se encontró el archivo de secretos '{SecretsFileName}'.");
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
                throw new InvalidOperationException("La clave 'GeminiApiKey' no se encontró o está vacía en secrets.json.");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al leer la API Key: {ex.Message}");
            }
        }
    }
}