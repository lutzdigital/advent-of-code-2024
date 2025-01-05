namespace Day11;

/// <summary>
/// Solution for day 11 of the AoC 2024.
///
/// See https://adventofcode.com/2024/day/11 for the description of the task and the input data.
/// </summary>
internal class Program
{
    static List<long> Blink(List<long> input)
    {
        List<long> output = [];

        // Create new stones according to the specified rules.
        foreach (var stone in input)
        {
            if (stone == 0)
            {
                // "0" becomes "1"
                output.Add(1);
            }
            else if ((stone.ToString().Length % 2) == 0)
            {
                // a number with an even number of digits is split into two numbers
                var stoneAsString = stone.ToString();
                var length = stoneAsString.Length;
                output.Add(long.Parse(stoneAsString[0..(length / 2)]));
                output.Add(long.Parse(stoneAsString[(length / 2)..]));
            }
            else
            {
                // otherwise just multiply the number with 2024
                output.Add(stone * 2024);
            }
        }

        return output;
    }

    static void Task1(List<long> initialStones)
    {
        int blinkCount = 25;
        var stones = initialStones.ToList();
        //Console.WriteLine($"{string.Join(' ', stones)}");

        for (int i = 1; i <= blinkCount; i++)
        {
            stones = Blink(stones);
            //Console.WriteLine($"{string.Join(' ', stones)}");
        }

        Console.WriteLine("Task 1:");
        Console.WriteLine($"Stone count: {stones.Count}");
    }

    static long CountStones(Dictionary<(long, int), long> solvedItems, (long, int) item)
    {
        // This function is doing the main work. It is doing a depth-first recursive descent to
        // count the the stones that are produced during the blinking.
        (long number, int blinks) = item;

        // Return value if it is already computed.
        if (solvedItems.TryGetValue(item, out long value))
        {
            return value;
        }

        // When a stone does not blink, it does not produce any further stones.
        if (blinks == 0)
        {
            // Store the result for later re-use.
            solvedItems.Add(item, 1);
            return 1;
        }

        // Get the numbers of the stones after blinking once.
        List<long> subNumbers = Blink([number]);

        // The result is the sum of the sub-results.
        long stoneCount = 0;
        foreach (var subNumber in subNumbers)
        {
            stoneCount += CountStones(solvedItems, (subNumber, blinks - 1));
        }

        // Store the result for later re-use.
        solvedItems.Add(item, stoneCount);

        return stoneCount;
    }

    static void Task2(List<long> initialStones)
    {
        int blinkCount = 75;

        // Dictionary for storing the results that were already computed.
        // This dictionary is essential for drastically reducing the amount of computation.
        Dictionary<(long, int), long> solvedItems = [];

        // The number of blinks is just too high to do it explicitely for every stone and every iteration (as we did in task 1).
        // So a different strategy is emplyed, we just count how many stones are produced and use a dynamic programming strategy:
        // Results calculated for lower blink counts are stored in a dictionary, so that they can be re-used.
        long stoneCount = 0;
        foreach (var stone in initialStones)
        {
            stoneCount += CountStones(solvedItems, (stone, blinkCount));
        }

        Console.WriteLine("Task 2:");
        Console.WriteLine($"Stone count: {stoneCount}");
    }

    static void Main(string[] _)
    {
        string line = File.ReadAllText("data/input.txt");
        var parts = line.Split(" ");
        var initialStones = parts.Select(long.Parse).ToList();

        Task1(initialStones);
        Task2(initialStones);
    }
}
