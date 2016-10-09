using System.Collections.Generic;
using System.Linq;

namespace Hangfire.ActivationExtensions.Interceptor
{
    public class JobActivatorFilterCollection
    {
        public IList<IJobActivatorFilter> Filters { get; set; }

        public JobActivatorFilterCollection()
        {
        }

        public JobActivatorFilterCollection(ICollection<IJobActivatorFilter> filters)
        {
            Filters = filters.ToList();
        }

        public JobActivatorFilterCollection(params IJobActivatorFilter[] filter)
        {
            Filters = filter.ToList();
        }
    }
}