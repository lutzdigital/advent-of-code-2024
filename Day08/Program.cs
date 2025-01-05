namespace Day08;

/// <summary>
/// Solution for day 8 of the AoC 2024.
///
/// See https://adventofcode.com/2024/day/8 for the description of the task and the input data.
/// </summary>
internal class Program
{
    static void Task1(Dictionary<char, List<(int, int)>> frequenciesAndAntennas, int colCount, int rowCount)
    {
        // Using a hash set to store the antinodes makes it easy to detect duplicates.
        HashSet<(int, int)> antinodes = [];

        // For every set of antennas with the same frequency, go through each pair of antennas and check whether their antinodes are within the map.
        foreach (var antennas in frequenciesAndAntennas.Values)
        {
            for (int index1 = 0; index1 < antennas.Count - 1; index1++)
            {
                for (int index2 = index1 + 1; index2 < antennas.Count; index2++)
                {
                    (var antenna1Col, var antenna1Row) = antennas[index1];
                    (var antenna2Col, var antenna2Row) = antennas[index2];

                    // Calculate delta distance between two antennas.
                    (var deltaCol, var deltaRow) = (antenna2Col - antenna1Col, antenna2Row - antenna1Row);

                    (var antinode1Col, var antinode1Row) = (antenna1Col - deltaCol, antenna1Row - deltaRow);
                    (var antinode2Col, var antinode2Row) = (antenna2Col + deltaCol, antenna2Row + deltaRow);

                    // First antinode is delta distance away from first antenna.
                    if (antinode1Col >= 0 && antinode1Col < colCount && antinode1Row >= 0 && antinode1Row < rowCount)
                    {
                        // Only add antinode if it is in map.
                        antinodes.Add((antinode1Col, antinode1Row));
                    }

                    // Second antinode is  dalta distance away from antenna 2.
                    if (antinode2Col >= 0 && antinode2Col < colCount && antinode2Row >= 0 && antinode2Row < rowCount)
                    {
                        // Only add antinode if it is in map.
                        antinodes.Add((antinode2Col, antinode2Row));
                    }
                }
            }
        }

        // The number of unique antinodes corresponds to the elements in  the hash set.
        Console.WriteLine("Task 1:");
        Console.WriteLine($"Unique antinode count: {antinodes.Count}");
    }

    static void Task2(Dictionary<char, List<(int, int)>> frequenciesAndAntennas, int colCount, int rowCount)
    {
        // The algorithm for the second task works in the same way as for the first task, except that now all positions in line with the two antennas are possible antinodes.
        HashSet<(int, int)> antinodes = [];

        foreach (var antennas in frequenciesAndAntennas.Values)
        {
            for (int index1 = 0; index1 < antennas.Count - 1; index1++)
            {
                for (int index2 = index1 + 1; index2 < antennas.Count; index2++)
                {
                    (var antenna1Col, var antenna1Row) = antennas[index1];
                    (var antenna2Col, var antenna2Row) = antennas[index2];

                    (var deltaCol, var deltaRow) = (antenna2Col - antenna1Col, antenna2Row - antenna1Row);

                    // Iterate away from the first antenna twoards the edge of the map.
                    (var antinode1Col, var antinode1Row) = (antenna1Col, antenna1Row);
                    while (antinode1Col >= 0 && antinode1Col < colCount && antinode1Row >= 0 && antinode1Row < rowCount)
                    {
                        antinodes.Add((antinode1Col, antinode1Row));
                        (antinode1Col, antinode1Row) = (antinode1Col - deltaCol, antinode1Row - deltaRow);
                    }

                    // Iterate away from the second antenna towards the edge of the map.
                    (var antinode2Col, var antinode2Row) = (antenna2Col, antenna2Row);
                    while (antinode2Col >= 0 && antinode2Col < colCount && antinode2Row >= 0 && antinode2Row < rowCount)
                    {
                        antinodes.Add((antinode2Col, antinode2Row));
                        (antinode2Col, antinode2Row) = (antinode2Col + deltaCol, antinode2Row + deltaRow);
                    }
                }
            }
        }

        Console.WriteLine("Task 2:");
        Console.WriteLine($"Unique antinode count: {antinodes.Count}");
    }

    static void Main(string[] _)
    {
        // Read map from input.
        string[] lines = File.ReadAllLines("data/input.txt");

        int rowCount = lines.Length;
        int colCount = lines[0].Length;


        // Iterate through map and collect all antennas in a dictionary, with the frequency as key.
        Dictionary<char, List<(int, int)>> frequenciesAndAntennas = [];
        for (int row = 0; row < rowCount; row++)
        {
            for (int col = 0; col < colCount; col++)
            {
                var frequency = lines[row][col];
                if (frequency != '.')
                {
                    if (frequenciesAndAntennas.TryGetValue(frequency, out var antennas))
                    {
                        // There is already an entry theere, just add the antenna position to the list.
                        antennas.Add((col, row));
                    }
                    else
                    {
                        // No entry there yet, insert a new list with the first antenna position.
                        frequenciesAndAntennas.Add(frequency, [(col, row)]);
                    }
                }
            }
        }

        Task1(frequenciesAndAntennas, colCount, rowCount);
        Task2(frequenciesAndAntennas, colCount, rowCount);
    }
}
