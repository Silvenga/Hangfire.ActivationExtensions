namespace Hangfire.ActivationExtensions.Interceptor
{
    using System;

    public class PassThroughScope : JobActivatorScope
    {
        private readonly JobActivatorFilterCollection _filterCollection;
        private readonly JobActivatorScope _scope;
        private object _activatedJob;
        private Type _type;

        public PassThroughScope(JobActivatorFilterCollection filterCollection, JobActivatorScope scope)
        {
            if (filterCollection == null)
            {
                throw new ArgumentNullException(nameof(filterCollection));
            }
            if (scope == null)
            {
                throw new ArgumentNullException(nameof(scope));
            }
            _filterCollection = filterCollection;
            _scope = scope;
        }

        public override object Resolve(Type type)
        {
            _type = type;
            foreach (var filter in _filterCollection.Filters)
            {
                filter.OnMaterializing(type);
            }

            _activatedJob = _scope.Resolve(type);

            foreach (var filter in _filterCollection.Filters)
            {
                filter.OnMaterialized(type, _activatedJob);
            }

            return _activatedJob;
        }

        public override void DisposeScope()
        {
            foreach (var filter in _filterCollection.Filters)
            {
                filter.OnScopeDisposing(_type, _activatedJob);
            }

            _scope.DisposeScope();

            foreach (var filter in _filterCollection.Filters)
            {
                filter.OnScopeDisposed(_type, _activatedJob);
            }
        }
    }
}