// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.Common.Behaviors;

/// <summary>
/// Pipeline behavior that validates requests before they reach handlers
/// </summary>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogManager _logger;

    public ValidationBehavior(ILogManager logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        // Validate request properties
        if (!TryValidateRequest(request, out var validationErrors))
        {
            _logger.Log(LogLevel.Warning, this, LogFunction.Other,
                "Validation failed for {RequestName}: {Errors}",
                requestName, string.Join(", ", validationErrors));

            // Throw validation exception
            throw new InvalidOperationException($"Validation failed: {string.Join(", ", validationErrors)}");
        }

        return await next();
    }

    private bool TryValidateRequest(TRequest request, out List<string> errors)
    {
        errors = new List<string>();

        // Basic validation - you can enhance this with FluentValidation
        var properties = typeof(TRequest).GetProperties();
        foreach (var prop in properties)
        {
            var value = prop.GetValue(request);
            
            // Check for required strings
            if (prop.PropertyType == typeof(string))
            {
                var stringValue = value as string;
                if (string.IsNullOrWhiteSpace(stringValue))
                {
                    // Check if property has [Required] attribute
                    var isRequired = prop.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.RequiredAttribute), false).Any();
                    if (isRequired)
                    {
                        errors.Add($"{prop.Name} is required");
                    }
                }
            }
        }

        return errors.Count == 0;
    }
}
