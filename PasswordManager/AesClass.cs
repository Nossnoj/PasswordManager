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

        public byte[] IV;

        public byte[] encryptedVault;

        private string Vault { get; set; }
        
        public AesClass(string Vault, byte[] vaultKey)
        {
            this.Vault = Vault;
            this.vaultKey = vaultKey;
            Start();
        }
        
        public void Start()
        {
            using(Aes myAes = Aes.Create())
            {
                IV = myAes.IV;
                byte[] encrypted = AESEncrypt(Vault, vaultKey, IV);
                encryptedVault = encrypted;

                string roundtrip = AESDecrypt(encrypted, vaultKey, IV);

                
            }        
        }

        static byte[] AESEncrypt(string Vault, byte[] key, byte[] IV)
        {
            byte[] encrypted;
            
            using(Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key; //vad är syftet med denna raden?
                aesAlg.IV = IV; //denna med?

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(Vault);
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
