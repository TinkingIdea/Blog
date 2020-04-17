using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Blog.Core.Helpers
{
    public class Encrypt
    {
        private static readonly string Key = "ddvlhrblog";

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetMD5(string input)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                string hash = sBuilder.ToString();
                return hash.ToUpper();
            }
        }

        #region DES加密解密
        /// <summary>
        /// Des加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string DesEncrypt(string input)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(Key.Substring(0, 8));
            byte[] keyIV = keyBytes;
            var des = new DESCryptoServiceProvider();
            var inputByteArray = Encoding.UTF8.GetBytes(input);
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, des.CreateEncryptor(keyBytes, keyIV), CryptoStreamMode.Write))
                {
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        /// <summary>
        /// Des解密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string DesDecrypt(string input)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(Key.Substring(0, 8));
            byte[] keyIV = keyBytes;
            var des = new DESCryptoServiceProvider();
            var inputByteArray = Convert.FromBase64String(input);
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, des.CreateDecryptor(keyBytes, keyIV), CryptoStreamMode.Write))
                {
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
        }
        #endregion
    }
}
