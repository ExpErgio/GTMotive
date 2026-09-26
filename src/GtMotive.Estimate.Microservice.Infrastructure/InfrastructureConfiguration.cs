using System;
using System.Diagnostics.CodeAnalysis;
using GtMotive.Estimate.Microservice.Domain;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

[assembly: CLSCompliant(false)]

namespace GtMotive.Estimate.Microservice.Infrastructure {
    [ExcludeFromCodeCoverage]
    public static class InfrastructureConfiguration {
        public static IServiceCollection AddPersistence(this IServiceCollection services, string connectionString) {
            services.AddDbContext<RentingDbContext>(options => options.UseSqlite(connectionString));
            services.AddScoped<IVehicleRepository, VehicleRepository>();
            services.AddScoped<IPersonRepository, PersonRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}
