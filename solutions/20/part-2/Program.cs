var lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\20\\input.txt");

var deltaMap = new int[4, 2] { { -1, 0 }, { 0, 1 }, { 1, 0 }, { 0, -1 } };

var racetrack = new int[lines.Length, lines[0].Length];
var start = new Point(0, 0);
var end = new Point(0, 0);

for (var y = 0; y < lines.Length; y++)
    for (var x = 0; x < lines[0].Length; x++)
        if (lines[y][x].Equals('#'))
            racetrack[y, x] = -1;
        else if (lines[y][x].Equals('S'))
            start = new Point(x, y);
        else if (lines[y][x].Equals('E'))
            end = new Point(x, y);

var pos = new Point(start.x, start.y);
int previousX = -1, previousY = -1;
while (!(pos.x == end.x && pos.y == end.y))
{
    int dY, dX;
    for (var i = 0; i < 4; i++)
    {
        dY = pos.y + deltaMap[i, 0];
        dX = pos.x + deltaMap[i, 1];

        if (racetrack[dY, dX] != -1 && !(dX == previousX && dY == previousY))
        {
            racetrack[dY, dX] = racetrack[pos.y, pos.x] + 1;

            previousX = pos.x;
            previousY = pos.y;

            pos.x = dX;
            pos.y = dY;

            break;
        }
    }
}

var answer = 0;
for (var y = 1; y < lines.Length - 1; y++)
    for (var x = 1; x < lines[0].Length - 1; x++)                           // loop over the grid for all starting coords
        if (racetrack[y, x] != -1)                                          // if it's on the track, continue
            for (var dY = 1; dY < lines.Length - 1; dY++)
                for (var dX = 1; dX < lines[0].Length - 1; dX++)            // loop over the grid again, for the end coords
                    if (racetrack[dY, dX] != -1 && !(y == dY && x == dX))   // if that's on the track, and not the same as the start, continue
                        cheat(x, y, dX, dY);

Console.WriteLine(answer / 2);

void cheat(int fromX, int fromY, int toX, int toY)
{
    var picoseconds = steps(fromX, fromY, toX, toY);
    if (picoseconds <= 20 && saved(racetrack[fromY, fromX], racetrack[toY, toX], picoseconds) >= 100)
        answer++;
}

int steps(int fromX, int fromY, int toX, int toY)
{
    return distance(fromX, toX) + distance(fromY, toY);
}

int saved(int from, int to, int steps)
{
    return from > to ? from - to - steps : to - from - steps;
}

int distance(int from, int to)
{
    return from > to ? from - to : to - from;
}

internal class Point(int x, int y)
{
    public int x = x;
    public int y = y;
}