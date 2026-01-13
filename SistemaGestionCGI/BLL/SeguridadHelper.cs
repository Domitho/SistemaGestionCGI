using System;
using System.Security.Cryptography;
using System.Text;

namespace SistemaGestionCGI.BLL // O tu namespace preferido
{
    public static class SeguridadHelper
    {
        // Método para encriptar una cadena con SHA-256
        public static string EncriptarSHA256(string texto)
        {
            if (string.IsNullOrEmpty(texto)) return string.Empty;

            using (SHA256 sha256 = SHA256.Create())
            {
                // Convertir la cadena a bytes
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(texto));

                // Convertir los bytes a string hexadecimal
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}