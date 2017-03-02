using System;
using System.Collections.Generic;

using FluentAssertions;

using Hangfire.ActivationExtensions.Interceptor;

using NSubstitute;

using Ploeh.AutoFixture;

using Xunit;

namespace Hangfire.ActivationExtensions.Tests.Interceptor
{
    public class PassThroughActivatorFacts
    {
        private static readonly Fixture AutoFixture = new Fixture();

        private readonly JobActivator _mockActivator;
        private readonly IJobActivatorFilter _mockFilter;
        private readonly JobActivatorFilterCollection _mockFilterCollection;

        public PassThroughActivatorFacts()
        {
            _mockActivator = Substitute.For<JobActivator>();
            var mockScope = Substitute.For<JobActivatorScope>();
            _mockFilter = Substitute.For<IJobActivatorFilter>();
            _mockFilterCollection = AutoFixture.Build<JobActivatorFilterCollection>()
                                                .With(x => x.Filters, new List<IJobActivatorFilter> {_mockFilter})
                                                .Create();

            _mockActivator.BeginScope(Arg.Any<JobActivatorContext>()).Returns(mockScope);
        }

        [Fact]
        public void ActivateJob_should_call_OnMaterializing_before_activating_job()
        {
            var type = AutoFixture.Create<Type>();
            var activator = new PassThroughActivator(_mockFilterCollection, _mockActivator);

            _mockActivator.When(x => x.ActivateJob(type)).Throw<SignalException>();

            // Act
            Action action = () => activator.ActivateJob(type);

            // Assert
            action.ShouldThrow<SignalException>();
            _mockFilter.Received().OnMaterializing(type, null);
        }

        [Fact]
        public void ActivateJob_should_call_OnMaterialized_after_constructing_job()
        {
            var type = AutoFixture.Create<Type>();
            var obj = AutoFixture.Create<object>();
            var activator = new PassThroughActivator(_mockFilterCollection, _mockActivator);

            _mockActivator.ActivateJob(type).Returns(obj);
            _mockActivator
                .When(x => x.ActivateJob(type))
                 .Do(info =>
                             _mockFilter.DidNotReceive().OnMaterialized(Arg.Any<Type>(), Arg.Any<object>(), Arg.Any<JobActivatorContext>())
                 );

            // Act
            activator.ActivateJob(type);

            // Assert
            _mockFilter.Received().OnMaterialized(type, obj, Arg.Any<JobActivatorContext>());
        }

        [Fact]
        public void BeginScope_should_return_passthrough_scope()
        {
            var activator = new PassThroughActivator(_mockFilterCollection, _mockActivator);

            // Act
#pragma warning disable 618
            Action action = () => activator.BeginScope();
#pragma warning restore 618

            // Assert
            action.ShouldThrow<NotImplementedException>();
        }

        [Fact]
        public void BeginScope_should_call_OnScopeCreating_before_creating_scope()
        {
            var context = ActivatorFixture();
            var activator = new PassThroughActivator(_mockFilterCollection, _mockActivator);

            _mockActivator.When(x => x.BeginScope(context)).Throw<SignalException>();

            // Act
            Action action = () => activator.BeginScope(context);

            // Assert
            action.ShouldThrow<SignalException>();
            _mockFilter.Received().OnScopeCreating(context);
        }

        [Fact]
        public void BeginScope_should_call_OnScopeCreated_after_creating_scope()
        {
            var context = ActivatorFixture();
            var scope = Substitute.For<JobActivatorScope>();
            var activator = new PassThroughActivator(_mockFilterCollection, _mockActivator);

            _mockActivator.BeginScope(context).Returns(scope);
            _mockActivator
                .When(x => x.BeginScope(context))
                 .Do(info =>
                             _mockFilter.DidNotReceive().OnScopeCreated(Arg.Any<JobActivatorContext>(), Arg.Any<JobActivatorScope>())
                 );

            // Act
            activator.BeginScope(context);

            // Assert
            _mockFilter.Received().OnScopeCreated(context, scope);
        }

        [Fact]
        public void ActivateJob_should_pass_returns_through()
        {
            var type = AutoFixture.Create<Type>();
            var obj = AutoFixture.Create<object>();
            var activator = new PassThroughActivator(_mockFilterCollection, _mockActivator);
            _mockActivator.ActivateJob(type).Returns(obj);

            // Act
            var result = activator.ActivateJob(type);

            // Assert
            result.Should().BeSameAs(obj);
        }

        [Fact]
        public void BeginScope_with_context_should_return_passthrough_scope()
        {
            var context = ActivatorFixture();
            var activator = new PassThroughActivator(_mockFilterCollection, _mockActivator);

            // Act
            var result = activator.BeginScope(context);

            // Assert
            result.Should().BeAssignableTo<PassThroughScope>();
        }

        private static JobActivatorContext ActivatorFixture()
        {
            // TODO Make tracking refs possible
            return null;
        }
    }

    public class SignalException : Exception
    {
    }
}