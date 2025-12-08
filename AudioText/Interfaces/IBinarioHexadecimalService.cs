

namespace AudioText.Interfaces
{
    /// <summary>
    /// Define el contrato para servicios que realizan conversiones de archivos a formatos de representación de datos (Binario, Hexadecimal, etc.).
    /// </summary>
    public interface IBinarioHexadecimalService
    {
        /// <summary>
        /// Procesa un archivo y retorna su contenido convertido al formato específico de la implementación (ej. Binario o Hexadecimal).
        /// </summary>
        /// <param name="rutaArchivo">Ruta absoluta del archivo a convertir.</param>
        /// <param name="progreso">Interfaz para reportar el avance de la operación (0-100%).</param>
        /// <returns>Una tarea que representa la operación asíncrona, conteniendo el string con los datos convertidos.</returns>
        Task<string> ObtenerConvercionDelArchivoAsync(string rutaArchivo, IProgress<int> progreso);
    }
}
