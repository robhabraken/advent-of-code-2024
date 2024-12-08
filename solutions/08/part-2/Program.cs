string[] lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\08\\input.txt");

var antennas = new Dictionary<char, List<Tuple<int, int>>>();

for (var y = 0; y < lines.Length; y++)
    for (var x = 0; x < lines[0].Length; x++)
        if (!lines[y][x].Equals('.'))
            if (!antennas.TryGetValue(lines[y][x], out List<Tuple<int, int>>? value))
                antennas.Add(lines[y][x], [new Tuple<int, int>(y, x)]);
            else
                value.Add(new Tuple<int, int>(y, x));

var antinodes = new HashSet<Tuple<int, int>>();

foreach (var frequency in antennas.Keys)
    for (var i = 0; i < antennas[frequency].Count; i++)
        for (var j = 0; j < antennas[frequency].Count; j++)
            if (i != j)
            {
                var dY = antennas[frequency][i].Item1 - antennas[frequency][j].Item1;
                var dX = antennas[frequency][i].Item2 - antennas[frequency][j].Item2;

                PlaceAntinode(antennas[frequency][i].Item1, antennas[frequency][i].Item2, dY, dX);
                PlaceAntinode(antennas[frequency][j].Item1, antennas[frequency][j].Item2, 0 - dY, 0 - dX);
            }

Console.WriteLine(antinodes.Count);

void PlaceAntinode(int y, int x, int dY, int dX)
{
    while (true)
    {
        if (y < 0 || y >= lines.Length || x < 0 || x >= lines[0].Length)
            break;

        antinodes.Add(new Tuple<int, int>(y, x));

        y += dY;
        x += dX;
    }
}