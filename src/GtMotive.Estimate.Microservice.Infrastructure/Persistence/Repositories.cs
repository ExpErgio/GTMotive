using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GtMotive.Estimate.Microservice.Infrastructure.Persistence {
    public sealed class VehicleRepository : IVehicleRepository {
        private readonly RentingDbContext _context;

        public VehicleRepository(RentingDbContext context) => _context = context;

        public void Add(Vehicle vehicle) => _context.Vehicles.Add(vehicle);

        public async Task<Vehicle> GetByIdAsync(Guid vehicleId, CancellationToken cancellationToken) =>
            await _context.Vehicles.Include(vehicle => vehicle.Renter).FirstOrDefaultAsync(vehicle => vehicle.Id == vehicleId, cancellationToken);

        public async Task<IReadOnlyCollection<Vehicle>> ListAsync(CancellationToken cancellationToken) =>
            await _context.Vehicles.Include(vehicle => vehicle.Renter).OrderBy(vehicle => vehicle.LicensePlate).ToListAsync(cancellationToken);

        public async Task<bool> IsRentingAsync(Guid personId, CancellationToken cancellationToken) =>
            await _context.Vehicles.AnyAsync(vehicle => vehicle.RenterId == personId, cancellationToken);
    }

    public sealed class PersonRepository : IPersonRepository {
        private readonly RentingDbContext _context;

        public PersonRepository(RentingDbContext context) => _context = context;

        public void Add(Person person) => _context.Persons.Add(person);

        public void Remove(Person person) => _context.Persons.Remove(person);

        public async Task<Person> GetByIdAsync(Guid personId, CancellationToken cancellationToken) =>
            await _context.Persons.FirstOrDefaultAsync(person => person.Id == personId, cancellationToken);

        public async Task<IReadOnlyCollection<Person>> ListAsync(CancellationToken cancellationToken) =>
            await _context.Persons.OrderBy(person => person.LastName).ThenBy(person => person.FirstName).ToListAsync(cancellationToken);
    }

    public sealed class UnitOfWork : IUnitOfWork {
        private readonly RentingDbContext _context;

        public UnitOfWork(RentingDbContext context) => _context = context;

        public async Task<int> Save() => await _context.SaveChangesAsync();
    }
}
