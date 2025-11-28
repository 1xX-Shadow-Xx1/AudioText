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
    /// Implementación concreta del contrato IEncriptador utilizando el algoritmo AES.
    /// (Cumple SRP: Solo se encarga de Encriptación/Desencriptación).
    /// </summary>
    public class AesTextEncryptor : IEncriptador
    {
        // Clave secreta (256 bits/32 bytes). NOTA: En producción, no debe estar aquí.
        private readonly byte[] _claveBytes = Encoding.UTF8.GetBytes("ClaveSecretaParaElProyectoAudioT");

        /// <inheritdoc />
        public string Encriptar(string textoPlano)
        {
            // Criterio 1.3: Al presionar "Encriptar Texto" deberá convertir...
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = _claveBytes;
                // Generar un IV único es crucial. Se incluirá en el resultado encriptado.
                aesAlg.GenerateIV();
                byte[] iv = aesAlg.IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    // Escribimos el IV al inicio del stream para desencriptar.
                    msEncrypt.Write(iv, 0, iv.Length);

                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(textoPlano);
                        }
                    }
                    // Retorna IV + Texto Encriptado en Base64.
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        /// <inheritdoc />
        public string Desencriptar(string textoEncriptado)
        {
            // Criterio 1.3 Opcional: Implementación de desencriptación (reversible).
            byte[] textoCompletoBytes = Convert.FromBase64String(textoEncriptado);
            const int IvLongitud = 16;

            if (textoCompletoBytes.Length < IvLongitud)
            {
                throw new CryptographicException("El formato del texto encriptado es inválido.");
            }

            // Extrae el IV de los primeros 16 bytes.
            byte[] iv = new byte[IvLongitud];
            Array.Copy(textoCompletoBytes, 0, iv, 0, IvLongitud);

            // Extrae el ciphertext real.
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
