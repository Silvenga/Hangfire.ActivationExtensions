namespace Hangfire.ActivationExtensions.Interceptor
{
    using System.Collections.Generic;

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