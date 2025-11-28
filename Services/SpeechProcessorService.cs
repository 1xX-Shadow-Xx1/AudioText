using AudioText.Interfaces;
using System.Speech.Recognition;


namespace AudioText.Services
{
    /// <summary>
    /// Implementación concreta del contrato IProcesadorAudio.
    /// Utiliza el motor de reconocimiento de voz de Windows (.NET System.Speech).
    /// </summary>
    public class SpeechProcessorService : IProcesadorAudio
    {
        /// <inheritdoc />
        public string ConvertirATexto(string rutaArchivo)
        {
            // Criterio 1.2: Al presionar "Convertir a Texto", debe mostrar dicha conversión.

            // Criterio 1.1: Controlando formatos. System.Speech SOLO acepta WAV.
            if (!File.Exists(rutaArchivo))
            {
                throw new FileNotFoundException("El archivo de audio no fue encontrado.");
            }
            if (Path.GetExtension(rutaArchivo).ToLower() != ".wav")
            {
                // Si la ruta no es .WAV, asumimos que el Helper ya hizo su trabajo
                // o lanzamos una excepción si se intenta usar directamente un formato no soportado.
                // Esta es la validación y control de formato que requiere el criterio 1.1.
                throw new InvalidOperationException(
                    "Error de formato: El motor de reconocimiento de Windows solo procesa archivos WAV. " +
                    "Utilice el conversor interno para preparar el archivo."
                );
            }

            // Inicializa el motor de reconocimiento.
            using (SpeechRecognitionEngine recognizer = new SpeechRecognitionEngine())
            {
                // Establece la entrada de audio al archivo WAV.
                recognizer.SetInputToWaveFile(rutaArchivo);

                // Carga la gramática de dictado (útil para texto libre).
                recognizer.LoadGrammar(new DictationGrammar());

                // Realiza el reconocimiento de forma sincrónica.
                RecognitionResult result = recognizer.Recognize();

                return result?.Text ?? "No se pudo reconocer texto en el audio.";
            }
        }
    }
}

