using Microsoft.VisualBasic;
using System.Dynamic;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml;

namespace PasswordManager
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            //Möjlig validering för att kontrollera att filerna faktikst döps till client och server





            Console.WriteLine("Använd följande kommandon:");
            Console.WriteLine("    init             <client> <server> {<pwd>}                             - Skapa en ny klient- och serverfil");
            Console.WriteLine("    create           <client> <server> {<pwd>} {<secret>}                  - Skapa en ny klient för en existerande server");
            Console.WriteLine("    get              <client> <server> [<prop>] {<pwd>}                    - Hämta ut lösenord");
            Console.WriteLine("    set              <client> <server> <prop> [-g] {<pwd>} {<value>}       - Spara ett lösenord");
            Console.WriteLine("    delete           <client> <server> <prop> {<pwd>}                      - Ta bort ett lösenord");
            Console.WriteLine("    secret           <client>                                              - Visa Secret Key");
            Console.WriteLine("    change           <client> <server> {<pwd>} {<new_pwd>}                 - Ändra huvudlösenord");

            /*
            string command = args[0];


            switch (command.ToLower())
            {
                case "init":
                    init(args[1], args[2], args[3]);
                    break;
                case "create": 
                    Create(args[1], args[2]);
                    break;
                case "get":
                    Get(args[1], args[2], args[3], args[4]);
                    break;
                case "set":
                    //Set();
                    break;
                case "set -g":
                    //Set();
                    break;
                case "delete":
                    //Delete();
                    break;
                case "secret":
                    //Secret();
                    break;
                case "change":
                    //Change();
                    break;
            }*/

            init("client.json", "server.json", "password");
            Create("client2.json", "server.json");

            Console.WriteLine();



            //skapat en getPath metod som automatiskt ger oss en färdig filväg där filerna ska lagras - just nu till desktop
            //ändrat lite med args i main-metoden och korrigerat switch satsen
            //liten ändring på create metoden med en conditional
            //git fuckar


        }

        static string getPath(string file)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fullPath = Path.Combine(desktopPath, file);

            return fullPath;
        }

        static void init(string clientName, string serverName, string mstrpwd)
        {


            //CLIENT & SECRET KEY
            string clientPath = getPath(clientName);
            byte[] secretKeyByteArray = GenerateByteArray(16);
            string secretKey = Convert.ToBase64String(secretKeyByteArray);
            Dictionary<string, string> secretDict = new Dictionary<string, string>();
            secretDict["secret"] = secretKey;
            string dictJson = JsonSerializer.Serialize(secretDict);
            File.WriteAllText(clientPath, dictJson); // HÄR SPARAS SECRET KEY INUTI CLIENT FILE
                                                     // Your secret key will be printed in plain-text to standard out" - spotta ut secret key i konsol?

            //SERVER
            Dictionary<string, string> server = new Dictionary<string, string>();
            string serverPath = getPath(serverName);


            //VAULT KEY
            byte[] vaultKey = makeVaultKey(mstrpwd, secretKeyByteArray);



            //VAULT
            Dictionary<string, string> vault = new Dictionary<string, string>();
            vault = Set(vault, "GOOGLE", "password");
            vault = Set(vault, "FACEBOOK", "password2");
            vault = Set(vault, "INSTAGRAM", "password3");
            vault = Set(vault, "YOUTUBE", "password4");
            vault = Set(vault, "NETFLIX", "password5");
            vault = Set(vault, "nus@sarling.se", "nus123");
            vault = Set(vault, "OKTAV", "password7");
            string jsonVault = JsonSerializer.Serialize(vault);

            // AES, KRYPTERING OCH LAGRING AV VAULT OCH IV I SERVER FIL
            AesClass aes = new AesClass(jsonVault, vaultKey); // just nu både encryptar vi och decryptar
            string IV = Convert.ToBase64String(aes.IV);
            string encryptedVault = Convert.ToBase64String(aes.encryptedVault);
            server["vault"] = encryptedVault;
            server["iv"] = IV;
            string serverJson = JsonSerializer.Serialize(server);
            File.WriteAllText(serverPath, serverJson);

        }






        static byte[] GenerateByteArray(int size)
        {
            byte[] byteArray = new byte[size];

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(byteArray);
            }

            return byteArray;

            //OVAN TAGET FRÅN https://learn.microsoft.com/mt-mt/dotnet/api/system.security.cryptography.randomnumbergenerator.getbytes?view=netcore-2.2
        }






        static byte[] makeVaultKey(string mstrpwd, byte[] scrtkey)
        {
            Rfc2898DeriveBytes vk = new Rfc2898DeriveBytes(mstrpwd, scrtkey, 1000, HashAlgorithmName.SHA256); //SHA256 eller annan variant?
            {
                return vk.GetBytes(32);
            }
        }




        static void Create(string clientPath, string serverPath) //användaren skriver in master pwd och secret key och sedan anropas attemptdecrypt
        {
            string server = getPath(serverPath);
            if (File.Exists(server))
            {
                string newClient = getPath(clientPath);

                Console.Write("Ange din Secret Key: ");
                string secretKey = Console.ReadLine();
                Dictionary<string, string> secretDict = new Dictionary<string, string>();
                secretDict["secret"] = secretKey;
                string dictJson = JsonSerializer.Serialize(secretDict);

                Console.Write("Ange din Master Password: ");
                string mstrpwd = Console.ReadLine();


                byte[] scrtKeyByteArray = Convert.FromBase64String(secretKey);
                byte[] vk = makeVaultKey(mstrpwd, scrtKeyByteArray);

                string serverBeforeDeserialize = File.ReadAllText(server);
                Dictionary<string, string> serverDict = JsonSerializer.Deserialize<Dictionary<string, string>>(serverBeforeDeserialize);

                var serverList = deserializeServer(server);
                var IV = serverList[0];
                var vault = serverList[1];

                AesClass aes = new AesClass(vault, vk, IV); // Detta funkar, men hur kan vi bevisa att den inte skickar ut skit?
                File.WriteAllText(newClient, dictJson);
            }
            else
            {
                Console.WriteLine("Här ska något error vara"); /////////////////////////////////////////////////////////////
            }
        }




        //används för alla som decryptar vault utom create
        static string attemptDecryptVault(string mstrpwd, string clientName, string serverName)
        {
            string clientPath = getPath(clientName);
            string serverPath = getPath(serverName);
            string serializedClient = File.ReadAllText(clientPath);
            Dictionary<string, string> secretDictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(serializedClient);
            string scrtKey = secretDictionary["secret"];
            byte[] scrtKeyByteArray = Convert.FromBase64String(scrtKey);
            string serializedServer = File.ReadAllText(serverPath);
            Dictionary<string, string> serverDictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(serializedServer);


            var serverList = deserializeServer(serverPath);
            var IV = serverList[0];
            var vault = serverList[1];

            string fakePass = "linussarling";

            byte[] vk = makeVaultKey(mstrpwd, scrtKeyByteArray);
            
            AesClass aes = new AesClass(vault, vk, IV);
            string decryptedVault = aes.decryptedVault;
            return decryptedVault;

        }




        static List<byte[]> deserializeServer(string server)
        {
            var serverList = new List<byte[]>();
            
            
            string serverBeforeDeserialize = File.ReadAllText(server);
            Dictionary<string, string> serverDict = JsonSerializer.Deserialize<Dictionary<string, string>>(serverBeforeDeserialize);
            string IV = serverDict["iv"];
            byte[] IVByte = Convert.FromBase64String(IV);
            string vault = serverDict["vault"];
            byte[] vaultByte = Convert.FromBase64String(vault);

            serverList.Add(IVByte);
            serverList.Add(vaultByte);

            return serverList;
        }






        static string Get(string clientPath, string serverPath, string prop, string mstrpwd)
        {
            string decryptedVault = attemptDecryptVault(mstrpwd, clientPath, serverPath);
            var deserializeVault = JsonSerializer.Deserialize<Dictionary<string, string>>(decryptedVault);
            //Om vi inte anger en prop, då ska Get lista ut alla props MEN inte deras tillhörande lösenord   
            if (String.IsNullOrEmpty(prop))
            {
                foreach (var kvp in deserializeVault)
                {
                    Console.WriteLine(kvp.Key);
                }
                Console.ReadLine();
            }


            string retrievedPwd = deserializeVault[prop];


            return retrievedPwd;      
        }          



        static Dictionary<string, string> Set(Dictionary<string, string> vault, string applikation, string pswrd)
        {
            vault[applikation] = pswrd;
            return vault;
        }


        /* 
        Lista med frågetecken:
         
         
         
         
         
          

         
         
         */
    }
}
