var lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\10\\input.txt");

var deltaMap = new int[4, 2] { { -1, 0 }, { 0, 1 }, { 1, 0 }, { 0, -1 } };
var reachable9s = new HashSet<Tuple<int, int>>();

var score = 0;
for (var y =  0; y < lines.Length; y++)
{
    for (var x = 0; x < lines[0].Length; x++)
    {
        if (lines[y][x].Equals('0'))
        {
            Walk(y, x);

            score += reachable9s.Count;
            reachable9s.Clear();
        }
    }
}

Console.WriteLine(score);

void Walk(int y, int x)
{
    var height = int.Parse($"{lines[y][x]}");
    if (height == 9)
    {
        reachable9s.Add(new Tuple<int, int>(y, x));
        return;
    }

    for (var i = 0; i < 4; i++)
    {
        var dY = y + deltaMap[i, 0];
        var dX = x + deltaMap[i, 1];

        if (dY >= 0 && dY < lines.Length && dX >= 0 && dX < lines[0].Length)
            if (int.Parse($"{lines[dY][dX]}") == height + 1)
                Walk(dY, dX);
    }
}