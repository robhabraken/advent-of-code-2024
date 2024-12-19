var lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\19\\input.txt");

var answer = 0;
var availablePatterns = lines[0].Split(", ");

for (var i = 2; i < lines.Length; i++)
{
    var possible = sortTowels(lines[i]);
    if (possible)
        answer++;
}

Console.WriteLine(answer);

bool sortTowels(string design)
{
    var selectPatterns = new List<string>();
    foreach (var pattern in availablePatterns)
        if (design.Contains(pattern))
            selectPatterns.Add(pattern);

    var condense = string.Empty.PadLeft(design.Length, '.');
    for (var i = 0; i < selectPatterns.Count; i++)
    {
        var lastIndex = 0;
        var crop = design;
        var indexes = new List<int>();

        while (crop.Contains(selectPatterns[i]))
        {
            indexes.Add(design.IndexOf(selectPatterns[i], lastIndex));
            lastIndex += crop.IndexOf(selectPatterns[i]) + selectPatterns[i].Length;
            crop = crop[(crop.IndexOf(selectPatterns[i]) + selectPatterns[i].Length)..];
        }

        foreach (var index in indexes)
            condense = condense[..index] + selectPatterns[i] + condense[(index + selectPatterns[i].Length)..];
    }

    return !condense.Contains('.');
}