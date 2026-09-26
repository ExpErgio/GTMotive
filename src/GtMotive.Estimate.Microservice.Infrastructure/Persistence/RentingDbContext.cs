using System;
using GtMotive.Estimate.Microservice.Domain;
using Microsoft.EntityFrameworkCore;

namespace GtMotive.Estimate.Microservice.Infrastructure.Persistence {
    public sealed class RentingDbContext : DbContext {
        public RentingDbContext(DbContextOptions<RentingDbContext> options) : base(options) { }

        public DbSet<Vehicle> Vehicles => Set<Vehicle>();

        public DbSet<Person> Persons => Set<Person>();

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            ArgumentNullException.ThrowIfNull(modelBuilder);
            modelBuilder.Entity<Vehicle>(vehicle => {
                vehicle.HasKey(entity => entity.Id);
                vehicle.Ignore(entity => entity.IsRented);
                vehicle.Property(entity => entity.LicensePlate).IsRequired();
                vehicle.Property(entity => entity.Brand).IsRequired();
                vehicle.Property(entity => entity.Model).IsRequired();
                vehicle.HasOne(entity => entity.Renter).WithMany().HasForeignKey(entity => entity.RenterId).OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Person>(person => {
                person.HasKey(entity => entity.Id);
                person.Ignore(entity => entity.FullName);
                person.Property(entity => entity.FirstName).IsRequired();
                person.Property(entity => entity.LastName).IsRequired();
            });
        }
    }
}
