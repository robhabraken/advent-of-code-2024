string[] lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\02\\input.txt");

var answer = 0;
foreach (var line in lines)
{
    var report = line.Split(' ').Select(int.Parse).ToArray();
    if (isSafe(report))
        answer++;
}

Console.WriteLine(answer);

static bool isSafe(int[] report)
{
    var direction = 0;

    for (int i = 1; i < report.Length; i++)
    {
        var delta = report[i] - report[i - 1];

        if (delta == 0 || delta > 3 || delta < -3)
            return false;

        if (direction == 0)
            direction = delta;
        else
            if ((direction > 0 && delta < 0) || (direction < 0 && delta > 0))
                return false;
    }

    return true;
}