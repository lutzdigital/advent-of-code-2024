namespace Day18;

/// <summary>
/// Solution for day 18 of the AoC 2024.
///
/// See https://adventofcode.com/2024/day/13 for the description of the task and the input data.
/// </summary>
internal class Program
{
    static bool CanBeVisited(List<List<int>> cells, int col, int row, int colCount, int rowCount)
    {
        // A cell can not be visited if it is out of range.
        if (col < 0 || col >= colCount || row < 0 || row >= rowCount)
        {
            return false;
        }

        // Check if there is a falling byte in the cell.
        return cells[row][col] == 0;
    }

    static int DoTheDijkstra(List<(int, int)> fallingBytes, int fallingByteCount, int colCount, int rowCount)
    {
        // Create an empty 2D map.
        List<List<int>> cells = [];
        for (int row = 0; row < rowCount; row++)
        {
            cells.Add(Enumerable.Repeat(0, colCount).ToList());
        }

        // Place the falling bytes on the map.
        for (int byteIndex = 0; byteIndex < fallingByteCount; byteIndex++)
        {
            (int col, int row) = fallingBytes[byteIndex];
            cells[row][col] = 1;
        }

        // Initialize the map of shortest distances with +Infinity (i.e. int.MaxValue) for all coordinates.
        List<List<int>> shortestDistances = [];
        for (int row = 0; row < rowCount; row++)
        {
            shortestDistances.Add(Enumerable.Repeat(int.MaxValue, colCount).ToList());
        }

        // List of possible directions (up, right, down, left).
        List<(int, int)> steps = [(1, 0), (0, 1), (-1, 0), (0, -1)];

        // Define start and end position.
        (int startCol, int startRow) = (0, 0);
        (int endCol, int endRow) = (colCount - 1, rowCount - 1);

        // Initialize the priority queue with only the start position and direction.
        PriorityQueue<(int, int, int), int> queue = new();
        queue.Enqueue((startCol, startRow, 0), 0);
        while (queue.Count > 0)
        {
            (int currCol, int currRow, int currDistance) = queue.Dequeue();

            // If the current item improves the score, the successor nodes must be checked as well.
            if (currDistance < shortestDistances[currRow][currCol])
            {
                shortestDistances[currRow][currCol] = currDistance;
                foreach ((int deltaCol, int deltaRow) in steps)
                {
                    if (CanBeVisited(cells, currCol + deltaCol, currRow + deltaRow, colCount, rowCount))
                    {
                        // Add the new coordinates with the new score to list of items to be processed.
                        queue.Enqueue((currCol + deltaCol, currRow + deltaRow, currDistance + 1), currDistance + 1);
                    }
                }
            }
        }

        return shortestDistances[endRow][endCol];
    }

    static void Main(string[] _)
    {
        // Create list of falling bytes from input.
        string[] lines = File.ReadAllLines("data/input.txt");

        List<(int, int)> fallingBytes = [];

        foreach (var line in lines)
        {
            var parts = line.Split(',');
            (int col, int row) = (int.Parse(parts[0]), int.Parse(parts[1]));
            fallingBytes.Add((col, row));
        }

        // Define map size.
        int colCount = 71;
        int rowCount = 71;

        // For task 1, we place the first 1024 falling bytes on the maps and apply Dijkstra's algorithm (see also the code for day 16).
        int fallingByteCountTask1 = 1024;
        int shortestDistance = DoTheDijkstra(fallingBytes, fallingByteCountTask1, colCount, rowCount);
        Console.WriteLine("Task 1:");
        Console.WriteLine($"Shortest distance: {shortestDistance}");


        // For taks 2, just increase the number of falling bytes until the end position cannot be reached anymore.
        int fallingByteCountTask2 = fallingByteCountTask1;
        for (int fallingByteCount = fallingByteCountTask1; fallingByteCount < fallingBytes.Count; fallingByteCount++)
        {
            // Check if Dijkstra's algorithm finds a way to the end position.
            if (DoTheDijkstra(fallingBytes, fallingByteCount, colCount, rowCount) == int.MaxValue)
            {
                fallingByteCountTask2 = fallingByteCount - 1;
                break;
            }
        }

        Console.WriteLine("Task 2:");
        Console.WriteLine($"First blocking byte: {fallingBytes[fallingByteCountTask2].Item1},{fallingBytes[fallingByteCountTask2].Item2}");
    }
}
