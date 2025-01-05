using System.Text.RegularExpressions;

namespace Day03;

/// <summary>
/// Solution for day 3 of the AoC 2024.
///
/// See https://adventofcode.com/2024/day/3 for the description of the task and the input data.
/// </summary>
internal class Program
{
    static void DoTask(string line, bool isTask2)
    {
        string pattern = @"(mul\([0-9]+,[0-9]+\)|do\(\)|don't\(\))";

        bool isEnabled = true;
        long result = 0;

        // The regular expression is doing the main work of parsing the input.
        // The following three patterns are found:
        // - "mul(X,Y)" (where X and Y are  sequences of one or more digits
        // - "do()"
        // - "don't()'
        // The given matching pattern works for both tasks; for the first task the enabling ("do()")and disabling ("don't()") is simply ignored.
        foreach (Match match in Regex.Matches(line, pattern))
        {
            string matchString = match.Value;
            if (matchString == "do()")
            {
                isEnabled = true;
            }
            else if (matchString == "don't()")
            {
                isEnabled = false;
            }
            else // multiplication
            {
                string[] parts = match.Value[4..^1].Split(',');
                long firstNumber = long.Parse(parts[0]);
                long secondNumber = long.Parse(parts[1]);
                long product = firstNumber * secondNumber;
                if (isEnabled || !isTask2)
                {
                    result += product;
                }
            }
        }

        Console.WriteLine($"Task {(isTask2 ? 2 : 1)}:");
        Console.WriteLine($"Result: {result}");

    }

    static void Main(string[] _)
    {
        string line = File.ReadAllText("data/input.txt");

        DoTask(line, false);
        DoTask(line, true);
    }
}
