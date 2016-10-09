using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Hangfire.ActivationExtensions.Interceptor;
using Hangfire.ActivationExtensions.Tests.Helpers;
using Hangfire.Common;

using NSubstitute;

using Ploeh.AutoFixture;

using Xunit;

namespace Hangfire.ActivationExtensions.Tests.Hangfire
{
    public class InternalFacts
    {
        private static readonly Fixture AutoFixture = new Fixture();

        private readonly HangfirePerformerFixture _performerFixture = new HangfirePerformerFixture();

        private readonly JobActivator _activator = Substitute.For<JobActivator>();
        private readonly JobActivatorScope _scope = Substitute.For<JobActivatorScope>();
        private readonly object _activatedJob = AutoFixture.Create<JobFixture>();
        private readonly Type _jobType = typeof(JobFixture);

        public InternalFacts()
        {
            _activator.BeginScope(Arg.Any<JobActivatorContext>()).Returns(_scope);
            _scope.Resolve(_jobType).Returns(_activatedJob);
        }

        [Fact]
        public void Static_invocation_functions()
        {
            var jobActivatorFilterCollection = new JobActivatorFilterCollection
            {
                Filters = new List<IJobActivatorFilter>
                {
                    Substitute.For<IJobActivatorFilter>()
                }
            };
            var performer = _performerFixture.CreatePerformer(new PassThroughActivator(jobActivatorFilterCollection, _activator));
            var job = _performerFixture.CreateBackgroundJob(Job.FromExpression(() => JobFixture.StaticMethod()));
            var context = _performerFixture.CreateContext(job);

            // Act
            performer.Perform(context);

            // Assert
        }

        [Fact]
        public void Scope_creation_hooks_called()
        {
            var mockFilter = Substitute.For<IJobActivatorFilter>();

            // Act
            CreateAndPerform<JobFixture>(mockFilter, x => x.InstanceMethod());

            // Assert
            mockFilter.Received().OnScopeCreating(Arg.Is<JobActivatorContext>(x => x != null));
            mockFilter.Received().OnScopeCreated(Arg.Is<JobActivatorContext>(x => x != null), Arg.Is(_scope));
        }

        [Fact]
        public void Job_materialization_hooks_called()
        {
            var mockFilter = Substitute.For<IJobActivatorFilter>();

            // Act
            CreateAndPerform<JobFixture>(mockFilter, x => x.InstanceMethod());

            // Assert
            mockFilter.Received().OnMaterializing(_jobType, Arg.Is<JobActivatorContext>(x => x != null));
            mockFilter.Received().OnMaterialized(_jobType, _activatedJob, Arg.Is<JobActivatorContext>(x => x != null));
        }

        [Fact]
        // ReSharper disable once IdentifierTypo
        public void Scope_disposation_hooks_called()
        {
            var mockFilter = Substitute.For<IJobActivatorFilter>();

            // Act
            CreateAndPerform<JobFixture>(mockFilter, x => x.InstanceMethod());

            // Assert
            mockFilter.Received().OnScopeDisposing(_jobType, _activatedJob, Arg.Is<JobActivatorContext>(x => x != null));
            mockFilter.Received().OnScopeDisposed(_jobType, _activatedJob, Arg.Is<JobActivatorContext>(x => x != null));
        }

        private void CreateAndPerform<T>(IJobActivatorFilter mockFilter, Expression<Action<T>> methodCall)
        {
            var jobActivatorFilterCollection = new JobActivatorFilterCollection(mockFilter);
            var performer = _performerFixture.CreatePerformer(new PassThroughActivator(jobActivatorFilterCollection, _activator));
            var job = _performerFixture.CreateBackgroundJob(Job.FromExpression(methodCall));
            var context = _performerFixture.CreateContext(job);

            performer.Perform(context);
        }
    }
}