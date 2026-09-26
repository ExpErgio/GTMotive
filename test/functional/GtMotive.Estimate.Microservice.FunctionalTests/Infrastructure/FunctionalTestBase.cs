namespace GtMotive.Estimate.Microservice.FunctionalTests.Infrastructure {
    public abstract class FunctionalTestBase {
        protected FunctionalTestBase(CompositionRootTestFixture fixture) => Fixture = fixture;

        protected CompositionRootTestFixture Fixture { get; }
    }
}
