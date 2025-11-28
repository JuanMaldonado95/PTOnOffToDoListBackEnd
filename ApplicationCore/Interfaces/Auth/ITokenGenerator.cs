using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces.Auth
{
    public interface ITokenGenerator
    {
        string CreateJwtSecurityToken(Dictionary<string, string> claimsDictionary);
    }
}
