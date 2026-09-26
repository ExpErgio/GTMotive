using System;
using System.IO;
using GtMotive.Estimate.Microservice.Api;
using GtMotive.Estimate.Microservice.Infrastructure;
using GtMotive.Estimate.Microservice.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(ApiConfiguration.ConfigureControllers).WithApiControllers();
builder.Services.AddPersistence(builder.Configuration.GetConnectionString("Renting"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Renting API", Version = "v1", Description = "Microservicio de gestión de una flota de renting." });
    var xmlPath = Path.Combine(AppContext.BaseDirectory, "GtMotive.Estimate.Microservice.Api.xml");
    if (File.Exists(xmlPath)) options.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

using (var scope = app.Services.CreateScope()) await scope.ServiceProvider.GetRequiredService<RentingDbContext>().Database.EnsureCreatedAsync();

app.UseSwagger();
app.UseSwaggerUI(options => {
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Renting API v1");
});
app.MapControllers();

await app.RunAsync();
