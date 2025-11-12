using MediatR;
using System.Diagnostics;

namespace IRRRL.Web.Features.Common.Behaviors;

/// <summary>
/// MediatR pipeline behavior that logs every request/response
/// Helps with debugging and performance monitoring
/// </summary>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        
        _logger.LogInformation("Handling {RequestName}", requestName);
        
        var stopwatch = Stopwatch.StartTime();
        
        try
        {
            var response = await next();
            
            var elapsed = stopwatch.GetElapsedTime();
            _logger.LogInformation(
                "Handled {RequestName} in {ElapsedMilliseconds}ms",
                requestName,
                elapsed.TotalMilliseconds);
            
            return response;
        }
        catch (Exception ex)
        {
            var elapsed = stopwatch.GetElapsedTime();
            _logger.LogError(
                ex,
                "Error handling {RequestName} after {ElapsedMilliseconds}ms",
                requestName,
                elapsed.TotalMilliseconds);
            throw;
        }
    }
}

