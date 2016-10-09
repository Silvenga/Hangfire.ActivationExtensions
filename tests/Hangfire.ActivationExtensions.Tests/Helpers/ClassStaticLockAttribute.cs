using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading;

using Xunit.Sdk;

namespace Hangfire.ActivationExtensions.Tests.Helpers
{
    // Orig by https://github.com/odinserj
    // https://github.com/HangfireIO/Hangfire/blob/129707d66fde24dc6379fb9d6b15fa0b8ca48605/tests/Hangfire.Core.Tests/Utils/StaticLockAttribute.cs

    public class ClassStaticLockAttribute : BeforeAfterTestAttribute
    {
        private readonly ConcurrentDictionary<Type, object> _locks = new ConcurrentDictionary<Type, object>();

        public override void Before(MethodInfo methodUnderTest)
        {
            var type = GetDeclaringType(methodUnderTest);
            _locks.TryAdd(type, new object());

            Monitor.Enter(_locks[type]);
        }

        public override void After(MethodInfo methodUnderTest)
        {
            var type = GetDeclaringType(methodUnderTest);

            Monitor.Exit(_locks[type]);
        }

        private static Type GetDeclaringType(MethodInfo methodInfo)
        {
            return methodInfo.DeclaringType;
        }
    }
}