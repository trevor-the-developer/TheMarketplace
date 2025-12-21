using FluentValidation;
using Marketplace.Core.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace Marketplace.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<MediaCreateWithFileValidator>();
        return services;
    }
}