string[] lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\06\\input.txt");

var map = new bool[lines.Length, lines[0].Length];
(int, int) location = (0, 0);
var direction = 0;
var deltaMap = new int[4, 2] {{-1, 0}, {0, 1}, {1, 0}, {0, -1}};

var answer = 0;
for (int y = 0; y < lines.Length; y++)
    if (lines[y].Contains('^'))
        location = (y, lines[y].IndexOf('^'));

while (true)
{
    if (!map[location.Item1, location.Item2])
        answer++;

    map[location.Item1, location.Item2] = true;

    var dY = location.Item1 + deltaMap[direction % 4, 0];
    var dX = location.Item2 + deltaMap[direction % 4, 1];

    if (dY < 0 || dY >= lines.Length || dX < 0 || dX >= lines[0].Length)
        break;

    if (lines[dY][dX].Equals('#'))
        direction++;
    else
    {
        location.Item1 = dY;
        location.Item2 = dX;
    }
}

Console.WriteLine(answer);