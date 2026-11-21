using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using Rira.Application.Common.Exceptions;
using Rira.Domain.Common;

namespace Rira.Application.Common.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var timer = Stopwatch.StartNew();
        logger.LogInformation("Handling {RequestName}", requestName);

        try
        {
            var response = await next();
            logger.LogInformation("Handled {RequestName} in {ElapsedMs} ms", requestName, timer.ElapsedMilliseconds);
            return response;
        }
        catch (DomainException exception)
        {
            var errorCodes = string.Join(", ", exception.Errors.Select(error => error.Code));
            logger.LogWarning("Expected domain failure handling {RequestName} in {ElapsedMs} ms. Error codes: {ErrorCodes}", requestName, timer.ElapsedMilliseconds, errorCodes);
            throw;
        }
        catch (Exception exception) when (exception is NotFoundException or ConflictException)
        {
            logger.LogWarning("Expected failure handling {RequestName} in {ElapsedMs} ms: {Failure}", requestName, timer.ElapsedMilliseconds, exception.Message);
            throw;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unexpected failure handling {RequestName} in {ElapsedMs} ms", requestName, timer.ElapsedMilliseconds);
            throw;
        }
    }
}
