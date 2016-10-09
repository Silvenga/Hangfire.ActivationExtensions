using System.Collections.Generic;

namespace Hangfire.ActivationExtensions.Interceptor
{
    public class JobActivatorFilterCollection
    {
        public ICollection<IJobActivatorFilter> Filters { get; set; }

        public JobActivatorFilterCollection()
        {
        }

        public JobActivatorFilterCollection(ICollection<IJobActivatorFilter> filters)
        {
            Filters = filters;
        }

        public JobActivatorFilterCollection(params IJobActivatorFilter[] filter)
        {
            Filters = filter;
        }
    }
}