using ApplicationCore.Entities.PTOnOff.Auth;
using ApplicationCore.Interfaces.Auth;
using ApplicationCore.Models.Auth;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WebApiPTBackOnOff.Shared.Utils;

namespace Infrastructure.Services.Auth
{
    public class UserService : IUserService
    {
        private readonly DbContextPTOnOff _context;

        public UserService(DbContextPTOnOff context)
        {
            _context = context;
        }

        public async Task<(bool isValid, tblAuth user)> ValidateUserCredentialsAsync(mLogin login)
        {
            try
            {
                if (string.IsNullOrEmpty(login.tUserName) || string.IsNullOrEmpty(login.tPassword))
                {
                    return (false, null);
                }

                var user = await _context.tblAuth.FirstOrDefaultAsync(u => u.tUserName == login.tUserName && u.tPasswordHash == login.tPassword);

                if (user != null)
                {
                    return (true, user);
                }

                return (false, null);
            }
            catch (Exception ex)
            {
                await GenericUtils.Log("UserService: Error en el servicio ValidateUserCredentialsAsync", ex);
                throw;
            }
        }

        public async Task<string> CreateMD5(string input)
        {
            try
            {
                using MD5 md5 = MD5.Create();
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                await GenericUtils.Log("AccountController: Error en el metodo CreateMD5", ex);
                throw;
            }
        }
    }
}
