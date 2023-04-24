using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApiAutores.Controllers;
using WebApiAutores.Tests.Mocks;

namespace WebApiAutores.Tests.PruebasUnitarias
{
    [TestClass]
    public class RootControllerTest
    {
        [TestMethod]
        public async Task SiUsuarioEsAdmin_4Links()
        {
            //Preparar
            var authorizationService = new AuthorizationServiceMock();
            authorizationService.Resultado = AuthorizationResult.Success();
            var rootController = new RootController(authorizationService);
            rootController.Url = new UrlHelperMock();

            //Ejecucion
            var resultado = await rootController.Get();
            //Verificar
            Assert.AreEqual(4, resultado.Value.Count());
        }

        [TestMethod]
        public async Task SiUsuarioNoEsAdmin_2Links()
        {
            //Preparar
            var authorizationService = new AuthorizationServiceMock();
            authorizationService.Resultado = AuthorizationResult.Failed();
            var rootController = new RootController(authorizationService);
            rootController.Url = new UrlHelperMock();

            //Ejecucion
            var resultado = await rootController.Get();
            //Verificar
            Assert.AreEqual(2, resultado.Value.Count());
        }

        [TestMethod]
        public async Task SiUsuarioNoEsAdmin_2Links_UsandoMock()
        {
            //Preparar
            var mockAuthorizationService = new Mock<IAuthorizationService>();
            mockAuthorizationService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<IEnumerable<IAuthorizationRequirement>>())).Returns(Task.FromResult(AuthorizationResult.Failed()));

            mockAuthorizationService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>())).Returns(Task.FromResult(AuthorizationResult.Failed()));

            var mockUrlHelper = new Mock<IUrlHelper>();
            mockUrlHelper.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(string.Empty);
            var rootController = new RootController(mockAuthorizationService.Object);
            rootController.Url = mockUrlHelper.Object;

            //Ejecucion
            var resultado = await rootController.Get();
            //Verificar
            Assert.AreEqual(2, resultado.Value.Count());
        }
    }
}
