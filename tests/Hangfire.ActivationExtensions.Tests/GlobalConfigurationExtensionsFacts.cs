using System.Collections.Generic;

using FluentAssertions;

using Hangfire.ActivationExtensions.Interceptor;
using Hangfire.ActivationExtensions.Tests.Helpers;

using NSubstitute;

using Ploeh.AutoFixture;

using Xunit;

namespace Hangfire.ActivationExtensions.Tests
{
    public class GlobalConfigurationExtensionsFacts
    {
        private static readonly Fixture AutoFixture = new Fixture();

        private readonly IGlobalConfiguration<JobActivator> _configurationFromActivator = Substitute.For<IGlobalConfiguration<JobActivator>>();
        private readonly JobActivator _activator = Substitute.For<JobActivator>();

        public GlobalConfigurationExtensionsFacts()
        {
        }

        [Fact, UseCurrentJobActivator]
        public void UseActivatorInterceptor_sets_current_activator()
        {
            var filterCollection = AutoFixture.Build<JobActivatorFilterCollection>()
                                               .With(x => x.Filters, new List<IJobActivatorFilter> {Substitute.For<IJobActivatorFilter>()})
                                               .Create();

            // Act
            _configurationFromActivator.UseActivator(_activator)
                                        .UseActivatorInterceptor(filterCollection);

            // Assert
            var current = JobActivator.Current.Should().BeAssignableTo<PassThroughActivator>().Subject;
            current.Activator.Should().Be(_activator);
            current.FilterCollection.Should().Be(filterCollection);
        }

        [Fact, UseCurrentJobActivator]
        public void UseActivatorInterceptor_merges_multiple_invocations()
        {
            var activator1 = Substitute.For<IJobActivatorFilter>();
            var activator2 = Substitute.For<IJobActivatorFilter>();

            // Act
            var passthrough = _configurationFromActivator.UseActivator(_activator)
                                                          .UseActivatorInterceptor(activator1)
                                                          .UseActivatorInterceptor(activator2)
                                                          .Entry;

            // Assert
            passthrough.FilterCollection.Filters.Should().ContainInOrder(activator1, activator2);
        }
    }
}