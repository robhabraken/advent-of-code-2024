var lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\15\\input.txt");

var directions = "^>v<";
var deltaMap = new int[4, 2] { { -1, 0 }, { 0, 1 }, { 1, 0 }, { 0, -1 } };

var moves = new List<string>();
var robot = new Obstacle(0, 0, ObstacleType.Robot);
var obstacles = new List<Obstacle>();

for (var y = 0; y < lines.Length; y++)
{
    if (lines[y].StartsWith('#'))
    {
        for (var x = 0; x < lines[y].Length; x++)
            if (lines[y][x].Equals('#'))
                obstacles.Add(new Obstacle(x * 2, y, ObstacleType.Wall));
            else if (lines[y][x].Equals('O'))
                obstacles.Add(new Obstacle(x * 2, y, ObstacleType.Box));
    }
    else if (!string.IsNullOrEmpty(lines[y]))
        moves.Add(lines[y]);

    if (lines[y].Contains('@'))
        robot = new Obstacle(lines[y].IndexOf('@') * 2, y, ObstacleType.Robot);
}

foreach (var line in moves)
    foreach (var move in line)
        attemptMove(obstacles, directions.IndexOf(move));

var answer = 0;
foreach (var obstacle in obstacles)
    if (obstacle.type == ObstacleType.Box)
        answer += 100 * obstacle.y + obstacle.x;

Console.WriteLine(answer);

void attemptMove(List<Obstacle> obstacles, int direction)
{
    var dY = robot.y + deltaMap[direction, 0];
    var dX = robot.x + deltaMap[direction, 1];

    Obstacle? obstruction = null;
    foreach (var obstacle in obstacles)
        if (obstacle.y == dY)
            if ((direction % 2 == 0 && obstacle.x == dX || obstacle.x + 1 == dX) ||
                (direction == 1 && obstacle.x == dX) ||
                (direction == 3 && obstacle.x + 1 == dX))
            {
                obstruction = obstacle;
                break;
            }

    if (obstruction != null && obstruction.type == ObstacleType.Wall)
        return;

    if (obstruction != null)
        if (obstruction.TryMove(obstacles, deltaMap, direction, false))
            obstruction.TryMove(obstacles, deltaMap, direction, true);
        else
            return;

    robot.y = dY;
    robot.x = dX;
}

internal class Obstacle(int x, int y, ObstacleType type)
{
    public int x = x;
    public int y = y;

    public ObstacleType type = type;

    public bool TryMove(List<Obstacle> obstacles, int[,] deltaMap, int direction, bool doMove)
    {
        var dX = x + deltaMap[direction, 1];
        var dY = y + deltaMap[direction, 0];

        if (direction % 2 == 0)
        {
            var boxes = new List<Obstacle>();
            foreach (var obstacle in obstacles)
                if (obstacle.y == dY && (obstacle.x == dX || obstacle.x == dX - 1 || obstacle.x == dX + 1))
                    if (obstacle.type == ObstacleType.Wall)
                        return false;
                    else
                        boxes.Add(obstacle);

            if (boxes.Count == 0)
            {
                if (doMove)
                    Move(deltaMap, direction);
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
                        Move(deltaMap, direction);
                    }
                    return true;
                }
            }
        }
        else
        {
            Obstacle? box = null;
            foreach (var obstacle in obstacles)
                if (obstacle.y == dY)
                    if ((direction == 1 && obstacle.x == dX + 1) || (direction == 3 && obstacle.x == dX - 1))
                        if (obstacle.type == ObstacleType.Wall)
                            return false;
                        else
                        {
                            box = obstacle;
                            break;
                        }

            if (box == null)
            {
                if (doMove)
                    Move(deltaMap, direction);
                return true;
            }
            else
            {
                if (box.TryMove(obstacles, deltaMap, direction, false))
                {
                    if (doMove)
                    {
                        box.TryMove(obstacles, deltaMap, direction, true);
                        Move(deltaMap, direction);
                    }
                    return true;
                }
            }
        }

        return false;
    }

    private void Move(int[,] deltaMap, int direction)
    {
        x += deltaMap[direction, 1];
        y += deltaMap[direction, 0];
    }
}

enum ObstacleType
{
    Box,
    Robot,
    Wall
}