using System;
using System.Threading;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.ApplicationCore.Vehicles;
using GtMotive.Estimate.Microservice.Domain;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using Moq;
using Xunit;

[assembly: CLSCompliant(false)]

namespace GtMotive.Estimate.Microservice.UnitTests {
    public class VehicleTests {
        [Fact]
        public void CreateRejectsAVehicleOlderThanFiveYears() {
            var exception = Assert.Throws<DomainException>(() => Vehicle.Create("1234BCD", "Renault", "Clio", 10000, 2018, 2026));

            Assert.Contains("5 años", exception.Message, StringComparison.Ordinal);
        }

        [Fact]
        public void CreateAcceptsAVehicleWithinTheFleetAgeLimit() {
            var vehicle = Vehicle.Create("1234BCD", "Renault", "Clio", 10000, 2024, 2026);

            Assert.False(vehicle.IsRented);
            Assert.Equal("1234BCD", vehicle.LicensePlate);
        }

        [Fact]
        public void RentToAndReturnToFleetChangeTheAvailability() {
            var vehicle = Vehicle.Create("1234BCD", "Renault", "Clio", 10000, 2024, 2026);
            var person = Person.Create("Ana", "García");

            vehicle.RentTo(person, 2026);
            Assert.True(vehicle.IsRented);
            Assert.Equal(person.Id, vehicle.RenterId);

            vehicle.ReturnToFleet();
            Assert.False(vehicle.IsRented);
            Assert.Null(vehicle.RenterId);
        }
    }

    public class RentVehicleHandlerTests {
        [Fact]
        public async Task RentRejectsASecondVehicleForTheSamePerson() {
            var vehicle = Vehicle.Create("1234BCD", "Renault", "Clio", 10000, 2024, DateTime.UtcNow.Year);
            var person = Person.Create("Ana", "García");
            var vehicles = new Mock<IVehicleRepository>();
            var persons = new Mock<IPersonRepository>();
            vehicles.Setup(repository => repository.GetByIdAsync(vehicle.Id, It.IsAny<CancellationToken>())).ReturnsAsync(vehicle);
            vehicles.Setup(repository => repository.IsRentingAsync(person.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            persons.Setup(repository => repository.GetByIdAsync(person.Id, It.IsAny<CancellationToken>())).ReturnsAsync(person);
            var handler = new RentVehicleHandler(vehicles.Object, persons.Object, new Mock<IUnitOfWork>().Object);

            var exception = await Assert.ThrowsAsync<DomainException>(
                () => handler.Handle(new RentVehicleCommand { VehicleId = vehicle.Id, PersonId = person.Id }, CancellationToken.None));

            Assert.Contains("solo se permite uno", exception.Message, StringComparison.Ordinal);
            Assert.False(vehicle.IsRented);
        }
    }
}
