using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BuildingBlocks.Behaviors.LoggingBehavior
{
    public class LoggingBehaviorr<TRequest, TResponse>(ILogger<LoggingBehaviorr<TRequest, TResponse>> logger) :
        IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull, IRequest<TResponse>
        where TResponse : notnull
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            logger.LogInformation("[Start] Handling {RequestName} with content: {@Request}", typeof(TRequest).Name, request);
            var timer = new Stopwatch();
            timer.Start();
            var response = await next();
            timer.Stop();
            var timeSpan = timer.Elapsed;
            if (timeSpan.Seconds >= 2)
            {
                logger.LogWarning("Handled {RequestName} in {ElapsedMilliseconds} ms with content: {@Request}", typeof(TRequest).Name, timeSpan.TotalMilliseconds, request);
            }
            logger.LogInformation("[Ended] {RequestName} in {ElapsedMilliseconds} ms with Response: {@Response}", typeof(TRequest).Name, timeSpan.TotalMilliseconds, response);
            return response;
        }
    }
}