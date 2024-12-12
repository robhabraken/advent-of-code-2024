var lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\12\\input.txt");

var visited = new bool[lines.Length, lines[0].Length];
var deltaMap = new int[4, 2] { { -1, 0 }, { 0, 1 }, { 1, 0 }, { 0, -1 } };

var answer = 0;
for (var y = 0; y < lines.Length; y++)
    for (var x = 0; x < lines[y].Length; x++)
        find(y, x);

Console.WriteLine(answer);

void find(int y, int x)
{
    if (visited[y, x])
        return;

    visited[y, x] = true;

    var plantType = lines[y][x];
    var plots = new List<Tuple<int, int>>() { new Tuple<int, int>(y, x) };
    var perimeter = 0;
    discover(y, x, ref plots, ref perimeter);

    // Console.WriteLine($"{lines[y][x]}: {plots.Count} * {perimeter} = {plots.Count * perimeter}");

    answer += (plots.Count * perimeter);
}

void discover(int y, int x, ref List<Tuple<int, int>> plots, ref int perimeter)
{
    int dY = 0, dX = 0;
    for (var i = 0; i < 4; i++)
    {
        dY = y + deltaMap[i, 0];
        dX = x + deltaMap[i, 1];

        if (dY >= 0 && dY < lines.Length && dX >= 0 && dX < lines[0].Length)
        {
            if (lines[dY][dX].Equals(lines[y][x]))
            {
                if (!visited[dY, dX])
                {
                    plots.Add(new Tuple<int, int>(dY, dX));
                    visited[dY, dX] = true;
                    discover(dY, dX, ref plots, ref perimeter);
                }
            }
            else
            {
                perimeter++;
            }
        }
        else
        {
            perimeter++;
        }
    }
}