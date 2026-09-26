using System;
using System.IO;
using GtMotive.Estimate.Microservice.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

[assembly: CLSCompliant(false)]

namespace GtMotive.Estimate.Microservice.InfrastructureTests.Infrastructure {
    public sealed class GenericInfrastructureTestServerFixture : IDisposable {
        public GenericInfrastructureTestServerFixture() {
            Server = new TestServer(new WebHostBuilder().UseContentRoot(Directory.GetCurrentDirectory()).UseStartup<Startup>());
            using var scope = Server.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<RentingDbContext>();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        public TestServer Server { get; }

        public void Dispose() => Server?.Dispose();
    }
}
