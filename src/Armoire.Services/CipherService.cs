using Armoire.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Armoire.Services
{
    public class CipherService : ICipherService
    {
        private static RNGCryptoServiceProvider _randomByteProvider;

        public CipherService()
        {
            _randomByteProvider = new RNGCryptoServiceProvider();
        }

        public string Encrypt<T>(string value, string password, string salt) where T : SymmetricAlgorithm, new()
        {
            DeriveBytes rgb = new Rfc2898DeriveBytes(password, Encoding.Unicode.GetBytes(salt));

            SymmetricAlgorithm algorithm = new T();

            byte[] rgbKey = rgb.GetBytes(algorithm.KeySize >> 3);
            byte[] rgbIv = rgb.GetBytes(algorithm.BlockSize >> 3);

            ICryptoTransform transform = algorithm.CreateEncryptor(rgbKey, rgbIv);

            using (var buffer = new MemoryStream())
            {
                using (var stream = new CryptoStream(buffer, transform, CryptoStreamMode.Write))
                {
                    using (var writer = new StreamWriter(stream, Encoding.Unicode))
                    {
                        writer.Write(value);
                    }
                }

                return Convert.ToBase64String(buffer.ToArray());
            }
        }

        public string Decrypt<T>(string text, string password, string salt) where T : SymmetricAlgorithm, new()
        {
            DeriveBytes rgb = new Rfc2898DeriveBytes(password, Encoding.Unicode.GetBytes(salt));

            SymmetricAlgorithm algorithm = new T();

            byte[] rgbKey = rgb.GetBytes(algorithm.KeySize >> 3);
            byte[] rgbIV = rgb.GetBytes(algorithm.BlockSize >> 3);

            ICryptoTransform transform = algorithm.CreateDecryptor(rgbKey, rgbIV);
            try
            {
                using (var buffer = new MemoryStream(Convert.FromBase64String(text)))
                {
                    using (var stream = new CryptoStream(buffer, transform, CryptoStreamMode.Read))
                    {
                        using (var reader = new StreamReader(stream, Encoding.Unicode))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
            catch
            { // silent failure
                return String.Empty;
            }
        }

        private static byte[] getBytes(int length)
        {
            var a = new byte[length];
            _randomByteProvider.GetBytes(a);
            return a;
        }

        public string GenerateSalt(int saltLength = 32)
        {
            if (saltLength < 16) return String.Empty;
            return BytesToHex(
                getBytes(saltLength));
        }

        private static string BytesToHex(ICollection<byte> toConvert)
        {
            var builder = new StringBuilder(toConvert.Count * 2);
            foreach (byte b in toConvert)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }

        public string ComputeSHA256Hash(string input, string salt)
        {
            SHA256CryptoServiceProvider md5 = new SHA256CryptoServiceProvider();
            byte[] data = md5.ComputeHash(Encoding.Default.GetBytes(input + salt));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        public bool SHA256HashMatches(string input, string salt, string hash)
        {
            return StringComparer.OrdinalIgnoreCase.Compare(ComputeSHA256Hash(input, salt), hash) == 0;
        }
    }
}
