string[] l = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\04\\input.txt");

var answer = 0;
for (var y = 1; y < l.Length - 1; y++)
    for (var x = 1; x < l[0].Length - 1; x++)
        if (l[y][x].Equals('A'))
            if (($"{l[y - 1][x - 1]}{l[y + 1][x + 1]}".Equals("SM") || $"{l[y - 1][x - 1]}{l[y + 1][x + 1]}".Equals("MS")) &&
                ($"{l[y + 1][x - 1]}{l[y - 1][x + 1]}".Equals("SM") || $"{l[y + 1][x - 1]}{l[y - 1][x + 1]}".Equals("MS")))
                answer++;

Console.WriteLine(answer);