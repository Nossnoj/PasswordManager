using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.ComponentModel.DataAnnotations;

namespace PasswordManager
{
    internal class AesClass
    {
        private byte[] vaultKey;

        public byte[] IV { get; set; }

        public byte[] encryptedVault;

        public string decryptedVault;
        
        
        public AesClass(string unencryptedVault, byte[] vaultKey)
        {
            this.vaultKey = vaultKey;
            AESEncrypt(unencryptedVault);
            //AESDecrypt(encryptedVault, vaultKey, IV);      ---- inte automatiskt anrop till decrypt efter encrypt
        }
        public AesClass(byte[] encryptedVault, byte[] vaultKey, byte[] IV)
        {
            this.vaultKey = vaultKey;
            this.IV = IV;
            AESDecrypt(encryptedVault, vaultKey, IV);
        }
        
        //eventuellt att skapa ett AES-objekt i en egen metod.

        private byte[] AESEncrypt(string unencryptedVault)
        {
            byte[] encrypted;
            
            using(Aes aes = Aes.Create())
            {
                aes.Key = vaultKey;
                aes.GenerateIV();
                IV = aes.IV;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(unencryptedVault);
                        }
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }
            encryptedVault = encrypted;
            return encrypted;
        }

        public string AESDecrypt(byte[] cipherText, byte[] Key, byte[] IV) //public static
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
                            try
                            {
                                plaintext = srDecrypt.ReadToEnd();
                            }
                            catch(Exception e)
                            {
                                Console.WriteLine("Fel lösenord");
                                Console.ReadLine();
                            }
                            
                        }
                    }
                }
            }
            decryptedVault = plaintext;
            return plaintext;
        }
    }
}
