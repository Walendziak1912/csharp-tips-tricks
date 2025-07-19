using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace ActionFiltersExample.Filters
{
    public class PerformanceActionFilter : IActionFilter
    {
        private readonly ILogger<PerformanceActionFilter> _logger;
        private Stopwatch? _stopwatch;

        public PerformanceActionFilter(ILogger<PerformanceActionFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _stopwatch = Stopwatch.StartNew();
            _logger.LogInformation($"Rozpoczęcie pomiaru wydajności dla akcji: {context.RouteData.Values["action"]}");
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _stopwatch?.Stop();
            
            if (_stopwatch != null)
            {
                var elapsedMs = _stopwatch.ElapsedMilliseconds;
                var actionName = context.RouteData.Values["action"];
                
                _logger.LogInformation($"Akcja {actionName} wykonana w {elapsedMs}ms");
                
                if (elapsedMs > 1000)
                {
                    _logger.LogWarning($"Akcja {actionName} trwała dłużej niż 1 sekundę: {elapsedMs}ms");
                }
            }
        }
    }
} 