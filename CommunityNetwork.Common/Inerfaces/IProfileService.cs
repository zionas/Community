using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityNetwork.Common.Inerfaces
{
    public interface IProfileService
    {
        Profile Login(string email, string password);
        bool CheckValidationToken(string email);
        Profile Register(Profile profile);
    }
}
