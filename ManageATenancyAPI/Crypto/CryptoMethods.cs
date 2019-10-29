
using Hackney.InterfaceStubs;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using ManageATenancyAPI.Configuration;
using Microsoft.Extensions.Options;

namespace Hackney.Plugin.Crypto
{
    public class CryptoMethods : ICryptoMethods, IService
    {

        public CryptoMethods(IOptions<AppConfiguration> appConfigurations)
        {
            if (string.IsNullOrEmpty(appConfigurations.Value.EncryptionKey))
            {

                throw new EmptyEncryptionKeyException();
            }
            EncryptionKey = appConfigurations.Value.EncryptionKey;
        }
        private static readonly byte[] Salt = Encoding.ASCII.GetBytes("58LKJ34234djfkl1");
        private const int EncryptionAlgorithmKeysize = 256;
        private string _encryptionKey;

        public string ServiceName
        {
            get
            {
                return "Hackney.Plugin.Crypto.CryptoMethods";
            }
        }

        public string EncryptionKey
        {
            set
            {
                this._encryptionKey = value;
            }
        }

        public string Encrypt(string textToEncrypt)
        {
            if (string.IsNullOrEmpty(this._encryptionKey))
                throw new EmptyEncryptionKeyException();
            RijndaelManaged rijndaelManaged = new RijndaelManaged();
            rijndaelManaged.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = rijndaelManaged.CreateEncryptor(new Rfc2898DeriveBytes(this._encryptionKey, CryptoMethods.Salt).GetBytes(32), CryptoMethods.Salt);
            using (MemoryStream memoryStream = new MemoryStream())
            {
                byte[] array;
                using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(textToEncrypt);
                    cryptoStream.Write(bytes, 0, bytes.Length);
                    cryptoStream.FlushFinalBlock();
                    array = memoryStream.ToArray();
                    cryptoStream.Close();
                }
                memoryStream.Close();
                return Convert.ToBase64String(array);
            }
        }

        public string Decrypt(string textToDecrypt)
        {
            if (string.IsNullOrEmpty(this._encryptionKey))
                throw new EmptyEncryptionKeyException();
            byte[] bytes = new Rfc2898DeriveBytes(this._encryptionKey, CryptoMethods.Salt).GetBytes(32);
            RijndaelManaged rijndaelManaged = new RijndaelManaged();
            rijndaelManaged.Mode = CipherMode.CBC;
            ICryptoTransform decryptor = rijndaelManaged.CreateDecryptor(bytes, CryptoMethods.Salt);
            byte[] buffer = Convert.FromBase64String(textToDecrypt);
            using (MemoryStream memoryStream = new MemoryStream(buffer))
            {
                byte[] numArray;
                int count;
                using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                {
                    numArray = new byte[buffer.Length];
                    count = cryptoStream.Read(numArray, 0, numArray.Length);
                    cryptoStream.Close();
                }
                memoryStream.Close();
                return Encoding.UTF8.GetString(numArray, 0, count);
            }
        }
    }
}

