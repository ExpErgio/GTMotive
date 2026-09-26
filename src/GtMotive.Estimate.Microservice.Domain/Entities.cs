using System;

namespace GtMotive.Estimate.Microservice.Domain {
    public sealed class Vehicle {
        public const int MaxFleetAgeInYears = 5;

        private Vehicle() { }

        public Guid Id { get; private set; }

        public string LicensePlate { get; private set; }

        public string Brand { get; private set; }

        public string Model { get; private set; }

        public int Mileage { get; private set; }

        public int ManufacturingYear { get; private set; }

        public Guid? RenterId { get; private set; }

        public Person Renter { get; private set; }

        public bool IsRented => RenterId is not null;

        public static Vehicle Create(string licensePlate, string brand, string model, int mileage, int manufacturingYear, int currentYear) {
            if (string.IsNullOrWhiteSpace(licensePlate)) throw new DomainException("La matrícula es obligatoria.");
            if (string.IsNullOrWhiteSpace(brand)) throw new DomainException("La marca es obligatoria.");
            if (string.IsNullOrWhiteSpace(model)) throw new DomainException("El modelo es obligatorio.");
            if (mileage < 0) throw new DomainException("Los kilómetros no pueden ser negativos.");
            if (manufacturingYear > currentYear) throw new DomainException("El año de fabricación no puede ser futuro.");
            if (IsOlderThanAllowed(manufacturingYear, currentYear)) throw new DomainException($"La flota no admite vehículos con más de {MaxFleetAgeInYears} años de antigüedad.");
            return new Vehicle { Id = Guid.NewGuid(), LicensePlate = licensePlate.Trim().ToUpperInvariant(), Brand = brand.Trim(), Model = model.Trim(), Mileage = mileage, ManufacturingYear = manufacturingYear };
        }

        public void RentTo(Person person, int currentYear) {
            ArgumentNullException.ThrowIfNull(person);
            if (IsRented) throw new DomainException("El vehículo ya está alquilado.");
            if (IsOlderThanAllowed(ManufacturingYear, currentYear)) throw new DomainException($"El vehículo supera los {MaxFleetAgeInYears} años de antigüedad y no puede alquilarse.");
            RenterId = person.Id;
            Renter = person;
        }

        public void ReturnToFleet() {
            if (!IsRented) throw new DomainException("El vehículo no está alquilado.");
            RenterId = null;
            Renter = null;
        }

        private static bool IsOlderThanAllowed(int manufacturingYear, int currentYear) => currentYear - manufacturingYear > MaxFleetAgeInYears;
    }

    public sealed class Person {
        private Person() { }

        public Guid Id { get; private set; }

        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        public string FullName => $"{FirstName} {LastName}";

        public static Person Create(string firstName, string lastName) {
            if (string.IsNullOrWhiteSpace(firstName)) throw new DomainException("El nombre es obligatorio.");
            if (string.IsNullOrWhiteSpace(lastName)) throw new DomainException("Los apellidos son obligatorios.");
            return new Person { Id = Guid.NewGuid(), FirstName = firstName.Trim(), LastName = lastName.Trim() };
        }
    }
}
