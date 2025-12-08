using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioText.Interfaces
{
    /// <summary>
    /// Define el contrato para servicios de encriptación y desencriptación de texto.
    /// Permite la implementación de diferentes algoritmos (AES, RSA, etc.) manteniendo una interfaz común.
    /// </summary>
    public interface IEncriptador
    {
        /// <summary>
        /// Encripta una cadena de texto plano utilizando el algoritmo configurado.
        /// </summary>
        /// <param name="textoPlano">El texto original que se desea proteger.</param>
        /// <returns>El texto cifrado resultante.</returns>
        string Encriptar(string textoPlano);

        /// <summary>
        /// Desencripta una cadena de texto cifrado para recuperar el contenido original.
        /// </summary>
        /// <param name="textoEncriptado">El texto cifrado que se desea procesar.</param>
        /// <returns>El texto plano original recuperado.</returns>
        string Desencriptar(string textoEncriptado);
    }
}
