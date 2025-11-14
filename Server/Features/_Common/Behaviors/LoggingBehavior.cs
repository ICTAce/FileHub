// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.Common.Behaviors;

/// <summary>
/// Pipeline behavior that logs all requests and their execution time
/// </summary>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogManager _logger;

    public LoggingBehavior(ILogManager logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            _logger.Log(LogLevel.Information, this, LogFunction.Other, 
                "Handling {RequestName}", requestName);

            var response = await next();

            stopwatch.Stop();
            _logger.Log(LogLevel.Information, this, LogFunction.Other,
                "Handled {RequestName} in {ElapsedMilliseconds}ms", 
                requestName, stopwatch.ElapsedMilliseconds);

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.Log(LogLevel.Error, this, LogFunction.Other,
                "Error handling {RequestName} after {ElapsedMilliseconds}ms: {Error}",
                requestName, stopwatch.ElapsedMilliseconds, ex.Message);
            throw;
        }
    }
}
