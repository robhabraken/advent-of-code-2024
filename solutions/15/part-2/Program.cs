var lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\15\\input.txt");

var directions = "^>v<";
var deltaMap = new int[4, 2] { { -1, 0 }, { 0, 1 }, { 1, 0 }, { 0, -1 } };

var warehouse = new List<string>();
var moves = new List<string>();
var robot = new Obstacle(0, 0, ObstacleType.Robot);

for (var y = 0; y < lines.Length; y++)
{
    if (lines[y].StartsWith('#'))
        warehouse.Add(lines[y]);
    else if (!string.IsNullOrEmpty(lines[y]))
        moves.Add(lines[y]);

    if (lines[y].Contains('@'))
        robot = new Obstacle(lines[y].IndexOf('@') * 2, y, ObstacleType.Robot);
}

var obstacles = new Obstacle?[warehouse.Count, warehouse[0].Length * 2];
for (var y = 0; y < warehouse.Count; y++)
    for (var x = 0; x < warehouse[0].Length; x++)
        if (warehouse[y][x].Equals('#'))
            obstacles[y, x * 2] = new Obstacle(x * 2, y, ObstacleType.Wall);
        else if (warehouse[y][x].Equals('O'))
            obstacles[y, x * 2] = new Obstacle(x * 2, y, ObstacleType.Box);

foreach (var line in moves)
    foreach (var move in line)
        attemptMove(directions.IndexOf(move));

var answer = 0;
for (var y = 0; y < obstacles.GetLength(0); y++)
    for (var x = 0; x < obstacles.GetLength(1); x++)
        if (obstacles[y, x] != null && obstacles[y, x]?.type == ObstacleType.Box)
            answer += 100 * y + x;

Console.WriteLine(answer);

void attemptMove(int direction)
{
    var dY = robot.y + deltaMap[direction, 0];
    var dX = robot.x + deltaMap[direction, 1];

    Obstacle? obstruction = null;
    if (direction != 3 && obstacles[dY, dX] != null)
        obstruction = obstacles[dY, dX];
    else if (direction != 1 && obstacles[dY, dX - 1] != null)
        obstruction = obstacles[dY, dX - 1];

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

    public bool TryMove(Obstacle?[,] obstacles, int[,] deltaMap, int direction, bool doMove)
    {
        var dX = x + deltaMap[direction, 1];
        var dY = y + deltaMap[direction, 0];

        var boxes = new List<Obstacle>();
        if (direction % 2 == 0 && obstacles[dY, dX] != null)
        {
            if (obstacles[dY, dX]?.type == ObstacleType.Wall)
                return false;
            else
                boxes.Add(obstacles[dY, dX]);
        }

        if (direction != 1 && obstacles[dY, dX - 1] != null)
        {
            if (obstacles[dY, dX - 1]?.type == ObstacleType.Wall)
                return false;
            else
                boxes.Add(obstacles[dY, dX - 1]);
        }

        if (direction != 3 && obstacles[dY, dX + 1] != null)
        {
            if (obstacles[dY, dX + 1]?.type == ObstacleType.Wall)
                return false;
            else
                boxes.Add(obstacles[dY, dX + 1]);
        }

        if (boxes.Count == 0)
        {
            if (doMove)
                Move(obstacles, deltaMap, direction);
            return true;
        }
        else
        {
            var possible = true;
            foreach (var box in boxes)
                possible = possible && box.TryMove(obstacles, deltaMap, direction, false);

            if (possible && doMove)
            {
                foreach (var box in boxes)
                    box.TryMove(obstacles, deltaMap, direction, true);
                Move(obstacles, deltaMap, direction);
            }
            return possible;
        }
    }

    private void Move(Obstacle?[,] obstacles, int[,] deltaMap, int direction)
    {
        obstacles[y, x] = null;

        x += deltaMap[direction, 1];
        y += deltaMap[direction, 0];

        obstacles[y, x] = this;
    }
}

enum ObstacleType
{
    Box,
    Robot,
    Wall
}