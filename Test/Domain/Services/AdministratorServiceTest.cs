using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using minimal_api.Domain.Entities;
using minimal_api.Domain.Services;
using minimal_api.Infrastructure.Db;

namespace Test.Domain.Services
{
    [TestClass]
    public class AdministratorServiceTest
    {
        private AppDbContext CriarContextoDeTeste()
        {
            var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var path = Path.GetFullPath(Path.Combine(assemblyPath ?? "", "..", "..", ".."));

            var builder = new ConfigurationBuilder()
                .SetBasePath(path ?? Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            var configuration = builder.Build();

            return new AppDbContext(configuration);
        }


        [TestMethod]
        public void TestandoSalvarAdministrador()
        {
            // Arrange
            var context = CriarContextoDeTeste();
            context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administrators");

            var adm = new Administrator();
            adm.Email = "teste@teste.com";
            adm.Password = "teste";
            adm.Profile = "Adm";

            var administratorService = new AdministratorService(context);

            // Act
            administratorService.PostAdministrator(adm);

            // Assert
            Assert.AreEqual(1, administratorService.GetAllAdministrators(1).Count());
        }

        [TestMethod]
        public void TestandoBuscaPorId()
        {
            // Arrange
            var context = CriarContextoDeTeste();
            context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administrators");

            var adm = new Administrator();
            adm.Email = "teste@teste.com";
            adm.Password = "teste";
            adm.Profile = "Adm";

            var administratorService = new AdministratorService(context);

            // Act
            administratorService.PostAdministrator(adm);
            var admDoBanco = administratorService.GetById(adm.Id);

            // Assert
            Assert.AreEqual(1, admDoBanco?.Id);
        }
    }
}