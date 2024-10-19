using minimal_api.Domain.Entities;

namespace Test.Domain.Entities
{
    [TestClass]
    public class VehicleTest
    {
        [TestMethod]
        public void TestPropertiesGetSet()
        {
            // Arrange
            var vehicle = new Vehicle();

            // Act
            vehicle.Id = 1;
            vehicle.Name = "teste";
            vehicle.Brand = "teste";
            vehicle.Year = 0;

            // Assert
            Assert.AreEqual(1, vehicle.Id);
            Assert.AreEqual("teste", vehicle.Name);
            Assert.AreEqual("teste", vehicle.Brand);
            Assert.AreEqual(0, vehicle.Year);
        }
    }
}