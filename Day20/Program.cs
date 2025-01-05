namespace Day20;

/// <summary>
/// Solution for day 20 of the AoC 2024.
///
/// See https://adventofcode.com/2024/day/20 for the description of the task and the input data.
/// </summary>
internal class Program
{
    static bool CanBeVisited(List<List<char>> cells, int col, int row)
    {
        // A cell can be visited if it is inside the map and if it is a free space.
        int colCount = cells[0].Count;
        int rowCount = cells.Count;
        if (col < 0 || col >= colCount || row < 0 || row >= rowCount)
        {
            return false;
        }

        return cells[row][col] == '.';
    }

    static List<(int, int)> CreateRaceTrack(List<List<char>> cells, int startCol, int startRow, int endCol, int endRow)
    {
        // Create the race track from the 2D map.

        // Available directions are up, right, down and left.
        List<(int, int)> steps = [(0, -1), (1, 0), (0, 1), (-1, 0)];

        List<(int, int)> raceTrack = [];
        (int prevCol, int prevRow) = (-1, -1);
        (int currCol, int currRow) = (startCol, startRow);

        // Race track is finished if the current position equals the end position.
        while (currCol != endCol || currRow != endRow)
        {
            raceTrack.Add((currCol, currRow));
            foreach ((int deltaCol, int deltaRow) in steps)
            {
                (int nextCol, int nextRow) = (currCol + deltaCol, currRow + deltaRow);
                // From the current position, find the next available position without going back.
                if (CanBeVisited(cells, nextCol, nextRow) && ((nextCol != prevCol) || (nextRow != prevRow)))
                {
                    (prevCol, prevRow) = (currCol, currRow);
                    (currCol, currRow) = (nextCol, nextRow);
                    break;
                }
            }
        }
        raceTrack.Add((currCol, currRow));

        return raceTrack;
    }

    static void Task1(List<List<char>> cells, List<(int, int)> raceTrack, Dictionary<(int, int), int> picoseconds)
    {
        // For the first task, just go through every position in the race track and check whether there is a cheat possible from there.

        // Available directions are up, right, down and left.
        List<(int, int)> steps = [(0, -1), (1, 0), (0, 1), (-1, 0)];

        List<(int, int, int, int, int)> cheats = [];
        for (int index = 0; index < raceTrack.Count; index++)
        {
            (int currCol, int currRow) = raceTrack[index];
            foreach ((int deltaCol, int deltaRow) in steps)
            {
                // From the current position, go two steps in the same direction and check if that position is available.
                (int cheat1Col, int cheat1Row) = (currCol + deltaCol, currRow + deltaRow);
                (int cheat2Col, int cheat2Row) = (currCol + 2 * deltaCol, currRow + 2 * deltaRow);
                if ((cells[cheat1Row][cheat1Col] == '#') && CanBeVisited(cells, cheat2Col, cheat2Row))
                {
                    // Add cheat only if it actually saves time.
                    if (picoseconds.TryGetValue((cheat2Col, cheat2Row), out int picosecond) && picosecond > index + 2)
                    {
                        cheats.Add((currCol, currRow, cheat2Col, cheat2Row, picosecond - index - 2));
                    }
                }
            }
        }

        // Only count cheats that save at least 100 picoseconds.
        int minSaving = 100;
        int goodCheatCount = 0;
        foreach (var cheat in cheats)
        {
            //Console.WriteLine($"Cheat ({cheat.Item1},{cheat.Item2}) -> ({cheat.Item3},{cheat.Item4}) saves {cheat.Item5} picoseconds");
            if (cheat.Item5 >= minSaving)
            {
                goodCheatCount++;
            }
        }

        Console.WriteLine("Task 1:");
        Console.WriteLine($"Cheats saving at least 100 picoseconds: {goodCheatCount}");
    }

    static void Task2(List<List<char>> cells, List<(int, int)> raceTrack, Dictionary<(int, int), int> picoseconds)
    {
        // For the second task, the algorithm is doing more or less the same, except that now all available positions
        // within a radius of 20 cells have to be checked.
        List<(int, int, int, int, int)> cheats = [];
        for (int index = 0; index < raceTrack.Count; index++)
        {
            (int currCol, int currRow) = raceTrack[index];
            // Check all 41 x 41 squares around the current positions.
            for (int deltaRow = -20; deltaRow <= 20; deltaRow++)
            {
                for (int deltaCol = -20; deltaCol <= 20; deltaCol++)
                {
                    // Check if distance is either too small or too large.
                    int cheatLength = Math.Abs(deltaCol) + Math.Abs(deltaRow);
                    if (cheatLength >= 2 && cheatLength <= 20)
                    {
                        (int targetCol, int targetRow) = (currCol + deltaCol, currRow + deltaRow);
                        if (CanBeVisited(cells, targetCol, targetRow))
                        {
                            // Add cheat only if it actually saves time.
                            if (picoseconds.TryGetValue((targetCol, targetRow), out int picosecond) && picosecond > index + cheatLength)
                            {
                                cheats.Add((currCol, currRow, targetCol, targetRow, picosecond - index - cheatLength));
                            }
                        }
                    }
                }
            }
        }

        // Only count cheats that save at least 100 picoseconds.
        int minSaving = 100;
        int goodCheatCount = 0;
        foreach (var cheat in cheats)
        {
            //Console.WriteLine($"Cheat ({cheat.Item1},{cheat.Item2}) -> ({cheat.Item3},{cheat.Item4}) saves {cheat.Item5} picoseconds");
            if (cheat.Item5 >= minSaving)
            {
                goodCheatCount++;
            }
        }

        Console.WriteLine("Task 2:");
        Console.WriteLine($"Cheats saving at least 100 picoseconds: {goodCheatCount}");
    }

    static void Main(string[] _)
    {
        // Create 2D map from input data.
        string[] lines = File.ReadAllLines("data/input.txt");
        int rowCount = lines.Length;
        int colCount = lines[0].Length;

        List<List<char>> cells = [];
        for (int row = 0; row < rowCount; row++)
        {
            cells.Add(Enumerable.Repeat('.', colCount).ToList());
        }

        // Find start and end position and transfer input data to map.
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

        // Create race track.
        List<(int, int)> raceTrack = CreateRaceTrack(cells, startCol, startRow, endCol, endRow);

        // For every position in the race track, store the time that it needs to get there.
        // These times are stored in a dictionary so that they can retrieved easily for every position in the map.
        Dictionary<(int, int), int> picoseconds = [];
        for (int index = 0; index < raceTrack.Count; index++)
        {
            (int currCol, int currRow) = raceTrack[index];
            picoseconds.Add((currCol, currRow), index);
        }

        Task1(cells, raceTrack, picoseconds);
        Task2(cells, raceTrack, picoseconds);
    }
}
