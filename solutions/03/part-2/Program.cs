using System.Text.RegularExpressions;

string[] lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\03\\input.txt");

var regex = new Regex(@"mul\(\d{1,3},\d{1,3}\)|do\(\)|don't\(\)");
var enabled = true;

var answer = 0;
foreach (var line in lines)
    foreach (var match in regex.Matches(line))
    {
        var instruction = $"{match}";
        if (enabled && instruction.StartsWith("mul"))
        {
            var numbers = instruction[4..^1].Split(',').Select(int.Parse).ToArray();
            answer += numbers[0] * numbers[1];
        }
        else if (instruction.Equals("do()"))
            enabled = true;
        else if (instruction.Equals("don't()"))
            enabled = false;
    }

Console.WriteLine(answer);