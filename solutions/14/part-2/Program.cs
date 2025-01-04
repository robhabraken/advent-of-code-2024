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

var seconds = 0;
var deviationCalibration = 0D;
var markers = new List<int>();
while (true)
{
    var average = new int[2];
    foreach (var robot in robots)
    {
        average[0] += robot.position.Item1;
        average[1] += robot.position.Item2;
    }

    average[0] /= robots.Count;
    average[1] /= robots.Count;

    var deviation = new int[2];
    foreach (var robot in robots)
    {
        deviation[0] += Math.Abs(average[0] - robot.position.Item1);
        deviation[1] += Math.Abs(average[1] - robot.position.Item2);

        robot.Move(dimensions);
    }

    deviation[0] /= robots.Count;
    deviation[1] /= robots.Count;

    if (deviationCalibration == 0)
        deviationCalibration = deviation[0] + deviation[1];
    else
    {
        var distribution = Math.Round(deviationCalibration / (deviation[0] + deviation[1]), 1);
        if (distribution > 1.1)
            markers.Add(seconds);

        if (markers.Count == 4)
            break;
    }

    seconds++;
}

if (markers[2] - markers[0] == dimensions.Item1)
    seconds = leastCommonMultiple(dimensions.Item1, markers[0], dimensions.Item2, markers[1]);
else
    seconds = leastCommonMultiple(dimensions.Item1, markers[1], dimensions.Item2, markers[0]);

Console.WriteLine(seconds);

static int leastCommonMultiple(int a, int offsetA, int b, int offsetB)
{
    var lcm = offsetA + a;
    while (true)
    {
        if ((lcm - offsetA) % a == 0 && (lcm - offsetB) % b == 0)
            return lcm;

        lcm += a;
    }
}

class Robot(Tuple<int, int> position, Tuple<int, int> velocity)
{
    public Tuple<int, int> position = position;
    public Tuple<int, int> velocity = velocity;

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
}