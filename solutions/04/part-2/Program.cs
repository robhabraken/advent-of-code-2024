string[] lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\04\\input.txt");

var answer = 0;
for (var line = 1; line < lines.Length - 1; line++)
    for (var character = 1; character < lines[line].Length - 1; character++)
        if (lines[line][character].Equals('A'))
            Search(line, character);

Console.WriteLine(answer);

void Search(int y, int x)
{
    var d1 = $"{lines[y - 1][x - 1]}A{lines[y + 1][x + 1]}";
    var d2 = $"{lines[y + 1][x - 1]}A{lines[y - 1][x + 1]}";

    if ((d1.Equals("SAM") || d1.Equals("MAS")) && (d2.Equals("SAM") || d2.Equals("MAS")))
        answer++;
}