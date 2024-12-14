var lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\14\\input.txt");

var robots = new List<Robot>();
// var dimensions = new Tuple<int, int>(11, 7);
var dimensions = new Tuple<int, int>(101, 103);

foreach (var line in lines)
{
    var input = line.Split(' ');
    var pos = input[0].Replace("p=", string.Empty).Split(',').Select(int.Parse).ToArray();
    var vel = input[1].Replace("v=", string.Empty).Split(',').Select(int.Parse).ToArray();

    robots.Add(new Robot() {
        position = new Tuple<int, int>(pos[0], pos[1]),
        velocity = new Tuple<int, int>(vel[0], vel[1])
    });
}

//for (var i = 0; i < 100; i++)
//{
//    foreach (var robot in robots)
//        robot.Move(dimensions);
//}
foreach (var robot in robots)
    robot.Move(dimensions, 100);

for (var y = 0; y < dimensions.Item2; y++)
{
    for (var x = 0; x < dimensions.Item1; x++)
    {
        var count = 0;
        foreach (var robot in robots)
        {
            if (robot.position.Item1 == x && robot.position.Item2 == y)
                count++;
        }
        if (count > 0)
            Console.Write(count);
        else
            Console.Write(".");
    }
    Console.WriteLine();
}

var quadrants = new int[4];
foreach (var  robot in robots)
{
    if (robot.position.Item1 < dimensions.Item1 / 2 && robot.position.Item2 < dimensions.Item2 / 2)
        quadrants[0]++;
    else if (robot.position.Item1 > dimensions.Item1 / 2 && robot.position.Item2 < dimensions.Item2 / 2)
        quadrants[1]++;
    else if (robot.position.Item1 < dimensions.Item1 / 2 && robot.position.Item2 > dimensions.Item2 / 2)
        quadrants[2]++;
    else if (robot.position.Item1 > dimensions.Item1 / 2 && robot.position.Item2 > dimensions.Item2 / 2)
        quadrants[3]++;
}

foreach (var q in quadrants)
    Console.WriteLine(q);

Console.WriteLine(quadrants[0] * quadrants[1] * quadrants[2] * quadrants[3]);

class Robot()
{
    public Tuple<int, int> position;
    public Tuple<int, int> velocity;

    public void Move(Tuple<int, int> dimensions)
    {
        var pX = position.Item1 + velocity.Item1;
        var pY = position.Item2 + velocity.Item2;


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