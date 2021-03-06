# Hangfire.ActivationExtensions

[![AppVeyor](https://img.shields.io/appveyor/ci/Silvenga/hangfire-activationextensions.svg?maxAge=2592000&style=flat-square)](https://ci.appveyor.com/project/Silvenga/hangfire-activationextensions)

Generic extensions for Hangfire's job activation. Provides an implementation to customize job activation during job creation. 

## Install

Latest releases can be found on [MyGet](https://www.myget.org/F/silvenga/api/v2).

> TODO: Create Nuget.org packages.
```
Install-Package Hangfire.ActivationExtensions
```

## Usage

The package provides an extension method for the IGlobalConfiguration interface, so you can enable the activator interceptor integration using the `GlobalConfiguration` class.

The below example uses the `NinjectActivator` from `Hangfire.Ninject`. 

```csharp
IKernel kernel = new StandardKernel();
// kernel.Bind...
// IJobActivatorFilter activationFilter = new ...

GlobalConfiguration.Configuration
                .UseNinjectActivator(kernel);
                .UseActivatorInterceptor(activationFilter);
```

OR using the default activator:

```csharp
// IJobActivatorFilter activationFilter = new ...

GlobalConfiguration.Configuration
                .UseDefaultActivatorInterceptor(activationFilter);
```

The variable `activationFilter` should be an implementation of `IJobActivatorFilter`. The interface is as below:

```csharp
public interface IJobActivatorFilter
{
    /// <summary>
    /// Called before the job is constructed by the current activator.
    /// </summary>
    /// <param name="jobType">The type of the job being created.</param>
    /// <param name="context"></param>
    void OnMaterializing(Type jobType, [CanBeNull] JobActivatorContext context);

    /// <summary>
    /// Called after the job is constructed by the current activator.
    /// </summary>
    /// <param name="jobType">The type of the job being created.</param>
    /// <param name="activatedJob">The object created by the activator. Should be of type found within jobType.</param>
    /// <param name="context"></param>
    void OnMaterialized(Type jobType, object activatedJob, [CanBeNull] JobActivatorContext context);

    /// <summary>
    /// Called before the scope is constructed by the current activator.
    /// </summary>
    /// <param name="context">Context of the activator.</param>
    void OnScopeCreating(JobActivatorContext context);

    /// <summary>
    /// Called after the scope is constructed by the current activator.
    /// </summary>
    /// <param name="context">Context of the activator.</param>
    /// <param name="createdScope">The scope created by the activator.</param>
    void OnScopeCreated(JobActivatorContext context, JobActivatorScope createdScope);

    /// <summary>
    /// Called before the scope has been disposed.
    /// </summary>
    /// <param name="jobType">The last type requested from the activator.</param>
    /// <param name="activatedJob">The last object materialized from the activator.</param>
    /// <param name="context"></param>
    void OnScopeDisposing(Type jobType, object activatedJob, JobActivatorContext context);

    /// <summary>
    /// Called after the scope has been disposed. This is normally when all objects within the scrope have been cleaned up.
    /// </summary>
    /// <param name="jobType">The last type requested from the activator.</param>
    /// <param name="activatedJob">The last object materialized from the activator.</param>
    /// <param name="context"></param>
    void OnScopeDisposed(Type jobType, object activatedJob, JobActivatorContext context);
}
```

## TODO

- [X] Handle usage with default activator
- [ ] Docs
- [ ] Nuget package
- [X] Setup CI
- [ ] Test `GlobalConfigurationExtensions`
- [X] Test using Hangfire directly
- [X] Support `JobActivatorContext` within hooks
- [X] Support filter merging