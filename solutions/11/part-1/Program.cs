var lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\11\\input.txt");

var cache = new Dictionary<long, List<long>>();
var stones = lines[0].Split(' ').Select(long.Parse).ToList();

stones.Sort();
for (var i = 0; i < 25; i++)
{
    var blink = new List<long>();
    for (var s = 0; s < stones.Count; s++)
        blink.AddRange(breakdownStones(stones[s]));

    stones = blink;
}

Console.WriteLine(stones.Count);

List<long> breakdownStones(long stoneEngraving)
{
    if (cache.TryGetValue(stoneEngraving, out List<long>? value))
        return value;

    var breakdown = new List<long>();

    var str = $"{stoneEngraving}";
    if (stoneEngraving == 0)
        breakdown.Add(1);
    else if (str.Length % 2 == 0)
    {
        breakdown.Add(long.Parse(str[..(str.Length / 2)]));
        breakdown.Add(long.Parse(str[(str.Length / 2)..]));
    }
    else
        breakdown.Add(stoneEngraving * 2024);

    cache.Add(stoneEngraving, breakdown);
    return breakdown;
}