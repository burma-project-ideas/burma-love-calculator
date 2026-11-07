using blazor_burma_love_calculator;
using System.Globalization;
var calculator = new LoveCalculatorService();
void Check(bool condition, string message) { if (!condition) throw new Exception(message); }
foreach (var (first, second, expected) in new[] { ("Alice", "Bob", 35), ("Bob", "Alice", 44), ("John", "Jane", 62), ("A", "B", 11) })
{
    var result = calculator.CalculateCompatibility(first, second);
    Check(result.Ok && result.Percentage == expected, $"Existing score changed: {first}/{second}");
}
Check(calculator.CalculateCompatibility(" Alice! ", "Bob123").Percentage == 35, "Sanitization changed");
Check(calculator.CalculateCompatibility("Alice", "Bob").Trace[0].Output.SequenceEqual(new[] { 2, 3, 2, 1 }), "Trace changed");
Check(!calculator.CalculateCompatibility("", "Bob").Ok, "Both names must be required");
Check(!calculator.CalculateCompatibility("မြန်မာ", "Bob").Ok, "Both names need English letters");
Check(!calculator.CalculateCompatibility("aaa", "aaa").Ok, "One distinct letter must be rejected");
var repeated = calculator.CalculateCompatibility(new string('a', 20), new string('b', 20));
Check(repeated.Ok && repeated.Percentage is >= 0 and <= 99, "Repeated letters must still produce a two-digit score");
CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("tr-TR");
Check(calculator.CalculateCompatibility("ALICE", "BOB").Percentage == 35, "Scores must not depend on machine culture");
Console.WriteLine("Calculator regression checks passed.");
