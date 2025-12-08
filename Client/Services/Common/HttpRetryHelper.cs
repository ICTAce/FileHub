// Licensed to ICTAce under the MIT license.

using System.Net;

namespace ICTAce.FileHub.Services.Common;

/// <summary>
/// Provides retry functionality for HTTP operations with exponential backoff.
/// </summary>
public static class HttpRetryHelper
{
    /// <summary>
    /// Default maximum number of retry attempts.
    /// </summary>
    public const int DefaultMaxRetries = 3;

    /// <summary>
    /// Default initial delay between retries in milliseconds.
    /// </summary>
    public const int DefaultInitialDelayMs = 500;

    /// <summary>
    /// Executes an HTTP operation with retry logic using exponential backoff.
    /// </summary>
    /// <typeparam name="T">The return type of the operation.</typeparam>
    /// <param name="operation">The async operation to execute.</param>
    /// <param name="maxRetries">Maximum number of retry attempts (default: 3).</param>
    /// <param name="initialDelayMs">Initial delay in milliseconds before first retry (default: 500ms).</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>The result of the operation.</returns>
    /// <exception cref="HttpRequestException">Thrown when all retry attempts fail.</exception>
    public static async Task<T> ExecuteWithRetryAsync<T>(
        Func<Task<T>> operation,
        int maxRetries = DefaultMaxRetries,
        int initialDelayMs = DefaultInitialDelayMs,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(operation);

        var lastException = default(Exception);

        for (var attempt = 0; attempt <= maxRetries; attempt++)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                return await operation().ConfigureAwait(false);
            }
            catch (HttpRequestException ex) when (IsTransientError(ex) && attempt < maxRetries)
            {
                lastException = ex;
                var delay = CalculateDelay(attempt, initialDelayMs);
                await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
            }
            catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested && attempt < maxRetries)
            {
                // Timeout - treat as transient
                lastException = ex;
                var delay = CalculateDelay(attempt, initialDelayMs);
                await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
            }
        }

        throw new HttpRequestException(
            $"Operation failed after {maxRetries + 1} attempts.",
            lastException);
    }

    /// <summary>
    /// Executes an HTTP operation with retry logic using exponential backoff (void return).
    /// </summary>
    /// <param name="operation">The async operation to execute.</param>
    /// <param name="maxRetries">Maximum number of retry attempts (default: 3).</param>
    /// <param name="initialDelayMs">Initial delay in milliseconds before first retry (default: 500ms).</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <exception cref="HttpRequestException">Thrown when all retry attempts fail.</exception>
    public static async Task ExecuteWithRetryAsync(
        Func<Task> operation,
        int maxRetries = DefaultMaxRetries,
        int initialDelayMs = DefaultInitialDelayMs,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(operation);

        await ExecuteWithRetryAsync(
            async () =>
            {
                await operation().ConfigureAwait(false);
                return true;
            },
            maxRetries,
            initialDelayMs,
            cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Determines if an HTTP exception represents a transient error that should be retried.
    /// </summary>
    private static bool IsTransientError(HttpRequestException ex)
    {
        // Retry on connection failures, timeouts, and server errors (5xx)
        if (ex.StatusCode.HasValue)
        {
            var statusCode = (int)ex.StatusCode.Value;
            return statusCode >= 500 || // Server errors
                   ex.StatusCode == HttpStatusCode.RequestTimeout ||
                   ex.StatusCode == HttpStatusCode.TooManyRequests;
        }

        // No status code means connection failure - retry
        return true;
    }

    /// <summary>
    /// Calculates the delay for the next retry attempt using exponential backoff with jitter.
    /// </summary>
    private static TimeSpan CalculateDelay(int attempt, int initialDelayMs)
    {
        // Exponential backoff: initialDelay * 2^attempt
        var exponentialDelay = initialDelayMs * Math.Pow(2, attempt);

        // Add jitter (±25%) to prevent thundering herd
        var jitter = Random.Shared.NextDouble() * 0.5 + 0.75; // 0.75 to 1.25
        var delayWithJitter = exponentialDelay * jitter;

        // Cap at 30 seconds
        var cappedDelay = Math.Min(delayWithJitter, 30000);

        return TimeSpan.FromMilliseconds(cappedDelay);
    }
}
