namespace Day05;

/// <summary>
/// Solution for day 5 of the AoC 2024.
///
/// See https://adventofcode.com/2024/day/5 for the description of the task and the input data.
/// </summary>
internal class Program
{
    internal class PageComparer : IComparer<int>
    {
        private readonly List<(int, int)> _rules;

        public PageComparer(List<(int, int)> rules)
        {
            _rules = rules;
        }

        public int Compare(int pageA, int pageB)
        {
            // For implementing the IComparer interface, there must be a methode 'Compare' that returns a value that is
            // - less than 0 if the first element ('pageA') comes before the second element ('pageB').
            // - greater than 0 if the second elemtn comes before the first
            // - 0 if both elements are considered equal

            // Check if there is a rule that indicates an ordering between the given pages.
            foreach (var rule in _rules)
            {
                var shouldBeFirstPage = rule.Item1;
                var shouldBeSecondPage = rule.Item2;

                if (pageA == shouldBeFirstPage && pageB == shouldBeSecondPage)
                {
                    // 'pageA' comes before 'pageB'
                    return -1;
                }

                if (pageA == shouldBeSecondPage && pageB == shouldBeFirstPage)
                {
                    // 'pageB' comes before 'pageA'
                    return 1;
                }
            }

            // If no rule exists, then elements cannot be compared.
            // For the purpose of the task they are considered equal.
            return 0;
        }
    }

    static bool IsCorrectUpdate(List<int> update, List<(int, int)> rules)
    {
        // Compare pages pair-wise. The order is incorrect only if there is a contradicting rule.
        var firstPage = update.First();
        foreach (var page in update.Skip(1))
        {
            var secondPage = page;
            foreach (var rule in rules)
            {
                var shouldBeFirstPage = rule.Item1;
                var shouldBeSecondPage = rule.Item2;

                // Found a contradicting rule.
                if (firstPage == shouldBeSecondPage && secondPage == shouldBeFirstPage)
                {
                    return false;
                }
            }
            firstPage = secondPage;
        }

        // No contradicting rule found.
        return true;
    }

    static List<int> ReorderUpdate(List<int> update, List<(int, int)> rules)
    {
        // Create a new list by sorting with a custom comparer which uses the given list of rules for comparison.
        var reorderedUpdate = update.ToList();
        reorderedUpdate.Sort(new PageComparer(rules));

        return reorderedUpdate;
    }

    static void Task1(List<List<int>> updates, List<(int, int)> rules)
    {
        var totalSum = 0;

        // Add up middle numbers of each correct update.
        foreach (var update in updates)
        {
            if (IsCorrectUpdate(update, rules))
            {
                var middleNumber = update[update.Count / 2];
                //Console.WriteLine(string.Join(',', update));
                //Console.WriteLine(middleNumber);
                totalSum += middleNumber;
            }
        }

        Console.WriteLine("Task 1:");
        Console.WriteLine($"Total sum: {totalSum}");
    }

    static void Task2(List<List<int>> updates, List<(int, int)> rules)
    {
        var totalSum = 0;

        foreach (var update in updates)
        {
            // Re-order updates that are not correct.
            if (!IsCorrectUpdate(update, rules))
            {
                var reorderedUpdate = ReorderUpdate(update, rules);
                var middleNumber = reorderedUpdate[reorderedUpdate.Count / 2];
                //Console.WriteLine(string.Join(',', reorderedUpdate));
                //Console.WriteLine(middleNumber);
                totalSum += middleNumber;
            }
        }

        Console.WriteLine("Task 2:");
        Console.WriteLine($"Total sum: {totalSum}");
    }

    static void Main(string[] _)
    {
        // Create rules and updates from input.
        string[] lines = File.ReadAllLines("data/input.txt");

        List<(int, int)> rules = [];
        List<List<int>> updates = [];

        var doReadRules = true;
        foreach (var line in lines)
        {
            // Empty line is separator between rules and updates. Rules come first.
            if (string.IsNullOrEmpty(line))
            {
                doReadRules = false;
                continue;
            }

            if (doReadRules)
            {
                var parts = line.Split('|');
                int first = int.Parse(parts[0]);
                int second = int.Parse(parts[1]);
                rules.Add((first, second));
            }
            else
            {
                List<int> update = [];
                var parts = line.Split(',');
                foreach (var part in parts)
                {
                    update.Add(int.Parse(part));
                }
                updates.Add(update);
            }
        }

        Task1(updates, rules);
        Task2(updates, rules);
    }
}
