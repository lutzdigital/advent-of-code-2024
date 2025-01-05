namespace Day13;

/// <summary>
/// Solution for day 13 of the AoC 2024.
///
/// See https://adventofcode.com/2024/day/13 for the description of the task and the input data.
/// </summary>
internal class Program
{
    static (bool, long) PlayTask1(long buttonA_X, long buttonA_Y, long buttonB_X, long buttonB_Y, long prize_X, long prize_Y)
    {
        // As the number of button presses is limited, one can do a brute-force search for the solution.
        // Just try all possible combinations and check whether a prize can be won.

        bool canWinPrize = false;
        long minTokenCount = long.MaxValue;

        for (long pressACount = 0; pressACount <= 100; pressACount++)
        {
            for (long pressBCount = 0; pressBCount < 100; pressBCount++)
            {
                if ((buttonA_X * pressACount + buttonB_X * pressBCount == prize_X) && (buttonA_Y * pressACount + buttonB_Y * pressBCount == prize_Y))
                {
                    canWinPrize = true;
                    long tokenCount = pressACount * 3 + pressBCount * 1;
                    if (tokenCount < minTokenCount)
                    {
                        minTokenCount = tokenCount;
                    }
                }
            }
        }

        return (canWinPrize, minTokenCount);
    }

    static (bool, long) PlayTask2(long buttonA_X, long buttonA_Y, long buttonB_X, long buttonB_Y, long prize_X, long prize_Y)
    {
        // A brute-force approach is not possible for task 2. So instead we solve a linear equation.

        bool canWinPrize = false;
        long minTokenCount = long.MaxValue;

        prize_X += 10000000000000;
        prize_Y += 10000000000000;

        // We are now solving a linear equation with the two unknown variables 'pressACount' and 'pressBCount'.
        // Equation  I: buttonA_X * pressACount + buttonB_X * pressBCount = prize_X
        // Equation II: buttonA_Y * pressACount + buttonB_Y * pressBCount = prize_Y
        // If these equations are linearly independent, we have unique values for the two variables.
        // However, these values might not be integer. As we cannot have fraction button presses here, no solution can be found then.

        // Do a Gaussian Elimination by merging the two equations.
        long tmp1 = buttonA_X * buttonB_Y - buttonA_Y * buttonB_X;
        long tmp2 = buttonA_X * prize_Y - buttonA_Y * prize_X;
        long pressBCount = tmp2 / tmp1;

        // Check whether 'pressBCount' is an integer.
        if (pressBCount * tmp1 == tmp2)
        {
            // Now solve for 'preassACount'.
            long tmp3 = (prize_X - pressBCount * buttonB_X);
            long pressACount = tmp3 / buttonA_X;

            // Check whether 'pressACount' is an integer.
            if (pressACount * buttonA_X == tmp3)
            {
                canWinPrize = true;
                minTokenCount = pressACount * 3 + pressBCount * 1;
            }
        }

        return (canWinPrize, minTokenCount);
    }

    static void Main(string[] _)
    {
        // Read data from input.
        string[] lines = File.ReadAllLines("data/input.txt");
        int lineCount = lines.Length;

        long totalTokenCountTask1 = 0;
        int totalPrizeCountTask1 = 0;

        long totalTokenCountTask2 = 0;
        int totalPrizeCountTask2 = 0;

        int lineIndex = 0;
        while (lineIndex < lineCount)
        {
            int clawMachine = lineIndex / 4;
            var parts0 = lines[lineIndex].Split(' ');
            (long buttonA_X, long buttonA_Y) = (long.Parse(parts0[2][2..^1]), long.Parse(parts0[3][2..]));

            var parts1 = lines[lineIndex + 1].Split(' ');
            (long buttonB_X, long buttonB_Y) = (long.Parse(parts1[2][2..^1]), long.Parse(parts1[3][2..]));

            var parts2 = lines[lineIndex + 2].Split(' ');
            (long prize_X, long prize_Y) = (long.Parse(parts2[1][2..^1]), long.Parse(parts2[2][2..]));

            //Console.WriteLine($"A_X: {buttonA_X} A_Y: {buttonA_Y} B_X: {buttonB_X} B_Y: {buttonB_Y} P_X: {prize_X} P_Y: {prize_Y}");
            // Sanity check, make sure that no input value is equal or less than zero.
            if (buttonA_X <= 0 || buttonA_Y <= 0 || buttonB_X <= 0 || buttonB_Y <= 0 || prize_X <= 0 || prize_Y <= 0)
            {
                throw new ArgumentException("Argument is <= 0!");
            }

            // Do task 1.
            (bool canWinPrizeTask1, long minTokenCountTask1) = PlayTask1(buttonA_X, buttonA_Y, buttonB_X, buttonB_Y, prize_X, prize_Y);
            if (canWinPrizeTask1)
            {
                //Console.WriteLine($"Task1, claw machine {clawMachine}: Can win prize with {minTokenCountTask1} tokens");
                totalPrizeCountTask1++;
                totalTokenCountTask1 += minTokenCountTask1;
            }
            else
            {
                //Console.WriteLine($"Task 1, claw machine {clawMachine}: Cannot win prize");
            }

            // Do task 2.
            (bool canWinPrizeTask2, long minTokenCountTask2) = PlayTask2(buttonA_X, buttonA_Y, buttonB_X, buttonB_Y, prize_X, prize_Y);
            if (canWinPrizeTask2)
            {
                //Console.WriteLine($"Task2, claw machine {clawMachine}: Can win prize with {minTokenCountTask2} tokens");
                totalPrizeCountTask2++;
                totalTokenCountTask2 += minTokenCountTask2;
            }
            else
            {
                //Console.WriteLine($"Task 2, claw machine {clawMachine}: Cannot win prize");
            }

            lineIndex += 4;
        }

        Console.WriteLine("Task 1:");
        //Console.WriteLine($"Prize count: {totalPrizeCountTask1}");
        Console.WriteLine($"Token count: {totalTokenCountTask1}");

        Console.WriteLine("Task 2:");
        //Console.WriteLine($"Prize count: {totalPrizeCountTask2}");
        Console.WriteLine($"Token count: {totalTokenCountTask2}");
    }
}
