namespace Hangfire.ActivationExtensions.Interceptor
{
    using System.Collections.Generic;

    public class JobActivatorFilterCollection
    {
        public ICollection<IJobActivatorFilter> Filters { get; set; }
    }
}