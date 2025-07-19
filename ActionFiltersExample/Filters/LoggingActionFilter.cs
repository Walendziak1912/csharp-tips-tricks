using Microsoft.AspNetCore.Mvc.Filters;

namespace ActionFiltersExample.Filters
{
    public class LoggingActionFilter : IActionFilter
    {
        private readonly ILogger<LoggingActionFilter> _logger;

        public LoggingActionFilter(ILogger<LoggingActionFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var controllerName = context.RouteData.Values["controller"];
            var actionName = context.RouteData.Values["action"];
            
            _logger.LogInformation($"=== WYKONYWANIE AKCJI ===");
            _logger.LogInformation($"Controller: {controllerName}");
            _logger.LogInformation($"Action: {actionName}");
            _logger.LogInformation($"Czas: {DateTime.Now:HH:mm:ss.fff}");
            _logger.LogInformation($"Parametry: {string.Join(", ", context.ActionArguments.Select(kvp => $"{kvp.Key}={kvp.Value}"))}");
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            var controllerName = context.RouteData.Values["controller"];
            var actionName = context.RouteData.Values["action"];
            
            _logger.LogInformation($"=== ZAKOŃCZENIE AKCJI ===");
            _logger.LogInformation($"Controller: {controllerName}");
            _logger.LogInformation($"Action: {actionName}");
            _logger.LogInformation($"Czas: {DateTime.Now:HH:mm:ss.fff}");
            _logger.LogInformation($"Status: {(context.Exception != null ? "BŁĄD" : "SUKCES")}");
            _logger.LogInformation($"");
        }
    }
} 