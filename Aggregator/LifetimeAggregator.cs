using System.Reflection;
using DiÆon.Attributes;
using DiÆon.Attributes.Base;
using DiÆon.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DiÆon.Aggregator;

public static class Aggregator
{
    public static IServiceCollection AggregateLifeTime(this IServiceCollection collection,Assembly assembly)
    {
        var definedTypes = assembly.DefinedTypes.
            Where(definedTypes => !definedTypes.IsInterface && definedTypes.CustomAttributes.
                    Any(x
                        =>
                            x.AttributeType==typeof(Singleton) || 
                            x.AttributeType==typeof(Scoped)    || 
                            x.AttributeType==typeof(Transient)
                    )
            );
        
        foreach (var defined in definedTypes)
        {
            if(defined.CustomAttributes.Count(x => x.AttributeType.BaseType == typeof(BaseLifetimeAttribute)) > 1)
                throw new MultipleLifetimeException();
            
            if (defined.CustomAttributes.Any(custom => custom.AttributeType == typeof(Singleton)))
            {
                var declaredInterface=defined.ImplementedInterfaces.
                    FirstOrDefault(type=>type.CustomAttributes.
                        Any(customAttributeData => customAttributeData.AttributeType==typeof(Singleton)));
                
                if (declaredInterface is not null)
                {
                    if (declaredInterface.IsGenericType)
                        declaredInterface = declaredInterface.GetGenericTypeDefinition();
                    
                    collection.TryAdd(new ServiceDescriptor(declaredInterface,defined,ServiceLifetime.Singleton));
                } else {
                    collection.TryAdd(new ServiceDescriptor(defined,defined,ServiceLifetime.Singleton));
                }
                    
                continue;
            }
            
            if (defined.CustomAttributes.Any(custom => custom.AttributeType == typeof(Scoped)))
            {
                var declaredInterface=defined.ImplementedInterfaces.
                    FirstOrDefault(type=>type.CustomAttributes.
                        Any(customAttributeData => customAttributeData.AttributeType==typeof(Scoped)));

                if (declaredInterface is not null)
                {
                    if (declaredInterface.IsGenericType)
                        declaredInterface = declaredInterface.GetGenericTypeDefinition();
                    
                    collection.TryAdd(new ServiceDescriptor(declaredInterface,defined,ServiceLifetime.Scoped));
                } else {
                    collection.TryAdd(new ServiceDescriptor(defined,defined,ServiceLifetime.Scoped));
                }
                
                continue;
            }
            
            if (defined.CustomAttributes.Any(custom => custom.AttributeType == typeof(Transient)))
            {
                var declaredInterface=defined.ImplementedInterfaces.
                    FirstOrDefault(type=>type.CustomAttributes.
                        Any(customAttributeData => customAttributeData.AttributeType==typeof(Transient)));

                if (declaredInterface is not null)
                {
                    if (declaredInterface.IsGenericType)
                        declaredInterface = declaredInterface.GetGenericTypeDefinition();
                    
                    collection.TryAdd(new ServiceDescriptor(declaredInterface, defined, ServiceLifetime.Transient));
                } else {
                    collection.TryAdd(new ServiceDescriptor(defined,defined,ServiceLifetime.Transient));
                }
            }
        }
        return collection;
    }
}