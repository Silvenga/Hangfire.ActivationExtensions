namespace Hangfire.ActivationExtensions.Interceptor
{
    using System;

    public class PassThroughJobActivator : JobActivator
    {
        private readonly JobActivatorFilterCollection _filterCollection;
        private readonly JobActivator _activator;

        public PassThroughJobActivator(JobActivatorFilterCollection filterCollection, JobActivator activator)
        {
            if (filterCollection == null)
            {
                throw new ArgumentNullException(nameof(filterCollection));
            }
            if (activator == null)
            {
                throw new ArgumentNullException(nameof(activator));
            }

            _filterCollection = filterCollection;
            _activator = activator;
        }

        public override object ActivateJob(Type jobType)
        {
            foreach (var filter in _filterCollection.Filters)
            {
                filter.OnMaterializing(jobType);
            }

            var activatedJob = _activator.ActivateJob(jobType);

            foreach (var filter in _filterCollection.Filters)
            {
                filter.OnMaterialized(jobType, activatedJob);
            }

            return activatedJob;
        }

        [Obsolete("Please implement/use the BeginScope(JobActivatorContext) method instead. Will be removed in 2.0.0.")]
        public override JobActivatorScope BeginScope()
        {
            throw new NotImplementedException($"Call of obsolete {nameof(BeginScope)} is not supported."); // TODO
        }

        public override JobActivatorScope BeginScope(JobActivatorContext context)
        {
            foreach (var filter in _filterCollection.Filters)
            {
                filter.OnScopeCreating(context);
            }

            var scope = _activator.BeginScope(context);

            foreach (var filter in _filterCollection.Filters)
            {
                filter.OnScopeCreated(context, scope);
            }

            return scope;
        }
    }
}