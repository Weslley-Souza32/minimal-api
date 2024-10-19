// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Configuration;
// using minimal_api.Domain.DTOs;
// using minimal_api.Domain.Entities;
// using minimal_api.Domain.Services;
// using minimal_api.Infrastructure.Db;
// using Moq;

// namespace Test.Domain.Services
// {
//     [TestClass]
//     public class AdministratorServiceTest
//     {
//         private AdministratorService _service;
//         private Mock<AppDbContext> _mockContext;

//         [TestInitialize]
//         public void Setup()
//         {
//             var options = new DbContextOptionsBuilder<AppDbContext>()
//                 .UseInMemoryDatabase(databaseName: "TestDb")
//                 .Options;

//             // Utilizando o novo construtor que aceita DbContextOptions
//             var context = new AppDbContext(options);
//             _service = new AdministratorService(context);

//             // Adicionando alguns administradores na base de dados de teste
//             context.Administrators.AddRange(
//                 new Administrator { Id = 1, Email = "admin1@test.com", Password = "password1" },
//                 new Administrator { Id = 2, Email = "admin2@test.com", Password = "password2" }
//             );
//             context.SaveChanges();
//         }

//         [TestMethod]
//         public void Login_ShouldReturnAdministrator_WhenCredentialsAreCorrect()
//         {
//             // Arrange
//             var loginDTO = new LoginDTO { Email = "admin1@test.com", Password = "password1" };

//             // Act
//             var result = _service.Login(loginDTO);

//             // Assert
//             Assert.IsNotNull(result);
//             Assert.AreEqual(1, result.Id);
//         }

//         [TestMethod]
//         public void Login_ShouldReturnNull_WhenCredentialsAreIncorrect()
//         {
//             // Arrange
//             var loginDTO = new LoginDTO { Email = "wrong@test.com", Password = "wrongpassword" };

//             // Act
//             var result = _service.Login(loginDTO);

//             // Assert
//             Assert.IsNull(result);
//         }

//         [TestMethod]
//         public void GetAllAdministrators_ShouldReturnAllAdmins_WhenNoPaging()
//         {
//             // Act
//             var result = _service.GetAllAdministrators(null);

//             // Assert
//             Assert.AreEqual(2, result.Count);
//         }

//         [TestMethod]
//         public void GetAllAdministrators_ShouldReturnPagedAdmins_WhenPageIsSpecified()
//         {
//             // Act
//             var result = _service.GetAllAdministrators(1);

//             // Assert
//             Assert.AreEqual(2, result.Count);
//         }

//         [TestMethod]
//         public void PostAdministrator_ShouldAddNewAdministrator()
//         {
//             // Arrange
//             var newAdmin = new Administrator { Id = 3, Email = "admin3@test.com", Password = "password3" };

//             // Act
//             var result = _service.PostAdministrator(newAdmin);

//             // Assert
//             Assert.IsNotNull(result);
//             Assert.AreEqual(3, result.Id);

//             var admins = _service.GetAllAdministrators(null);
//             Assert.AreEqual(3, admins.Count); // Deve ter 3 admins agora
//         }

//         [TestMethod]
//         public void GetById_ShouldReturnAdministrator_WhenIdExists()
//         {
//             // Act
//             var result = _service.GetById(1);

//             // Assert
//             Assert.IsNotNull(result);
//             Assert.AreEqual(1, result.Id);
//         }

//         [TestMethod]
//         public void GetById_ShouldReturnNull_WhenIdDoesNotExist()
//         {
//             // Act
//             var result = _service.GetById(99);

//             // Assert
//             Assert.IsNull(result);
//         }
//     }
// }