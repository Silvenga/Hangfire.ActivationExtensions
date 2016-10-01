namespace Hangfire.ActivationExtensions.Tests
{
    using System.Collections.Generic;

    using FluentAssertions;

    using global::Hangfire.ActivationExtensions.Interceptor;

    using NSubstitute;

    using Ploeh.AutoFixture;

    using Xunit;

    public class GlobalConfigurationExtensionsFacts
    {
        private static readonly Fixture AutoFixture = new Fixture();

        private readonly IGlobalConfiguration<JobActivator> _configurationFromActivator = Substitute.For<IGlobalConfiguration<JobActivator>>();
        private readonly JobActivator _activator = Substitute.For<JobActivator>();
        private readonly JobActivatorFilterCollection _filterCollection;

        public GlobalConfigurationExtensionsFacts()
        {
            _filterCollection = AutoFixture.Build<JobActivatorFilterCollection>()
                                            .With(x => x.Filters, new List<IJobActivatorFilter> {Substitute.For<IJobActivatorFilter>()})
                                            .Create();
        }

        [Fact]
        public void UseActivatorInterceptor_sets_current_activator()
        {
            // Act
            _configurationFromActivator.UseActivator(_activator)
                                        .UseActivatorInterceptor(_filterCollection);

            // Assert
            var current = JobActivator.Current.Should().BeAssignableTo<PassThroughJobActivator>().Subject;
            current.Activator.Should().Be(_activator);
            current.FilterCollection.Should().Be(_filterCollection);
        }
    }
}