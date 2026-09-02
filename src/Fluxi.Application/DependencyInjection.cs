using FluentValidation;
using Fluxi.Application.Abstractions.Behaviors;
using Fluxi.Application.Abstractions.Messaging;
using Fluxi.SharedKernel.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Fluxi.Application;

public static class DependencyInjection
{
    #region Methods

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.Scan(scan => scan.FromAssembliesOf(typeof(DependencyInjection))
            .AddClasses(classes => classes
                .AssignableTo(typeof(IQueryHandler<,>))
                .Where(type => !type.IsGenericTypeDefinition), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime()
            .AddClasses(classes => classes
                .AssignableTo(typeof(ICommandHandler<>))
                .Where(type => !type.IsGenericTypeDefinition), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime()
            .AddClasses(classes => classes
                .AssignableTo(typeof(ICommandHandler<,>))
                .Where(type => !type.IsGenericTypeDefinition), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        DecorateRegisteredServices(
            services,
            typeof(ICommandHandler<,>),
            typeof(ValidationDecorator.CommandHandler<,>));
        DecorateRegisteredServices(
            services,
            typeof(ICommandHandler<>),
            typeof(ValidationDecorator.CommandBaseHandler<>));

        DecorateRegisteredServices(
            services,
            typeof(IQueryHandler<,>),
            typeof(LoggingDecorator.QueryHandler<,>));
        DecorateRegisteredServices(
            services,
            typeof(ICommandHandler<,>),
            typeof(LoggingDecorator.CommandHandler<,>));
        DecorateRegisteredServices(
            services,
            typeof(ICommandHandler<>),
            typeof(LoggingDecorator.CommandBaseHandler<>));

        services.Scan(scan => scan.FromAssembliesOf(typeof(DependencyInjection))
            .AddClasses(classes => classes.AssignableTo(typeof(IDomainEventHandler<>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);

        return services;
    }

    private static void DecorateRegisteredServices(
        IServiceCollection services,
        Type serviceTypeDefinition,
        Type decoratorTypeDefinition)
    {
        Type[] registeredServiceTypes = [
            .. services
                .Where(descriptor =>
                    descriptor.ServiceType.IsGenericType &&
                    !descriptor.ServiceType.IsGenericTypeDefinition &&
                    descriptor.ServiceType.GetGenericTypeDefinition() == serviceTypeDefinition)
                .Select(descriptor => descriptor.ServiceType)
                .Distinct()
        ];

        foreach (Type registeredServiceType in registeredServiceTypes)
        {
            Type decoratorType = decoratorTypeDefinition
                .MakeGenericType(registeredServiceType.GetGenericArguments());

            services.Decorate(registeredServiceType, decoratorType);
        }
    }

    #endregion
}