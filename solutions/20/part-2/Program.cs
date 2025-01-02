var lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\20\\input.txt");

var deltaMap = new int[4, 2] { { -1, 0 }, { 0, 1 }, { 1, 0 }, { 0, -1 } };

var racetrack = new int[lines.Length, lines[0].Length];
var singletrack = new List<Point>();

var pos = new Point(0, 0);
var end = new Point(0, 0);

for (var y = 0; y < lines.Length; y++)
    for (var x = 0; x < lines[0].Length; x++)
        if (lines[y][x].Equals('#'))
            racetrack[y, x] = -1;
        else if (lines[y][x].Equals('S'))
            pos = new Point(x, y);
        else if (lines[y][x].Equals('E'))
            end = new Point(x, y);

int previousX = -1, previousY = -1;
singletrack.Add(new Point(pos.x, pos.y));
while (!(pos.x == end.x && pos.y == end.y))
{
    int dY, dX;
    for (var i = 0; i < 4; i++)
    {
        dY = pos.y + deltaMap[i, 0];
        dX = pos.x + deltaMap[i, 1];

        if (racetrack[dY, dX] != -1 && !(dX == previousX && dY == previousY))
        {
            singletrack.Add(new Point(dX, dY));

            previousX = pos.x;
            previousY = pos.y;

            pos.x = dX;
            pos.y = dY;

            break;
        }
    }
}

var answer = 0;
for (var from = 0; from < singletrack.Count; from++)
    for (var to = from + 1; to < singletrack.Count; to++)
        cheat(from, to);

Console.WriteLine(answer);

void cheat(int a, int b)
{
    var picoseconds = distance(singletrack[a].x, singletrack[b].x) + distance(singletrack[a].y, singletrack[b].y);
    if (picoseconds <= 20 && saved(a, b, picoseconds) >= 100)
        answer++;
}

int saved(int a, int b, int steps) => a > b ? a - b - steps : b - a - steps;

int distance(int a, int b) => a > b ? a - b : b - a;

class Point(int x, int y)
{
    public int x = x;
    public int y = y;
}