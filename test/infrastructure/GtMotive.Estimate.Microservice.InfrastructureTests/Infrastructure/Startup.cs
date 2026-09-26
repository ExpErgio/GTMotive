using GtMotive.Estimate.Microservice.Api;
using GtMotive.Estimate.Microservice.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace GtMotive.Estimate.Microservice.InfrastructureTests.Infrastructure {
    internal sealed class Startup {
        private Startup() { }

        public static void Configure(IApplicationBuilder app) {
            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }

        public static void ConfigureServices(IServiceCollection services) {
            services.AddControllers(ApiConfiguration.ConfigureControllers).WithApiControllers();
            services.AddPersistence("Data Source=infrastructure-tests.db");
        }
    }
}
