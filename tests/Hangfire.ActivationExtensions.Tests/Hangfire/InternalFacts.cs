using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Hangfire.ActivationExtensions.Interceptor;
using Hangfire.Common;
using Hangfire.Server;
using Hangfire.Storage;

using NSubstitute;

using Ploeh.AutoFixture;

using Xunit;

namespace Hangfire.ActivationExtensions.Tests.Hangfire
{
    public class InternalFacts
    {
        private static readonly Fixture AutoFixture = new Fixture();

        private readonly IStorageConnection _storage = Substitute.For<IStorageConnection>();
        private readonly IJobCancellationToken _token = Substitute.For<IJobCancellationToken>();
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
            var performer = CreatePerformer(new PassThroughActivator(jobActivatorFilterCollection, _activator));
            var job = CreateBackgroundJob(Job.FromExpression(() => JobFixture.StaticMethod()));
            var context = CreateContext(job);

            // Act
            performer.Perform(context);

            // Assert
        }

        [Fact]
        public void Instance_invocation_functions()
        {
            var jobActivatorFilterCollection = new JobActivatorFilterCollection
            {
                Filters = new List<IJobActivatorFilter>
                {
                    Substitute.For<IJobActivatorFilter>()
                }
            };
            var performer = CreatePerformer(new PassThroughActivator(jobActivatorFilterCollection, _activator));
            var job = CreateBackgroundJob(Job.FromExpression<JobFixture>(x => x.InstanceMethod()));
            var context = CreateContext(job);

            // Act
            performer.Perform(context);

            // Assert
        }

        [Fact]
        public void Scope_creation_hooks_called()
        {
            var mockFilter = Substitute.For<IJobActivatorFilter>();

            // Act
            CreateAndPerform(mockFilter);

            // Assert
            mockFilter.Received().OnScopeCreating(Arg.Any<JobActivatorContext>());
            mockFilter.Received().OnScopeCreated(Arg.Any<JobActivatorContext>(), Arg.Is(_scope));
        }

        [Fact]
        public void Job_materialization_hooks_called()
        {
            var mockFilter = Substitute.For<IJobActivatorFilter>();

            // Act
            CreateAndPerform(mockFilter);

            // Assert
            mockFilter.Received().OnMaterializing(_jobType);
            mockFilter.Received().OnMaterialized(_jobType, _activatedJob);
        }

        [Fact]
        // ReSharper disable once IdentifierTypo
        public void Scope_disposation_hooks_called()
        {
            var mockFilter = Substitute.For<IJobActivatorFilter>();

            // Act
            CreateAndPerform(mockFilter);

            // Assert
            mockFilter.Received().OnScopeDisposing(_jobType, _activatedJob);
            mockFilter.Received().OnScopeDisposed(_jobType, _activatedJob);
        }

        private void CreateAndPerform(IJobActivatorFilter mockFilter)
        {
            var jobActivatorFilterCollection = new JobActivatorFilterCollection(mockFilter);
            var performer = CreatePerformer(new PassThroughActivator(jobActivatorFilterCollection, _activator));
            var job = CreateBackgroundJob(Job.FromExpression<JobFixture>(x => x.InstanceMethod()));
            var context = CreateContext(job);

            performer.Perform(context);
        }

        private IBackgroundJobPerformer CreatePerformer(JobActivator activator)
        {
            var memberInfo = Type.GetType("Hangfire.Server.CoreBackgroundJobPerformer,Hangfire.Core");
            if (memberInfo != null)
            {
                var ctor = memberInfo.
                    GetConstructors(BindingFlags.Instance | BindingFlags.Public)
                    .Single();

                return (IBackgroundJobPerformer) ctor.Invoke(new object[] {activator});
            }

            throw new Exception("Type does not exist.");
        }

        private PerformContext CreateContext(BackgroundJob job)
        {
            return new PerformContext(_storage, job, _token);
        }

        private BackgroundJob CreateBackgroundJob(Job job)
        {
            return new BackgroundJob("JobId", job, DateTime.UtcNow);
        }
    }
}