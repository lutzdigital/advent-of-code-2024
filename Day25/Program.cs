/// <summary>
/// Solution for day 25 of the AoC 2024.
///
/// See https://adventofcode.com/2024/day/25 for the description of the task and the input data.
/// </summary>
namespace Day25;

internal class Program
{
    static void Main(string[] _)
    {
        // Read definitions of locks and keys from input.
        string[] lines = File.ReadAllLines("data/input.txt");

        // Stroe locks and keys by their pin heights. Hash sets are used so that only unique keys and locks are stored.
        HashSet<(int, int, int, int, int)> locks = [];
        HashSet<(int, int, int, int, int)> keys = [];

        // Every key or lck consists of 7 lineses, with an empty line between them.
        for (int lineIndex = 0; lineIndex < lines.Length; lineIndex += 8)
        {
            // Check first line to detecte whehter it is a lock or a key.
            if (lines[lineIndex] == "#####")
            {
                // Lock; for every pin, go down through the next lines until a '.' is found.  
                List<int> pinHeights = [];
                for (int pinIndex = 0; pinIndex < 5; pinIndex++)
                {
                    int height = 0;
                    while (lines[lineIndex + 1 + height][pinIndex] == '#')
                    {
                        height++;
                    }
                    pinHeights.Add(height);
                }
                locks.Add((pinHeights[0], pinHeights[1], pinHeights[2], pinHeights[3], pinHeights[4]));
            }
            else
            {
                // Key; for every pin, go through the lines in reverse until a '.' is found.  
                List<int> pinHeights = [];
                for (int pinIndex = 0; pinIndex < 5; pinIndex++)
                {
                    int height = 0;
                    while (lines[lineIndex + 5 - height][pinIndex] == '#')
                    {
                        height++;
                    }
                    pinHeights.Add(height);
                }
                keys.Add((pinHeights[0], pinHeights[1], pinHeights[2], pinHeights[3], pinHeights[4]));
            }
        }

        //Console.WriteLine($"Lock count: {locks.Count}");
        //foreach (var l in locks)
        //{
        //    Console.WriteLine($"Lock heights: ({l.Item1}, {l.Item2}, {l.Item3}, {l.Item4}, {l.Item5})");
        //}

        //Console.WriteLine($"Key count: {keys.Count}");
        //foreach (var k in keys)
        //{
        //    Console.WriteLine($"Key heights: ({k.Item1}, {k.Item2}, {k.Item3}, {k.Item4}, {k.Item5})");
        //}

        // Find all unique lock/key combinations where all pin heights add up to 5 or less.
        int uniquePairs = 0;
        foreach (var l in locks)
        {
            foreach (var k in keys)
            {
                if (l.Item1 + k.Item1 <= 5 && l.Item2 + k.Item2 <= 5 && l.Item3 + k.Item3 <= 5 &&l.Item4 + k.Item4 <= 5 && l.Item5 + k.Item5 <= 5)
                {
                    uniquePairs++;
                }
            }
        }

        Console.WriteLine("Task 1:");
        Console.WriteLine($"Unique pairs: {uniquePairs}");
    }
}
