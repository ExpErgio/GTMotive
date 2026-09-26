using System;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.ApplicationCore;
using GtMotive.Estimate.Microservice.Infrastructure;
using GtMotive.Estimate.Microservice.Infrastructure.Persistence;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

[assembly: CLSCompliant(false)]

namespace GtMotive.Estimate.Microservice.FunctionalTests.Infrastructure {
    public sealed class CompositionRootTestFixture : IDisposable {
        private readonly ServiceProvider _serviceProvider;

        public CompositionRootTestFixture() {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddUseCases();
            services.AddPersistence("Data Source=functional-tests.db");
            _serviceProvider = services.BuildServiceProvider();
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<RentingDbContext>();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        public async Task Send(IRequest request) {
            using var scope = _serviceProvider.CreateScope();
            await scope.ServiceProvider.GetRequiredService<IMediator>().Send(request);
        }

        public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request) {
            using var scope = _serviceProvider.CreateScope();
            return await scope.ServiceProvider.GetRequiredService<IMediator>().Send(request);
        }

        public void Dispose() => _serviceProvider.Dispose();
    }
}
