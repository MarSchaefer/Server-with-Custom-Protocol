using System.Security.Cryptography;
using System.Text;

namespace Server.Source.Classes
{
    /// <summary>
    /// Basiert auf challenge-response
    /// </summary>
    public class Verschluesselung
    {
        // salt pro server neu rollen (statisch bei einem server)
        private Random _random = new Random();

        public Verschluesselung() {
        
        }

        public static string StringToSha512(string txt)
        {
            if (txt == String.Empty)
                throw new Exception("StringToSha512: String war leer");

            byte[] hash;

            using (SHA512 shaM = SHA512.Create())
            {
                hash = shaM.ComputeHash(Encoding.Default.GetBytes(txt));
            }

            txt = "";

            foreach (byte b in hash)
                txt += b.ToString("X");

            return txt;
        }
    }
}

/*
 * class Program
    {

        static string klartextpasswort = "Hallooo", gehashtespasswort, Salt = "1234";
        static string gehashtespasswortMitSalt;
        static Random r;

        static void Main(string[] args)
        {

            gehashtespasswort = StringToSha512(klartextpasswort);//Server u Client haben passwort lokal gespeichert
            gehashtespasswortMitSalt = StringToSha512(klartextpasswort + Salt); // Server hat das passwort mit salt gespeichert

            r = new Random();
            string challange = r.Next().ToString();//Server baut challange (zb random nummer )

            //Server schickt challange, salt an den client
            // Der Client nimmt sein passwort + salt, hasht es, dieser hash wird wiederum mit der challange nochmal verhasht und schickt es dem server
            string challangestringvomClient = Client(challange, Salt);

            //Der server macht das selbe und vergleicht die strings

            string serverChallangeHashSelbst = StringToSha512(StringToSha512(klartextpasswort + Salt) + challange);

            Console.WriteLine("Server hash: " + serverChallangeHashSelbst + "\nClient hash: " + challangestringvomClient + "\n");


            if (serverChallangeHashSelbst == challangestringvomClient)
            {
                Console.WriteLine("Authentifikation erfolgreich!");
            }


        }

        static string Client(string challange, string salt) => StringToSha512(StringToSha512(klartextpasswort + salt) + challange);

        static string StringToSha512(string txt)
        {
            if (txt == String.Empty)          
                throw new Exception("StringToSha512: String war leer");                        

            byte[] hash;

            using (SHA512 shaM = new SHA512Managed())
            {
               hash = shaM.ComputeHash(Encoding.Default.GetBytes(txt));
            }

            txt = "";

            foreach (byte b in hash)           
                txt += b.ToString("X");
            
            return txt;
        }
    }
*/
