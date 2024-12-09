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

for (var i = fileblocks.Count - 1; i >= 0; i--)
{
    if (fileblocks[i] >= 0)
    {
        var freespace = freeSpaceIndex();
        if (freespace < 0)
            break;

        fileblocks[freespace] = fileblocks[i];
        fileblocks[i] = -1;
    }
}

long checksum = 0;
for (var i = 0; i < fileblocks.Count && fileblocks[i] >= 0; i++)
    checksum += fileblocks[i] * i;

Console.WriteLine(checksum);

int freeSpaceIndex()
{
    var firstEmptyBlockIndex = -1;
    for (var i = 0; i < fileblocks.Count; i++)
    {
        if (firstEmptyBlockIndex < 0 && fileblocks[i] == -1)
            firstEmptyBlockIndex = i;

        if (firstEmptyBlockIndex >= 0 && fileblocks[i] >= 0)
            return firstEmptyBlockIndex;
    }
    return -1;
}