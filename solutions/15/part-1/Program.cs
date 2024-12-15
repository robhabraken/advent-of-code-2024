var lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\15\\input.txt");

var directions = "^>v<";
var deltaMap = new int[4, 2] { { -1, 0 }, { 0, 1 }, { 1, 0 }, { 0, -1 } };

var mapList = new List<string>();
var moves = new List<string>();
var robot = new Point(0, 0);

var answer = 0;
for (var i = 0; i < lines.Length; i++)
{
    if (lines[i].StartsWith('#'))
        mapList.Add(lines[i]);
    else if (!string.IsNullOrEmpty(lines[i]))
        moves.Add(lines[i]);

    if (lines[i].Contains('@'))
        robot = new Point(lines[i].IndexOf('@'), i);
}

var map = new char[mapList.Count, mapList[0].Length];
for (var y = 0; y < mapList.Count; y++)
    for (var x = 0; x < mapList[y].Length; x++)
        map[y, x] = mapList[y][x];

drawMap();

foreach (var line in moves)
    foreach (var move in line)
        performMove(robot, directions.IndexOf(move));

for (var y = 0; y < map.GetLength(0); y++)
    for (var x = 0; x < map.GetLength(1); x++)
        if (map[y, x].Equals('O'))
            answer += 100 * y + x;

Console.WriteLine(answer);

void drawMap()
{
    //for (var y = 0; y < map.GetLength(0); y++)
    //{
    //    for (var x = 0; x < map.GetLength(1); x++)
    //        Console.Write(map[y, x]);
    //    Console.WriteLine();
    //}
}

void performMove(Point robot, int direction)
{
    //Console.WriteLine($"Moving in direction {directions[direction]}");

    var dY = robot.y + deltaMap[direction, 0];
    var dX = robot.x + deltaMap[direction, 1];

    if (map[dY, dX].Equals('.'))
    {
        map[robot.y, robot.x] = '.';
        map[dY, dX] = '@';
        robot.y = dY;
        robot.x = dX;
    }

    if (map[dY, dX].Equals('O'))
    {
        int nextY = dY, nextX = dX, steps = 0;
        bool wall = false, empty = false;
        while (!wall && !empty)
        {
            nextY += deltaMap[direction, 0];
            nextX += deltaMap[direction, 1];
            steps++;

            if (map[nextY, nextX].Equals('.'))
                empty = true;
            else if (map[nextY, nextX].Equals('#'))
                wall = true;
        }

        if (empty)
        {
            int previousY = 0, previousX = 0;
            for (var i = 0; i <= steps; i++)
            {
                previousY = nextY - deltaMap[direction, 0];
                previousX = nextX - deltaMap[direction, 1];
                
                map[nextY, nextX] = map[previousY, previousX];

                if (map[previousY, previousX].Equals('@'))
                {
                    map[robot.y, robot.x] = '.';
                    robot.y = nextY;
                    robot.x = nextX;
                }

                nextY = previousY;
                nextX = previousX;
            }


        }
    }

    drawMap();
}

internal class Point(int x, int y)
{
    public int x = x;
    public int y = y;
}