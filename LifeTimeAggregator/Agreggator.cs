using System.Reflection;
using DiÆon.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DiÆon.LifeTimeAggregator;

public class Aggregator
{
    public void AggregateLifeTime(Assembly assembly,ref ServiceCollection collection)
    {
        var singletonDefined = assembly.DefinedTypes.
            Where(definedTypes => definedTypes.CustomAttributes.
                Any(x
                    =>
                        x.AttributeType==typeof(Singleton) || 
                        x.AttributeType==typeof(Scoped)    || 
                        x.AttributeType==typeof(Scoped)
                )
            );
        
        foreach (var defined in singletonDefined)
        {
            if(defined.CustomAttributes.Any(custom => custom.AttributeType==typeof(Singleton)))
                collection.TryAdd(new ServiceDescriptor(defined,defined,ServiceLifetime.Singleton));
            else if(defined.CustomAttributes.Any(custom => custom.AttributeType==typeof(Scoped)))
                collection.TryAdd(new ServiceDescriptor(defined,defined,ServiceLifetime.Scoped));
            else if(defined.CustomAttributes.Any(custom => custom.AttributeType==typeof(Transient)))
                collection.TryAdd(new ServiceDescriptor(defined,defined,ServiceLifetime.Transient));
        }
    }
}