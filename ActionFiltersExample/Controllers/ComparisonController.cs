using Microsoft.AspNetCore.Mvc;
using ActionFiltersExample.Filters;

namespace ActionFiltersExample.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComparisonController : ControllerBase
    {
        private readonly ILogger<ComparisonController> _logger;

        public ComparisonController(ILogger<ComparisonController> logger)
        {
            _logger = logger;
        }

        // 1. IActionFilter z ServiceFilter (zalecane)
        [HttpGet("service-filter")]
        [ServiceFilter(typeof(LoggingActionFilter))]
        public IActionResult ServiceFilterExample()
        {
            _logger.LogInformation("Akcja z ServiceFilter (IActionFilter)");
            return Ok(new { Message = "ServiceFilter - pełne wsparcie DI", Timestamp = DateTime.Now });
        }

        // 2. ActionFilterAttribute - prosty (bez DI)
        [HttpGet("simple-attribute")]
        [SimpleLoggingActionFilter]
        public IActionResult SimpleAttributeExample()
        {
            _logger.LogInformation("Akcja z SimpleLoggingActionFilterAttribute");
            return Ok(new { Message = "Simple Attribute - bez DI", Timestamp = DateTime.Now });
        }

        // 3. ActionFilterAttribute - zaawansowany (z ręcznym DI)
        [HttpGet("advanced-attribute")]
        [AdvancedLoggingActionFilter]
        public IActionResult AdvancedAttributeExample()
        {
            _logger.LogInformation("Akcja z AdvancedLoggingActionFilterAttribute");
            return Ok(new { Message = "Advanced Attribute - ręczne DI", Timestamp = DateTime.Now });
        }

        // 4. Porównanie - wszystkie trzy filtry na raz
        [HttpGet("all-filters")]
        [ServiceFilter(typeof(LoggingActionFilter))]
        [SimpleLoggingActionFilter]
        [AdvancedLoggingActionFilter]
        public IActionResult AllFiltersExample()
        {
            _logger.LogInformation("Akcja ze wszystkimi typami filtrów");
            return Ok(new { 
                Message = "Wszystkie filtry - zobacz różnice w logach", 
                Timestamp = DateTime.Now,
                Note = "Sprawdź konsolę - zobaczysz 3 różne style logowania"
            });
        }

        // 5. Test z parametrami
        [HttpGet("with-params/{id}")]
        [ServiceFilter(typeof(LoggingActionFilter))]
        [SimpleLoggingActionFilter]
        public IActionResult WithParamsExample(int id, [FromQuery] string name = "test")
        {
            _logger.LogInformation($"Akcja z parametrami: id={id}, name={name}");
            return Ok(new { 
                Id = id, 
                Name = name, 
                Message = "Zobacz jak filtry logują parametry",
                Timestamp = DateTime.Now 
            });
        }
    }
} 