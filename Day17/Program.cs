namespace Day17;

/// <summary>
/// Solution for day 17 of the AoC 2024.
///
/// See https://adventofcode.com/2024/day/17 for the description of the task and the input data.
/// </summary>
internal class Program
{
    static void PrintRegisters(ulong registerA, ulong registerB, ulong registerC)
    {
        // Debugging output of the registers.
        Console.WriteLine($"Register A: {registerA}");
        Console.WriteLine($"Register B: {registerB}");
        Console.WriteLine($"Register C: {registerC}");
    }

    static void PrintProgram(List<ulong> program)
    {
        // Debugging output of the program.
        Console.WriteLine($"Program : {string.Join(',', program)}");
    }

    static void PrintOutput(List<ulong> output)
    {
        // Debugging output of the output.
        Console.WriteLine($"Output : {string.Join(',', output)}");
    }

    static ulong ToComboOperand(ulong literalOperand, ulong registerA, ulong registerB, ulong registerC)
    {
        // Convert literal operand to combo operand according to the given rules.
        return literalOperand switch
        {
            >= 0 and <= 3 => literalOperand,
            4 => registerA,
            5 => registerB,
            6 => registerC,
            _ => throw new ArgumentException($"Literal operand {literalOperand} cannot be converted to combo operand")
        };
    }

    static List<ulong> RunProgram(List<ulong> program, ulong registerA, ulong registerB, ulong registerC)
    {
        List<ulong> output = [];

        // Run the program and write the output to a list.
        // All comands, except for the "jnz" command, increase the instruction pointer by two.
        // Program runs as long as the instruction pointer points to a valid instruction.
        int instructionPointer = 0;
        while (instructionPointer < program.Count)
        {
            ulong opcode = program[instructionPointer];
            ulong literalOperand = program[instructionPointer + 1];

            switch (opcode)
            {
                case 0: // "adv"
                    registerA = registerA >> (int)ToComboOperand(literalOperand, registerA, registerB, registerC);
                    instructionPointer += 2;
                    break;

                case 1: // "bxl"
                    registerB = registerB ^ literalOperand;
                    instructionPointer += 2;
                    break;

                case 2: // "bst"
                    registerB = ToComboOperand(literalOperand, registerA, registerB, registerC) % 8;
                    instructionPointer += 2;
                    break;

                case 3: // "jnz"
                    instructionPointer = registerA == 0
                        ? instructionPointer + 2
                        : (int)literalOperand;
                    break;

                case 4: // "bxc"
                    registerB = registerB ^ registerC;
                    instructionPointer += 2;
                    break;

                case 5: // "out"
                    output.Add(ToComboOperand(literalOperand, registerA, registerB, registerC) % 8);
                    instructionPointer += 2;
                    break;

                case 6: // "bdv"
                    registerB = registerA  >> (int)ToComboOperand(literalOperand, registerA, registerB, registerC);
                    instructionPointer += 2;
                    break;

                case 7: // "cdv"
                    registerC = registerA >> (int)ToComboOperand(literalOperand, registerA, registerB, registerC);
                    instructionPointer += 2;
                    break;

                default:
                    throw new ArgumentException($"Opcode {opcode} not in valid range");
            }

        }

        return output;
    }

    static List<ulong> RunNative(ulong registerA)
    {
        // This method translates the program code directly to C# code so that it is clearer what is going on in the program.
        // Note that your program code might differ, as you might have a different input file.
        List<ulong> output = [];

        // As the program code shows, registers B and C are iniitalized through the value of register A.
        ulong registerB;
        ulong registerC;

        // Essentially the program loops until register 0 is zero. In every loop iteration, the contents of register A is divided by 8
        // (which is  equal to a right-shift by 3 bits in inteeger arithmetics). In every iteration, there is an out put of one value between 0 and 7.
        do
        {
            registerB = registerA % 8;
            registerB ^= 5;
            registerC = registerA >> (int)registerB;
            registerB ^= 6;
            registerB ^= registerC;
            output.Add(registerB % 8);
            registerA >>= 3;
        }
        while (registerA != 0);

        return output;
    }

    static void Main(string[] _)
    {
        // Create map and movements from input.
        string[] lines = File.ReadAllLines("data/input.txt");

        List<ulong> program = [];
        ulong registerA = 0;
        ulong registerB = 0;
        ulong registerC = 0;

        var doReadInstructions = false;

        foreach (var line in lines)
        {
            // Empty line is separator between map and movements. Map comes first.
            if (string.IsNullOrEmpty(line))
            {
                doReadInstructions = true;
                continue;
            }

            if (doReadInstructions)
            {
                // Join strings, ignoring line breaks.
                program = line.Split(' ')[1].Split(',').Select(ulong.Parse).ToList();
            }
            else
            {
                string[] parts = line.Split(' ');
                if (parts[1] == "A:")
                {
                    registerA = ulong.Parse(parts[2]); 
                }
                else if (parts[1] == "B:")
                {
                    registerB = ulong.Parse(parts[2]);
                }
                else if (parts[1] == "C:")
                {
                    registerC = ulong.Parse(parts[2]);
                }
            }
        }

        //PrintRegisters(registerA, registerB, registerC);
        //Console.WriteLine();
        //PrintProgram(program);
        //Console.WriteLine();

        // For solving the first task, the program code is run.
        List<ulong> output1 = RunProgram(program, registerA, registerB, registerC);
        Console.WriteLine("Task 1:");
        PrintOutput(output1);

        // The idea for solving the second task is to go backwards: Find possible values for register A that - when used as an input for the program -
        // lead to the last value of the desired output. There can be at most 8 possible values (from 0 to 7) that deliver the desired output.
        // Then try to find the input that corresponds to the one but last output value and so on.
        // The program continually right-shifts register A by three bits; that step is reversed and register A is shifted to left now.

        ulong minRegisterA = ulong.MaxValue;
        Queue<ulong> queue = [];

        // Find possible input(s) for last value of output.
        List<ulong> output2 = [];
        for (ulong possibleRegisterA = 0; possibleRegisterA < 8; possibleRegisterA++)
        {
            output2 = RunProgram(program, possibleRegisterA, 0, 0);
            int output2Count = output2.Count;
            if (output2Count >= 1 && output2Count <= program.Count && program[^(output2Count)..].SequenceEqual(output2))
            {
                queue.Enqueue(possibleRegisterA);
            }
        }

        // Go backwards until the full program sequence is found.
        while (queue.Count > 0)
        {
            // 'head' contains the input values(s) that are needed for the last output values.
            ulong head = queue.Dequeue();
            for (ulong tail = 0; tail < 8; tail++)
            {
                ulong possibleRegisterA = (head << 3) + tail;
                output2 = RunProgram(program, possibleRegisterA, 0, 0);
                int output2Count = output2.Count;
                if (output2Count >= 1 && output2Count <= program.Count && program[^(output2Count)..].SequenceEqual(output2))
                {
                    if (output2Count < program.Count)
                    {
                        queue.Enqueue(possibleRegisterA);
                    }
                    else if (possibleRegisterA < minRegisterA)
                    {
                        minRegisterA = possibleRegisterA;
                    }
                    //PrintOutput(output2);
                    //Console.WriteLine($"Solution: {possibleRegisterA, 20} = 0b{Convert.ToString((long)possibleRegisterA, 2).PadLeft(20, '0')}");
                }
            }
        }

        Console.WriteLine("Task 2:");
        Console.WriteLine($"Min register A: {minRegisterA}");
    }
}
