using System.Reflection;
using DiÆon.Attributes;
using DiÆon.Attributes.Base;
using DiÆon.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace DiÆon.Aggregator;

public static class Aggregator
{
//-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Scans the provided assembly for non-interface types annotated with 
    /// lifetime attributes (<see cref="Singleton"/>, <see cref="Scoped"/>, or <see cref="Transient"/>),
    /// and registers them in the <see cref="IServiceCollection"/> with the corresponding service lifetime.
    /// 
    /// If a type is decorated with more than one lifetime attribute, a <see cref="MultipleLifetimeException"/> is thrown.
    /// The method also attempts to find a corresponding interface annotated with the same attribute to use for registration,
    /// otherwise it registers the implementation type as itself.
    /// </summary>
    /// <param name="collection">The <see cref="IServiceCollection"/> to which the services will be added.</param>
    /// <param name="assembly">The assembly to scan for types with lifetime attributes.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> with all discovered and registered services.</returns>
//-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public static IServiceCollection AggregateLifeTime(this IServiceCollection collection,Assembly assembly)
    {
        IEnumerable<TypeInfo> typesWithLifetimeAttributes = assembly.DefinedTypes.
            Where(definedTypes => !definedTypes.IsInterface && definedTypes.CustomAttributes.
                Any(attr
                    =>
                    attr.AttributeType == typeof(Singleton) || 
                    attr.AttributeType == typeof(Scoped)    || 
                    attr.AttributeType == typeof(Transient)
                )
            );
        
        foreach (var typeInfo in typesWithLifetimeAttributes)
        {
            if(typeInfo.CustomAttributes.Count(x => x.AttributeType.BaseType == typeof(BaseLifetimeAttribute)) > 1)
                throw new MultipleLifetimeException();
            
            var lifetimeAttributeType = typeInfo.CustomAttributes
                .First(attr =>
                    attr.AttributeType == typeof(Scoped)    ||
                    attr.AttributeType == typeof(Transient) ||
                    attr.AttributeType == typeof(Singleton)).AttributeType;

            var descriptor = LifeSeeker(typeInfo, lifetimeAttributeType);
            collection.Add(descriptor);
        }
        return collection;
    }
//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- 
    /// <summary>
    /// Creates a <see cref="ServiceDescriptor"/> based on the specified implementation type and its associated lifetime attribute.
    /// 
    /// If the implementation type implements an interface that is also annotated with the same lifetime attribute,
    /// that interface will be used as the service type; otherwise, the implementation type is used as its own service type.
    /// </summary>
    /// <param name="implementationType">The concrete implementation type to be registered.</param>
    /// <param name="lifetimeAttributeType">The type of the lifetime attribute (e.g., <see cref="Scoped"/>, <see cref="Transient"/>, or <see cref="Singleton"/>).</param>
    /// <returns>A <see cref="ServiceDescriptor"/> representing the service registration.</returns>
//-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private static ServiceDescriptor LifeSeeker(TypeInfo implementationType,Type lifetimeAttributeType)
    {
        Type? serviceInterface=implementationType.ImplementedInterfaces.
            FirstOrDefault(type=>type.CustomAttributes.
                Any(customAttributeData => customAttributeData.AttributeType == lifetimeAttributeType));
                
        if (serviceInterface is not null)
        {
            if (serviceInterface.IsGenericType)
                serviceInterface = serviceInterface.GetGenericTypeDefinition();
                    
            return new ServiceDescriptor(serviceInterface,implementationType,GetLifetimeFromAttribute(lifetimeAttributeType));
        } else {
            return new ServiceDescriptor(implementationType,implementationType,GetLifetimeFromAttribute(lifetimeAttributeType));
        }
    }
//-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Maps a lifetime attribute type to a corresponding <see cref="ServiceLifetime"/> enum value.
    /// </summary>
    /// <param name="attributeType">The type of the attribute (must be <see cref="Scoped"/>, <see cref="Transient"/>, or <see cref="Singleton"/>).</param>
    /// <returns>The corresponding <see cref="ServiceLifetime"/>.</returns>
//-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private static ServiceLifetime GetLifetimeFromAttribute(Type attributeType)
    {
        return attributeType == typeof(Scoped)    ? ServiceLifetime.Scoped    :
               attributeType == typeof(Transient) ? ServiceLifetime.Transient :
            ServiceLifetime.Singleton;
    }
//-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
}
