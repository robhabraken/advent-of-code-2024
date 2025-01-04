var lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\06\\input.txt");

(int, int) location = (0, 0);
var referenceMap = new bool[lines.Length, lines[0].Length];
var deltaMap = new int[4, 2] { { -1, 0 }, { 0, 1 }, { 1, 0 }, { 0, -1 } };

var answer = 0;
for (int y = 0; y < lines.Length; y++)
    if (lines[y].Contains('^'))
        location = (y, lines[y].IndexOf('^'));

walk(ref referenceMap, (location.Item1, location.Item2), -1, -1);

var testMap = new bool[lines.Length, lines[0].Length];
for (int y = 0; y < lines.Length; y++)
    for (int x = 0; x < lines[0].Length; x++)
        if (referenceMap[y, x])
            if (walk(ref testMap, (location.Item1, location.Item2), y, x))
                answer++;

Console.WriteLine(answer);

bool walk(ref bool[,] map, (int, int) location, int obstacleY, int obstacleX)
{
    var visitedDirections = new bool[lines.Length, lines[0].Length, 4];
    var direction = 0;

    while (true)
    {
        map[location.Item1, location.Item2] = true;

        visitedDirections[location.Item1, location.Item2, direction % 4] = true;

        var dY = location.Item1 + deltaMap[direction % 4, 0];
        var dX = location.Item2 + deltaMap[direction % 4, 1];

        if (dY < 0 || dY >= lines.Length || dX < 0 || dX >= lines[0].Length)
            return false; // walking off of the grid

        if (visitedDirections[dY, dX, direction % 4])
            return true; // loop detected

        if (lines[dY][dX].Equals('#') || (dY == obstacleY && dX == obstacleX))
            direction++;
        else
        {
            location.Item1 = dY;
            location.Item2 = dX;
        }
    }
}