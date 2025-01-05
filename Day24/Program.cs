namespace Day24;

/// <summary>
/// Solution for day 24 of the AoC 2024.
///
/// See https://adventofcode.com/2024/day/24 for the description of the task and the input data.
/// </summary>
internal class Program
{
    // Data structure for node.
    internal class Node
    {
        public int? Output { get; set; }
        public string? Operation { get; set; }
        public string? Input0Name { get; set; }
        public string? Input1Name { get; set; }
    }

    static int Evaluate(string name, Dictionary<string, Node> nodes)
    {
        Node node = nodes[name];

        // Check if result has already been calculated.
        // For the Xnn and Ynn inputs the output is already set.
        if (node.Output.HasValue)
        {
            return node.Output.Value;
        }

        string? operation = node.Operation;
        string? input0Name = node.Input0Name;
        string? input1Name = node.Input1Name;

        if (operation is null || input0Name is null || input1Name is null)
        {
            throw new ArgumentOutOfRangeException($"Node {name} contains invalid data");
        }

        // Output of node depends on the two inputs.
        int input0 = Evaluate(input0Name, nodes);
        int input1 = Evaluate(input1Name, nodes);

        // Combine the two inputs.
        int output = node.Operation switch
        {
            "AND" => (input0 == 1 && input1 == 1) ? 1 : 0,
            "OR" => (input0 == 1 || input1 == 1) ? 1 : 0,
            "XOR" => ((input0 == 1 && input1 == 0) || (input0 == 0 && input1 == 1)) ? 1 : 0,
            _ => throw new ArgumentOutOfRangeException($"Node {name} contains invalid data")
        };

        // Store result to avoid recalculation.
        node.Output = output;

        return output;
    }

    static long Calculate(Dictionary<string, Node> nodes, List<(string, int)> inputValues)
    {
        // Reset all outputs.
        foreach (Node node in nodes.Values)
        {
            node.Output = null;
        }

        // Set the outputs of all input nodes.
        foreach (var inputNode in inputValues)
        {
            (string name, int output) = inputNode;
            nodes[name].Output = output;
        }

        // Calculate result by evaluating all output nodes z00, z01, z02 etc. and combining them.
        long result = 0;
        int outputIndex = 0;
        string outputName = string.Format("z{0:00}", outputIndex);
        while (nodes.ContainsKey(outputName))
        {
            long output = Evaluate(outputName, nodes);
            result += output << outputIndex;
            outputIndex++;
            outputName = string.Format("z{0:00}", outputIndex);
        }

        return result;
    }

    static List<(string, int)> CreateInputValues(long x, long y, int maxInputIndex)
    {
        List<(string, int)> inputValues = [];

        for (int inputIndex = 0; inputIndex <= maxInputIndex; inputIndex++)
        {
            int inputX = ((x & (1L << inputIndex)) != 0) ? 1 : 0;
            inputValues.Add((string.Format("x{0:00}", inputIndex), inputX));

            int inputY = ((y & (1L << inputIndex)) != 0) ? 1 : 0;
            inputValues.Add((string.Format("y{0:00}", inputIndex), inputY));
        }

        return inputValues;
    }

    static List<string> FindDependencies(Dictionary<string, Node> nodes, string nodeName)
    {
        List<string> dependencies = [];
        Queue<string> queue = [];
        queue.Enqueue(nodeName);

        while (queue.Count > 0)
        {
            string currentNodeName = queue.Dequeue();
            Node currentNode = nodes[currentNodeName];
            dependencies.Add(currentNodeName);
            if (currentNode.Operation != "INP")
            {
                queue.Enqueue(currentNode.Input0Name!);
                queue.Enqueue(currentNode.Input1Name!);
            }
        }

        return dependencies;
    }

    static void Task1(Dictionary<string, Node> nodes, List<(string, int)> inputValues)
    {
        long result = Calculate(nodes, inputValues);

        Console.WriteLine("Task 1:");
        Console.WriteLine($"Result: {result}");
    }

    static void Test(Dictionary<string, Node> nodes)
    {
        // This is a test method to identify the bits that are incorrectly wired in the adder simulation.
        // For every every bit, it tests the result if the bit is set only in one input, but not the other.
        // Additionally it also tests whether the carry-over from the preceding bit works.
        int maxInputIndex = 44;

        for (int inputIndex = 0; inputIndex <= maxInputIndex; inputIndex++)
        {
            // Xn is set, but not Yn.
            long x0 = 1L << inputIndex;
            long y0 = 0;
            List<(string, int)> inputValues0 = CreateInputValues(x0, y0, maxInputIndex);
            long result0 = Calculate(nodes, inputValues0);
            // If result is not equal, there is an error.
            if (result0 != x0 + y0)
            {
                int actualBit = Convert.ToString(result0, 2).Length - 1;
                Console.WriteLine($"X: {x0:B} Y: {y0:B} Expected: {(x0 + y0):B} Actual: {result0:B}");
                Console.WriteLine($"Expected bit: {inputIndex} Actual bit: {actualBit}");
                Console.WriteLine("");
            }

            // Yn is set, but not Xn.
            long x1 = 0;
            long y1 = 1L << inputIndex;
            List<(string, int)> inputValues1 = CreateInputValues(x1, y1, maxInputIndex);
            long result1 = Calculate(nodes, inputValues1);
            // If result is not equal, there is an error.
            if (result1 != x1 + y1)
            {
                int actualBit = Convert.ToString(result1, 2).Length - 1;
                Console.WriteLine($"X: {x1:B} Y: {y1:B} Expected: {(x1 + y1):B} Actual: {result1:B}");
                Console.WriteLine($"Expected bit: {inputIndex} Actual bit: {actualBit}");
                Console.WriteLine("");
            }

            // X(n-1) and Y(n-1) are set.
            long x2 = 1L << (inputIndex - 1);
            long y2 = 1L << (inputIndex - 1);
            List<(string, int)> inputValues2 = CreateInputValues(x2, y2, maxInputIndex);
            long result2 = Calculate(nodes, inputValues2);
            // If result is not equal, there is an error.
            if (result2 != x2 + y2)
            {
                int actualBit = Convert.ToString(result2, 2).Length - 1;
                Console.WriteLine($"X: {x2:B} Y: {y2:B} Expected: {(x2 + y2):B} Actual: {result2:B}");
                Console.WriteLine($"Expected bit: {inputIndex} Actual bit: {actualBit}");
                Console.WriteLine("");
            }
        }
    }

    static void Task2(Dictionary<string, Node> nodes)
    {
        // Solving task 2 is a manual process, so there is no exact algorithm that solves the task automatically.
        // Thet text specifies that the simulated machine should work as an adder for two binary numbers.
        // That means that for given inputs Xn, Yn and output Zn the follwing equation holds.
        //
        // Equation: Xn XOR Yn XOR Cn = Zn
        // 
        // Cn denotes the carry flag from the lower inputs.
        // So you have to look first which bits seem to be wrong. You can use the 'Test()' method for that.
        //
        Test(nodes);

        // The you have to look into your input file and find the input and output nodes that are involved in the calculation of the faulty bits.
        // Start with the lowest bit first.
        // If for example bit 6 of the output is wrong, then look for the nodes labeled "x06", "y06", and "z06". There will be some more nodes connected to
        // these nodes  which are used for calculating the carry flag for this bit or the next bit. Use the equaion above to find out which pair
        // of wires has been swapped. Swap thes wires in the input file and test whether the bit gets calculated correctly then.
        // Continue with next higher bit until all swapped wires have been identified and repaired.
        // Another method that might help is 'FindDependencies()' which you can use to identify the dependencies (the nodes it depends on) for a given node.
        // Use it like below:
        string nodeName = "z05";
        List<string> dependencies = FindDependencies(nodes, nodeName);
        Console.WriteLine($"{nodeName}: {string.Join(',', dependencies)}");
    }

    static void Main(string[] _)
    {
        // Create map and movements from input.
        string[] lines = File.ReadAllLines("data/input.txt");

        Dictionary<string, Node> nodes = [];
        List<(string, int)> inputValues = [];

        var doReadGates = false;

        foreach (var line in lines)
        {
            // Empty line is separator between inputs and gates. Inputs come first.
            if (string.IsNullOrEmpty(line))
            {
                doReadGates = true;
                continue;
            }

            if (doReadGates)
            {
                var parts = line.Split(' ');
                string input0Name = parts[0];
                string operation = parts[1];
                string input1Name = parts[2];
                string name = parts[4];
                // Create a gate.
                nodes.Add(name, new Node
                {
                    Output = null,
                    Operation = operation,
                    Input0Name = input0Name,
                    Input1Name = input1Name,
                });
            }
            else
            {
                var parts = line.Split(": ");
                string name = parts[0];
                int output = int.Parse(parts[1]);
                // Create an input node. The value of the input is not yet set here.
                nodes.Add(name, new Node
                {
                    Output = null,
                    Operation = "INP",
                    Input0Name = null,
                    Input1Name = null,
                });
                // Store input values in a separate list.
                inputValues.Add((name, output));
            }
        }

        Task1(nodes, inputValues);
        Task2(nodes);
    }
}
