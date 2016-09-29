namespace Hangfire.ActivationExtensions.Interceptor
{
    using System;

    public interface IJobActivatorFilter
    {
        /// <summary>
        /// Called before the job is constructed by the current activator.
        /// </summary>
        /// <param name="jobType">The type of the job being created.</param>
        void OnMaterializing(Type jobType);

        /// <summary>
        /// Called after the job is constructed by the current activator.
        /// </summary>
        /// <param name="jobType">The type of the job being created.</param>
        /// <param name="activatedJob">The object created by the activator. Should be of type found within jobType.</param>
        void OnMaterialized(Type jobType, object activatedJob);

        /// <summary>
        /// Called before the scope is constructed by the current activator.
        /// </summary>
        /// <param name="context">Context of the activator.</param>
        void OnScopeCreating(JobActivatorContext context);

        /// <summary>
        /// Called after the scope is constructed by the current activator.
        /// </summary>
        /// <param name="context">Context of the activator.</param>
        /// <param name="createdScope">The scope created by the activator.</param>
        void OnScopeCreated(JobActivatorContext context, JobActivatorScope createdScope);

        /// <summary>
        /// 
        /// </summary>
        void OnScopeDisposing(Type jobType, object activatedJob);

        /// <summary>
        /// 
        /// </summary>
        void OnScopeDisposed(Type jobType, object activatedJob);
    }
}