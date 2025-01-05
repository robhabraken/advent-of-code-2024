var diskmap = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\09\\input.txt")[0];

var length = 0;
for (var i = 0; i < diskmap.Length; i++)
    length += diskmap[i] - '0';

var blocks = new int[length + 1];
blocks[^1] = -1;

var needle = 0;
for (var i = 0; i < diskmap.Length; i++)
    for (var j = 0; j < diskmap[i] - '0'; j++)
        if (i % 2 == 0)
            blocks[needle++] = i / 2;
        else
            blocks[needle++] = -1;

var currentFile = diskmap.Length / 2;
var searchFrom = 0;
for (var i = blocks.Length - 1; i >= 0; i--)
{
    if (i > 0 && blocks[i] == currentFile && blocks[i - 1] != currentFile)
    {
        var fileLength = 1;
        for (; fileLength < 9; fileLength++)
            if (blocks[i + fileLength] != currentFile)
                break;

        var firstEmptyIndex = -1;
        for (var j = searchFrom; j < blocks.Length - fileLength && j < i; j++)
        {
            if (blocks[j] == -1)
            {
                if (firstEmptyIndex == -1)
                    firstEmptyIndex = j;

                var fits = true;
                for (var k = 1; k < fileLength; k++)
                {
                    if (blocks[j + k] > -1)
                    {
                        fits = false;
                        break;
                    }
                }

                if (fits)
                {
                    for (var k = 0; k < fileLength; k++)
                    {
                        blocks[j + k] = currentFile;
                        blocks[i + k] = -1;
                    }
                    break;
                }
            }
        }

        if (firstEmptyIndex > -1)
            searchFrom = firstEmptyIndex;

        if (searchFrom == i || currentFile == 0)
            break;

        currentFile--;
    }
}

long checksum = 0;
for (var i = 0; i < blocks.Length; i++)
    if (blocks[i] > -1)
        checksum += blocks[i] * i;

Console.WriteLine(checksum);