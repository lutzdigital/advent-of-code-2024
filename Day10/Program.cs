namespace Day10;

/// <summary>
/// Solution for day 10 of the AoC 2024.
///
/// See https://adventofcode.com/2024/day/10 for the description of the task and the input data.
/// </summary>
internal class Program
{
    static void FindTrailheads(List<List<int>> cells, int colCount, int rowCount, int col, int row, int level, HashSet<(int, int)> trailheads)
    {
        // The position we are at is either out of the map area or has an invalid level.
        if (col < 0 || col >= colCount || row < 0 || row >= rowCount || cells[row][col] != level)
        {
            return;
        }

        if (level == 9)
        {
            // We found a 9-height position, so we put it into the hash set.
            // We might have reached this very 9-height position through a different route, but the hash set will store only the distinc 9-height positions.
            trailheads.Add((col, row));
            return;
        }

        // From every posiiton, we have to check whether the 4 neighboring positions can be used for the next step.
        FindTrailheads(cells, colCount, rowCount, col, row - 1, level + 1, trailheads);
        FindTrailheads(cells, colCount, rowCount, col + 1, row, level + 1, trailheads);
        FindTrailheads(cells, colCount, rowCount, col, row + 1, level + 1, trailheads);
        FindTrailheads(cells, colCount, rowCount, col - 1, row, level + 1, trailheads);
    }

    static int GetTrailheadRating(List<List<int>> cells, int colCount, int rowCount, int col, int row, int level)
    {
        // The position we are at is either out of the map area or has an invalid level.
        if (col < 0 || col >= colCount || row < 0 || row >= rowCount || cells[row][col] != level)
        {
            return 0;
        }

        if (level == 9)
        {
            // We found a 9-height position, so we increase the rating.
            return 1;
        }

        // From every posiiton, we have to check whether the 4 neighboring positions can be used for the next step.
        return
            GetTrailheadRating(cells, colCount, rowCount, col, row - 1, level + 1) +
            GetTrailheadRating(cells, colCount, rowCount, col + 1, row, level + 1) +
            GetTrailheadRating(cells, colCount, rowCount, col, row + 1, level + 1) +
            GetTrailheadRating(cells, colCount, rowCount, col - 1, row, level + 1);
    }

    static void Main(string[] _)
    {
        // Read input and transform it into a two-dimensional map.
        string[] lines = File.ReadAllLines("data/input.txt");

        int rowCount = lines.Length;
        int colCount = lines[0].Length;

        List<List<int>> cells = [];
        for (int row = 0; row < rowCount; row++)
        {
            cells.Add(Enumerable.Repeat(0, colCount).ToList());
        }

        for (int row = 0; row < rowCount; row++)
        {
            for (int col = 0; col < colCount; col++)
            {
                cells[row][col] = int.Parse(lines[row][col].ToString());
            }
        }

        // Go trough each cell with a level of 0 and calculate the trailhead score and rating.
        long totalSum1 = 0;
        long totalSum2 = 0;
        for (int row = 0; row < rowCount; row++)
        {
            for (int col = 0; col < colCount; col++)
            {
                if (cells[row][col] == 0)
                {
                    // Calculating the trailhead score and rating is very similar and is done using recursion.

                    // Trailhead score = number of distinct 9-height positions that can be reached.
                    // A hash set is used so that only the distinct 9-height positions are stored. 
                    HashSet<(int, int)> trailheads = [];
                    FindTrailheads(cells, colCount, rowCount, col, row, 0, trailheads);
                    int trailheadScore = trailheads.Count;
                    totalSum1 += trailheadScore;

                    // Trailhead rating = number of distinct ways to reach a 9-height position.
                    int trailheadRating = GetTrailheadRating(cells, colCount, rowCount, col, row, 0);
                    totalSum2 += trailheadRating;

                    //Console.WriteLine($"Trailhead: [{col}, {row}] Score: {trailheadScore} Rating {trailheadRating}");
                }
            }
        }

        Console.WriteLine("Task 1:");
        Console.WriteLine($"Total sum: {totalSum1}");
        Console.WriteLine("Task 2:");
        Console.WriteLine($"Total sum: {totalSum2}");
    }
}
