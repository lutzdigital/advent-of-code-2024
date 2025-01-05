namespace Day16;

/// <summary>
/// Solution for day 16 of the AoC 2024.
///
/// See https://adventofcode.com/2024/day/16 for the description of the task and the input data.
/// </summary>
internal class Program
{
    static void Main(string[] _)
    {
        // Read input data and transform it to a map. Extract start and end position as well.
        string[] lines = File.ReadAllLines("data/input.txt");
        int rowCount = lines.Length;
        int colCount = lines[0].Length;

        List<List<char>> cells = [];
        for (int row = 0; row < rowCount; row++)
        {
            cells.Add(Enumerable.Repeat('.', colCount).ToList());
        }

        (int startCol, int startRow) = (-1, -1);
        (int endCol, int endRow) = (-1, -1);
        for (int row = 0; row < rowCount; row++)
        {
            for (int col = 0; col < colCount; col++)
            {
                char c = lines[row][col];
                if (c == '.' || c == '#')
                {
                    cells[row][col] = c;
                }
                else if (c == 'S')
                {
                    (startCol, startRow) = (col, row);
                }
                else if (c == 'E')
                {
                    (endCol, endRow) = (col, row);
                }
                else
                {
                    throw new ArgumentException($"Cell ({col},{row}) has invalid value '{c}'");
                }
            }
        }

        // In the following, Dijkstra's algorithm (finding the shortest paths from a given start node to all other nodes) is implemented.
        // This is done using a 3D map with the dimensions 'row', 'column' and 'direction'. It is important that the direction
        // is treated as an extra dimension, as there might be different 'shortest' paths (paths with the lowest score)
        // depending on the final direction at the target position.

        // Initialize the 3D map with +Infinity (i.e. long.MaxValue) for all coordinates and directions.
        List<List<List<long>>> lowestScores = [];
        for (int row = 0; row < rowCount; row++)
        {
            List<List<long>> singleRow = [];
            for (int col = 0; col < colCount; col++)
            {
                singleRow.Add(Enumerable.Repeat(long.MaxValue, 4).ToList());
            }
            lowestScores.Add(singleRow);
        }

        // List of possible directions (up, right, down, left).
        List<(int, int)> steps = [(1, 0), (0, 1), (-1, 0), (0, -1)];

        // Initialize the priority queue with only the start position and direction.
        PriorityQueue<(int, int, int, long), long> queue = new();
        queue.Enqueue((startCol, startRow, 0, 0), 0);

        while (queue.Count > 0)
        {
            (int currCol, int currRow, int currDir, long currScore) = queue.Dequeue();

            // If the current item improves the score, the successor nodes must be checked as well.
            if (currScore < lowestScores[currRow][currCol][currDir])
            {
                lowestScores[currRow][currCol][currDir] = currScore;
                (int deltaCol, int deltaRow) = steps[currDir];

                // Check if the reindeer can move forward (go one step into the current direction).
                if (cells[currRow + deltaRow][currCol + deltaCol] == '.')
                {
                    // Add the new coordinates with the new score to list of items to be processed.
                    queue.Enqueue((currCol + deltaCol, currRow + deltaRow, currDir, currScore + 1), currScore + 1);
                }

                // Also check the score for rotating clockwise or anti-clockwise.
                queue.Enqueue((currCol, currRow, (currDir + 1) % 4, currScore + 1000), currScore + 1000);
                queue.Enqueue((currCol, currRow, (currDir + 3) % 4, currScore + 1000), currScore + 1000);
            }
        }

        // For the end position there are 4 scores availaible (one for each diurection). Since it is enough to reach the end position,
        // the lowest score is the final result.
        long lowestScore = lowestScores[endRow][endCol].Min();

        Console.WriteLine("Task 1:");
        Console.WriteLine($"Lowest score: {lowestScore}");

        // For the second taks, we track back from the end position to the start position, only going through the positions that were part of the (or one)
        // optimal path.

        // Start by inserting the final position and direction into the queue.
        // There might be more than one direction for the end position with the lowest score.
        var directionsWithLowestScore = Enumerable.Range(0, 4).Where(dir => lowestScores[endRow][endCol][dir] == lowestScore);
        Queue<(int, int, int)> visited = [];
        foreach (int dir in directionsWithLowestScore)
        {
            visited.Enqueue((endCol, endRow, dir));
        }

        // Keep a hash set for all unique visited cells.
        HashSet<(int, int)> uniqueVisited = [];
        while (visited.Count > 0)
        {
            (int col, int row, int dir) = visited.Dequeue();
            uniqueVisited.Add((col, row));

            long score = lowestScores[row][col][dir];

            // Rotate the direction only if has been part of the optimal path.
            if (lowestScores[row][col][(dir + 1) % 4] == score - 1000)
            {
                visited.Enqueue((col, row, (dir + 1) % 4));
            }
            if (lowestScores[row][col][(dir + 3) % 4] == score - 1000)
            {
                visited.Enqueue((col, row, (dir + 3) % 4));
            }

            // Go backwards by one step only if it part opf the optimal path.
            (int deltaCol, int deltaRow) = steps[dir];
            if (cells[row - deltaRow][col - deltaCol] != '#' && lowestScores[row - deltaRow][col - deltaCol][dir] == score - 1)
            {
                visited.Enqueue((col - deltaCol, row - deltaRow, dir));
            }
        }

        // Extract the number of unique visited cells from the hash set.
        int totalTiles = uniqueVisited.Count;

        Console.WriteLine("Task 2:");
        Console.WriteLine($"Total tiles: {totalTiles}");
    }
}
