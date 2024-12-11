var lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\11\\input.txt");

var cache = new Dictionary<long, List<long>>();

var stones = new Dictionary<long, long>(); // engraving, multiplier
foreach (var stone in lines[0].Split(' ').Select(long.Parse).ToList())
    stones.Add(stone, 1);

for (var i = 0; i < 75; i++)
{
    var blink = new Dictionary<long, long>();
    foreach (var stone in stones.Keys)
    {
        var multiplier = stones[stone];
        foreach (var newStone in changeStones(stone))
            if (!blink.TryAdd(newStone, multiplier))
                blink[newStone] += multiplier;
    }
    stones = blink;
}

var answer = 0L;
foreach (var stone in stones.Keys)
    answer += stones[stone];

Console.Write(answer);

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