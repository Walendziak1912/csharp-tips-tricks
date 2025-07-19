using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ActionFiltersExample.Filters
{
    public class CacheActionFilter : IActionFilter
    {
        private static readonly Dictionary<string, (object Result, DateTime Expiry)> _cache = new();
        private readonly ILogger<CacheActionFilter> _logger;

        public CacheActionFilter(ILogger<CacheActionFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var cacheKey = GenerateCacheKey(context);
            
            if (_cache.TryGetValue(cacheKey, out var cachedItem))
            {
                if (DateTime.Now < cachedItem.Expiry)
                {
                    _logger.LogInformation($"Zwracam wynik z cache dla klucza: {cacheKey}");
                    context.Result = new OkObjectResult(cachedItem.Result);
                    return;
                }
                else
                {
                    _logger.LogInformation($"Usuwam wygasły cache dla klucza: {cacheKey}");
                    _cache.Remove(cacheKey);
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is OkObjectResult okResult)
            {
                var cacheKey = GenerateCacheKey(context);
                var expiry = DateTime.Now.AddMinutes(5); // Cache na 5 minut
                
                _cache[cacheKey] = (okResult.Value!, expiry);
                _logger.LogInformation($"Zapisano wynik do cache z kluczem: {cacheKey}, wygasa: {expiry:HH:mm:ss}");
            }
        }

        private string GenerateCacheKey(ActionExecutingContext context)
        {
            var controller = context.RouteData.Values["controller"];
            var action = context.RouteData.Values["action"];
            var parameters = string.Join("_", context.ActionArguments.Select(kvp => $"{kvp.Key}_{kvp.Value}"));
            
            return $"{controller}_{action}_{parameters}";
        }

        private string GenerateCacheKey(ActionExecutedContext context)
        {
            var controller = context.RouteData.Values["controller"];
            var action = context.RouteData.Values["action"];
            
            return $"{controller}_{action}";
        }
    }
} 