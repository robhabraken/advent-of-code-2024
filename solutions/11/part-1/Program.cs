var lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\11\\input.txt");

var cache = new Dictionary<long, List<long>>();
var stones = lines[0].Split(' ').Select(long.Parse).ToList();

for (var i = 0; i < 25; i++)
{
    var blink = new List<long>();
    for (var s = 0; s < stones.Count; s++)
        blink.AddRange(changeStones(stones[s]));

    stones = blink;
}

Console.WriteLine(stones.Count);

List<long> changeStones(long engraving)
{
    if (cache.TryGetValue(engraving, out List<long>? value))
        return value;

    var change = new List<long>();

    var str = $"{engraving}";
    if (engraving == 0)
        change.Add(1);
    else if (str.Length % 2 == 0)
    {
        change.Add(long.Parse(str[..(str.Length / 2)]));
        change.Add(long.Parse(str[(str.Length / 2)..]));
    }
    else
        change.Add(engraving * 2024);

    cache.Add(engraving, change);
    return change;
}