using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ActionFiltersExample.Filters
{
    public class AdvancedLoggingActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Ręczne pobieranie serwisów z DI (bardziej skomplikowane)
            var logger = context.HttpContext.RequestServices
                .GetService<ILogger<AdvancedLoggingActionFilterAttribute>>();
            
            var controllerName = context.RouteData.Values["controller"];
            var actionName = context.RouteData.Values["action"];
            
            logger?.LogInformation($"=== ADVANCED FILTER - WYKONYWANIE AKCJI ===");
            logger?.LogInformation($"Controller: {controllerName}");
            logger?.LogInformation($"Action: {actionName}");
            logger?.LogInformation($"Czas: {DateTime.Now:HH:mm:ss.fff}");
            logger?.LogInformation($"Parametry: {string.Join(", ", context.ActionArguments.Select(kvp => $"{kvp.Key}={kvp.Value}"))}");
            
            // Możemy też pobrać inne serwisy
            var configuration = context.HttpContext.RequestServices
                .GetService<IConfiguration>();
            
            var environment = context.HttpContext.RequestServices
                .GetService<IWebHostEnvironment>();
            
            logger?.LogInformation($"Environment: {environment?.EnvironmentName}");
            logger?.LogInformation($"ConnectionString: {configuration?["ConnectionStrings:DefaultConnection"]}");
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var logger = context.HttpContext.RequestServices
                .GetService<ILogger<AdvancedLoggingActionFilterAttribute>>();
            
            var controllerName = context.RouteData.Values["controller"];
            var actionName = context.RouteData.Values["action"];
            
            logger?.LogInformation($"=== ADVANCED FILTER - ZAKOŃCZENIE AKCJI ===");
            logger?.LogInformation($"Controller: {controllerName}");
            logger?.LogInformation($"Action: {actionName}");
            logger?.LogInformation($"Czas: {DateTime.Now:HH:mm:ss.fff}");
            logger?.LogInformation($"Status: {(context.Exception != null ? "BŁĄD" : "SUKCES")}");
            logger?.LogInformation($"");
        }
    }
} 