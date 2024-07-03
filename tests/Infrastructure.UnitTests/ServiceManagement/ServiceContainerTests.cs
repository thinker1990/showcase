using Infrastructure.ServiceManagement;

namespace Infrastructure.UnitTests.ServiceManagement;

internal sealed class ServiceContainerTests
{
    private ServiceContainer _container = default!;

    [SetUp]
    public void Setup()
    {
        _container = new ServiceContainer();
    }

    [Test]
    public void ShouldRegisterInterfaceWithImplementationCorrectly()
    {
        var service = new Alpha();

        _container.Register<IAlpha, Alpha>(service);

        _container.GetService<IAlpha>().Should().Be(service);
    }

    [Test]
    public void ShouldRegisterAbstractClassWithImplementationCorrectly()
    {
        var service = new Concrete();

        _container.Register<Abstract, Concrete>(service);

        _container.GetService<Abstract>().Should().Be(service);
    }

    [Test]
    public void ShouldGetCorrespondingServiceWhenRegisterDifferentServicesWithSameImplementation()
    {
        var service = new Compound();

        _container.Register<IAlpha, Compound>(service);
        _container.Register<IBeta, Compound>(service);

        _container.GetService<IAlpha>().Should().Be(service);
        _container.GetService<IBeta>().Should().Be(service);
    }

    [Test]
    public void ShouldThrowExceptionWhenRegisterSameServiceMultiplyTimes()
    {
        _container.Register<IAlpha, Alpha>(new Alpha());

        var tryRegister = () => _container.Register<IAlpha, Compound>(new Compound());

        tryRegister.Should().Throw<EntityDuplicateException>();
    }

    [Test]
    public void ShouldThrowExceptionWhenTryGetServiceNotRegistered()
    {
        _container.Register<IAlpha, Alpha>(new Alpha());

        var tryGet = _container.GetService<IBeta>;

        tryGet.Should().Throw<EntityNotFoundException>();
    }

    [Test]
    public void ShouldThrowExceptionWhenTryGetServiceWithImplementationType()
    {
        _container.Register<IAlpha, Alpha>(new Alpha());

        var tryGet = _container.GetService<Alpha>;

        tryGet.Should().Throw<EntityNotFoundException>();
    }

    private interface IAlpha;

    private interface IBeta;

    private class Alpha : IAlpha;

    private class Compound : IAlpha, IBeta;

    private abstract class Abstract;

    private sealed class Concrete : Abstract;
}