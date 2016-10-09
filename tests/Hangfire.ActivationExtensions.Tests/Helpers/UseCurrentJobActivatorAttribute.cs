using System;
using System.Reflection;

namespace Hangfire.ActivationExtensions.Tests.Helpers
{
    public class UseCurrentJobActivatorAttribute : ClassStaticLockAttribute
    {
        [ThreadStatic] private static JobActivator _threadJobActivator;

        public override void Before(MethodInfo methodUnderTest)
        {
            base.Before(methodUnderTest);
            _threadJobActivator = JobActivator.Current;
        }

        public override void After(MethodInfo methodUnderTest)
        {
            JobActivator.Current = _threadJobActivator;
            base.After(methodUnderTest);
        }
    }
}