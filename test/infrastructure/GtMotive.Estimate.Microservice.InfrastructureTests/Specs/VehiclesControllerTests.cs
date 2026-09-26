using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.InfrastructureTests.Infrastructure;
using Xunit;

namespace GtMotive.Estimate.Microservice.InfrastructureTests.Specs {
    [Collection(TestCollections.TestServer)]
    public sealed class VehiclesControllerTests : InfrastructureTestBase {
        public VehiclesControllerTests(GenericInfrastructureTestServerFixture fixture) : base(fixture) { }

        [Fact]
        public async Task CreateVehicleAcceptsAWellFormedRequest() {
            using var client = Fixture.Server.CreateClient();

            var response = await client.PostAsJsonAsync("/api/vehicles", new { licensePlate = "9999ZZZ", brand = "Renault", model = "Clio", mileage = 12000, manufacturingYear = 2024 });

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task CreateVehicleRejectsARequestWithMissingFields() {
            using var client = Fixture.Server.CreateClient();

            var response = await client.PostAsJsonAsync("/api/vehicles", new { brand = "Renault" });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
