var lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\11\\input.txt");


var stones = lines[0].Split(' ').Select(long.Parse).ToList();

for (var i = 0; i < 25; i++)
{
    var blink = new List<long>();
    for (var s = 0; s < stones.Count; s++)
    {
        var str = $"{stones[s]}";

        if (stones[s] == 0)
            blink.Add(1);
        else if (str.Length % 2 == 0)
        {
            blink.Add(long.Parse(str[..(str.Length / 2)]));
            blink.Add(long.Parse(str[(str.Length / 2)..]));
        }
        else
            blink.Add(stones[s] * 2024);
    }
    stones = blink;

    //foreach (var stone in blink)
    //    Console.Write($"{stone} ");
    //Console.WriteLine();
}

Console.WriteLine(stones.Count);
