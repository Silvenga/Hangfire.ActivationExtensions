namespace Hangfire.ActivationExtensions.Interceptor
{
    using System;

    public interface IJobActivatorFilter
    {
        void OnMaterializing(Type jobType);

        void OnMaterialized(Type jobType, object activatedJob);

        void OnScopeCreating(JobActivatorContext context);

        JobActivatorScope OnScopeCreated(JobActivatorContext context, JobActivatorScope createdScope);
    }
}