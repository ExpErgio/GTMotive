using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using MediatR;

namespace GtMotive.Estimate.Microservice.ApplicationCore.Vehicles {
    public sealed class CreateVehicleCommand : IRequest<VehicleResponse> {
        [Required] public string LicensePlate { get; set; }

        [Required] public string Brand { get; set; }

        [Required] public string Model { get; set; }

        [Required] public int? Mileage { get; set; }

        [Required] public int? ManufacturingYear { get; set; }
    }

    public sealed class ListVehiclesQuery : IRequest<IReadOnlyCollection<VehicleResponse>> {
    }

    public sealed class RentVehicleCommand : IRequest<VehicleResponse> {
        public Guid VehicleId { get; set; }

        [Required] public Guid? PersonId { get; set; }
    }

    public sealed class ReturnVehicleCommand : IRequest<VehicleResponse> {
        public Guid VehicleId { get; set; }
    }

    public sealed class VehicleResponse {
        public Guid Id { get; set; }

        public string LicensePlate { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }

        public int Mileage { get; set; }

        public int ManufacturingYear { get; set; }

        public bool IsRented { get; set; }

        public Guid? RenterId { get; set; }

        public string RenterName { get; set; }

        public static VehicleResponse From(Vehicle vehicle) {
            ArgumentNullException.ThrowIfNull(vehicle);
            return new VehicleResponse { Id = vehicle.Id, LicensePlate = vehicle.LicensePlate, Brand = vehicle.Brand, Model = vehicle.Model, Mileage = vehicle.Mileage, ManufacturingYear = vehicle.ManufacturingYear, IsRented = vehicle.IsRented, RenterId = vehicle.RenterId, RenterName = vehicle.Renter?.FullName };
        }
    }

    public sealed class CreateVehicleHandler : IRequestHandler<CreateVehicleCommand, VehicleResponse> {
        private readonly IVehicleRepository _vehicles;
        private readonly IUnitOfWork _unitOfWork;

        public CreateVehicleHandler(IVehicleRepository vehicles, IUnitOfWork unitOfWork) {
            _vehicles = vehicles;
            _unitOfWork = unitOfWork;
        }

        public async Task<VehicleResponse> Handle(CreateVehicleCommand request, CancellationToken cancellationToken) {
            ArgumentNullException.ThrowIfNull(request);
            var vehicle = Vehicle.Create(request.LicensePlate, request.Brand, request.Model, request.Mileage.Value, request.ManufacturingYear.Value, DateTime.UtcNow.Year);
            _vehicles.Add(vehicle);
            await _unitOfWork.Save();
            return VehicleResponse.From(vehicle);
        }
    }

    public sealed class ListVehiclesHandler : IRequestHandler<ListVehiclesQuery, IReadOnlyCollection<VehicleResponse>> {
        private readonly IVehicleRepository _vehicles;

        public ListVehiclesHandler(IVehicleRepository vehicles) => _vehicles = vehicles;

        public async Task<IReadOnlyCollection<VehicleResponse>> Handle(ListVehiclesQuery request, CancellationToken cancellationToken) {
            var vehicles = await _vehicles.ListAsync(cancellationToken);
            return [.. vehicles.Select(VehicleResponse.From)];
        }
    }

    public sealed class RentVehicleHandler : IRequestHandler<RentVehicleCommand, VehicleResponse> {
        private readonly IVehicleRepository _vehicles;
        private readonly IPersonRepository _persons;
        private readonly IUnitOfWork _unitOfWork;

        public RentVehicleHandler(IVehicleRepository vehicles, IPersonRepository persons, IUnitOfWork unitOfWork) {
            _vehicles = vehicles;
            _persons = persons;
            _unitOfWork = unitOfWork;
        }

        public async Task<VehicleResponse> Handle(RentVehicleCommand request, CancellationToken cancellationToken) {
            ArgumentNullException.ThrowIfNull(request);
            var vehicle = await _vehicles.GetByIdAsync(request.VehicleId, cancellationToken) ?? throw new NotFoundException($"El vehículo {request.VehicleId} no existe.");
            var person = await _persons.GetByIdAsync(request.PersonId.Value, cancellationToken) ?? throw new NotFoundException($"La persona {request.PersonId} no existe.");
            if (await _vehicles.IsRentingAsync(person.Id, cancellationToken)) throw new DomainException("La persona ya tiene un vehículo alquilado y solo se permite uno al mismo tiempo.");
            vehicle.RentTo(person, DateTime.UtcNow.Year);
            await _unitOfWork.Save();
            return VehicleResponse.From(vehicle);
        }
    }

    public sealed class ReturnVehicleHandler : IRequestHandler<ReturnVehicleCommand, VehicleResponse> {
        private readonly IVehicleRepository _vehicles;
        private readonly IUnitOfWork _unitOfWork;

        public ReturnVehicleHandler(IVehicleRepository vehicles, IUnitOfWork unitOfWork) {
            _vehicles = vehicles;
            _unitOfWork = unitOfWork;
        }

        public async Task<VehicleResponse> Handle(ReturnVehicleCommand request, CancellationToken cancellationToken) {
            ArgumentNullException.ThrowIfNull(request);
            var vehicle = await _vehicles.GetByIdAsync(request.VehicleId, cancellationToken) ?? throw new NotFoundException($"El vehículo {request.VehicleId} no existe.");
            vehicle.ReturnToFleet();
            await _unitOfWork.Save();
            return VehicleResponse.From(vehicle);
        }
    }
}
