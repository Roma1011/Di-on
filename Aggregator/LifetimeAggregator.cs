using System.Reflection;
using DiÆon.Attributes;
using DiÆon.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DiÆon.Aggregator;

public class LifetimeAggregator
{
    public void Aggregate(Assembly assembly,ref ServiceCollection collection)
    {
        var singletonDefined = assembly.DefinedTypes.
            Where(definedTypes => !definedTypes.IsInterface && definedTypes.CustomAttributes.
                Any(x
                    =>
                        x.AttributeType==typeof(Singleton) || 
                        x.AttributeType==typeof(Scoped)    || 
                        x.AttributeType==typeof(Transient)
                )
            );
        
        foreach (var defined in singletonDefined)
        {
            if(defined.CustomAttributes.Select(x =>x.AttributeType.BaseType).Count() > 1)
                throw new MultipleLifetimeException();
            
            if (defined.CustomAttributes.Any(custom => custom.AttributeType == typeof(Singleton)))
            {
                var declaredInterface=defined.ImplementedInterfaces.
                    FirstOrDefault(type=>type.CustomAttributes.
                        Any(customAttributeData => customAttributeData.AttributeType==typeof(Singleton)));
                
                if(declaredInterface is not null)
                    collection.TryAdd(new ServiceDescriptor(declaredInterface,defined,ServiceLifetime.Singleton));
                else
                    collection.TryAdd(new ServiceDescriptor(defined,defined,ServiceLifetime.Singleton));
                
                continue;
            }
            
            if (defined.CustomAttributes.Any(custom => custom.AttributeType == typeof(Scoped)))
            {
                var declaredInterface=defined.ImplementedInterfaces.
                    FirstOrDefault(type=>type.CustomAttributes.
                        Any(customAttributeData => customAttributeData.AttributeType==typeof(Scoped)));
                
                if(declaredInterface is not null)
                    collection.TryAdd(new ServiceDescriptor(declaredInterface,defined,ServiceLifetime.Scoped));
                else
                    collection.TryAdd(new ServiceDescriptor(defined,defined,ServiceLifetime.Scoped));
                
                continue;
            }
            
            if (defined.CustomAttributes.Any(custom => custom.AttributeType == typeof(Transient)))
            {
                var declaredInterface=defined.ImplementedInterfaces.
                    FirstOrDefault(type=>type.CustomAttributes.
                        Any(customAttributeData => customAttributeData.AttributeType==typeof(Transient)));
                
                if(declaredInterface is not null)
                    collection.TryAdd(new ServiceDescriptor(declaredInterface,defined,ServiceLifetime.Transient));
                else
                    collection.TryAdd(new ServiceDescriptor(defined,defined,ServiceLifetime.Transient));
            }
        }
    }
}