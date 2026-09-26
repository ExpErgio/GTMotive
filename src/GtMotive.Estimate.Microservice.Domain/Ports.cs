using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GtMotive.Estimate.Microservice.Domain {
    public interface IVehicleRepository {
        void Add(Vehicle vehicle);

        Task<Vehicle> GetByIdAsync(Guid vehicleId, CancellationToken cancellationToken);

        Task<IReadOnlyCollection<Vehicle>> ListAsync(CancellationToken cancellationToken);

        Task<bool> IsRentingAsync(Guid personId, CancellationToken cancellationToken);
    }

    public interface IPersonRepository {
        void Add(Person person);

        void Remove(Person person);

        Task<Person> GetByIdAsync(Guid personId, CancellationToken cancellationToken);

        Task<IReadOnlyCollection<Person>> ListAsync(CancellationToken cancellationToken);
    }
}
