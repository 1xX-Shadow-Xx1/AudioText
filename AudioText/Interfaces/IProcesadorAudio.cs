using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioText.Interfaces
{
    /// <summary>
    /// Define el contrato para cualquier servicio que convierta un archivo de audio a texto.
    /// Esto permite cambiar el motor de reconocimiento (ej: a Google Cloud) sin modificar
    /// el código del formulario (Cumple DIP).
    /// </summary>
    public interface IProcesadorAudio
    {
        /// <summary>
        /// Convierte el contenido de un archivo de audio a una cadena de texto.
        /// </summary>
        /// <param name="rutaArchivo">La ruta completa al archivo de audio.</param>
        /// <returns>La cadena de texto transcrita.</returns>
        Task<string> ConvertirATextoAsync(string rutaArchivo, IProgress<string> progresoTexto = null, IProgress<int> progresoPorcentaje = null);
    }
}
