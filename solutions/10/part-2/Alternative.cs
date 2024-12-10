var lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\10\\input.txt");

var map = new int[lines.Length, lines[0].Length];
var cache = new int[lines.Length, lines[0].Length];
var deltaMap = new int[4, 2] { { -1, 0 }, { 0, 1 }, { 1, 0 }, { 0, -1 } };

for (var y = 0; y < lines.Length; y++)
    for (var x = 0; x < lines[y].Length; x++)
        map[y, x] = int.Parse($"{lines[y][x]}");

var rating = 0;
for (var i = 8; i >= 0; i--)
    for (var y = 0; y < map.GetLength(0); y++)
        for (var x = 0; x < map.GetLength(1); x++)
            if (map[y, x] == i)
                CountPaths(y, x);

Console.WriteLine(rating);

void CountPaths(int y, int x)
{
    for (var i = 0; i < 4; i++)
        if ((y + deltaMap[i, 0]) >= 0 && (y + deltaMap[i, 0]) < lines.Length && (x + deltaMap[i, 1]) >= 0 && (x + deltaMap[i, 1]) < lines[0].Length)
            if (map[(y + deltaMap[i, 0]), (x + deltaMap[i, 1])] == map[y, x] + 1)
                if (map[(y + deltaMap[i, 0]), (x + deltaMap[i, 1])] == 9)
                    cache[y, x]++;
                else
                    cache[y, x] += cache[(y + deltaMap[i, 0]), (x + deltaMap[i, 1])];

    if (map[y, x] == 0)
        rating += cache[y, x];
}