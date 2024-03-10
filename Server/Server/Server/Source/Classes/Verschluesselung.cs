using Server.Source.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Server.Source.Classes
{
    /// <summary>
    /// Basiert auf challenge-response
    /// </summary>
    public class Verschluesselung
    {
        // salt pro server neu rollen (statisch bei einem server)
        private string _salt = "1234"; // hash(klartext + salt) => das was auf server gespeichert ist

        private Random _random = new Random();

        public Verschluesselung() {
        
        }

        private string makeChallenge(RegisteredUser registeredUserData)
        {
            //StringToSha512(klartextpasswort + _salt); // Server hat das passwort mit salt gespeichert
            string gehashtespasswortMitSalt = registeredUserData.Password; 

            //Server baut challange (zb random nummer )
            _random = new Random();
            string challenge = _random.Next().ToString();

            string challengeSolution = StringToSha512(gehashtespasswortMitSalt + challenge);
            return challengeSolution;
        }

        public static string StringToSha512(string txt)
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
