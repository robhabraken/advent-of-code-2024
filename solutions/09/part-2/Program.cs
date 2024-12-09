var diskmap = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\09\\input.txt")[0];

var fileblocks = new List<int>();

var id = 0;
for (var i = 0; i < diskmap.Length; i++)
{
    var digit = int.Parse($"{diskmap[i]}");
    if (i % 2 == 1)
    {
        for (var j = 0; j < digit; j++)
            fileblocks.Add(-1);
    }
    else
    {
        for (var j = 0; j < digit; j++)
            fileblocks.Add(id);
        id++;
    }
}

var currentId = int.MaxValue;
for (var i = fileblocks.Count - 1; i >= 0; i--)
{
    if (fileblocks[i] >= 0)
    {
        var blockstart = -1;
        for (var j = i; j >= 0 && fileblocks[j] == fileblocks[i]; j--)
            blockstart = j;

        var length = i - blockstart + 1;

        if (fileblocks[i] < currentId)
        {
            currentId = fileblocks[i];

            var freespace = freeSpaceIndex(length, blockstart);
            if (freespace >= 0)
            {
                for (var j = 0; j < length; j++)
                {
                    fileblocks[freespace + j] = fileblocks[i];
                    fileblocks[blockstart + j] = -1;
                }
            }
        }

        i = blockstart;
    }
}

long checksum = 0;
for (var i = 0; i < fileblocks.Count; i++)
    if (fileblocks[i] >= 0)
        checksum += fileblocks[i] * i;

Console.WriteLine(checksum);

int freeSpaceIndex(int length, int maxIndex)
{
    var firstEmptyBlockIndex = -1;
    for (var i = 0; i < fileblocks.Count && i < maxIndex && firstEmptyBlockIndex < 0; i++)
    {
        if (firstEmptyBlockIndex < 0 && fileblocks[i] == -1)
        {
            var valid = true;
            for (var j = 0; j < length; j++)
                if (fileblocks[i + j] >= 0)
                    valid = false;

            if (valid)
                firstEmptyBlockIndex = i;
        }
    }
    return firstEmptyBlockIndex;
}