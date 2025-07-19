using Microsoft.AspNetCore.Mvc;
using ActionFiltersExample.Filters;

namespace ActionFiltersExample.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(LoggingActionFilter))] // Globalny filter dla całego kontrolera
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [ServiceFilter(typeof(CacheActionFilter))] // Cache dla listy użytkowników
        [ServiceFilter(typeof(PerformanceActionFilter))] // Pomiar wydajności
        public IActionResult GetUsers()
        {
            _logger.LogInformation("Pobieranie listy użytkowników");
            
            // Symulacja opóźnienia
            Thread.Sleep(100);
            
            var users = new[]
            {
                new { Id = 1, Name = "Jan Kowalski", Email = "jan@example.com" },
                new { Id = 2, Name = "Anna Nowak", Email = "anna@example.com" },
                new { Id = 3, Name = "Piotr Wiśniewski", Email = "piotr@example.com" }
            };

            return Ok(users);
        }

        [HttpGet("{id}")]
        [ServiceFilter(typeof(ValidationActionFilter))] // Walidacja parametru id
        [ServiceFilter(typeof(PerformanceActionFilter))] // Pomiar wydajności
        public IActionResult GetUser(int id)
        {
            _logger.LogInformation($"Pobieranie użytkownika o ID: {id}");
            
            // Symulacja opóźnienia
            Thread.Sleep(50);
            
            var user = new { Id = id, Name = $"Użytkownik {id}", Email = $"user{id}@example.com" };
            
            return Ok(user);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationActionFilter))] // Walidacja danych wejściowych
        [ServiceFilter(typeof(PerformanceActionFilter))] // Pomiar wydajności
        public IActionResult CreateUser([FromBody] CreateUserRequest request)
        {
            _logger.LogInformation($"Tworzenie użytkownika: {request.Name}");
            
            // Symulacja opóźnienia
            Thread.Sleep(200);
            
            var newUser = new { Id = Random.Shared.Next(100, 1000), Name = request.Name, Email = request.Email };
            
            return CreatedAtAction(nameof(GetUser), new { id = newUser.Id }, newUser);
        }

        [HttpPut("{id}")]
        [ServiceFilter(typeof(ValidationActionFilter))] // Walidacja parametrów
        [ServiceFilter(typeof(PerformanceActionFilter))] // Pomiar wydajności
        public IActionResult UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            _logger.LogInformation($"Aktualizacja użytkownika o ID: {id}");
            
            // Symulacja opóźnienia
            Thread.Sleep(150);
            
            var updatedUser = new { Id = id, Name = request.Name, Email = request.Email };
            
            return Ok(updatedUser);
        }

        [HttpDelete("{id}")]
        [ServiceFilter(typeof(ValidationActionFilter))] // Walidacja parametru id
        [ServiceFilter(typeof(PerformanceActionFilter))] // Pomiar wydajności
        public IActionResult DeleteUser(int id)
        {
            _logger.LogInformation($"Usuwanie użytkownika o ID: {id}");
            
            // Symulacja opóźnienia
            Thread.Sleep(100);
            
            return NoContent();
        }

        [HttpGet("slow")]
        [ServiceFilter(typeof(PerformanceActionFilter))] // Ten filter pokaże ostrzeżenie o długim czasie wykonania
        public IActionResult SlowOperation()
        {
            _logger.LogInformation("Wykonywanie wolnej operacji");
            
            // Symulacja bardzo wolnej operacji
            Thread.Sleep(2000);
            
            return Ok(new { Message = "Wolna operacja zakończona", Timestamp = DateTime.Now });
        }
    }

    public class CreateUserRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class UpdateUserRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
} 