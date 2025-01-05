var diskmap = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\09\\input.txt")[0];

var fileblocks = new List<int>();
var firstEmptyBlock = 0;

for (int i = 0, id = 0; i < diskmap.Length; i++)
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

for (int i = fileblocks.Count - 1, id = int.MaxValue; i >= 0; i--)
{
    if (fileblocks[i] >= 0 && fileblocks[i] < id)
    {
        var filestart = i;
        for (var j = i; j >= 0 && fileblocks[j] == fileblocks[i]; j--)
            filestart = j;

        var length = i - filestart + 1;

        id = fileblocks[i];

        var freespace = freeSpaceIndex(length, filestart);
        if (freespace >= 0)
        {
            for (var j = 0; j < length; j++)
            {
                fileblocks[freespace + j] = fileblocks[i];
                fileblocks[filestart + j] = -1;
            }
        }

        i = filestart;
    }
}

long checksum = 0;
for (var i = 0; i < fileblocks.Count; i++)
    if (fileblocks[i] >= 0)
        checksum += fileblocks[i] * i;

Console.WriteLine(checksum);

int freeSpaceIndex(int length, int maxIndex)
{
    var firstEmptyBlockFound = false;
    for (var i = firstEmptyBlock; i < maxIndex; i++)
    {
        if (fileblocks[i] == -1)
        {
            if (!firstEmptyBlockFound)
            {
                firstEmptyBlockFound = true;
                firstEmptyBlock = i;
            }

            var fileFits = true;
            for (var j = 0; j < length; j++)
                if (fileblocks[i + j] >= 0)
                    fileFits = false;

            if (fileFits)
                return i;
        }
    }
    return -1;
}