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
                        x.AttributeType==typeof(SingletonAttribute) || 
                        x.AttributeType==typeof(ScopedAttribute)    || 
                        x.AttributeType==typeof(ScopedAttribute)
                )
            );
        
        foreach (var defined in singletonDefined)
        {
            if(defined.CustomAttributes.Any(custom => custom.AttributeType==typeof(SingletonAttribute)))
                collection.TryAdd(new ServiceDescriptor(defined,defined,ServiceLifetime.Singleton));
            else if(defined.CustomAttributes.Any(custom => custom.AttributeType==typeof(ScopedAttribute)))
                collection.TryAdd(new ServiceDescriptor(defined,defined,ServiceLifetime.Scoped));
            else if(defined.CustomAttributes.Any(custom => custom.AttributeType==typeof(TransientAttribute)))
                collection.TryAdd(new ServiceDescriptor(defined,defined,ServiceLifetime.Transient));
        }
    }
}