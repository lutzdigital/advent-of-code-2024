namespace Day21;

/// <summary>
/// Solution for day 21 of the AoC 2024.
///
/// See https://adventofcode.com/2024/day/21 for the description of the task and the input data.
/// </summary>
internal class Program
{
    static (int, int) CoordinatesNumericKeypad(char key)
    {
        // Convert a given key to the coordinates on the numeric keypad.
        (int col, int row) = key switch
        {
            '7' => (0, 0),
            '8' => (1, 0),
            '9' => (2, 0),
            '4' => (0, 1),
            '5' => (1, 1),
            '6' => (2, 1),
            '1' => (0, 2),
            '2' => (1, 2),
            '3' => (2, 2),
            '0' => (1, 3),
            'A' => (2, 3),
            _ => throw new ArgumentOutOfRangeException($"Numeric key {key} out of range")
        };

        return (col, row);
    }

    static (int, int) CoordinatesDirectionalKeypad(char key)
    {
        // Convert a given key to the coordinates on the directional keypad.
        (int col, int row) = key switch
        {
            '^' => (1, 0),
            'A' => (2, 0),
            '<' => (0, 1),
            'v' => (1, 1),
            '>' => (2, 1),
            _ => throw new ArgumentOutOfRangeException($"Directional key {key} out of range")
        };

        return (col, row);
    }

    static void CalculateInputNumericKeypad(string inputSoFar, List<string> inputs, (int, int) startPos, (int, int) endPos)
    {
        // Calculate the possible paths from a given start position to a given end position on the numeric keypad.
        // There might be multiple paths with the same length.
        (int startCol, int startRow) = startPos;
        (int endCol, int endRow) = endPos;

        if (startCol == endCol && startRow == endRow)
        {
            inputs.Add(inputSoFar + "A");
            return;
        }

        // Only go left if the robot arm does not get to the coordinates (0, 3), as there is no key.
        if (startCol > endCol && (startCol > 1 || startRow < 3))
        {
            CalculateInputNumericKeypad(inputSoFar + '<', inputs, (startCol - 1, startRow), endPos);
        }

        if (startCol < endCol)
        {
            CalculateInputNumericKeypad(inputSoFar + '>', inputs, (startCol + 1, startRow), endPos);
        }

        if (startRow > endRow)
        {
            CalculateInputNumericKeypad(inputSoFar + '^', inputs, (startCol, startRow - 1), endPos);
        }

        // Only go down if the robot arm does not get to the coordinates (0, 3), as there is no key.
        if (startRow < endRow && (startRow < 2 || startCol > 0))
        {
            CalculateInputNumericKeypad(inputSoFar + 'v', inputs, (startCol, startRow + 1), endPos);
        }
    }

    static void CalculateInputDirectionalKeypad(string inputSoFar, List<string> inputs, (int, int) startPos, (int, int) endPos)
    {
        // Calculate the possible paths from a given start position to a given end position on the numeric keypad.
        // There might be multiple paths with the same length.
        (int startCol, int startRow) = startPos;
        (int endCol, int endRow) = endPos;

        if (startCol == endCol && startRow == endRow)
        {
            inputs.Add(inputSoFar + "A");
            return;
        }

        // Only go left if the robot arm does not get to the coordinates (0, 0), as there is no key.
        if (startCol > endCol && (startCol > 1 || startRow > 0))
        {
            CalculateInputDirectionalKeypad(inputSoFar + '<', inputs, (startCol - 1, startRow), endPos);
        }

        if (startCol < endCol)
        {
            CalculateInputDirectionalKeypad(inputSoFar + '>', inputs, (startCol + 1, startRow), endPos);
        }

        // Only go up if the robot arm does not get to the coordinates (0, 0), as there is no key.
        if (startRow > endRow && (startCol > 0))
        {
            CalculateInputDirectionalKeypad(inputSoFar + '^', inputs, (startCol, startRow - 1), endPos);
        }

        if (startRow < endRow)
        {
            CalculateInputDirectionalKeypad(inputSoFar + 'v', inputs, (startCol, startRow + 1), endPos);
        }
    }

    static List<string> CalculateNextLevelInputs(string input)
    {
        List<string> nextLevelInputs = [""];
        char startKey = 'A';
        foreach (var endKey in input)
        {
            // Calculate possible inputs going form the start key to the end key of the given sequence.
            List<string> inputsPerKey = [];
            CalculateInputDirectionalKeypad("", inputsPerKey, CoordinatesDirectionalKeypad(startKey), CoordinatesDirectionalKeypad(endKey));
            List<string> newInputsTemp = [];
            foreach (var inputBegin in nextLevelInputs)
            {
                foreach (var inputPerKey in inputsPerKey)
                {
                    newInputsTemp.Add(inputBegin + inputPerKey);
                }
            }
            nextLevelInputs = newInputsTemp;
            startKey = endKey;
        }

        return nextLevelInputs;
    }

    static long CalculateLength(Dictionary<(string, int), long> calculatedLengths, string input, int level)
    {
        // Recursively calculate the length of the input on the directional keypad.
        // Store reuslts in a dictionary to avoid recalculation.
        if (calculatedLengths.TryGetValue((input, level), out long calculatedLength))
        {
            return calculatedLength;
        }

        // The last directional keypad is controlled by a human, not by a robot arm.
        // There the caluclated length is the string length of the input.
        if (level == 0)
        {
            calculatedLengths[(input, level)] = input.Length;
            return input.Length;
        }

        // Split the input that is needed for the keypad 'above' by the 'A' symbol-
        // So every part sequence starts and ends with an 'A'.
        List<string> parts = input.Split('A').SkipLast(1).Select(s => s + "A").ToList();

        long length = 0;
        foreach (string part in parts)
        {
            long minNextLevelLength = long.MaxValue;
            // Go through all possible shortest input sequences for this part sequence.
            List<string> nextLevelInputs = CalculateNextLevelInputs(part);
            foreach(string nextLevelInput in nextLevelInputs)
            {
                // Calculate the length of the keypad 'below' and find the minimum.
                long nextLevelLength = CalculateLength(calculatedLengths, nextLevelInput, level - 1);
                if (nextLevelLength < minNextLevelLength)
                {
                    minNextLevelLength = nextLevelLength;
                }
            }
            length += minNextLevelLength;
        }

        // Store the result back in the dictionary.
        calculatedLengths[(input, level)] = length;

        return length;
    }

    static void DoTask(string[] numericalInputs, bool isTask2)
    {
        int level = isTask2
            ? 25
            : 2;

        long totalComplexity = 0;

        Dictionary<(string, int), long> calculatedLengths = [];

        // Calculate complexity of all codes.
        foreach (string numericalInput in numericalInputs)
        {
            // Translate button presses on numeric keypad to presses on the next unterlying directional keypad
            List<string> directionalInputs = [""];
            char startKey = 'A';
            foreach (char endKey in numericalInput)
            {
                List<string> inputsPerKey = [];
                CalculateInputNumericKeypad("", inputsPerKey, CoordinatesNumericKeypad(startKey), CoordinatesNumericKeypad(endKey));
                List<string> newInputs1 = [];
                foreach (var input1Begin in directionalInputs)
                {
                    foreach (var inputPerKey in inputsPerKey)
                    {
                        newInputs1.Add(input1Begin + inputPerKey);
                    }
                }
                directionalInputs = newInputs1;
                startKey = endKey;
            }

            // Calculate directional input for the given number of directional keypads.
            long minDirectionalLength = long.MaxValue;
            foreach (string directionalInput in directionalInputs)
            {
                // Call to recursive method.
                long directionalLength = CalculateLength(calculatedLengths, directionalInput, level);
                if (directionalLength < minDirectionalLength)
                {
                    minDirectionalLength = directionalLength;
                }
            }

            int numericPart = int.Parse(numericalInput[..^1]);
            long complexity = minDirectionalLength * numericPart;
            totalComplexity += complexity;
        }

        Console.WriteLine($"Task {(isTask2 ? 2 : 1)}:");
        Console.WriteLine($"Total complexity: {totalComplexity}");

    }

    static void Main(string[] _)
    {
        string[] numericalInputs = File.ReadAllLines("data/input.txt");

        DoTask(numericalInputs, false);
        DoTask(numericalInputs, true);
    }
}
