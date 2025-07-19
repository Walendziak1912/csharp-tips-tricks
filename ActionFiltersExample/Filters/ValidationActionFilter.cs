using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ActionFiltersExample.Filters
{
    public class ValidationActionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Sprawdź czy są parametry do walidacji
            if (!context.ActionArguments.Any())
                return;

            var errors = new List<string>();

            // Sprawdź parametry typu int
            foreach (var argument in context.ActionArguments)
            {
                if (argument.Value is int intValue)
                {
                    if (intValue < 0)
                    {
                        errors.Add($"Parametr {argument.Key} nie może być ujemny");
                    }
                }
                
                if (argument.Value is string stringValue)
                {
                    if (string.IsNullOrWhiteSpace(stringValue))
                    {
                        errors.Add($"Parametr {argument.Key} nie może być pusty");
                    }
                }
            }

            // Jeśli są błędy, zwróć BadRequest
            if (errors.Any())
            {
                context.Result = new BadRequestObjectResult(new
                {
                    Message = "Błędy walidacji",
                    Errors = errors
                });
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Nic nie robimy po wykonaniu akcji
        }
    }
} 