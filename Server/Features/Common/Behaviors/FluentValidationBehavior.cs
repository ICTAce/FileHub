using FluentValidation;

namespace ICTAce.FileHub.Features.Common.Behaviors;

/// <summary>
/// MediatR pipeline behavior that validates requests using FluentValidation
/// Automatically runs before handlers execute
/// </summary>
public class FluentValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly ILogManager _logger;

    public FluentValidationBehavior(
        IEnumerable<IValidator<TRequest>> validators,
        ILogManager logger)
    {
        _validators = validators;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // If no validators registered, skip validation
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        // Run all validators in parallel
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken))
        );

        // Collect all failures
        var failures = validationResults
            .SelectMany(result => result.Errors)
            .Where(failure => failure != null)
            .ToList();

        if (failures.Any())
        {
            var requestName = typeof(TRequest).Name;
            var errors = string.Join("; ", failures.Select(f => $"{f.PropertyName}: {f.ErrorMessage}"));

            _logger.Log(LogLevel.Warning, this, LogFunction.Other,
                "Validation failed for {RequestName}: {Errors}",
                requestName, errors);

            // Throw validation exception with detailed error information
            throw new ValidationException(failures);
        }

        return await next();
    }
}
