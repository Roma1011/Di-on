![6703159](https://github.com/user-attachments/assets/3480d234-190c-4c78-afe2-1f4c0bd81918)

# DiÆon
## ⚡ Overview
This project provides a utility for automatically registering services in a .NET application using custom attributes (`Singleton`, `Scoped`, and `Transient`). It simplifies the process of configuring dependency injection by scanning assemblies and registering services based on their lifetime.


## 🚀 Features

- 🛠️ **Custom Attributes**: Use `[Singleton]`, `[Scoped]`, and `[Transient]` to define the lifetime of your services.
- 🔄 **Automatic Registration**: Automatically scans assemblies and registers services with the appropriate lifetime in the `IServiceCollection`.
- 🏷️ **Interface Binding**: Automatically binds implementations to their interfaces if defined.


### 📦 Installation
Just clone or include the DiÆon class in your project. No external dependencies are required.

## 📖 Usage
### 🔷 1. Define your services and annotate them with the appropriate lifetime attribute:
```
using DiÆon.Attributes;

[Scoped]
public Interface IMyService
{
    // Implementation
}

[Scoped]
public class MyScopedService : IMyService
{
    // Implementation
}


[Singleton]
public class MyScopedService
{
    // Implementation
}


```
### 🔷 2. Use the Aggregator class to register services in your Startup or Program class:
```
using DiÆon.LifeTimeAggregator;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

var services = new ServiceCollection();
var aggregator = new Aggregator();

aggregator.AggregateLifeTime(Assembly.GetExecutingAssembly(), ref services);
```
### Attributes
```
[Singleton]: Registers the service with a singleton lifetime.
[Scoped]: Registers the service with a scoped lifetime.
[Transient]: Registers the service with a transient lifetime.
```

### ⚠️ Error Handling
```

```
### 👨‍💻 Contributing
Feel free to submit issues or create pull requests to enhance DiÆon.


