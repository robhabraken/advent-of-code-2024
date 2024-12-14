var lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\13\\input.txt");

var answer = 0L;
for (var i = 0; i < lines.Length; i += 4)
{
    var A = lines[i][12..].Replace(" Y+", string.Empty).Split(',').Select(double.Parse).ToArray();
    var B = lines[i + 1][12..].Replace(" Y+", string.Empty).Split(',').Select(double.Parse).ToArray();
    var prize = lines[i + 2][9..].Replace(" Y=", string.Empty).Split(',').Select(double.Parse).ToArray();

    for (var j = 0; j < prize.Length; j++)
        prize[j] += 10000000000000;

    var x = (prize[1] - (B[1] * prize[0] / B[0])) / (A[1] - (B[1] * A[0] / B[0]));
    var y = (prize[0] - (x * A[0])) / B[0];

    if (x > 0 && y > 0 && Math.Round(x, 2) == Math.Round(x, 0) && Math.Round(y, 2) == Math.Round(y, 0))
        answer += (long)Math.Round(x, 0) * 3 + (long)Math.Round(y, 0);
}

Console.WriteLine(answer);