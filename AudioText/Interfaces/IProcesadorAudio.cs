namespace AudioText.Interfaces
{
    /// <summary>
    /// Define el contrato para servicios de transcripción de audio a texto (Speech-to-Text).
    /// Permite desacoplar la lógica de la aplicación del motor de reconocimiento específico (Whisper, Google, Azure, etc.).
    /// </summary>
    public interface IProcesadorAudio
    {
        /// <summary>
        /// Realiza la transcripción asíncrona de un archivo de audio a texto.
        /// </summary>
        /// <param name="rutaArchivo">Ruta absoluta del archivo de audio a transcribir.</param>
        /// <param name="progresoTexto">Interfaz para reportar actualizaciones parciales del texto transcrito (streaming de texto).</param>
        /// <param name="progresoPorcentaje">Interfaz para reportar el porcentaje estimado de completitud (0-100%).</param>
        /// <returns>Una tarea que contiene el texto completo transcrito al finalizar.</returns>
        Task<string> ConvertirATextoAsync(string rutaArchivo, IProgress<string> progresoTexto, IProgress<int> progresoPorcentaje);
    }
}
