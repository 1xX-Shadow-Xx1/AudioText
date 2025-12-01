using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioText.Interfaces
{
    /// <summary>
    /// Define el contrato para cualquier servicio de encriptación reversible de texto.
    /// Permite cambiar el algoritmo de encriptación (ej: de AES a RSA) sin afectar
    /// el código que lo consume (Cumple OCP).
    /// </summary>
    public interface IEncriptador
    {
        /// <summary>
        /// Convierte una cadena de texto plano en una cadena de texto encriptada.
        /// </summary>
        /// <param name="textoPlano">El texto original a encriptar.</param>
        /// <returns>El texto encriptado.</returns>
        string Encriptar(string textoPlano);

        /// <summary>
        /// Desencripta una cadena de texto encriptada a su formato original (reversible).
        /// (Cumple con el Criterio 1.3 - Opcional)
        /// </summary>
        /// <param name="textoEncriptado">El texto encriptado a desencriptar.</param>
        /// <returns>El texto plano original.</returns>
        string Desencriptar(string textoEncriptado);
    }
}
