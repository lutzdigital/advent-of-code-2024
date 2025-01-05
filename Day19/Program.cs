namespace Day19;

/// <summary>
/// Solution for day 19 of the AoC 2024.
///
/// See https://adventofcode.com/2024/day/19 for the description of the task and the input data.
/// </summary>
internal class Program
{
    static bool IsPossible (string design, List<string> patterns)
    {
        // This method recursively finds the first possibility to create the design from the given patterns.

        // This is the exit condition of the recursion: An empty design can always be created.
        if (design.Length == 0)
        {
            return true;
        }

        // Check whether the design starts with one of the patterns. If yes: check recursively the remainder of the design.
        foreach (string pattern in patterns)
        {
            if (design.StartsWith(pattern) && IsPossible(design[(pattern.Length)..], patterns))
            {
                return true;
            }
        }

        // The design could not be created.
        return false;
    }

    static long PossibleArrangements(string design, List<string> patterns, Dictionary<string, long> dict)
    {
        // Check if a (sub-) arrangement has already been calculated.
        if (dict.TryGetValue(design, out long storedPossibleArrangementCount))
        {
            return storedPossibleArrangementCount;
        }

        // Exit condition of the recursion: An empty design has always one possible arrangement.
        if (design.Length == 0)
        {
            return 1;
        }

        long possibleArrangementCount = 0;
        foreach (string pattern in patterns)
        {
            // If the design starts with one of the patterns, check the number of arrangements for the remainder of the design.
            // The recursive call might return 0 if there are no arrangements possible.
            if (design.StartsWith(pattern))
            {
                possibleArrangementCount += PossibleArrangements(design[(pattern.Length)..], patterns, dict);
            }
        }

        // Store the result in the dictionary.
        dict.Add(design, possibleArrangementCount);

        return possibleArrangementCount;
    }

    static void Main(string[] _)
    {
        // Create map with falling bytes from input.
        string[] lines = File.ReadAllLines("data/input.txt");

        // The first line of the input describes the patterns.
        List<string> patterns = new(lines[0].Split(", "));

        // Lines 3 to the end describe the designs.
        List<string> designs = lines.Skip(2).ToList();

        // Task 1 and 2 are done in the same loop. For the first task, it is enough to implement a recursive backtracking algorithm.
        // For the second task, results are stored in a dictionary to avoid recalculation.
        int possibleDesignCount = 0;
        long totalPossibleArrangementCount = 0;
        foreach (var design in designs)
        {
            Console.WriteLine(design);
            possibleDesignCount += IsPossible(design, patterns) ? 1 : 0;
            totalPossibleArrangementCount += PossibleArrangements(design, patterns, []);
        }

        Console.WriteLine("Task1:");
        Console.WriteLine($"Possible designs: {possibleDesignCount}");

        Console.WriteLine("Task2:");
        Console.WriteLine($"Total possible arrangements: {totalPossibleArrangementCount}");
    }
}
