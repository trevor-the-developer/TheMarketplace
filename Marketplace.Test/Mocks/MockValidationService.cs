using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.Core;
using Marketplace.Core.Validation;

namespace Marketplace.Test.Mocks;

public class MockValidationService : IValidationService
{
    private readonly MockValidationServiceOptions _options;

    public MockValidationService(MockValidationServiceOptions? options = null)
    {
        _options = options ?? new MockValidationServiceOptions();
    }

    public Task<Result<T>> ValidateAsync<T>(T request) where T : class
    {
        if (_options.ValidationErrors.Any())
            return Task.FromResult(Result<T>.ValidationFailure("Validation failed", _options.ValidationErrors));

        return Task.FromResult(Result<T>.Success(request));
    }

    public Task<List<string>> ValidateAndGetErrorsAsync<T>(T request) where T : class
    {
        return Task.FromResult(_options.ValidationErrors.ToList());
    }
}

public class MockValidationServiceOptions
{
    public List<string> ValidationErrors { get; set; } = new();
}