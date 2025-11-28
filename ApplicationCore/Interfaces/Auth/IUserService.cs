using ApplicationCore.Entities.PTOnOff.Auth;
using ApplicationCore.Models.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces.Auth
{
    public interface IUserService
    {
        Task<(bool isValid, tblAuth user)> ValidateUserCredentialsAsync(mLogin login);
        Task<string> CreateMD5(string input);
    }
}
