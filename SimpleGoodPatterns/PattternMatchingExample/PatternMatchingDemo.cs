using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleGoodPatterns.PattternMatchingExample
{

    public abstract record Shape;
    public record Circle(double Radius) : Shape;
    public record Rectangle(double Width, double Height) : Shape;
    public record Triangle(double Base, double Height) : Shape;
    public record Person(string Name, int Age, string City);
    public enum WeatherType { Sunny, Rainy, Cloudy, Snowy }

    public class PatternMatchingDemo
    {
        // 1. Podstawowy switch expression
        public static string GetDayType(DayOfWeek day) => day switch
        {
            DayOfWeek.Monday or DayOfWeek.Tuesday or DayOfWeek.Wednesday
                or DayOfWeek.Thursday or DayOfWeek.Friday => "Dzień roboczy",
            DayOfWeek.Saturday or DayOfWeek.Sunday => "Weekend",
            _ => "Nieznany dzień"
        };

        // 2. Pattern matching z typami (type patterns)
        public static double CalculateArea(Shape shape) => shape switch
        {
            Circle { Radius: var r } => Math.PI * r * r,
            Rectangle { Width: var w, Height: var h } => w * h,
            Triangle { Base: var b, Height: var h } => 0.5 * b * h,
            _ => throw new ArgumentException("Nieznany kształt")
        };

        // 3. Pattern matching z właściwościami (property patterns)
        public static string ClassifyPerson(Person person) => person switch
        {
            { Age: < 18 } => "Dziecko",
            { Age: >= 18 and < 65, City: "Warszawa" } => "Dorosły z Warszawy",
            { Age: >= 18 and < 65 } => "Dorosły",
            { Age: >= 65 } => "Senior",
            _ => "Niesklasyfikowany"
        };
        // 4. Pattern matching z guards (when clauses w starszym stylu)
        public static string GetActivitySuggestion(WeatherType weather, int temperature) => weather switch
        {
            WeatherType.Sunny when temperature > 25 => "Idź na plażę!",
            WeatherType.Sunny when temperature > 15 => "Spacer w parku",
            WeatherType.Sunny => "Słońce, ale chłodno - weź kurtkę",
            WeatherType.Rainy => "Zostań w domu z książką",
            WeatherType.Cloudy when temperature > 20 => "Dobra pogoda na jogging",
            WeatherType.Cloudy => "Może pójść do centrum handlowego",
            WeatherType.Snowy => "Czas na narten!",
            _ => "Sprawdź prognozę pogody"
        };

        // 5. Tuple patterns - porównywanie wielu wartości jednocześnie
        public static string GetRockPaperScissorsResult(string player1, string player2) => (player1, player2) switch
        {
            ("kamień", "nożyce") or ("papier", "kamień") or ("nożyce", "papier") => "Gracz 1 wygrywa!",
            ("nożyce", "kamień") or ("kamień", "papier") or ("papier", "nożyce") => "Gracz 2 wygrywa!",
            var (p1, p2) when p1 == p2 => "Remis!",
            _ => "Nieprawidłowy ruch"
        };

        // 6. Pattern matching z kolekcjami (C# 11+)
        public static string AnalyzeNumbers(int[] numbers) => numbers switch
        {
            [] => "Pusta tablica",
            [var single] => $"Jeden element: {single}",
            [var first, var second] => $"Dwa elementy: {first}, {second}",
            [var first, .., var last] => $"Pierwszy: {first}, ostatni: {last}",
            _ => "Więcej elementów"
        };

        // 7. Nested patterns - zagnieżdżone wzorce
        public static string ProcessData(object data) => data switch
        {
            Person { Name: var name, Age: > 18 } => $"Dorosła osoba: {name}",
            List<int> { Count: > 0 } list when list.All(x => x > 0) => "Lista dodatnich liczb",
            List<string> { Count: 0 } => "Pusta lista stringów",
            Dictionary<string, int> dict when dict.ContainsKey("score") => $"Wynik: {dict["score"]}",
            null => "Brak danych",
            _ => "Nieznany typ danych"
        };

        // 8. Relational patterns (C# 9+)
        public static string GetAgeGroup(int age) => age switch
        {
            < 0 => "Nieprawidłowy wiek",
            >= 0 and < 13 => "Dziecko",
            >= 13 and < 20 => "Nastolatek",
            >= 20 and < 60 => "Dorosły",
            >= 60 => "Senior",
        };

        // 9. Logical patterns z not
        public static bool IsWorkingHour(int hour) => hour switch
        {
            >= 9 and <= 17 and not 12 => true, // 9-17 ale nie 12 (przerwa obiadowa)
            _ => false
        };

        // 10. Pattern matching w LINQ
        public static void LinqWithPatterns()
        {
            var shapes = new List<Shape>
            {
                new Circle(5),
                new Rectangle(10, 20),
                new Triangle(8, 12)
            };

            var areas = shapes.Select(shape => shape switch
            {
                Circle c => Math.PI * c.Radius * c.Radius,
                Rectangle r => r.Width * r.Height,
                Triangle t => 0.5 * t.Base * t.Height,
                _ => 0
            }).ToList();

            Console.WriteLine($"Pola powierzchni: {string.Join(", ", areas.Select(a => a.ToString("F2")))}");
        }
    }

    public class Program
    {
        static void Main(string[] args)
        {
            // Test różnych pattern matching examples
            Console.WriteLine("=== Pattern Matching Examples ===\n");

            // 1. Podstawowy switch
            Console.WriteLine($"Poniedziałek: {PatternMatchingDemo.GetDayType(DayOfWeek.Monday)}");
            Console.WriteLine($"Sobota: {PatternMatchingDemo.GetDayType(DayOfWeek.Saturday)}\n");

            // 2. Obliczanie pól
            var circle = new Circle(5);
            var rectangle = new Rectangle(10, 20);
            Console.WriteLine($"Pole koła o promieniu 5: {PatternMatchingDemo.CalculateArea(circle):F2}");
            Console.WriteLine($"Pole prostokąta 10x20: {PatternMatchingDemo.CalculateArea(rectangle):F2}\n");

            // 3. Klasyfikacja osób
            var person1 = new Person("Anna", 25, "Warszawa");
            var person2 = new Person("Jan", 70, "Kraków");
            Console.WriteLine($"{person1.Name}: {PatternMatchingDemo.ClassifyPerson(person1)}");
            Console.WriteLine($"{person2.Name}: {PatternMatchingDemo.ClassifyPerson(person2)}\n");

            // 4. Sugestie aktywności
            Console.WriteLine($"Słonecznie, 30°C: {PatternMatchingDemo.GetActivitySuggestion(WeatherType.Sunny, 30)}");
            Console.WriteLine($"Deszczowo: {PatternMatchingDemo.GetActivitySuggestion(WeatherType.Rainy, 20)}\n");

            // 5. Gra kamień-papier-nożyce
            Console.WriteLine($"Kamień vs Nożyce: {PatternMatchingDemo.GetRockPaperScissorsResult("kamień", "nożyce")}\n");

            // 6. Analiza tablic
            Console.WriteLine($"Pusta tablica: {PatternMatchingDemo.AnalyzeNumbers(new int[] { })}");
            Console.WriteLine($"Jeden element: {PatternMatchingDemo.AnalyzeNumbers(new[] { 42 })}");
            Console.WriteLine($"Wiele elementów: {PatternMatchingDemo.AnalyzeNumbers(new[] { 1, 2, 3, 4, 5 })}\n");

            // 7. Grupy wiekowe
            Console.WriteLine($"Wiek 15: {PatternMatchingDemo.GetAgeGroup(15)}");
            Console.WriteLine($"Wiek 35: {PatternMatchingDemo.GetAgeGroup(35)}\n");

            // 8. Godziny pracy
            Console.WriteLine($"Godzina 10: {(PatternMatchingDemo.IsWorkingHour(10) ? "Godzina pracy" : "Nie godzina pracy")}");
            Console.WriteLine($"Godzina 12: {(PatternMatchingDemo.IsWorkingHour(12) ? "Godzina pracy" : "Przerwa obiadowa")}\n");

            // 9. LINQ z pattern matching
            PatternMatchingDemo.LinqWithPatterns();
        }
    }
}
