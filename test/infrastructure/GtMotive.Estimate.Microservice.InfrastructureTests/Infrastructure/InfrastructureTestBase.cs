namespace GtMotive.Estimate.Microservice.InfrastructureTests.Infrastructure {
    public abstract class InfrastructureTestBase {
        protected InfrastructureTestBase(GenericInfrastructureTestServerFixture fixture) => Fixture = fixture;

        protected GenericInfrastructureTestServerFixture Fixture { get; }
    }
}
