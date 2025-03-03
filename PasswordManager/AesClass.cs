using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace PasswordManager
{
    internal class AesClass
    {
        private byte[] vaultKey { get; set; }
        
        public AesClass(byte[] vaultKey)
        {
            this.vaultKey = vaultKey;
            Start();
        }
        
        public void Start()
        {
            string dataIn = "Nånting nånting";

            using(Aes myAes = Aes.Create())
            {
                byte[] encrypted = AESEncrypt(dataIn, vaultKey, myAes.IV);

                string roundtrip = AESDecrypt(encrypted, myAes.Key, myAes.IV);
            }        
        }

        static byte[] AESEncrypt(string dataIn, byte[] key, byte[] IV)
        {
            byte[] encrypted;
            
            using(Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key; //vill ha 32 bytes men får just nu 48 bytes
                aesAlg.IV = IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(dataIn);
                        }
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }
            return encrypted;
        }

        static string AESDecrypt(byte[] cipherText, byte[] Key, byte[] IV)
        {
            string plaintext = null;

            using(Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using(MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using(CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using(StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return plaintext;
        }
    }
}
