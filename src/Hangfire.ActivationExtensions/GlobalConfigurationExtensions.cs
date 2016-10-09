using System;
using System.Collections.Generic;
using System.Linq;

using Hangfire.ActivationExtensions.Interceptor;
using Hangfire.Annotations;

namespace Hangfire.ActivationExtensions
{
    public static class GlobalConfigurationExtensions
    {
        private static IGlobalConfiguration<PassThroughActivator> InternalUseActivator(this IGlobalConfiguration configuration,
                                                                                       JobActivatorFilterCollection filterCollection,
                                                                                       JobActivator currentActivator)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            return configuration.UseActivator(new PassThroughActivator(filterCollection, currentActivator));
        }

        public static IGlobalConfiguration<PassThroughActivator> UseActivatorInterceptor<T>([NotNull] this IGlobalConfiguration<T> configuration,
                                                                                            JobActivatorFilterCollection filterCollection,
                                                                                            JobActivator currentActivator = null) where T : JobActivator
        {
            if (filterCollection == null)
            {
                throw new ArgumentNullException(nameof(filterCollection));
            }
            return configuration.InternalUseActivator(filterCollection, currentActivator ?? configuration.Entry);
        }

        public static IGlobalConfiguration<PassThroughActivator> UseActivatorInterceptor<T>([NotNull] this IGlobalConfiguration<T> configuration,
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

        public static IGlobalConfiguration<PassThroughActivator> UseActivatorInterceptor<T>([NotNull] this IGlobalConfiguration<T> configuration,
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

        public static IGlobalConfiguration<PassThroughActivator> UseDefaultActivatorInterceptor([NotNull] this IGlobalConfiguration configuration,
                                                                                                JobActivatorFilterCollection filterCollection,
                                                                                                JobActivator currentActivator = null)
        {
            if (filterCollection == null)
            {
                throw new ArgumentNullException(nameof(filterCollection));
            }
            return configuration.InternalUseActivator(filterCollection, currentActivator ?? JobActivator.Current);
        }

        public static IGlobalConfiguration<PassThroughActivator> UseDefaultActivatorInterceptor([NotNull] this IGlobalConfiguration configuration,
                                                                                                IEnumerable<IJobActivatorFilter> filters,
                                                                                                JobActivator currentActivator = null)
        {
            if (filters == null)
            {
                throw new ArgumentNullException(nameof(filters));
            }

            var filterCollection = new JobActivatorFilterCollection
            {
                Filters = filters.ToList()
            };
            return configuration.UseDefaultActivatorInterceptor(filterCollection, currentActivator);
        }

        public static IGlobalConfiguration<PassThroughActivator> UseDefaultActivatorInterceptor([NotNull] this IGlobalConfiguration configuration,
                                                                                                IJobActivatorFilter filter,
                                                                                                JobActivator currentActivator = null)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            var filterCollection = new JobActivatorFilterCollection
            {
                Filters = new List<IJobActivatorFilter> {filter}
            };
            return configuration.UseDefaultActivatorInterceptor(filterCollection, currentActivator);
        }
    }
}