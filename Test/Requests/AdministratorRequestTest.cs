using System.Net;
using System.Text;
using System.Text.Json;
using minimal_api.Domain.DTOs;
using minimal_api.Domain.ViewsModal;
using Test.Helpers;

namespace Test.Requests
{
    [TestClass]
    public class AdministratorRequestTest
    {
        [ClassInitialize]
        public static void ClassInit(TestContext testContext)
        {
            Setup.ClassInit(testContext);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Setup.ClassCleanup();
        }

        [TestMethod]
        public async Task TestarGetSetPropriedades()
        {
            // Arrange
            var loginDTO = new LoginDTO
            {
                Email = "adm@teste.com",
                Password = "123456"
            };

            var content = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8, "Application/json");

            // Act
            var response = await Setup.client.PostAsync("/administradores/login", content);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();
            var admLogado = JsonSerializer.Deserialize<AdministratorLoggedIn>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.IsNotNull(admLogado?.Email ?? "");
            Assert.IsNotNull(admLogado?.Profile ?? "");
            Assert.IsNotNull(admLogado?.Token ?? "");

            Console.WriteLine(admLogado?.Token);
        }
    }
}