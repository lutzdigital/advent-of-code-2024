namespace Day06;

/// <summary>
/// Solution for day 6 of the AoC 2024.
///
/// See https://adventofcode.com/2024/day/6 for the description of the task and the input data.
/// </summary>
internal class Program
{
    static HashSet<(int, int, int)> DoPatrol(List<List<char>> cells, int startCol, int startRow, int colCount, int rowCount)
    {
        // The main work of the algorithm is done here.
        // The guard walks through the map until it either is at the edge of the area or until it is in the same position again, facing into the same direction.
        // If the guard is blocked by an obstruction, he/she turns clockwise by 90 degrees.

        // The result of this function is a hash map with the visited positions with their directions. If there is a loop, an empty hash map is returned.

        // The patrol through the lab is given in Cartesian (2D) coordinates: First coordinate is X (column), second is Y (row).
        // In the map, (0, 0) is the upper left corner. 

        // The directions in the order 'Up', 'Right', 'Down', 'Left'.
        List<(int, int)> directions = [(0, -1), (1, 0), (0, 1), (-1, 0)];

        (int currCol, int currRow) = (startCol, startRow);
        int currDir = 0;
        bool isLoop = false;

        // In order to detect loops, the already visited positions with the corresponding directions are stored in a hash map.
        // NOTE: It is important to also store the direction so that self-intersections of the patrol are not detected as loops.
        HashSet<(int, int, int)> positionsAndDirections = [];

        while (true)
        {
            if (positionsAndDirections.Contains((currCol, currRow, currDir)))
            {
                isLoop = true;
                break;
            }

            //Console.WriteLine($"Position: [{currCol}, {currRow}] Dir: {currDir}");
            positionsAndDirections.Add((currCol, currRow, currDir));

            if (currCol == 0 || currCol == colCount-1 || currRow == 0 || currRow == rowCount - 1)
            {
                // Guard is on edge of area -> patrol is finished.
                break;
            }

            (int nextCol, int nextRow) = (currCol + directions[currDir].Item1, currRow + directions[currDir].Item2);
            if (cells[nextRow][nextCol] == '#')
            {
                // There is an obstruction, turn to the right.
                currDir = (currDir + 1) % directions.Count;
            }
            else
            {
                // No obstruction, go ahead.
                (currCol, currRow) = (nextCol, nextRow);
            }
        }

        return isLoop
            ? []
            : positionsAndDirections;
    }

    static void DoTask(bool isTask2, List<List<char>> cells, int startCol, int startRow, int colCount, int rowCount)
    {
        if (startCol < 0 || startCol >= colCount || startRow < 0 || startRow >= rowCount)
        {
            Console.WriteLine($"Start position [{startCol}, {startRow}] is invalid");
            return;
        }

        HashSet<(int, int, int)> positionsAndDirections = DoPatrol(cells, startCol, startRow, colCount, rowCount);

        // Get distinct positions of route.
        List<(int, int)> visitedPositions = [];
        foreach ((int col, int row, int _) in positionsAndDirections)
        {
            visitedPositions.Add((col, row));
        }
        List<(int, int)> distinctPositions = visitedPositions.Distinct().ToList();

        // In the first task, only the number of distinct positions is important.
        if (!isTask2)
        {
            Console.WriteLine("Task 1:");
            Console.WriteLine($"Distinct position count: {distinctPositions.Count}");
            return;
        }

        // The second task is solved like this: For every position that the guard has visited in the first task, we try to insert an obstruction
        // and check whether the guard then is caught in a loop.
        int possibleObstructionCount = 0;
        foreach (var (possibleCol, possibleRow) in distinctPositions)
        {
            // An obstruction cannot be placed on the starting position of the guard.
            if (possibleCol == startCol && possibleRow == startRow)
            {
                continue;
            }

            // This should not happen, as the guard cannot have visited an obstructed position.
            if (cells[possibleRow][possibleCol] == '#')
            {
                throw new Exception($"Error in algorithm, position [{possibleCol}, {possibleRow}] is already obstructed");
            }

            // Insert an obstruction and check whether the guard will be caught in a loop, then remove the obstruction.
            cells[possibleRow][possibleCol] = '#';

            // An empty hash map indicates a loop.
            if (DoPatrol(cells, startCol, startRow, colCount, rowCount).Count == 0)
            {
                possibleObstructionCount++;
            }

            cells[possibleRow][possibleCol] = '.';
        }

        Console.WriteLine("Task 2:");
        Console.WriteLine($"Possible obstruction count: {possibleObstructionCount}");
    }

    static void Main(string[] _)
    {
        // Read input from file and transform it into a two-dimensional array of characters.
        // This is done so that the data can be manipulated more easily (rather than working with the strings directly).
        string[] lines = File.ReadAllLines("data/input.txt");

        int rowCount = lines.Length;
        int colCount = lines[0].Length;

        List<List<char>> cells = [];
        for (int row = 0; row < rowCount; row++)
        {
            cells.Add(Enumerable.Repeat('.', colCount).ToList());
        }

        (int startCol, int startRow) = (-1, -1);

        // Copy the contents of the strings to the two-dimensional array, except for the starting position, which is stored separately.
        for (int row = 0; row < rowCount; row++)
        {
            for (int col = 0; col < colCount; col++)
            {
                // Check whether symbol denotes the starting position.
                if (lines[row][col] == '^')
                {
                    (startRow, startCol) = (row, col);
                    cells[row][col] = '.';
                }
                else
                {
                    cells[row][col] = lines[row][col];
                }
            }
        }

        DoTask(false, cells, startCol, startRow, colCount, rowCount);
        DoTask(true, cells, startCol, startRow, colCount, rowCount);
    }
}
