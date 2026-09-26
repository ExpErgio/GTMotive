using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using MediatR;

namespace GtMotive.Estimate.Microservice.ApplicationCore.Persons {
    public sealed class CreatePersonCommand : IRequest<PersonResponse> {
        [Required] public string FirstName { get; set; }

        [Required] public string LastName { get; set; }
    }

    public sealed class ListPersonsQuery : IRequest<IReadOnlyCollection<PersonResponse>> {
    }

    public sealed class DeletePersonCommand : IRequest {
        public Guid PersonId { get; set; }
    }

    public sealed class PersonResponse {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public static PersonResponse From(Person person) {
            ArgumentNullException.ThrowIfNull(person);
            return new PersonResponse { Id = person.Id, FirstName = person.FirstName, LastName = person.LastName };
        }
    }

    public sealed class CreatePersonHandler : IRequestHandler<CreatePersonCommand, PersonResponse> {
        private readonly IPersonRepository _persons;
        private readonly IUnitOfWork _unitOfWork;

        public CreatePersonHandler(IPersonRepository persons, IUnitOfWork unitOfWork) {
            _persons = persons;
            _unitOfWork = unitOfWork;
        }

        public async Task<PersonResponse> Handle(CreatePersonCommand request, CancellationToken cancellationToken) {
            ArgumentNullException.ThrowIfNull(request);
            var person = Person.Create(request.FirstName, request.LastName);
            _persons.Add(person);
            await _unitOfWork.Save();
            return PersonResponse.From(person);
        }
    }

    public sealed class ListPersonsHandler : IRequestHandler<ListPersonsQuery, IReadOnlyCollection<PersonResponse>> {
        private readonly IPersonRepository _persons;

        public ListPersonsHandler(IPersonRepository persons) => _persons = persons;

        public async Task<IReadOnlyCollection<PersonResponse>> Handle(ListPersonsQuery request, CancellationToken cancellationToken) {
            var persons = await _persons.ListAsync(cancellationToken);
            return [.. persons.Select(PersonResponse.From)];
        }
    }

    public sealed class DeletePersonHandler : IRequestHandler<DeletePersonCommand> {
        private readonly IPersonRepository _persons;
        private readonly IVehicleRepository _vehicles;
        private readonly IUnitOfWork _unitOfWork;

        public DeletePersonHandler(IPersonRepository persons, IVehicleRepository vehicles, IUnitOfWork unitOfWork) {
            _persons = persons;
            _vehicles = vehicles;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeletePersonCommand request, CancellationToken cancellationToken) {
            ArgumentNullException.ThrowIfNull(request);
            var person = await _persons.GetByIdAsync(request.PersonId, cancellationToken) ?? throw new NotFoundException($"La persona {request.PersonId} no existe.");
            if (await _vehicles.IsRentingAsync(person.Id, cancellationToken)) throw new DomainException("No se puede eliminar una persona que tiene un vehículo alquilado.");
            _persons.Remove(person);
            await _unitOfWork.Save();
        }
    }
}
