using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ActionFiltersExample.Filters
{
    public class SimpleLoggingActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var controllerName = context.RouteData.Values["controller"];
            var actionName = context.RouteData.Values["action"];
            
            // Używamy Console.WriteLine zamiast ILogger (bo nie mamy DI)
            Console.WriteLine($"=== SIMPLE FILTER - WYKONYWANIE AKCJI ===");
            Console.WriteLine($"Controller: {controllerName}");
            Console.WriteLine($"Action: {actionName}");
            Console.WriteLine($"Czas: {DateTime.Now:HH:mm:ss.fff}");
            Console.WriteLine($"Parametry: {string.Join(", ", context.ActionArguments.Select(kvp => $"{kvp.Key}={kvp.Value}"))}");
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var controllerName = context.RouteData.Values["controller"];
            var actionName = context.RouteData.Values["action"];
            
            Console.WriteLine($"=== SIMPLE FILTER - ZAKOŃCZENIE AKCJI ===");
            Console.WriteLine($"Controller: {controllerName}");
            Console.WriteLine($"Action: {actionName}");
            Console.WriteLine($"Czas: {DateTime.Now:HH:mm:ss.fff}");
            Console.WriteLine($"Status: {(context.Exception != null ? "BŁĄD" : "SUKCES")}");
            Console.WriteLine($"");
        }
    }
} 