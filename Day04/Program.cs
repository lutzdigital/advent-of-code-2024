namespace Day04;

/// <summary>
/// Solution for day 4 of the AoC 2024.
///
/// See https://adventofcode.com/2024/day/4 for the description of the task and the input data.
/// </summary>
internal class Program
{

    static bool IsMatch(string[] lines, char c, int col, int row, int colCount, int rowCount)
    {
        // If the requested column and row are outside the limits, it cannot be a match.
        if (col < 0 || col >= colCount || row < 0 || row >= rowCount)
        {
            return false;
        }

        // Check if there is a match with the requested character.
        return lines[row][col] == c;
    }

    static void Task1(string[] lines)
    {
        int rowCount = lines.Length;
        int colCount = lines[0].Length;

        int matchCount = 0;
        // Go through all characters of the inputs row by row, column by column
        for (int row = 0; row < rowCount; row++)
        {
            for (int col = 0; col < colCount; col++)
            {
                // If an 'X' is found, go through the neighboring characters in all 4 directions (up, right, down left)
                if (IsMatch(lines, 'X', col, row, colCount, rowCount))
                {
                    // The 4 directions.
                    List<(int, int)> steps = [(1, 0), (-1, 0), (0, 1), (0, -1), (1, 1), (-1, 1), (1, -1), (-1, -1)];

                    foreach (var step in steps)
                    {
                        var (deltaCol1, deltaRow1) = (step.Item1, step.Item2);
                        var (deltaCol2, deltaRow2) = (deltaCol1 * 2, deltaRow1 * 2);
                        var (deltaCol3, deltaRow3) = (deltaCol1 * 3, deltaRow1 * 3);

                        // Check for the missing letters 'MAS'.
                        if (IsMatch(lines, 'M', col + deltaCol1, row + deltaRow1, colCount, rowCount) &&
                            IsMatch(lines, 'A', col + deltaCol2, row + deltaRow2, colCount, rowCount) &&
                            IsMatch(lines, 'S', col + deltaCol3, row + deltaRow3, colCount, rowCount))
                        {
                            //Console.WriteLine($"Found match from [{col}, {row}] to [{col + deltaCol3}, {row + deltaRow3}]");
                            matchCount++;
                        }
                    }
                }
            }
        }

        Console.WriteLine("Task 1:");
        Console.WriteLine($"Match count: {matchCount}");
    }

    static void Task2(string[] lines)
    {
        int rowCount = lines.Length;
        int colCount = lines[0].Length;

        int matchCount = 0;
        // Like in task1, go through all characters of the inputs row by row, column by column
        for (int row = 0; row < rowCount; row++)
        {
            for (int col = 0; col < colCount; col++)
            {
                // This time, look for an 'A' and check whether there are letters 'M' and 'S' forming a cross.
                if (IsMatch(lines, 'A', col, row, colCount, rowCount))
                {
                    if
                    (
                        // Check for "MAS" or "SAM" going form upper left to lower right.
                        (
                            (IsMatch(lines, 'M', col - 1, row - 1, colCount, rowCount) && IsMatch(lines, 'S', col + 1, row + 1, colCount, rowCount))
                            ||
                            (IsMatch(lines, 'S', col - 1, row - 1, colCount, rowCount) && IsMatch(lines, 'M', col + 1, row + 1, colCount, rowCount))
                        )
                        &&
                        // Check for "MAS" or "SAM" going form upper right to lower left.
                        (
                            (IsMatch(lines, 'M', col + 1, row - 1, colCount, rowCount) && IsMatch(lines, 'S', col - 1, row + 1, colCount, rowCount))
                            ||
                            (IsMatch(lines, 'S', col + 1, row - 1, colCount, rowCount) && IsMatch(lines, 'M', col - 1, row + 1, colCount, rowCount))
                        )
                    )
                    {
                        //Console.WriteLine($"Found match at [{col}, {row}]");
                        matchCount++;
                    }
                }
            }
        }

        Console.WriteLine("Task 2:");
        Console.WriteLine($"Match count: {matchCount}");
    }

    static void Main(string[] _)
    {
        // Create reports from input.
        string[] lines = File.ReadAllLines("data/input.txt");

        Task1(lines);
        Task2(lines);
    }
}
