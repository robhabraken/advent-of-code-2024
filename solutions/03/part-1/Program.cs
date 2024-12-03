using System.Text.RegularExpressions;

string[] lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\03\\input.txt");

var regex = new Regex(@"mul\([0-9]{1,3},[0-9]{1,3}\)");

var answer = 0;
foreach (var line in lines)
    foreach (var match in regex.Matches(line))
    {
        var numbers = $"{match}"[4..^1].Split(',').Select(int.Parse).ToArray();
        answer += numbers[0] * numbers[1];
    }

Console.WriteLine(answer);