using Server.Source.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Server.Source.Classes
{
    public class HashAlgorithms
    {
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
