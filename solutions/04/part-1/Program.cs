string[] lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\04\\input.txt");

var answer = 0;
for (var line = 0; line < lines.Length; line++)
    for (var character = 0; character < lines[line].Length; character++)
        if (lines[line][character].Equals('X'))
            for (var x = -1; x <= 1; x++)
                for (var y = -1; y <= 1; y++)
                    Search(line, character, x, y);

Console.WriteLine(answer);

void Search(int y, int x, int dX, int dY)
{
    var str = string.Empty;
    for (var step = 1; step <= 4; step++, y += dX, x += dY)
        if (y >= 0 && y < lines.Length && x >= 0 && x < lines[y].Length)
            str += lines[y][x];

    if (str.Equals("XMAS"))
        answer++;
}