using ApplicationCore.Interfaces.Auth;
using ApplicationCore.Models.Auth;
using Infrastructure.Services.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using WebApiPTBackOnOff.Shared.Utils;

namespace WebApiPTBackOnOff.Controllers.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IUserService _authService;
        private readonly ITokenGenerator _tokenGenerator;

        public AuthController(IUserService authService, ITokenGenerator tokenGenerator)
        {
            _authService = authService;
            _tokenGenerator = tokenGenerator;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] mLogin login)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Credenciales inválidas.");
                }

                login.tPassword = await _authService.CreateMD5(login.tPassword);

                var (isValid, user) = await _authService.ValidateUserCredentialsAsync(login);

                if (isValid)
                {
                    var claimsDictionary = new Dictionary<string, string>
                    {
                        { "id", user.iIDUser.ToString() },
                        { "user", user.tUserName }
                    };

                    string token = _tokenGenerator.CreateJwtSecurityToken(claimsDictionary);

                    return Ok(new mLoginResponse
                    {
                        Token = token,
                    });
                }

                return Unauthorized("Usuario o contraseña incorrectos.");
            }
            catch (Exception ex)
            {
                await GenericUtils.Log("AuthController: Error en el servicio Login", ex);
                throw;
            }
        }
    }
}
