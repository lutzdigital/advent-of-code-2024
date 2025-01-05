namespace Day01;

/// <summary>
/// Solution for day 1 of the AoC 2024.
///
/// See https://adventofcode.com/2024/day/1 for the description of the task and the input data.
/// </summary>
internal class Program
{
    static void Task1(List<long> first, List<long> second)
    {
        // Sorting is done in place. We use a copy here, so that the passed lists are not changed.
        List<long> copyFirst = new(first);
        List<long> copySecond = new(second);

        copyFirst.Sort();
        copySecond.Sort();

        // Just go through both lists simultaneously and add up the differences.
        long totalSum = 0;
        for (int i = 0; i < copyFirst.Count; i++)
        {
            totalSum += Math.Abs(copyFirst[i] - copySecond[i]);
        }

        Console.WriteLine("Task 1:");
        Console.WriteLine($"Total sum: {totalSum}");
    }

    static void Task2(List<long> first, List<long> second)
    {
        // Group elements of second list so that the occurrence is counted.
        var secondGrouped = second.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());

        // For every element in the first list, check how often it appears in the second list - if it exists at all.
        long similarityScore = 0;
        foreach (var f in first)
        {
            if (secondGrouped.TryGetValue(f, out int count))
            {
                similarityScore += f * count; 
            }
        }

        Console.WriteLine("Task 2:");
        Console.WriteLine($"Similarity score: {similarityScore}");
    }

    static void Main(string[] _)
    {
        string[] lines = File.ReadAllLines("data/input.txt");

        List<long> first = [];
        List<long> second = [];

        // Create lists from input.
        for (int i = 0; i < lines.Length; i++)
        {
            try
            {
                var parts = lines[i].Split("   ");
                long firstNumber = long.Parse(parts[0]);
                long secondNumber = long.Parse(parts[1]);
                first.Add(firstNumber);
                second.Add(secondNumber);
            }
            catch (FormatException)
            {
                Console.WriteLine($"Unable to parse '{lines[i]}'");
            }
        }

        Task1(first, second);
        Task2(first, second);
    }
}
