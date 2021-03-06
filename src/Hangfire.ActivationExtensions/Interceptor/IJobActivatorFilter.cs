﻿using System;

using JetBrains.Annotations;

namespace Hangfire.ActivationExtensions.Interceptor
{
    public interface IJobActivatorFilter
    {
        /// <summary>
        /// Called before the job is constructed by the current activator.
        /// </summary>
        /// <param name="jobType">The type of the job being created.</param>
        /// <param name="context"></param>
        void OnMaterializing(Type jobType, [CanBeNull] JobActivatorContext context);

        /// <summary>
        /// Called after the job is constructed by the current activator.
        /// </summary>
        /// <param name="jobType">The type of the job being created.</param>
        /// <param name="activatedJob">The object created by the activator. Should be of type found within jobType.</param>
        /// <param name="context"></param>
        void OnMaterialized(Type jobType, object activatedJob, [CanBeNull] JobActivatorContext context);

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
        /// Called before the scope has been disposed.
        /// </summary>
        /// <param name="jobType">The last type requested from the activator.</param>
        /// <param name="activatedJob">The last object materialized from the activator.</param>
        /// <param name="context"></param>
        void OnScopeDisposing(Type jobType, object activatedJob, JobActivatorContext context);

        /// <summary>
        /// Called after the scope has been disposed. This is normally when all objects within the scrope have been cleaned up.
        /// </summary>
        /// <param name="jobType">The last type requested from the activator.</param>
        /// <param name="activatedJob">The last object materialized from the activator.</param>
        /// <param name="context"></param>
        void OnScopeDisposed(Type jobType, object activatedJob, JobActivatorContext context);
    }
}