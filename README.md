# Hangfire.ActivationExtensions

Generic extensions for Hangfire's job activation. Provides an implementation to customize job activation during job creation. 

## Usage

The package provides an extension method for the IGlobalConfiguration interface, so you can enable activator interceptor integration using the `GlobalConfiguration` class.

The below example uses the `NinjectActivator` from `Hangfire.Ninject`. 

```csharp
IKernel kernel = new StandardKernel();
// kernel.Bind...
// IJobActivatorFilter activationFilter = new ...

GlobalConfiguration.Configuration
                .UseNinjectActivator(kernel);
                .UseActivatorInterceptor(activationFilter);
```

The variable `activationFilter` should be an implementation of `IJobActivatorFilter`. The interface is as below:

```csharp
public interface IJobActivatorFilter
{
    /// <summary>
    /// Called before the job is constructed by the current activator.
    /// </summary>
    /// <param name="jobType">The type of the job being created.</param>
    void OnMaterializing(Type jobType);

    /// <summary>
    /// Called after the job is constructed by the current activator.
    /// </summary>
    /// <param name="jobType">The type of the job being created.</param>
    /// <param name="activatedJob">The object created by the activator. Should be of type found within jobType.</param>
    void OnMaterialized(Type jobType, object activatedJob);

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
    JobActivatorScope OnScopeCreated(JobActivatorContext context, JobActivatorScope createdScope);
}
    }
```

## TODO

- [ ] Handle usage with default activator
- [ ] Docs
- [ ] Nuget package
- [ ] Setup CI
- [ ] Test `GlobalConfigurationExtensions`