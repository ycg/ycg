using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Ycg.Crypto
{
    public static class DESCryptoHelper
    {
        /// <summary>
        /// Custom DES key.
        /// The key size must is 8 in the C#.
        /// </summary>
        public static string Key = "00880623529463403";

        /// <summary>
        /// The key is java.
        /// </summary>
        public static byte[] KeyJava = { 65, 66, 43, 61, 33, 38, 36, 35 };

        /// <summary>
        /// Custom init vector.
        /// </summary>
        public static byte[] IVKey = { 31, 12, 65, 67, 4, 121, 0, 125 };

        /// <summary>
        /// Des service instance.
        /// </summary>
        private static DESCryptoServiceProvider _desCryptoService;

        /// <summary>
        /// Use des encrypt.
        /// First use utf8 convert content.
        /// End use base64 generate encrypt content.
        /// </summary>
        /// <param name="content">Text.</param>
        /// <returns>Encrypt content.</returns>
        public static string Encrypt(string content)
        {
            return Encrypt(Encoding.UTF8.GetBytes(Key.Substring(0, 8)), IVKey, content);
        }

        /// <summary>
        /// Use des encrypt for java.
        /// First use utf8 convert content.
        /// End use base64 generate encrypt content.
        /// </summary>
        /// <param name="content">Text.</param>
        /// <returns>Encrypt content.</returns>
        public static string EncryptJava(string content)
        {
            return Encrypt(KeyJava, KeyJava, content);
        }

        /// <summary/>
        /// Use des encrypt.
        /// First use utf8 convert content.
        /// End use base64 generate encrypt content.
        /// <param name="key">DES encrypt key.</param>
        /// <param name="ivKey">DES encrypt iv key.</param>
        /// <param name="content">Content.</param>
        /// <returns>Encrypt content.</returns>
        public static string Encrypt(byte[] key, byte[] ivKey, string content)
        {
            using (_desCryptoService = new DESCryptoServiceProvider { Key = key, IV = ivKey })
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(content);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, _desCryptoService.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(inputBytes, 0, inputBytes.Length);
                        cryptoStream.FlushFinalBlock();
                    }
                    return Convert.ToBase64String(memoryStream.ToArray());
                }
            }
        }

        /// <summary>
        /// Use des decrypt for java.
        /// First use base64 convert content.
        /// End use utf8 generate decrypt content.
        /// </summary>
        /// <param name="content">Content.</param>
        /// <returns>Decrypt content.</returns>
        public static string Decrypt(string content)
        {
            return Decrypt(Encoding.UTF8.GetBytes(Key.Substring(0, 8)), IVKey, content);
        }

        /// <summary>
        /// Use des decrypt.
        /// First use base64 convert content.
        /// End use utf8 generate decrypt content.
        /// </summary>
        /// <param name="content">Content.</param>
        /// <returns>Decrypt content.</returns>
        public static string DecryptJava(string content)
        {
            return Decrypt(KeyJava, KeyJava, content);
        }

        /// <summary>
        /// Use des decrypt.
        /// First use base64 convert content.
        /// End use utf8 generate decrypt content.
        /// </summary>
        /// <param name="key">DES encrypt key.</param>
        /// <param name="ivKey">DES encrypt iv key.</param>
        /// <param name="content">Content.</param>
        /// <returns>Decrypt content.</returns>
        public static string Decrypt(byte[] key, byte[] ivKey, string content)
        {
            using (_desCryptoService = new DESCryptoServiceProvider { Key = key, IV = ivKey })
            {
                byte[] inputBytes = Convert.FromBase64String(content);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, _desCryptoService.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(inputBytes, 0, inputBytes.Length);
                        cryptoStream.FlushFinalBlock();
                    }
                    return Encoding.UTF8.GetString(memoryStream.ToArray());
                }
            }
        }

        /// <summary>
        /// Create des service instance.
        /// </summary>
        /// <param name="key">Custom des key.</param>
        /// <param name="ivKey">Custom des init vector.</param>
        private static void CreateInstance(byte[] key, byte[] ivKey)
        {
            _desCryptoService = new DESCryptoServiceProvider
            {
                Key = key,
                IV = ivKey,
            };
        }
    }
}