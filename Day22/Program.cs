namespace Day22;

/// <summary>
/// Solution for day 22 of the AoC 2024.
///
/// See https://adventofcode.com/2024/day/22 for the description of the task and the input data.
/// </summary>
internal class Program
{
    static ulong CalculateNextSecretNumber(ulong secretNumber)
    {
        // Calculate next secret number according to given rules.
        // The operation 'modulo 16777216' is equivalent to a bit-wise AND with 16777215.
        ulong firstResult = secretNumber << 6;
        secretNumber ^= firstResult;
        secretNumber &= 16777215;

        ulong secondResult = secretNumber >> 5;
        secretNumber ^= secondResult;
        secretNumber &= 16777215;

        ulong thirdResult = secretNumber << 11;
        secretNumber ^= thirdResult;
        secretNumber &= 16777215;

        return secretNumber;
    }

    static void Main(string[] _)
    {
        // Read the initial secret numbers from intput.
        string[] lines = File.ReadAllLines("data/input.txt");
        List<ulong> initialSecretNumbers = lines.Select(ulong.Parse).ToList();

        // All sceret numbers for all buyers.
        List<List<ulong>> secretNumberListsForAllBuyers = [];
        // All changes for all buyers (needed for task 2).
        List<List<int>> changeListsForAllBuyers = [];

        // Go through every secret number and calculate the next 2000 secret numbers as well as the changes.
        foreach (ulong initialSecretNumber in initialSecretNumbers)
        {
            List<ulong> secretNumberList = Enumerable.Repeat<ulong>(0, 2001).ToList();
            List<int> changeList = Enumerable.Repeat(0, 2000).ToList();

            secretNumberList[0] = initialSecretNumber;
            for (int secretNumberIndex = 1; secretNumberIndex <= 2000; secretNumberIndex++)
            {
                secretNumberList[secretNumberIndex] = CalculateNextSecretNumber(secretNumberList[secretNumberIndex - 1]);
                changeList[secretNumberIndex - 1] = (int)(secretNumberList[secretNumberIndex] % 10) - (int)(secretNumberList[secretNumberIndex - 1] % 10);
            }

            secretNumberListsForAllBuyers.Add(secretNumberList);
            changeListsForAllBuyers.Add(changeList);
        }

        // For task 1, just add up all 2000th secret numbers.
        ulong totalSum = 0;
        foreach (var secretNumberList in secretNumberListsForAllBuyers)
        {
            totalSum += secretNumberList[2000];
        }

        Console.WriteLine("Task 1");
        Console.WriteLine($"Total sum: {totalSum}");

        // For task 2, go through all change lists and store all sequences of four consecutive changes in a dictionary
        // with the accumulated expected amount of bananas. An additional hash set is needed to detect whether a buyer has already seen
        // a sequence, as a buyer will sell immediately if it sees that sequence. 
        Dictionary<(int, int, int, int), ulong> allPatternsForAllBuyers = [];
        for (int listIndex = 0; listIndex < changeListsForAllBuyers.Count; listIndex++)
        {
            HashSet<(int, int, int, int)> allPatternsForOneBuyer = [];
            var changeList = changeListsForAllBuyers[listIndex];
            for (int changeIndex = 3; changeIndex < 2000; changeIndex++)
            {
                var pattern = (changeList[changeIndex - 3], changeList[changeIndex - 2], changeList[changeIndex - 1], changeList[changeIndex]);
                if (allPatternsForOneBuyer.Add(pattern))
                {
                    ulong oldValue = allPatternsForAllBuyers.TryGetValue(pattern, out ulong value) ? value : 0;
                    allPatternsForAllBuyers[pattern] = (secretNumberListsForAllBuyers[listIndex][changeIndex + 1] % 10) + oldValue;
                }
            }
        }

        // Get the maximum accumulated sum for all encountered sequences.
        ulong maxBananaCount = allPatternsForAllBuyers.Max(x => x.Value);

        Console.WriteLine("Task 2:");
        Console.WriteLine($"Max banana count: {maxBananaCount}");
    }
}
