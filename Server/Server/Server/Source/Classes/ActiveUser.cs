using Server.Source.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Source.Classes
{
    public class ActiveUser
    {
        public ActiveUser()
        {
            
        }

        public bool IsUserRegistered(
            in IReadOnlyDictionary<string, RegisteredUser> registeredUsers,
            RegisteredUser userRequestData)
        {
            if (registeredUsers.TryGetValue(userRequestData.UserName, out var registeredUser))
            {
                return true;
            }

            return false;
        }
    }
}