namespace Server.Source.DataModel
{
    public class RegisteredUser
    {
        /// <summary>
        /// UserName muss einmalig sein (unique)
        /// </summary>
        public string UserName;

        /// <summary>
        /// Um das Passwort zu erzeugen, muss der Klartext mit dem Salt vom Server 
        /// konkatiniert und anschließend alles gehasht werden
        /// Bsp.: StringToSha512(klartextpasswort + _salt)
        /// </summary>
        public string Password;

        public RegisteredUser(string userName, string password)
        {
            this.UserName = userName;
            this.Password = password;
        }
    }
}
