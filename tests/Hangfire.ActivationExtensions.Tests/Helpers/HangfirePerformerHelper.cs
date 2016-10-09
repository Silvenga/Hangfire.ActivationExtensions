using System;
using System.Linq;
using System.Reflection;

using Hangfire.Common;
using Hangfire.Server;
using Hangfire.Storage;

using NSubstitute;

namespace Hangfire.ActivationExtensions.Tests.Helpers
{
    public class HangfirePerformerFixture
    {
        private readonly IStorageConnection _storage = Substitute.For<IStorageConnection>();
        private readonly IJobCancellationToken _token = Substitute.For<IJobCancellationToken>();

        public IStorageConnection Storage => _storage;
        public IJobCancellationToken Token => _token;

        public IBackgroundJobPerformer CreatePerformer(JobActivator activator)
        {
            var memberInfo = Type.GetType("Hangfire.Server.CoreBackgroundJobPerformer,Hangfire.Core");
            if (memberInfo != null)
            {
                var ctor = memberInfo.
                    GetConstructors(BindingFlags.Instance | BindingFlags.Public)
                    .Single();

                return (IBackgroundJobPerformer) ctor.Invoke(new object[] { activator });
            }

            throw new Exception("Type does not exist.");
        }

        public PerformContext CreateContext(BackgroundJob job)
        {
            return new PerformContext(_storage, job, _token);
        }

        public BackgroundJob CreateBackgroundJob(Job job)
        {
            return new BackgroundJob("JobId", job, DateTime.UtcNow);
        }
    }
}
