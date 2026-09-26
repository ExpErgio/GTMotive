using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using GtMotive.Estimate.Microservice.Api.Filters;
using GtMotive.Estimate.Microservice.ApplicationCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

[assembly: CLSCompliant(false)]

namespace GtMotive.Estimate.Microservice.Api {
    [ExcludeFromCodeCoverage]
    public static class ApiConfiguration {
        public static void ConfigureControllers(MvcOptions options) {
            ArgumentNullException.ThrowIfNull(options);
            options.Filters.Add<BusinessExceptionFilter>();
        }

        public static IMvcBuilder WithApiControllers(this IMvcBuilder builder) {
            ArgumentNullException.ThrowIfNull(builder);
            builder.AddApplicationPart(typeof(ApiConfiguration).GetTypeInfo().Assembly);
            builder.Services.AddUseCases();
            return builder;
        }
    }
}
