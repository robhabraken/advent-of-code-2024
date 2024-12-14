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
    else if (deviationCalibration / (deviation[0] + deviation[1]) > 1.5D)
        break;

    seconds++;
}

Console.WriteLine(seconds);

internal class Robot(Tuple<int, int> position, Tuple<int, int> velocity)
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