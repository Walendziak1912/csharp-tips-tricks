# Action Filters Example

Ten projekt demonstruje różne typy Action Filtrów w ASP.NET Core.

## Czym są Action Filtry?

Action Filtry to mechanizm w ASP.NET Core, który pozwala na wykonywanie kodu przed i po wykonaniu akcji kontrolera. Są to atrybuty, które można zastosować do:
- Kontrolerów (wszystkie akcje)
- Poszczególnych akcji
- Globalnie (wszystkie akcje w aplikacji)

## Typy Action Filtrów

### 1. IActionFilter
- `OnActionExecuting` - wykonywane przed akcją
- `OnActionExecuted` - wykonywane po akcji

### 2. IAsyncActionFilter
- Asynchroniczna wersja IActionFilter

### 3. IAuthorizationFilter
- Wykonywane przed IActionFilter
- Używane do autoryzacji

### 4. IExceptionFilter
- Wykonywane gdy wystąpi wyjątek

### 5. IResultFilter
- Wykonywane przed i po zwróceniu wyniku

## IActionFilter vs ActionFilterAttribute

### IActionFilter (ZALECANE)
```csharp
public class LoggingActionFilter : IActionFilter
{
    private readonly ILogger<LoggingActionFilter> _logger;
    
    public LoggingActionFilter(ILogger<LoggingActionFilter> logger)
    {
        _logger = logger; // Pełne wsparcie DI!
    }
}
```

**Zalety:**
- ✅ Pełne wsparcie Dependency Injection
- ✅ Łatwiejsze testowanie
- ✅ Czystszy kod
- ✅ Większa elastyczność

**Użycie:**
```csharp
[ServiceFilter(typeof(LoggingActionFilter))]
public IActionResult GetUsers() { }
```

### ActionFilterAttribute
```csharp
public class SimpleLoggingActionFilterAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Brak DI w konstruktorze!
        Console.WriteLine("Logowanie...");
    }
}
```

**Wady:**
- ❌ Brak DI w konstruktorze
- ❌ Trudniejsze testowanie
- ❌ Ręczne pobieranie serwisów

**Użycie:**
```csharp
[SimpleLoggingActionFilter]
public IActionResult GetUsers() { }
```

## Filtry w tym projekcie

### 1. LoggingActionFilter (IActionFilter)
**Cel**: Logowanie informacji o wykonywanych akcjach
**Funkcjonalność**:
- Loguje nazwę kontrolera i akcji
- Loguje parametry wejściowe
- Loguje czas wykonania
- Loguje status (sukces/błąd)

**Użycie**:
```csharp
[ServiceFilter(typeof(LoggingActionFilter))]
public class UserController : ControllerBase
```

### 2. ValidationActionFilter (IActionFilter)
**Cel**: Walidacja parametrów wejściowych
**Funkcjonalność**:
- Sprawdza czy parametry int nie są ujemne
- Sprawdza czy parametry string nie są puste
- Zwraca BadRequest z listą błędów

**Użycie**:
```csharp
[ServiceFilter(typeof(ValidationActionFilter))]
public IActionResult GetUser(int id)
```

### 3. PerformanceActionFilter (IActionFilter)
**Cel**: Pomiar wydajności akcji
**Funkcjonalność**:
- Mierzy czas wykonania akcji
- Loguje czas wykonania
- Ostrzega gdy akcja trwa dłużej niż 1 sekundę

**Użycie**:
```csharp
[ServiceFilter(typeof(PerformanceActionFilter))]
public IActionResult GetUsers()
```

### 4. CacheActionFilter (IActionFilter)
**Cel**: Cachowanie wyników akcji
**Funkcjonalność**:
- Cache na 5 minut
- Klucz cache bazuje na nazwie kontrolera, akcji i parametrach
- Automatyczne usuwanie wygasłych wpisów

**Użycie**:
```csharp
[ServiceFilter(typeof(CacheActionFilter))]
public IActionResult GetUsers()
```

### 5. SimpleLoggingActionFilterAttribute (ActionFilterAttribute)
**Cel**: Demonstracja prostego ActionFilterAttribute
**Funkcjonalność**:
- Logowanie bez DI (Console.WriteLine)
- Prosty przykład bez zależności

**Użycie**:
```csharp
[SimpleLoggingActionFilter]
public IActionResult GetUsers()
```

### 6. AdvancedLoggingActionFilterAttribute (ActionFilterAttribute)
**Cel**: Demonstracja ActionFilterAttribute z ręcznym DI
**Funkcjonalność**:
- Ręczne pobieranie serwisów z IServiceProvider
- Pokazuje jak używać DI w ActionFilterAttribute

**Użycie**:
```csharp
[AdvancedLoggingActionFilter]
public IActionResult GetUsers()
```

## Jak uruchomić

1. Uruchom projekt:
```bash
dotnet run
```

2. Otwórz Swagger UI: `https://localhost:7001/swagger`

3. Użyj pliku `ActionFiltersExample.http` do testowania

## Przykłady testów

### Test walidacji
```http
GET /api/user/-1
```
Powinno zwrócić błąd walidacji (ujemne ID)

### Test cache
```http
GET /api/user
```
Pierwsze wywołanie - normalny czas
Drugie wywołanie - wynik z cache (szybszy)

### Test wydajności
```http
GET /api/user/slow
```
Powinno wygenerować ostrzeżenie o długim czasie wykonania

### Porównanie filtrów
```http
GET /api/comparison/all-filters
```
Zobacz różnice między IActionFilter a ActionFilterAttribute w logach

## Rejestracja filtrów

Filtry IActionFilter muszą być zarejestrowane w kontenerze DI w `Program.cs`:

```csharp
builder.Services.AddScoped<LoggingActionFilter>();
builder.Services.AddScoped<ValidationActionFilter>();
builder.Services.AddScoped<PerformanceActionFilter>();
builder.Services.AddScoped<CacheActionFilter>();
```

ActionFilterAttribute NIE wymaga rejestracji!

## Sposoby użycia filtrów

### 1. ServiceFilter (zalecane dla IActionFilter)
```csharp
[ServiceFilter(typeof(LoggingActionFilter))]
public IActionResult GetUsers()
```

### 2. TypeFilter
```csharp
[TypeFilter(typeof(LoggingActionFilter))]
public IActionResult GetUsers()
```

### 3. Atrybut (tylko dla ActionFilterAttribute)
```csharp
[SimpleLoggingActionFilter]
public IActionResult GetUsers()
```

## Kolejność wykonywania

1. IAuthorizationFilter
2. IActionFilter.OnActionExecuting
3. Akcja kontrolera
4. IActionFilter.OnActionExecuted
5. IResultFilter.OnResultExecuting
6. Wynik
7. IResultFilter.OnResultExecuted

## Logi

Sprawdź logi w konsoli, aby zobaczyć działanie filtrów:
- Informacje o wykonywaniu akcji
- Czas wykonania
- Ostrzeżenia o wydajności
- Informacje o cache
- Różnice między typami filtrów 