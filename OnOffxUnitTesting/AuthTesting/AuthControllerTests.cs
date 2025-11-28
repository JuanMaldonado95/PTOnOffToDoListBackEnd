using ApplicationCore.Entities.PTOnOff;
using ApplicationCore.Entities.PTOnOff.Auth;
using ApplicationCore.Interfaces.Auth;
using ApplicationCore.Models.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiPTBackOnOff.Controllers.Auth;
using Xunit;

namespace OnOffxUnitTesting.AuthTesting
{
    public class AuthControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<ITokenGenerator> _mockTokenGenerator;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _mockTokenGenerator = new Mock<ITokenGenerator>();

            _controller = new AuthController(_mockUserService.Object, _mockTokenGenerator.Object);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [Fact(DisplayName = "Login_DebeRetornarOk_YTokenEnCasoDeExito")]
        public async Task Login_DebeRetornarOk_YTokenEnCasoDeExito()
        {
            var inputLogin = new mLogin { tUserName = "testuser", tPassword = "rawpassword" };
            const string expectedHashedPassword = "HASHED_999";
            var validUser = new tblAuth { iIDUser = 101, tUserName = "testuser" };
            const string expectedToken = "TOKEN_TEST";

            _mockUserService.Setup(s => s.CreateMD5(inputLogin.tPassword))
                .ReturnsAsync(expectedHashedPassword);

            _mockUserService.Setup(s => s.ValidateUserCredentialsAsync(
                It.Is<mLogin>(l => l.tPassword == expectedHashedPassword)))
                .ReturnsAsync((true, validUser));

            _mockTokenGenerator.Setup(g => g.CreateJwtSecurityToken(It.IsAny<Dictionary<string, string>>()))
                .Returns(expectedToken);

            var result = await _controller.Login(inputLogin);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<mLoginResponse>(okResult.Value);

            Assert.Equal(expectedToken, response.Token);
        }

        [Fact(DisplayName = "Login_DebeRetornarUnauthorized_SiCredencialesNoCoinciden")]
        public async Task Login_DebeRetornarUnauthorized_SiCredencialesNoCoinciden()
        {
            var inputLogin = new mLogin { tUserName = "testuser", tPassword = "wrong" };

            _mockUserService.Setup(s => s.CreateMD5(It.IsAny<string>()))
                .ReturnsAsync("HASHED");

            _mockUserService.Setup(s => s.ValidateUserCredentialsAsync(It.IsAny<mLogin>()))
                .ReturnsAsync((false, null));

            var result = await _controller.Login(inputLogin);

            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Usuario o contraseña incorrectos.", unauthorized.Value);
        }

        [Fact(DisplayName = "Login_DebeRetornarBadRequest_SiModelStateNoEsValido")]
        public async Task Login_DebeRetornarBadRequest_SiModelStateNoEsValido()
        {
            var inputLogin = new mLogin { tUserName = "", tPassword = "" };

            _controller.ModelState.AddModelError("tUserName", "El nombre de usuario es requerido.");

            var result = await _controller.Login(inputLogin);

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Credenciales inválidas.", bad.Value);
        }

        [Fact(DisplayName = "Login_DebeRelanzarExcepcion_SiElServicioFalla")]
        public async Task Login_DebeRelanzarExcepcion_SiElServicioFalla()
        {
            var inputLogin = new mLogin { tUserName = "test", tPassword = "pass" };

            _mockUserService.Setup(s => s.CreateMD5(It.IsAny<string>()))
                .ThrowsAsync(new InvalidOperationException("Simulación de error"));

            await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.Login(inputLogin));
        }
    }
}