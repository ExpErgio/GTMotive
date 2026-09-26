using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

[assembly: CLSCompliant(false)]

namespace GtMotive.Estimate.Microservice.ApplicationCore {
    [ExcludeFromCodeCoverage]
    public static class ApplicationConfiguration {
        public static IServiceCollection AddUseCases(this IServiceCollection services) =>
            services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(typeof(ApplicationConfiguration).GetTypeInfo().Assembly));
    }
}
