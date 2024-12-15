var lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\15\\input.txt");

var directions = "^>v<";
var deltaMap = new int[4, 2] { { -1, 0 }, { 0, 1 }, { 1, 0 }, { 0, -1 } };

var mapList = new List<string>();
var moves = new List<string>();
var robot = new Point(0, 0);

var obstacles = new List<Obstacle>();

var answer = 0;
for (var y = 0; y < lines.Length; y++)
{
    if (lines[y].StartsWith('#'))
    {
        mapList.Add(lines[y].Replace("#", "##").Replace("O", "[]").Replace(".", "..").Replace("@", "@."));
        for (var x = 0; x < lines[y].Length; x++)
        {
            if (lines[y][x].Equals('#'))
                obstacles.Add(new Obstacle(x * 2, y, TileType.Wall));
            else if (lines[y][x].Equals('O'))
                obstacles.Add(new Obstacle(x * 2, y, TileType.Box));
        }
    }
    else if (!string.IsNullOrEmpty(lines[y]))
        moves.Add(lines[y]);

    if (lines[y].Contains('@'))
        robot = new Point(lines[y].IndexOf('@') * 2, y);
}

var map = new char[mapList.Count, mapList[0].Length];


drawMap();

foreach (var line in moves)
    foreach (var move in line)
    {
        performMove(obstacles, directions.IndexOf(move));
        drawMap();
    }

foreach (var obstacle in obstacles)
    if (obstacle.tileType == TileType.Box)
        answer += 100 * obstacle.y + obstacle.x;

Console.WriteLine(answer);

void drawMap()
{
    //map = new char[mapList.Count, mapList[0].Length];
    //for (var y = 0; y < map.GetLength(0); y++)
    //{
    //    for (var x = 0; x < map.GetLength(1); x++)
    //    {
    //        if (map[y, x].Equals('\0'))
    //            map[y, x] = '.';
    //        foreach (var obstacle in obstacles)
    //        {
    //            if (obstacle.y == y && obstacle.x == x)
    //                if (obstacle.tileType == TileType.Wall)
    //                {
    //                    map[y, x] = '#';
    //                    map[y, x + 1] = '#';
    //                }
    //                else
    //                {
    //                    map[y, x] = '[';
    //                    map[y, x + 1] = ']';
    //                }
    //            if (robot.y == y && robot.x == x)
    //                map[y, x] = '@';
    //        }
    //        Console.Write(map[y, x]);
    //    }
    //    Console.WriteLine();
    //}
    //Console.WriteLine();
}

void performMove(List<Obstacle> obstacles, int direction)
{
    //Console.WriteLine($"Moving in direction {directions[direction]}");

    var dY = robot.y + deltaMap[direction, 0];
    var dX = robot.x + deltaMap[direction, 1];

    Obstacle obstruction = null;
    foreach (var obstacle in obstacles)
    {
        if (obstacle.y == dY)
        {
            if ((direction % 2 == 0 && obstacle.x == dX || obstacle.x + 1 == dX) ||
                (direction == 1 && obstacle.x == dX) ||
                (direction == 3 && obstacle.x + 1 == dX))
            {
                obstruction = obstacle;
                break;
            }
        }
    }

    if (obstruction != null && obstruction.tileType == TileType.Wall)
        return;

    if (obstruction != null)
    {
        if (obstruction.TryMove(obstacles, deltaMap, direction, false))
            obstruction.TryMove(obstacles, deltaMap, direction, true);
        else
            return;
    }

    robot.y = dY;
    robot.x = dX;
}

internal class Point(int x, int y)
{
    public int x = x;
    public int y = y;
}

internal class Obstacle(int x, int y, TileType tileType)
{
    public int x = x;
    public int y = y;
    public int width = 2;

    public TileType tileType = tileType;

    public bool TryMove(List<Obstacle> obstacles, int[,] deltaMap, int direction, bool doMove)
    {
        var dX = x + deltaMap[direction, 1];
        var dY = y + deltaMap[direction, 0];

        if (direction % 2 == 0)
        {
            var boxes = new List<Obstacle>();
            foreach (var obstacle in obstacles)
            {
                if (obstacle.y == dY && (obstacle.x == dX || obstacle.x == dX - 1 || obstacle.x == dX + 1))
                {
                    if (obstacle.tileType == TileType.Wall)
                        return false;
                    else
                        boxes.Add(obstacle);
                }
            }

            if (boxes.Count == 0)
            {
                if (doMove)
                    move(deltaMap, direction);
                return true;
            }
            else
            {
                var possible = true;
                foreach (var box in boxes)
                    possible = possible && box.TryMove(obstacles, deltaMap, direction, false);

                if (possible)
                {
                    if (doMove) { 
                        foreach (var box in boxes)
                            box.TryMove(obstacles, deltaMap, direction, true);
                        move(deltaMap, direction);
                    }
                    return true;
                }
            }
        }
        else
        {
            Obstacle box = null;
            foreach (var obstacle in obstacles)
            {
                if (obstacle.y == dY)
                {
                    if ((direction == 1 && obstacle.x == dX + 1) || (direction == 3 && obstacle.x == dX - 1))
                    {
                        if (obstacle.tileType == TileType.Wall)
                            return false;
                        else
                        {
                            box = obstacle;
                            break;
                        }
                    }
                }
            }

            if (box == null)
            {
                if (doMove)
                    move(deltaMap, direction);
                return true;
            }
            else
            {
                if (box.TryMove(obstacles, deltaMap, direction, false))
                {
                    if (doMove)
                    {
                        box.TryMove(obstacles, deltaMap, direction, true);
                        move(deltaMap, direction);
                    }
                    return true;
                }
            }
        }

        return false;
    }

    private void move(int[,] deltaMap, int direction)
    {
        x += deltaMap[direction, 1];
        y += deltaMap[direction, 0];
    }
}

enum TileType
{
    Wall,
    Box
}