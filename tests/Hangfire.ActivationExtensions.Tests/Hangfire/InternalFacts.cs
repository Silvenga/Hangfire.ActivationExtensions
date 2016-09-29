namespace Hangfire.ActivationExtensions.Tests.Hangfire
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using global::Hangfire.ActivationExtensions.Interceptor;
    using global::Hangfire.Common;
    using global::Hangfire.Server;
    using global::Hangfire.Storage;

    using NSubstitute;

    using Xunit;

    public class InternalFacts
    {
        private readonly IStorageConnection _storage = Substitute.For<IStorageConnection>();
        private readonly IJobCancellationToken _token = Substitute.For<IJobCancellationToken>();

        [Fact]
        public void Static_invocation_functions()
        {
            var jobActivatorFilterCollection = new JobActivatorFilterCollection
            {
                Filters = new List<IJobActivatorFilter>
                {
                    new ActivatorFixture()
                }
            };
            var performer = CreatePerformer(new PassThroughJobActivator(jobActivatorFilterCollection, JobActivator.Current));
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
                    new ActivatorFixture()
                }
            };
            var performer = CreatePerformer(new PassThroughJobActivator(jobActivatorFilterCollection, JobActivator.Current));
            var job = CreateBackgroundJob(Job.FromExpression<JobFixture>(x => x.InstanceMethod()));
            var context = CreateContext(job);

            // Act
            performer.Perform(context);

            // Assert
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

    public class ActivatorFixture : IJobActivatorFilter
    {
        public void OnMaterializing(Type jobType)
        {
        }

        public void OnMaterialized(Type jobType, object activatedJob)
        {
        }

        public void OnScopeCreating(JobActivatorContext context)
        {
        }

        public void OnScopeCreated(JobActivatorContext context, JobActivatorScope createdScope)
        {
        }

        public void OnScopeDisposing(Type jobType, object activatedJob)
        {
        }

        public void OnScopeDisposed(Type jobType, object activatedJob)
        {
        }
    }
}