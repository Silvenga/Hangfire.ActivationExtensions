using System;
using System.Collections.Generic;

using FluentAssertions;

using Hangfire.ActivationExtensions.Interceptor;

using NSubstitute;

using Ploeh.AutoFixture;

using Xunit;

namespace Hangfire.ActivationExtensions.Tests.Interceptor
{
    public class PassThroughScropeFacts
    {
        private static readonly Fixture AutoFixture = new Fixture();
        private readonly JobActivatorFilterCollection _filters;
        private readonly JobActivatorScope _internalScope;

        public PassThroughScropeFacts()
        {
            _filters = AutoFixture.Build<JobActivatorFilterCollection>()
                                   .With(x => x.Filters, new List<IJobActivatorFilter>())
                                   .Create();

            _internalScope = Substitute.For<JobActivatorScope>();
        }

        [Fact]
        public void When_Resolve_is_called_multiple_times_then_throw()
        {
            var scope = new PassThroughScope(_filters, _internalScope);
            var type = AutoFixture.Create<Type>();
            scope.Resolve(type);

            // Act
            Action action = () => scope.Resolve(type);

            // Assert
            action.ShouldThrow<NotSupportedException>();
        }
    }
}