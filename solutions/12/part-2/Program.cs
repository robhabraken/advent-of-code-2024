var lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\12\\input.txt");

var visited = new bool[lines.Length, lines[0].Length];
var deltaMap = new int[4, 2] { { -1, 0 }, { 0, 1 }, { 1, 0 }, { 0, -1 } };

var answer = 0;
for (var y = 0; y < lines.Length; y++)
    for (var x = 0; x < lines[y].Length; x++)
        if (!visited[y, x])
            visitRegion(y, x);

Console.WriteLine(answer);

void visitRegion(int y, int x)
{
    visited[y, x] = true;

    var plots = new List<Tuple<int, int>>() { new(y, x) };
    var sides = new List<Tuple<int, int, int, int>>();

    discover(y, x, ref plots, ref sides);
    answer += (plots.Count * countSides(sides));
}

void discover(int y, int x, ref List<Tuple<int, int>> plots, ref List<Tuple<int, int,int, int>> sides)
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
                    discover(dY, dX, ref plots, ref sides);
                }
            }
            else
                addSide(ref sides, i, dY, dX);
        }
        else
            addSide(ref sides, i, dY, dX);
    }
}

void addSide(ref List<Tuple<int, int, int, int>> sides, int i, int dY, int dX)
{
    if (deltaMap[i, 0] != 0)
        sides.Add(new Tuple<int, int, int, int>(i % 2, dY * 2 - deltaMap[i, 0], dX, i));
    else
        sides.Add(new Tuple<int, int, int, int>(i % 2, dX * 2 - deltaMap[i, 1], dY, i));
}

int countSides(List<Tuple<int, int, int, int>> sides)
{
    var counter = 0;
    var sortedSides = sides.OrderBy(t => t.Item1).ThenBy(t => t.Item2).ThenBy(t => t.Item3).ThenBy(t => t.Item4).ToList();
    int orientation = -1, axis = -2, plot = -2, direction = -1;    
    foreach (var side in sortedSides)
    {
        if ((side.Item1 != orientation || side.Item2 != axis ||
            !(side.Item3 == plot || side.Item3 == plot + 1)) ||
            side.Item1 == orientation && side.Item4 != direction)
            counter++;

        orientation = side.Item1;
        axis = side.Item2;
        plot = side.Item3;
        direction = side.Item4;
    }
    return counter;
}