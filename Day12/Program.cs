namespace Day12;

/// <summary>
/// Solution for day 12 of the AoC 2024.
///
/// See https://adventofcode.com/2024/day/12 for the description of the task and the input data.
/// </summary>
internal class Program
{
    static bool IsDifferent(string[] lines, int colCount, int rowCount, char plant, int col, int row)
    {
        // A neighbor cell belongs to a different region if it is either outside of the map or contains a different plant.
        if (col < 0 || col >= colCount || row < 0 || row >= rowCount)
        {
            return true;
        }

        return lines[row][col] != plant;
    }

    static (long, long, long) CalculateRegion(string[] lines, List<List<bool>> hasBeenVisited, int colCount, int rowCount, int col, int row)
    {
        // This function is doing the main work of the algorithm.
        // Starting from the cell [col, row], all neighbors are checked whether they are also part of the region. Whenever a new cell is found,
        // it is stored in a queue and then in turn the neighbors of that cell are checked.
        // the 'hasBeenVisited' map stores which cells have already been processed.
        long area = 0;
        long perimeter = 0;

        Queue<(int, int)> queue = [];
        queue.Enqueue((col, row));

        // Every fence is stored in a hashset, with column, row and the side (up, right, down, left) of the cell on which the fence is located.
        HashSet<(int, int, char)> fences = [];

        while (queue.Count > 0)
        {
            (int currCol, int currRow) = queue.Dequeue();
            if (hasBeenVisited[currRow][currCol])
            {
                continue;
            }

            char plant = lines[row][col];
            area += 1;

            List<(int, int, char)> steps =
            [
                (currCol, currRow - 1, 'U'),
                (currCol + 1, currRow, 'R'),
                (currCol, currRow + 1, 'D'),
                (currCol - 1, currRow, 'L'),
            ];

            // Go through all 4 neighbors.
            foreach (var step in steps)
            {
                if (IsDifferent(lines, colCount, rowCount, plant, step.Item1, step.Item2))
                {
                    // If the neighbor cell contains a different plant or is outside of the map, a fence is added.
                    perimeter += 1;
                    fences.Add((step.Item1, step.Item2, step.Item3));
                }
                else
                {
                    // The neighbor cell contains the same plant --> add it to the queue for further processing.
                    queue.Enqueue((step.Item1, step.Item2));
                }
            }

            hasBeenVisited[currRow][currCol] = true;
        }

        // The perimeter of the fence is the number of fence parts.
        // For calculating the sides of the fence, those fence parts that have a neighbor fence part in the cell above or to the left are not counted.
        long sides = perimeter;
        foreach (var fence in fences)
        {
            (int fenceCol, int fenceRow, char direction) = fence;
            if (direction is 'U' or 'D')
            {
                if (fences.Contains((fenceCol - 1, fenceRow, direction)))
                {
                    sides--;
                }
            }
            else if (direction is 'L' or 'R')
            {
                if (fences.Contains((fenceCol, fenceRow - 1, direction)))
                {
                    sides--;
                }
            }
        }

        return (area, perimeter, sides);
    }

    static void Main(string[] _)
    {
        // Read input map.
        string[] lines = File.ReadAllLines("data/input.txt");
        int rowCount = lines.Length;
        int colCount = lines[0].Length;

        // Create a map for storing which cells have already been processed.
        List<List<bool>> hasBeenVisited = [];
        for (int row = 0; row < rowCount; row++)
        {
            hasBeenVisited.Add(Enumerable.Repeat(false, colCount).ToList());
        }

        long totalCostTask1 = 0;
        long totalCostTask2 = 0;

        // Process all cells and calculate the regions.
        for (int row = 0; row < rowCount; row++)
        {
            for (int col = 0; col < colCount; col++)
            {
                // Cells might have been visited already in the earlier calculations.
                if (!hasBeenVisited[row][col])
                {
                    (long area, long perimeter, long sides) = CalculateRegion(lines, hasBeenVisited, colCount, rowCount, col, row);
                    //Console.WriteLine($"Region: [{col}, {row}] Area: {area}, Perimeter: {perimeter} Sides: {sides}");

                    long costTask1 = area * perimeter;
                    long costTask2 = area * sides;
                    //Console.WriteLine($"Cost (task 1): {costTask1}");
                    //Console.WriteLine($"Cost (task 2): {costTask2}");
                    totalCostTask1 += costTask1;
                    totalCostTask2 += costTask2;
                }
            }
        }

        Console.WriteLine("Task 1");
        Console.WriteLine($"Total cost: {totalCostTask1}");
        Console.WriteLine("Task 2");
        Console.WriteLine($"Total cost: {totalCostTask2}");
    }
}
