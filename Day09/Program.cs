using System.Text;

namespace Day09;

/// <summary>
/// Solution for day 9 of the AoC 2024.
///
/// See https://adventofcode.com/2024/day/9 for the description of the task and the input data.
/// </summary>
internal class Program
{
    static List<long> CreateBlockList(string line)
    {
        // Uncompress the disk map to create a block list.
        bool isFreeSpace = false;
        long fileId = 0;

        List<long> blocks = [];
        foreach (var c in line)
        {
            long blockCount = long.Parse(c.ToString());
            if (isFreeSpace)
            {
                for (int i = 0; i < blockCount; i++)
                {
                    // Free space has a file ID of -1.
                    blocks.Add(-1);
                }
            }
            else
            {
                for (int i = 0; i < blockCount; i++)
                {
                    // Occupied space has a file ID that gets incremented with every file.
                    blocks.Add(fileId);
                }
                fileId++;
            }
            isFreeSpace = !isFreeSpace;
        }

        return blocks;
    }

    static string BlockListToString(List<long> blocks)
    {
        // Do a debug output.
        var sb = new StringBuilder();
        foreach (var b in blocks)
        {
            if (b == -1)
            {
                sb.Append('.');
            }
            else
            {
                sb.Append(b);
            }
        }

        return sb.ToString();
    }

    static long CalculateCheckSum(List<long> blocks)
    {
        // Checksum is calculated from occupied blocks.
        long checkSum = 0;
        for (int i = 0; i < blocks.Count; i++)
        {
            if (blocks[i] != -1)
            {
                checkSum += i * blocks[i];
            }
        }

        return checkSum;
    }

    static void DoCompactingTask1(List<long> blocks)
    {
        int firstBlock = 0;
        int lastBlock = blocks.Count - 1;

        // Go through the list simultaneously from beginning and from end and swap free blocks at the beginning with occupied blocks at the end.
        while (firstBlock < lastBlock)
        {
            if (blocks[firstBlock] != -1)
            {
                firstBlock++;
            }
            else if (blocks[lastBlock] == -1)
            {
                lastBlock--;
            }
            else
            {
                // Do the block swap.
                (blocks[firstBlock], blocks[lastBlock]) = (blocks[lastBlock], blocks[firstBlock]);
                firstBlock++;
                lastBlock--;
            }
        }
    }

    static int FindFirstFit(List<long> blocks, int firstBlock, int firstOldBlock, int blockCount)
    {
        // This method finds the first free space with 'blockCount' free blocks.
        int currBlock = firstBlock;
        int emptyCount = 0;

        while (currBlock < firstOldBlock)
        {
            if (blocks[currBlock] == -1)
            {
                currBlock++;
                emptyCount++;

                if (emptyCount >= blockCount)
                {
                    return currBlock - emptyCount;
                }
            }
            else
            {
                currBlock++;
                emptyCount = 0;
            }
        }

        return firstOldBlock;
    }

    static void DoCompactingTask2(List<long> blocks)
    {
        int firstBlock = 0;
        int lastBlock = blocks.Count - 1;

        // Again, go through the list simultaneously from beginning and from end.
        while (firstBlock < lastBlock)
        {
            if (blocks[firstBlock] != -1)
            {
                firstBlock++;
            }
            else if (blocks[lastBlock] == -1)
            {
                lastBlock--;
            }
            else
            {
                // Now we have to find a free place that is large enough so that the occupied block from the end can be placed here.
                int currBlock = lastBlock;

                long fileId = blocks[lastBlock];
                while (currBlock >= 0 && blocks[currBlock] == fileId)
                {
                    currBlock--;
                }
                int blockCount = lastBlock - currBlock;
                int firstOldBlock = currBlock + 1;

                int firstNewBlock = FindFirstFit(blocks, firstBlock, firstOldBlock, blockCount);
                for (int i = 0; i < blockCount; i++)
                {
                    blocks[firstOldBlock + i] = -1;
                    blocks[firstNewBlock + i] = fileId;
                }
                lastBlock = firstOldBlock - 1;
            }
        }
    }

    static void DoTask(string line, bool isTask2)
    {
        List<long> blocks = CreateBlockList(line);

        //Console.WriteLine("Before compacting:");
        //Console.WriteLine(BlockListToString(blocks));

        // The tasks only differ by the way the disk space is compacted.
        if (!isTask2)
        {
            DoCompactingTask1(blocks);
        }
        else
        {
            DoCompactingTask2(blocks);
        }

        //Console.WriteLine("After compacting:");
        //Console.WriteLine(BlockListToString(blocks));

        long checkSum = CalculateCheckSum(blocks);

        Console.WriteLine($"Task {(isTask2 ? 2 : 1)}:");
        Console.WriteLine($"Checksum: {checkSum}");
    }

    static void Main(string[] _)
    {
        // read disk map from input.
        string line = File.ReadAllText("data/input.txt");

        DoTask(line, false);
        DoTask(line, true);
    }
}
