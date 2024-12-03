using System.Text.RegularExpressions;

string[] lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\03\\input.txt");

var regex = new Regex(@"mul\([0-9]{1,3},[0-9]{1,3}\)|do\(\)|don't\(\)");
var enabled = true;

var answer = 0;
foreach (var line in lines)
{
    var memoryLine = line;
    while (true)
    {
        var match = regex.Match(memoryLine);

        if (!match.Success)
            break;

        if (enabled && match.Value.StartsWith("mul"))
        {
            var numbers = $"{match.Value}"[4..^1].Split(',').Select(int.Parse).ToArray();
            answer += numbers[0] * numbers[1];
        }
        else if (match.Value.Equals("do()"))
            enabled = true;
        else if (match.Value.Equals("don't()"))
            enabled = false;

        memoryLine = memoryLine[(match.Index + match.Length)..];
    }

}

Console.WriteLine(answer);