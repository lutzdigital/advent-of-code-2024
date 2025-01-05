namespace Day23;

/// <summary>
/// Solution for day 23 of the AoC 2024.
///
/// See https://adventofcode.com/2024/day/23 for the description of the task and the input data.
/// </summary>
internal class Program
{
    static void Task1(string[] lines)
    {
        // Both the computers and the available direct connections are stored in a has set so that only the unique computers and connections are stored.
        // Connections are stored with the alphabetically first computer, then the alphabetically second computer, separated by a comma. 
        HashSet<string> computers = [];
        HashSet<string> connections = [];

        foreach (string line in lines)
        {
            var parts = line.Split('-');

            computers.Add(parts[0]);
            computers.Add(parts[1]);

            // Make sure that the computers involved in a connection are stored alphabetically.
            connections.Add(string.Join(',', parts.ToList().OrderBy(s => s)));
        }

        // Store sets of 3 in a hash set so that only the unique sets are counted.
        HashSet<string> setsOf3 = [];

        // For any given connection of two computers, check whether there is a third computer that connects to both.
        foreach (string connection in connections)
        {
            string firstComputer = connection[0..2];
            string secondComputer = connection[3..5];

            foreach (string thirdComputer in computers)
            {
                if (thirdComputer != firstComputer && thirdComputer != secondComputer)
                {
                    string connectionToFirst = string.Join(',', new List<string> { firstComputer, thirdComputer }.OrderBy(s => s));
                    string connectionToSecond = string.Join(',', new List<string> { secondComputer, thirdComputer }.OrderBy(s => s));

                    // If thrid computer connects to the other two computers, that is a set of 3 computers.
                    if (connections.Contains(connectionToFirst) && connections.Contains(connectionToSecond))
                    {
                        string setof3 = string.Join(',', new List<string> { firstComputer, secondComputer, thirdComputer }.ToList().OrderBy(s => s));
                        setsOf3.Add(setof3);
                    }
                }
            }
        }

        // Filter according to the given condition.
        var setsOf3ThatStartWithT = setsOf3.Where(s => s[0] == 't' || s[3] == 't' || s[6] == 't').ToList();

        Console.WriteLine("Task 1:");
        Console.WriteLine($"Count of sets: {setsOf3ThatStartWithT.Count}");
    }

    static void Task2(string[] lines)
    {
        // For every set of computers, create an adjacency list, i.e. a list of computers that connect directly to all members of the set.
        Dictionary<string, List<string>> adjacencies = [];

        // Create a queue for gradually expanding a set of inter-connected computers. The first item of a tuple is a
        // set of computers that are known to be inter-connected. The second item is a computer that is a candidate for joining this set.
        Queue<(List<string>, string)> queue = [];

        // Create initial adjacency lists and initial items for the queue.
        // The initial adjacencies consist only of single computers and their direct connections.
        // The initial queue items are only those that are given by the initial two-computer connections.
        foreach (string line in lines)
        {
            var parts = line.Split('-');

            List<string> oldAdjacencies0 = (adjacencies.TryGetValue(parts[0], out List<string>? adjacencies0)) ? adjacencies0 : [];
            List<string> newAdjacencies0 = new(oldAdjacencies0.Append(parts[1]));
            adjacencies[parts[0]] = newAdjacencies0;

            List<string> oldAdjacencies1 = (adjacencies.TryGetValue(parts[1], out List<string>? adjacencies1)) ? adjacencies1 : [];
            List<string> newAdjacencies1 = new(oldAdjacencies1.Append(parts[0]));
            adjacencies[parts[1]] = newAdjacencies1;

            queue.Enqueue(([parts[0]], parts[1]));
        }

        // Now gradually expand the sets of inter-connected computers.
        while (queue.Count > 0)
        {
            (List<string> group, string candidate) = queue.Dequeue();

            // To avoid unnecessary re-calculations, the sets of inter-connected computers are sotred in the 'adjacencies' dictionary.
            // // The key to the dictionary are the names of the computers, ordered alphabetically and separated by commas.
            List<string> expandedGroup = new(group.Append(candidate).OrderBy(s => s));
            string expandedGroupKey = string.Join(',', expandedGroup);

            // Only expand group if the expansion is not already in the dictionary.
            if (!adjacencies.ContainsKey(expandedGroupKey))
            {
                // Expand the group by intersectiong the direct connections of the group memeber with the direct connections of the candidate.
                // The candidate is known to connect to all computers of the group, but it is not known whether it also connects
                // to all other computers the group connects to.
                string groupKey = string.Join(',', group);
                string candidateKey = candidate;

                List<string> groupAdjacencies = adjacencies[groupKey];
                List<string> candidateAdjacencies = adjacencies[candidateKey];
                List<string> expandedGroupAdjacencies = groupAdjacencies.Intersect(candidateAdjacencies).ToList();

                // Store the result of the expansion in the dictionary.
                adjacencies.Add(expandedGroupKey, expandedGroupAdjacencies);

                // The new candidates for expansion are thos computers that are in the intersected list of adjacencies.
                foreach (string newCandidate in expandedGroupAdjacencies)
                {
                    queue.Enqueue((expandedGroup, newCandidate));
                }
            }
        }

        // Search for the largest set of inter-connected computers. The keys to the dictionary already contain the computers stored alphabetically.
        int maxLength = 0;
        string password = "";
        foreach (var key in adjacencies.Keys)
        {
            if (key.Length > maxLength)
            {
                maxLength = key.Length;
                password = key;
            }
        }

        Console.WriteLine("Task 2:");
        Console.WriteLine($"Password: {password}");

    }

    static void Main(string[] _)
    {
        string[] lines = File.ReadAllLines("data/input.txt");

        Task1(lines);
        Task2(lines);
    }
}
