using System;
using System.Linq;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.ApplicationCore.Persons;
using GtMotive.Estimate.Microservice.ApplicationCore.Vehicles;
using GtMotive.Estimate.Microservice.Domain;
using GtMotive.Estimate.Microservice.FunctionalTests.Infrastructure;
using Xunit;

namespace GtMotive.Estimate.Microservice.FunctionalTests.Specs {
    [Collection(TestCollections.Functional)]
    public sealed class RentingFlowTests : FunctionalTestBase {
        public RentingFlowTests(CompositionRootTestFixture fixture) : base(fixture) { }

        [Fact]
        public async Task AVehicleIsRentedAndReturnedThroughTheWholeStack() {
            var person = await CreatePerson();
            var vehicle = await CreateVehicle("1111AAA");

            var rented = await Fixture.Send(new RentVehicleCommand { VehicleId = vehicle.Id, PersonId = person.Id });
            Assert.True(rented.IsRented);
            Assert.Equal(person.Id, rented.RenterId);

            var listed = await Fixture.Send(new ListVehiclesQuery());
            Assert.True(listed.Single(candidate => candidate.Id == vehicle.Id).IsRented);

            var returned = await Fixture.Send(new ReturnVehicleCommand { VehicleId = vehicle.Id });
            Assert.False(returned.IsRented);
            Assert.Null(returned.RenterId);
        }

        [Fact]
        public async Task APersonCannotRentTwoVehiclesAtTheSameTime() {
            var person = await CreatePerson();
            var first = await CreateVehicle("2222BBB");
            var second = await CreateVehicle("3333CCC");
            await Fixture.Send(new RentVehicleCommand { VehicleId = first.Id, PersonId = person.Id });

            var exception = await Assert.ThrowsAsync<DomainException>(
                () => Fixture.Send(new RentVehicleCommand { VehicleId = second.Id, PersonId = person.Id }));

            Assert.Contains("solo se permite uno", exception.Message, StringComparison.Ordinal);
        }

        [Fact]
        public async Task TheFleetRejectsVehiclesOlderThanFiveYears() {
            var command = new CreateVehicleCommand { LicensePlate = "4444DDD", Brand = "Seat", Model = "Ibiza", Mileage = 90000, ManufacturingYear = DateTime.UtcNow.Year - 6 };

            var exception = await Assert.ThrowsAsync<DomainException>(() => Fixture.Send(command));

            Assert.Contains("5 años", exception.Message, StringComparison.Ordinal);
        }

        [Fact]
        public async Task APersonHoldingAVehicleCannotBeDeleted() {
            var person = await CreatePerson();
            var vehicle = await CreateVehicle("5555EEE");
            await Fixture.Send(new RentVehicleCommand { VehicleId = vehicle.Id, PersonId = person.Id });

            await Assert.ThrowsAsync<DomainException>(() => Fixture.Send(new DeletePersonCommand { PersonId = person.Id }));
        }

        private Task<PersonResponse> CreatePerson() =>
            Fixture.Send(new CreatePersonCommand { FirstName = "Ana", LastName = Guid.NewGuid().ToString("N")[..8] });

        private Task<VehicleResponse> CreateVehicle(string licensePlate) =>
            Fixture.Send(new CreateVehicleCommand { LicensePlate = licensePlate, Brand = "Renault", Model = "Clio", Mileage = 15000, ManufacturingYear = DateTime.UtcNow.Year - 1 });
    }
}
