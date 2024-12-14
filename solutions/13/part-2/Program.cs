var lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\13\\input.txt");

var answer = 0L;
for (var i = 0; i < lines.Length; i += 4)
{
    var A = lines[i][12..].Replace(" Y+", string.Empty).Split(',').Select(double.Parse).ToArray();
    var B = lines[i + 1][12..].Replace(" Y+", string.Empty).Split(',').Select(double.Parse).ToArray();
    var prize = lines[i + 2][9..].Replace(" Y=", string.Empty).Split(',').Select(double.Parse).ToArray();

    for (var j = 0; j < prize.Length; j++)
        prize[j] += 10000000000000;

    var a = (prize[1] - (B[1] * prize[0] / B[0])) / (A[1] - (B[1] * A[0] / B[0]));
    var b = (prize[0] - (a * A[0])) / B[0];

    if (a > 0 && b > 0 && Math.Round(a, 2) == Math.Round(a, 0) && Math.Round(b, 2) == Math.Round(b, 0))
        answer += (long)Math.Round(a, 0) * 3 + (long)Math.Round(b, 0);
}

Console.WriteLine(answer);