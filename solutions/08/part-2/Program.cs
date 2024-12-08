var lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\08\\input.txt");

var antennas = new Dictionary<char, List<Tuple<int, int>>>();
var antinodes = new HashSet<Tuple<int, int>>();

for (var y = 0; y < lines.Length; y++)
    for (var x = 0; x < lines[0].Length; x++)
        if (!lines[y][x].Equals('.'))
            if (!antennas.TryGetValue(lines[y][x], out List<Tuple<int, int>>? value))
                antennas.Add(lines[y][x], [new Tuple<int, int>(y, x)]);
            else
            {
                foreach (var antenna in antennas[lines[y][x]])
                {
                    var dY = y - antenna.Item1;
                    var dX = x - antenna.Item2;

                    PlaceAntinode(y, x, dY, dX);
                    PlaceAntinode(antenna.Item1, antenna.Item2, 0 - dY, 0 - dX);
                }

                value.Add(new Tuple<int, int>(y, x));
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