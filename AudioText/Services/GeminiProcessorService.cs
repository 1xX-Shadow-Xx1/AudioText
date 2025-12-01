using AudioText.Interfaces;
using Mscc.GenerativeAI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AudioText.Services
{
    public class GeminiProcessorService : IProcesadorAudio
    {
        // ⚠️ RECUERDA: Pega aquí tu API KEY de Google AI Studio
        private const string ApiKey = "AIzaSyBkP5B-1xsGD-W9UnLvuMRQ3AmGm8pEgzc";

        public async Task<string> ConvertirATextoAsync(string rutaArchivo, IProgress<string> progresoTexto = null, IProgress<int> progresoPorcentaje = null)
        {
            if (!File.Exists(rutaArchivo)) throw new FileNotFoundException("Archivo no encontrado.");

            return await Task.Run(async () =>
            {
                try
                {
                    progresoTexto?.Report("Conectando con Gemini 1.5 Flash...\r\n");
                    progresoPorcentaje?.Report(10);

                    // 1. Inicializar el cliente
                    var googleAI = new GoogleAI(ApiKey);

                    // CORRECCIÓN 1: Usamos el string directo en lugar del Enum para evitar errores
                    var model = googleAI.GenerativeModel("gemini-2.5-flash");

                    // 2. Preparar audio
                    byte[] audioBytes = File.ReadAllBytes(rutaArchivo);
                    string base64Audio = Convert.ToBase64String(audioBytes);
                    string mimeType = ObtenerMimeType(rutaArchivo);

                    progresoTexto?.Report($"Subiendo audio ({mimeType}) a la IA...\r\n");
                    progresoPorcentaje?.Report(30);

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
                    var response = await model.GenerateContent(request);

                    progresoPorcentaje?.Report(100);

                    if (response.Text != null)
                    {
                        return response.Text;
                    }
                    else
                    {
                        return "Gemini no devolvió texto (posible audio vacío).";
                    }
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
    }
}