var lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\19\\input.txt");

var answer = 0;
var availablePatterns = lines[0].Split(", ");

for (var i = 2; i < lines.Length; i++)
    if (sortTowels(lines[i]))
        answer++;

Console.WriteLine(answer);

bool sortTowels(string design)
{
    var selectPatterns = new List<string>();
    foreach (var pattern in availablePatterns)
        if (design.Contains(pattern))
            selectPatterns.Add(pattern);

    var merge = "".PadLeft(design.Length, '.');
    for (var i = 0; i < selectPatterns.Count; i++)
    {
        var lastIndex = 0;
        var indexes = new List<int>();

        while (design[lastIndex..].Contains(selectPatterns[i]))
        {
            indexes.Add(design.IndexOf(selectPatterns[i], lastIndex));
            lastIndex = design.IndexOf(selectPatterns[i], lastIndex) + selectPatterns[i].Length;
        }

        foreach (var index in indexes)
            merge = merge[..index] + selectPatterns[i] + merge[(index + selectPatterns[i].Length)..];
    }

    return !merge.Contains('.');
}