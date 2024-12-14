var lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\14\\input.txt");

var robots = new List<Robot>();
var dimensions = new Tuple<int, int>(101, 103);

foreach (var line in lines)
{
    var input = line.Split(' ');
    var pos = input[0].Replace("p=", string.Empty).Split(',').Select(int.Parse).ToArray();
    var vel = input[1].Replace("v=", string.Empty).Split(',').Select(int.Parse).ToArray();

    robots.Add(new Robot(new Tuple<int, int>(pos[0], pos[1]), new Tuple<int, int>(vel[0], vel[1])));
}

/**
 * This solution can't do without hard-coded values: I discovered a pattern of positions that stood out from
 * the rest, and this started occuring after 77 seconds for my input, with a steady cycle of 101 seconds after that.
 * I knew I had to observe those states, and those states only - also making my code a bit faster than iterating over
 * every state for each second.
 * 
 * When I found the Christmas tree in the output I knew what to look for and hard-coded it, just for aesthetics!
 * Because I actually only need to check one of the lines (I chose the bottom of the needles), but what fun would that be?
  */
var patternStart = 77;
var patternIncrement = 101;
var christmasTreePicture = new string[33]
{
    "1111111111111111111111111111111",
    "1.............................1",
    "1.............................1",
    "1.............................1",
    "1.............................1",
    "1..............1..............1",
    "1.............111.............1",
    "1............11111............1",
    "1...........1111111...........1",
    "1..........111111111..........1",
    "1............11111............1",
    "1...........1111111...........1",
    "1..........111111111..........1",
    "1.........11111111111.........1",
    "1........1111111111111........1",
    "1..........111111111..........1",
    "1.........11111111111.........1",
    "1........1111111111111........1",
    "1.......111111111111111.......1",
    "1......11111111111111111......1",
    "1........1111111111111........1",
    "1.......111111111111111.......1",
    "1......11111111111111111......1",
    "1.....1111111111111111111.....1",
    "1....111111111111111111111....1",
    "1.............111.............1",
    "1.............111.............1",
    "1.............111.............1",
    "1.............................1",
    "1.............................1",
    "1.............................1",
    "1.............................1",
    "1111111111111111111111111111111"
};

var christmasTree = christmasTreePicture[24].Replace(".", "0");

foreach (var robot in robots)
    robot.Move(dimensions, patternStart);

var seconds = 0;
while (true)
{
    var bathroom = string.Empty;
    for (var x = 38; x < 69; x++)
    {
        var count = 0;
        foreach (var robot in robots)
            if (robot.position.Item1 == x && robot.position.Item2 == 53)
                count++;

        bathroom += $"{count}";
    }

    if (bathroom.Contains(christmasTree))
        break;

    foreach (var robot in robots)
        robot.Move(dimensions, patternIncrement);

    seconds++;
}

Console.WriteLine(patternStart + seconds * patternIncrement);

internal class Robot(Tuple<int, int> position, Tuple<int, int> velocity)
{
    public Tuple<int, int> position = position;
    public Tuple<int, int> velocity = velocity;

    public void Move(Tuple<int, int> dimensions, int steps)
    {
        var pX = position.Item1 + (velocity.Item1 * steps) % dimensions.Item1;
        var pY = position.Item2 + (velocity.Item2 * steps) % dimensions.Item2;


        if (pX < 0)
            pX += dimensions.Item1;
        if (pX >= dimensions.Item1)
            pX -= dimensions.Item1;

        if (pY < 0)
            pY += dimensions.Item2;
        if (pY >= dimensions.Item2)
            pY -= dimensions.Item2;

        position = new Tuple<int, int>(pX, pY);
    }
}