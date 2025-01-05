namespace Day02;

/// <summary>
/// Solution for day 2 of the AoC 2024.
///
/// See https://adventofcode.com/2024/day/2 for the description of the task and the input data.
/// </summary>
internal class Program
{
    static bool IsAscending(List<long> report)
    {
        if (report.Count == 1)
        {
            return true;
        }

        long previous = report[0];
        foreach (var level in report.Skip(1))
        {
            // A report is not ascending if the a level is not greater than the previous level or differs more than 3.
            if (level <= previous || level > previous + 3)
            {
                return false;
            }
            previous = level;
        }

        return true;
    }

    static bool IsDescending(List<long> report)
    {
        if (report.Count == 1)
        {
            return true;
        }

        long previous = report[0];
        foreach (var level in report.Skip(1))
        {
            // A report is not descending if the a level is not less than the previous level or differs more than 3.
            if (level >= previous || level < previous - 3)
            {
                return false;
            }
            previous = level;
        }

        return true;
    }

    static bool IsProblemDampenerSafe(List<long> report)
    {
        if (report.Count == 1)
        {
            return true;
        }

        for (int i = 0; i < report.Count; i++)
        {
            // Create a modified report by emitting one level at a time and check whether it is safe then.
            // NOTE: If a report is safe without omitting a level, it is also safe if one is omitted.
            List<long> modifiedReport = [];
            for (int j = 0; j < report.Count; j++)
            {
                if (j == i)
                {
                    continue;
                }
                modifiedReport.Add(report[j]);
            }

            if (IsAscending(modifiedReport) || IsDescending(modifiedReport))
            {
                return true;
            }
        }

        return false;
    }

    static void Task1(List<List<long>> reports)
    {
        long safeReportCount = 0;

        // A report is safe if it is either ascending or descending.
        foreach (var report in reports)
        {
            if (IsAscending(report) || IsDescending(report))
            {
                safeReportCount++;
            }
        }

        Console.WriteLine("Task 1:");
        Console.WriteLine($"Safe report count: {safeReportCount}");
    }

    static void Task2(List<List<long>> reports)
    {
        long safeReportCount = 0;

        foreach (var report in reports)
        {
            // A report is safe if one level can be omitted such that it is either ascending or descending then.
            if (IsProblemDampenerSafe(report))
            {
                safeReportCount++;
            }
        }

        Console.WriteLine("Task 2:");
        Console.WriteLine($"Safe report count: {safeReportCount}");
    }

    static void Main(string[] _)
    {
        // Create reports from input.
        string[] lines = File.ReadAllLines("data/input.txt");

        List<List<long>> reports = [];
        for (int i = 0; i < lines.Length; i++)
        {
            try
            {
                var parts = lines[i].Split(" ");
                var report = parts.Select(long.Parse).ToList();
                if (report is null || report.Count == 0)
                {
                    throw new FormatException();
                }

                reports.Add(report);
            }
            catch (FormatException)
            {
                Console.WriteLine($"Unable to parse '{lines[i]}'");
            }
        }

        // Tasks differ only by their definition of a safe report.
        Task1(reports);
        Task2(reports);
    }
}
