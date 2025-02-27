using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager
{
    internal class temp
    {
        //Ska kanske göra om ovan till funktioner för att undvika upprepning

        /*string test = "Test mening som ska enkryptas";

        using(Aes myAes = Aes.Create())
        {
            byte[] encrypted = EncryptStringToBytes_Aes(test, myAes.Key, myAes.IV);
            Console.WriteLine(test);
            Console.WriteLine(string.Join(", ", encrypted));
        }

        */


    }
    /*static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
    {

        if (plainText == null || plainText.Length <= 0)
            throw new ArgumentNullException("plainText");
        if (Key == null || Key.Length <= 0)
            throw new ArgumentNullException("Key");
        if (IV == null || IV.Length <= 0)
            throw new ArgumentNullException("IV");
        byte[] encrypted;

        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Key;
            aesAlg.IV = IV;

            // Create an encryptor to perform the stream transform.
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            // Create the streams used for encryption.
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        //Write all data to the stream.
                        swEncrypt.Write(plainText);
                    }
                }

                encrypted = msEncrypt.ToArray();
            }
        }

        // Return the encrypted bytes from the memory stream.
        return encrypted;
    }*/
}

