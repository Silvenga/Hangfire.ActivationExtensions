using System;

namespace Hangfire.ActivationExtensions.Interceptor
{
    public class PassThroughScope : JobActivatorScope
    {
        private readonly JobActivatorFilterCollection _filterCollection;
        private readonly JobActivatorScope _scope;
        private readonly JobActivatorContext _context;
        private object _activatedJob;
        private Type _type;

        public PassThroughScope(JobActivatorFilterCollection filterCollection, JobActivatorScope scope, JobActivatorContext context)
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
            _context = context;
        }

        public override object Resolve(Type type)
        {
            if (_type != default(Type))
            {
                throw new NotSupportedException(
                    $"{nameof(Resolve)} was called more than once, this should not normally occur. Please contact the developer of {nameof(ActivationExtensions)}.");
            }
            _type = type;

            foreach (var filter in _filterCollection.Filters)
            {
                filter.OnMaterializing(type, _context);
            }

            _activatedJob = _scope.Resolve(type);

            foreach (var filter in _filterCollection.Filters)
            {
                filter.OnMaterialized(type, _activatedJob, _context);
            }

            return _activatedJob;
        }

        public override void DisposeScope()
        {
            foreach (var filter in _filterCollection.Filters)
            {
                filter.OnScopeDisposing(_type, _activatedJob, _context );
            }

            _scope.DisposeScope();

            foreach (var filter in _filterCollection.Filters)
            {
                filter.OnScopeDisposed(_type, _activatedJob, _context);
            }
        }
    }
}