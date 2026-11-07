using System.Text.RegularExpressions;

namespace blazor_burma_love_calculator;

public record TraceStep(int Round, List<int> Input, List<int> Output);

public class CalculationResult
{
    public bool Ok { get; set; }
    public string Error { get; set; } = string.Empty;
    public int Percentage { get; set; }
    public List<TraceStep> Trace { get; set; } = new();
}

public class LoveCalculatorService
{
    public CalculationResult CalculateCompatibility(string name1, string name2)
    {
        if (string.IsNullOrWhiteSpace(name1) || string.IsNullOrWhiteSpace(name2)
            || name1.Length > 80 || name2.Length > 80
            || !Regex.IsMatch(name1, "[a-zA-Z]") || !Regex.IsMatch(name2, "[a-zA-Z]"))
        {
            return new CalculationResult { Ok = false, Error = "Enter two names, each containing English letters and no more than 80 characters." };
        }

        string combined = SanitizeCombinedNames(name1, name2);

        if (string.IsNullOrEmpty(combined))
        {
            return new CalculationResult
            {
                Ok = false,
                Error = "Only English letters (a-z) are counted. Please enter valid names."
            };
        }

        var charCounts = new Dictionary<char, int>();
        var seenChars = new List<char>();

        foreach (var c in combined)
        {
            if (!charCounts.ContainsKey(c))
            {
                seenChars.Add(c);
                charCounts[c] = 0;
            }
            charCounts[c]++;
        }

        if (seenChars.Count < 2)
        {
            return new CalculationResult
            {
                Ok = false,
                Error = "Need at least two unique English letters to run this calculation flow."
            };
        }

        var currentCounts = seenChars.SelectMany(c => charCounts[c].ToString().Select(digit => digit - '0')).ToList();
        var trace = new List<TraceStep>();

        while (currentCounts.Count > 2)
        {
            var roundInput = new List<int>(currentCounts);
            var nextCounts = new List<int>();
            int left = 0;
            int right = currentCounts.Count - 1;

            while (left <= right)
            {
                if (left == right)
                {
                    nextCounts.Add(currentCounts[left]);
                }
                else
                {
                    int sum = currentCounts[left] + currentCounts[right];
                    if (sum >= 10)
                    {
                        nextCounts.Add(sum / 10);
                        nextCounts.Add(sum % 10);
                    }
                    else
                    {
                        nextCounts.Add(sum);
                    }
                }
                left++;
                right--;
            }

            currentCounts = nextCounts;
            trace.Add(new TraceStep(trace.Count + 1, roundInput, new List<int>(currentCounts)));
        }

        if (currentCounts.Count != 2)
        {
            return new CalculationResult
            {
                Ok = false,
                Error = "Calculation could not reduce to a final two-digit result."
            };
        }

        int percentage = int.Parse($"{currentCounts[0]}{currentCounts[1]}");

        return new CalculationResult
        {
            Ok = true,
            Percentage = percentage,
            Trace = trace
        };
    }

    private string SanitizeCombinedNames(string name1, string name2)
    {
        return Regex.Replace((name1 + name2).ToLowerInvariant(), "[^a-z]", "");
    }
}
