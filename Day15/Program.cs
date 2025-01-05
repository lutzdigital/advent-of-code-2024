using System.Text;

namespace Day15;

/// <summary>
/// Solution for day 15 of the AoC 2024.
///
/// See https://adventofcode.com/2024/day/15 for the description of the task and the input data.
/// </summary>
internal class Program
{
    internal class SmallerRowComparer : IComparer<(int, int)>
    {
        // Custom comparer for sorting the rows (smaller row numbers come before larger row numbers)
        public int Compare((int, int) cellA, (int, int) cellB)
        {
            (int colA, int rowA) = cellA;
            (int colB, int rowB) = cellB;

            if (rowA < rowB)
            {
                return -1;
            }

            if (rowA > rowB)
            {
                return 1;
            }

            return colB - colA;
        }
    }

    internal class LargerRowComparer : IComparer<(int, int)>
    {
        // Custom comparer for sorting the rows (larger row numbers come before smaller row numbers)
        public int Compare((int, int) cellA, (int, int) cellB)
        {
            (int colA, int rowA) = cellA;
            (int colB, int rowB) = cellB;

            if (rowA < rowB)
            {
                return 1;
            }

            if (rowA > rowB)
            {
                return -1;
            }

            return colB - colA;
        }
    }

    static string CellsToString(List<List<char>> cells)
    {
        // Debugging output of the map.
        var sb = new StringBuilder();
        foreach (var cellRow in cells)
        {
            sb.AppendLine(string.Join("", cellRow.Select(x => x).ToArray()));
        }

        return sb.ToString();
    }

    static (int, int) FindRobot(List<List<char>> cells)
    {
        // Find robot by looking for the '@' symbol.
        int colCount = cells[0].Count;
        int rowCount = cells.Count;

        for (int row = 0; row < rowCount; row++)
        {
            for (int col = 0; col < colCount; col++)
            {
                if (cells[row][col] == '@')
                {
                    return (col, row);
                }
            }
        }

        throw new ArgumentException("No robot found");
    }

    static long CalculateTotalSum(List<List<char>> cells)
    {
        // Calculate the result sum.
        int colCount = cells[0].Count;
        int rowCount = cells.Count;

        long totalSum = 0;
        for (int row = 0; row < rowCount; row++)
        {
            for (int col = 0; col < colCount; col++)
            {
                if (cells[row][col] == 'O' || cells[row][col] == '[')
                {
                    totalSum += row * 100 + col;
                }
            }
        }

        return totalSum;
    }

    static void Task1(List<List<char>> cells, string allMoves)
    {
        //Console.WriteLine(CellsToString(cells));

        (int currCol, int currRow) = FindRobot(cells);

        // Move the robot according to the input.
        foreach (char move in allMoves)
        {
            // Translate input symbol to a movement.
            (int deltaCol, int deltaRow) = move switch
            {
                '^' => (0, -1),
                '>' => (1, 0),
                'v' => (0, 1),
                '<' => (-1, 0),
                _ => throw new ArgumentException($"Move {move} out of range")
            };

            // Go through all cells in front of the robot. Loops continues as long as there is a '0' symbol.
            (int emptyCol, int emptyRow) = (currCol + deltaCol, currRow + deltaRow);
            while (cells[emptyRow][emptyCol] == 'O')
            {
                (emptyCol, emptyRow) = (emptyCol + deltaCol, emptyRow + deltaRow);
            }

            // Loop ended, check whether there is a free cell ('.') or a wall ('#').
            if (cells[emptyRow][emptyCol] == '#')
            {
                // Movement is blocked by wall, do nothing.
            }
            else
            {
                // There is a free cell. Move all boxes forward, this is done by replacing the free cell with a '0'
                // and moving one step forward.
                cells[emptyRow][emptyCol] = 'O';
                cells[currRow + deltaRow][currCol + deltaCol] = '@';
                cells[currRow][currCol] = '.';
                (currCol, currRow) = (currCol + deltaCol, currRow + deltaRow);
            }

        }
        long totalSum = CalculateTotalSum(cells);

        Console.WriteLine("Task 1:");
        Console.WriteLine($"Total sum: {totalSum}");
    }

    static bool CanMoveUp(List<List<char>> cells, int col, int row, List<(int, int)> cellsToMoveUp)
    {
        // This method finds the cells whose content can and should be moved up.
        cellsToMoveUp.Add((col, row));

        // Check tile above the current one.
        char tile = cells[row - 1][col];
        if (tile == '.')
        {
            // Tile is empty, can move up.
            return true;
        }

        if (tile == '#')
        {
            // Tile is a wall, cannot move up
            return false;
        }

        if (tile == '[')
        {
            // Tile above is part of a box. Check that tile and the neighboring tile whether it can be moved up.
            return CanMoveUp(cells, col, row - 1, cellsToMoveUp) && CanMoveUp(cells, col + 1, row - 1, cellsToMoveUp);
        }

        if (tile == ']')
        {
            // Tile above is part of a box. Check that tile and the neighboring tile whether it can be moved up.
            return CanMoveUp(cells, col - 1, row - 1, cellsToMoveUp) && CanMoveUp(cells, col, row - 1, cellsToMoveUp);
        }

        throw new ArgumentException($"Tile {tile} is not in range");
    }

    static bool CanMoveDown(List<List<char>> cells, int col, int row, List<(int, int)> cellsToMoveDown)
    {
        // This method finds the cells whose content can and should be moved down.
        cellsToMoveDown.Add((col, row));

        // Check tile below the current one.
        char tile = cells[row + 1][col];
        if (tile == '.')
        {
            // Tile is empty, can move down.
            return true;
        }

        if (tile == '#')
        {
            // Tile is a wall, cannot move down
            return false;
        }

        if (tile == '[')
        {
            // Tile below is part of a box. Check that tile and the neighboring tile whether it can be moved down.
            return CanMoveDown(cells, col, row + 1, cellsToMoveDown) && CanMoveDown(cells, col + 1, row + 1, cellsToMoveDown);
        }

        if (tile == ']')
        {
            // Tile below is part of a box. Check that tile and the neighboring tile whether it can be moved down.
            return CanMoveDown(cells, col - 1, row + 1, cellsToMoveDown) && CanMoveDown(cells, col, row + 1, cellsToMoveDown);
        }

        throw new ArgumentException($"Tile {tile} is not in range");
    }

    static void Task2(List<List<char>> cells, string allMoves)
    {
        int colCount = cells[0].Count;
        int rowCount = cells.Count;

        // Create the doubled cells according to the given replacement rules.
        List<List<char>> doubledCells = [];
        for (int row = 0; row < rowCount; row++)
        {
            doubledCells.Add(Enumerable.Repeat('.', colCount * 2).ToList());
            for (int col = 0; col < colCount; col++)
            {
                (char c0, char c1) = cells[row][col] switch
                {
                    '#' => ('#', '#'),
                    'O' => ('[', ']'),
                    '.' => ('.', '.'),
                    '@' => ('@', '.'),
                    _ => throw new ArgumentException($"Tile '{cells[row][col]}' is out of range")
                };

                doubledCells[row][col * 2] = c0;
                doubledCells[row][col * 2 + 1] = c1;
            }
        }

        //Console.WriteLine(CellsToString(doubledCells));

        (int currCol, int currRow) = FindRobot(doubledCells);

        // Move the robot according to the input.
        foreach (char move in allMoves)
        {
            // Left and right moves are treated in a similar way to task 1.
            if (move == '>' || move == '<')
            {
                (int deltaCol, int deltaRow) = ((move == '>' ? 1 : -1), 0);
                (int emptyCol, int emptyRow) = (currCol + deltaCol, currRow + deltaRow);
                while (doubledCells[emptyRow][emptyCol] == '[' || doubledCells[emptyRow][emptyCol] == ']')
                {
                    (emptyCol, emptyRow) = (emptyCol + deltaCol, emptyRow + deltaRow);
                }
                if (doubledCells[emptyRow][emptyCol] == '#')
                {
                    // Movement is blocked by wall, do nothing.
                }
                else
                {
                    // There is a free cell. Move all boxes forward.
                    while (emptyCol != currCol || emptyRow != currRow)
                    {
                        doubledCells[emptyRow][emptyCol] = doubledCells[emptyRow - deltaRow][emptyCol - deltaCol];
                        (emptyCol, emptyRow) = (emptyCol - deltaCol, emptyRow - deltaRow);
                    }
                    doubledCells[currRow][currCol] = '.';
                    (currCol, currRow) = (currCol + deltaCol, currRow + deltaRow);
                }
            }
            else if (move == '^')
            {
                // Moving the robot up is different from moving it left/right.
                List<(int, int)> cellsToMoveUp = [];
                // Check recursively whether the box (and the boxes before this box and so on) can be moved up.
                if (CanMoveUp(doubledCells, currCol, currRow, cellsToMoveUp))
                {
                    List<(int, int)> sortedCellsToMoveUp = cellsToMoveUp.Distinct().ToList();
                    // Sort the affected boxs by their row, so that the top-most boxes get noved first.
                    sortedCellsToMoveUp.Sort(new SmallerRowComparer());

                    // Move the boxes and the robot.
                    foreach ((int movingCol, int movingRow) in sortedCellsToMoveUp)
                    {
                        doubledCells[movingRow - 1][movingCol] = doubledCells[movingRow][movingCol];
                        doubledCells[movingRow][movingCol] = '.';
                    }

                    currRow--;
                }
            }
            else if (move == 'v')
            {
                // Moving down is like moving up.
                List<(int, int)> cellsToMoveDown = [];
                if (CanMoveDown(doubledCells, currCol, currRow, cellsToMoveDown))
                {
                    // Check recursively whether the box (and the boxes before this box and so on) can be moved down.
                    List<(int, int)> sortedCellsToMoveDown = cellsToMoveDown.Distinct().ToList();
                    sortedCellsToMoveDown.Sort(new LargerRowComparer());

                    // Move the boxes and the robot.
                    foreach ((int movingCol, int movingRow) in sortedCellsToMoveDown)
                    {
                        doubledCells[movingRow + 1][movingCol] = doubledCells[movingRow][movingCol];
                        doubledCells[movingRow][movingCol] = '.';
                    }

                    currRow++;
                }
            }
            else
            {
                throw new ArgumentException($"Move {move} out of range");
            }

            //Console.WriteLine(CellsToString(doubledCells));
        }

        long totalSum = CalculateTotalSum(doubledCells);

        Console.WriteLine("Task 2:");
        Console.WriteLine($"Total sum: {totalSum}");
    }

    static void Main(string[] _)
    {
        // Create map and movements from input.
        string[] lines = File.ReadAllLines("data/input.txt");

        List<List<char>> cells = [];
        List<string> moves = [];
        var doReadMoves = false;

        foreach (var line in lines)
        {
            // Empty line is separator between map and movements. Map comes first.
            if (string.IsNullOrEmpty(line))
            {
                doReadMoves = true;
                continue;
            }

            if (doReadMoves)
            {
                // Join strings, ignoring line breaks.
                moves.Add(line);
            }
            else
            {
               cells.Add(line.Select(c => c).ToList());
            }
        }

        string allMoves = string.Join("", moves);

        Task1(cells, allMoves);
        Task2(cells, allMoves);

    }
}
