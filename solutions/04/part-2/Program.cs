string[] lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\04\\input.txt");

var answer = 0;
for (var y = 1; y < lines.Length - 1; y++)
    for (var x = 1; x < lines[y].Length - 1; x++)
        if (lines[y][x].Equals('A'))
        {
            var d1 = $"{lines[y - 1][x - 1]}A{lines[y + 1][x + 1]}";
            var d2 = $"{lines[y + 1][x - 1]}A{lines[y - 1][x + 1]}";

            if ((d1.Equals("SAM") || d1.Equals("MAS")) && (d2.Equals("SAM") || d2.Equals("MAS")))
                answer++;
        }

Console.WriteLine(answer);