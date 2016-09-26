namespace Hangfire.ActivationExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Hangfire.ActivationExtensions.Interceptor;
    using Hangfire.Annotations;

    public static class GlobalConfigurationExtensions
    {
        public static IGlobalConfiguration<PassThroughJobActivator> UseActivatorInterceptor<T>([NotNull] this IGlobalConfiguration<T> configuration,
                                                                                               JobActivatorFilterCollection filterCollection,
                                                                                               JobActivator currentActivator = null) where T : JobActivator
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            if (filterCollection == null)
            {
                throw new ArgumentNullException(nameof(filterCollection));
            }

            return configuration.UseActivator(new PassThroughJobActivator(filterCollection, currentActivator ?? JobActivator.Current));
        }

        public static IGlobalConfiguration<PassThroughJobActivator> UseActivatorInterceptor<T>([NotNull] this IGlobalConfiguration<T> configuration,
                                                                                               IEnumerable<IJobActivatorFilter> filters,
                                                                                               JobActivator currentActivator = null) where T : JobActivator
        {
            if (filters == null)
            {
                throw new ArgumentNullException(nameof(filters));
            }

            var filterCollection = new JobActivatorFilterCollection
            {
                Filters = filters.ToList()
            };
            return configuration.UseActivatorInterceptor(filterCollection, currentActivator);
        }

        public static IGlobalConfiguration<PassThroughJobActivator> UseActivatorInterceptor<T>([NotNull] this IGlobalConfiguration<T> configuration,
                                                                                               IJobActivatorFilter filter,
                                                                                               JobActivator currentActivator = null) where T : JobActivator
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            var filterCollection = new JobActivatorFilterCollection
            {
                Filters = new List<IJobActivatorFilter> {filter}
            };
            return configuration.UseActivatorInterceptor(filterCollection, currentActivator);
        }
    }
}