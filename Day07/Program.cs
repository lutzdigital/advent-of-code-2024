namespace Day07;

/// <summary>
/// Solution for day 7 of the AoC 2024.
///
/// See https://adventofcode.com/2024/day/7 for the description of the task and the input data.
/// </summary>
internal class Program
{

    static bool CanBeSolvedTask1(long result, long currValue, List<long> operands, int currIndex)
    {
        // For solving the task, all possible combinations of operators are tried out. This is done using a recursive depth-first search.

        // Once there are no more operands, the maximum iteration is reached and the result has to be checked.
        if (currIndex == operands.Count)
        {
            return currValue == result;
        }

        // Numbers can become very large. Therefore this an early exit, to avoid superflous iterations.
        // The current value cannot get smaller in later iterations, so we can exit if we are already past the target result.
        if (currValue > result)
        {
            return false;
        }

        // In the first iteration the current value is just the first operand.
        if (currIndex == 0)
        {
            return CanBeSolvedTask1(result, operands[currIndex], operands, currIndex + 1);
        }

        // For the other operands we do recursive calls for both available operators.
        // The equation can be solved if at least one operator leads to a successful result.
        return
        (
            CanBeSolvedTask1(result, currValue * operands[currIndex], operands, currIndex + 1)
            ||
            CanBeSolvedTask1(result, currValue + operands[currIndex], operands, currIndex + 1)
        );
    }

    static bool CanBeSolvedTask2(long result, long currValue, List<long> operands, int currIndex)
    {
        // The second task can be solved in the same way as the first one, except for the additional concatenation operator.
        // The concatenation is done by converting the numbers to strings, conacatenating the strings and parsing the result back to a number.
        // This is not the computationally most efficient way (as the main work of the concatenation then is delegated to the conversion routines),
        // but it only takes one line of code.
        if (currIndex == operands.Count)
        {
            return currValue == result;
        }

        // Again, use an early exit if the result is already too large. Like the other operations, the concatenation can make numbers only bigger.
        if (currValue > result)
        {
            return false;
        }

        if (currIndex == 0)
        {
            return CanBeSolvedTask2(result, operands[currIndex], operands, currIndex + 1);
        }

        return
        (
            CanBeSolvedTask2(result, currValue * operands[currIndex], operands, currIndex + 1)
            ||
            CanBeSolvedTask2(result, currValue + operands[currIndex], operands, currIndex + 1)
            ||
            CanBeSolvedTask2(result, long.Parse($"{currValue}{operands[currIndex]}"), operands, currIndex + 1)
        );
    }

    static void Main(string[] _)
    {
        // Read results and operands from input.
        string[] lines = File.ReadAllLines("data/input.txt");

        List<(long, List<long>)> equations = [];
        foreach (var line in lines)
        {
            var parts = line.Split(' ');

            // Result has an extra colon, therefore the last character of the string is omitted.
            long result = long.Parse(parts[0][..^1]);

            List<long> operands = [];
            foreach (var part in parts.Skip(1))
            {
                operands.Add(long.Parse(part));
            }
            equations.Add((result, operands));
        }


        long totalCalibrationResultTask1 = 0;
        long totalCalibrationResultTask2 = 0;
        foreach (var (result, operands) in equations)
        {
            // Both checks are done simultaneously.
            var canBeSolvedTask1 = CanBeSolvedTask1(result, 0, operands, 0);
            //Console.WriteLine($"Can be solved (task 1): {canBeSolvedTask1}");
            var canBeSolvedTask2 = CanBeSolvedTask2(result, 0, operands, 0);
            //Console.WriteLine($"Can be solved (task 2): {canBeSolvedTask2}");

            if (canBeSolvedTask1)
            {
                totalCalibrationResultTask1 += result;
            }

            if (canBeSolvedTask2)
            {
                totalCalibrationResultTask2 += result;
            }
        }

        Console.WriteLine("Task 1");
        Console.WriteLine($"Total calibration result: {totalCalibrationResultTask1}");
        Console.WriteLine("Task 2");
        Console.WriteLine($"Total calibration result: {totalCalibrationResultTask2}");
    }
}
