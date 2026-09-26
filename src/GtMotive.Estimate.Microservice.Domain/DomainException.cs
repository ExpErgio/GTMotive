using System;

[assembly: CLSCompliant(false)]

namespace GtMotive.Estimate.Microservice.Domain {
    public class DomainException : Exception {
        public DomainException() { }

        public DomainException(string message) : base(message) { }

        public DomainException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class NotFoundException : DomainException {
        public NotFoundException() { }

        public NotFoundException(string message) : base(message) { }

        public NotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
