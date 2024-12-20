var lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\19\\input.txt");

var answer = 0L;
var availablePatterns = lines[0].Split(", ");

for (var i = 2; i < lines.Length; i++)
    answer += sortTowels(lines[i]);

Console.WriteLine(answer);

long sortTowels(string design)
{
    var towels = new List<string>[design.Length];
    for (var i = 0; i < design.Length; i++)
        towels[i] = [];

    foreach (var pattern in availablePatterns)
    {
        if (design.Contains(pattern))
        {
            var lastIndex = 0;
            var indexes = new List<int>();

            while (design[lastIndex..].Contains(pattern))
            {
                indexes.Add(design.IndexOf(pattern, lastIndex));
                towels[design.IndexOf(pattern, lastIndex)].Add(pattern);
                lastIndex = design.IndexOf(pattern, lastIndex) + 1;
            }
        }
    }

    var possibilities = new long[design.Length];
    for (var i = towels.Length - 1; i >= 0; i--)
        foreach (var towel in towels[i])
            if (i + towel.Length < design.Length)
                possibilities[i] += possibilities[i + towel.Length];
            else
                possibilities[i]++;

    return possibilities[0];
}