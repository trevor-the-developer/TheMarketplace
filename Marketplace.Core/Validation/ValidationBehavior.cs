using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Marketplace.Core.Validation
{
    /// <summary>
    /// Service to handle FluentValidation in handlers
    /// </summary>
    public interface IValidationService
    {
        Task<Result<T>> ValidateAsync<T>(T request) where T : class;
        Task<List<string>> ValidateAndGetErrorsAsync<T>(T request) where T : class;
    }

    public class ValidationService : IValidationService
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<Result<T>> ValidateAsync<T>(T request) where T : class
        {
            var validator = _serviceProvider.GetService<IValidator<T>>();
            if (validator == null)
            {
                // No validator found, return success
                return Result<T>.Success(request);
            }

            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<T>.ValidationFailure("Validation failed", errors);
            }

            return Result<T>.Success(request);
        }

        public async Task<List<string>> ValidateAndGetErrorsAsync<T>(T request) where T : class
        {
            var validator = _serviceProvider.GetService<IValidator<T>>();
            if (validator == null)
            {
                return new List<string>();
            }

            var validationResult = await validator.ValidateAsync(request);
            return validationResult.Errors.Select(e => e.ErrorMessage).ToList();
        }
    }
}
