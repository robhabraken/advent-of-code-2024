var lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\19\\input.txt");

var answer = 0;
var availablePatterns = lines[0].Split(", ");

for (var i = 2; i < lines.Length; i++)
    if (sortTowels(lines[i]))
        answer++;

Console.WriteLine(answer);

bool sortTowels(string design)
{
    var merge = "".PadLeft(design.Length, '.');
    foreach (var pattern in availablePatterns)
    {
        if (design.Contains(pattern))
        {
            var lastIndex = 0;
            var indexes = new List<int>();

            while (design[lastIndex..].Contains(pattern))
            {
                indexes.Add(design.IndexOf(pattern, lastIndex));
                lastIndex = design.IndexOf(pattern, lastIndex) + pattern.Length;
            }

            foreach (var index in indexes)
                merge = merge[..index] + pattern + merge[(index + pattern.Length)..];
        }
    }

    return !merge.Contains('.');
}