var lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\13\\input.txt");

var answer = 0;
for (var i = 0; i < lines.Length; i += 4)
{
    var A = lines[i][12..].Replace(" Y+", string.Empty).Split(',').Select(int.Parse).ToArray();
    var B = lines[i + 1][12..].Replace(" Y+", string.Empty).Split(',').Select(int.Parse).ToArray();
    var prize = lines[i + 2][9..].Replace(" Y=", string.Empty).Split(',').Select(int.Parse).ToArray();

    var outcomesA = new List<Tuple<int, int>>();
    var outcomesB = new List<Tuple<int, int>>();

    for (var push = 0; push <= 100; push++)
    {
        if (A[0] * push <= prize[0] && A[1] * push <= prize[1])
            outcomesA.Add(new Tuple<int, int>(A[0] * push, A[1] * push));

        if (B[0] * push <= prize[0] && B[1] * push <= prize[1])
            outcomesB.Add(new Tuple<int, int>(B[0] * push, B[1] * push));
    }

    for (var j = 0; j < outcomesA.Count; j++)
        for (var k = 0; k < outcomesB.Count; k++)
            if (outcomesA[j].Item1 + outcomesB[k].Item1 == prize[0] &&
                outcomesA[j].Item2 + outcomesB[k].Item2 == prize[1])
                answer += j * 3 + k;
}

Console.WriteLine(answer);