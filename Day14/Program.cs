using System.Text;

namespace Day14;

/// <summary>
/// Solution for day 14 of the AoC 2024.
///
/// See https://adventofcode.com/2024/day/14 for the description of the task and the input data.
/// </summary>
internal class Program
{
    static string CellsToString(List<List<int>> cells)
    {
        // Convert the cells with the robots to a long string with line breaks.

        int rowCount = cells.Count;

        var sb = new StringBuilder();
        for (int row = 0; row < rowCount; row++)
        {
            string rowAsString = string.Join("", cells[row].Select(x => x > 0 ? x.ToString() : ".").ToArray());
            sb.AppendLine(rowAsString);
        }

        return sb.ToString();
    }

    static List<(int, int)> CalculateNewPositions(List<(int, int)> oldPositions, List<(int, int)> velocities)
    {
        // Calculate the new positions for all robots, given their old positions and their velocities.

        int colCount = 101;
        int rowCount = 103;

        List<(int, int)> newPositions = [];
        for(int i = 0; i < oldPositions.Count; i++)
        {
            (int oldCol, int oldRow) = oldPositions[i];
            (int deltaCol, int deltaRow) = velocities[i];
            // Caclulate new positions using the modulo operation.
            // NOTE: Before using the modulo operation, 'colCount' resp. 'rowCount' are added so that the values are positive.
            (int newCol, int newRow) = ((oldCol + deltaCol + colCount) % colCount, (oldRow + deltaRow + rowCount) % rowCount);
            newPositions.Add((newCol, newRow));
        }

        return newPositions;
    }

    static bool HasRobot(List<List<int>> cells, int col, int row)
    {
        // Check wehter there is at least one robot in a given cell.
        // Cells outside the allowed area are not counted.

        int colCount = cells[0].Count;
        int rowCount = cells.Count;

        if (col < 0 || col >= colCount || row < 0 || row >= rowCount)
        {
            return false;
        }

        return cells[row][col] != 0;
    }

    static int CountPositionsWithTwoNeighbors(List<List<int>> cells, List<(int, int)> positions)
    {
        // Check whether a robot in a given cell has at least two neighbors in the adjacent 8 cells.

        int count = 0;
        foreach ((int col, int row) in positions)
        {
            List<(int, int)> steps = [(0, -1), (1, -1), (1, 0), (1, 1), (0, 1), (-1, 1), (-1, 0), (-1, -1)];
            foreach ((int deltaCol, int deltaRow) in steps)
            {
                if (HasRobot(cells, col + deltaCol, row + deltaRow))
                {
                    count++;
                }
            }
        }

        return count;
    }

    static void Task1(List<(int, int)> initialPositions, List<(int, int)> velocities)
    {
        // For task 1, it is enough to calculatre the new positions for every robot for every iteration.
        // The robots are not interfering with each other, so there is no need to have a map.
        List<(int, int)> positions = initialPositions;
        for (long iteration = 1; iteration <= 100; iteration++)
        {
            positions = CalculateNewPositions(positions, velocities);
        }

        long upperLeftCount = 0;
        long upperRightCount = 0;
        long lowerLeftCount = 0;
        long lowerRightCount = 0;
        foreach ((int col, int row) in positions)
        {
            if (col < 50 && row < 51) { upperLeftCount++; }
            else if (col > 50 && row < 51) { upperRightCount++; }
            else if (col < 50 && row > 51) { lowerLeftCount++; }
            else if (col > 50 && row > 51) { lowerRightCount++; }
        }

        //Console.WriteLine($"{upperLeftCount} {upperRightCount} {lowerLeftCount} {lowerRightCount}");
        long safetyFactor = upperLeftCount * upperRightCount * lowerLeftCount * lowerRightCount;
        Console.WriteLine("Task 1:");
        Console.WriteLine($"Safety factor: {safetyFactor}");
    }

    static void Task2(List<(int, int)> initialPositions, List<(int, int)> velocities)
    {
        // For task 2, it was a bit difficult to find a pattern by which the christmas tree can be found, as there was no indication what exact shape
        // the christmas tree has. Also, the taks said 'most of the robots should arrange themselves', not all robots.
        // After some experimentation I found that it is enough to search for a high number of 'adjacent robots'.
        // Here, 'Adjacent robots' means that for a robot there are at least two neighboring cells that also contain robots.

        int colCount = 101;
        int rowCount = 103;

        List<List<int>> cells = [];
        for (int row = 0; row < rowCount; row++)
        {
            cells.Add(Enumerable.Repeat(0, colCount).ToList());
        }

        List<(int, int)> positions = initialPositions;

        // For every iteration, a map is created. This is done incrementally: the robots are added to the map,
        // then the map is checked, then the robots are removed from the map.
        for (long iteration = 1; iteration <= 10000; iteration++)
        {
            positions = CalculateNewPositions(positions, velocities);
            foreach ((int col, int row) in positions)
            {
                cells[row][col]++;
            }

            // Do many iterations and look for spike in the numbers odf adjacent robots.
            int hasTwoNeighborsCount = CountPositionsWithTwoNeighbors(cells, positions);
            if (hasTwoNeighborsCount > 1000)
            {
                // Uncomment the following lines to visually check the result.
                //Console.WriteLine(hasTwoNeighborsCount);
                //Console.WriteLine(CellsToString(cells));
                Console.WriteLine("Task 2:");
                Console.WriteLine($"Iteration: {iteration}");
                break;
            }

            foreach ((int col, int row) in positions)
            {
                cells[row][col]--;
            }
        }
    }

    static void Main(string[] _)
    {
        string[] lines = File.ReadAllLines("data/input.txt");

        List<(int, int)> initialPositions = [];
        List<(int, int)> velocities = [];
        foreach (var line in lines)
        {
            var parts = line.Split(' ');
            var positionAsString = parts[0][2..].Split(',');
            (int col, int row) = (int.Parse(positionAsString[0]), int.Parse(positionAsString[1]));
            initialPositions.Add((col, row));

            var velocityAsString = parts[1][2..].Split(',');
            (int deltaCol, int deltaRow) = (int.Parse(velocityAsString[0]), int.Parse(velocityAsString[1]));
            velocities.Add((deltaCol, deltaRow));

            // Sanity check that velocities are in valid range.
            if (deltaCol < -101 || deltaCol > 101 || deltaRow < -103 || deltaRow > 103)
            {
                throw new ArgumentException($"Velocity {deltaCol},{deltaRow} is out of range");
            }
        }

        Task1(initialPositions, velocities);
        Task2(initialPositions, velocities);
    }
}
