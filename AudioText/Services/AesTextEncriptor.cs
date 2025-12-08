using AudioText.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AudioText.Services
{
    /// <summary>
    /// Implementación concreta del servicio de encriptación utilizando el algoritmo estándar AES (Advanced Encryption Standard).
    /// Esta clase encapsula toda la lógica criptográfica, cumpliendo con el Principio de Responsabilidad Única (SRP).
    /// </summary>
    public class AesTextEncryptor : IEncriptador
    {
        // Clave secreta estática para el proyecto.
        // ADVERTENCIA DE SEGURIDAD: En un entorno de producción real, esta clave NUNCA debe estar hardcodeada en el código fuente.
        // Debería obtenerse de un gestor de secretos seguro (Azure Key Vault, AWS Secrets Manager, variables de entorno, etc.).
        private readonly byte[] _claveBytes = Encoding.UTF8.GetBytes("ClaveSecretaParaElProyectoAudioT");

        /// <inheritdoc />
        /// <summary>
        /// Encripta el texto plano utilizando AES con un Vector de Inicialización (IV) aleatorio para cada operación.
        /// </summary>
        public string Encriptar(string textoPlano)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = _claveBytes;
                // Generamos un IV único para cada encriptación. Esto asegura que el mismo texto plano produzca diferentes textos cifrados,
                // protegiendo contra ataques de análisis de patrones.
                aesAlg.GenerateIV();
                byte[] iv = aesAlg.IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    // Prependemos el IV al inicio del stream cifrado. Es necesario para poder desencriptar después.
                    // El IV no es secreto, pero debe ser único.
                    msEncrypt.Write(iv, 0, iv.Length);

                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(textoPlano);
                        }
                    }
                    // Retornamos la combinación de IV + Texto Cifrado codificada en Base64 para facilitar su almacenamiento y transporte.
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Desencripta el texto cifrado recuperando primero el IV y luego aplicando la transformación inversa.
        /// </summary>
        public string Desencriptar(string textoEncriptado)
        {
            byte[] textoCompletoBytes = Convert.FromBase64String(textoEncriptado);
            const int IvLongitud = 16; // El tamaño del bloque AES es de 128 bits (16 bytes).

            if (textoCompletoBytes.Length < IvLongitud)
            {
                throw new CryptographicException("El formato del texto encriptado es inválido o está corrupto.");
            }

            // 1. Extraemos el IV de los primeros 16 bytes.
            byte[] iv = new byte[IvLongitud];
            Array.Copy(textoCompletoBytes, 0, iv, 0, IvLongitud);

            // 2. Extraemos el contenido cifrado real (Ciphertext).
            int cipherLongitud = textoCompletoBytes.Length - IvLongitud;
            byte[] cipherBytes = new byte[cipherLongitud];
            Array.Copy(textoCompletoBytes, IvLongitud, cipherBytes, 0, cipherLongitud);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = _claveBytes;
                aesAlg.IV = iv;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
