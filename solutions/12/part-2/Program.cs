using static System.Collections.Specialized.BitVector32;

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
    var sections = new List<Tuple<int, int, int, int>>();

    discover(y, x, ref plots, ref sections);
    answer += (plots.Count * countSides(sections));
}

void discover(int y, int x, ref List<Tuple<int, int>> plots, ref List<Tuple<int, int,int, int>> sections)
{
    int dY, dX;
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
                    discover(dY, dX, ref plots, ref sections);
                }
            }
            else
                addPerimeterSection(ref sections, i, dY, dX);
        }
        else
            addPerimeterSection(ref sections, i, dY, dX);
    }
}

void addPerimeterSection(ref List<Tuple<int, int, int, int>> sections, int i, int dY, int dX)
{
    if (deltaMap[i, 0] != 0)
        sections.Add(new Tuple<int, int, int, int>(i % 2, dY * 2 - deltaMap[i, 0], dX, i));
    else
        sections.Add(new Tuple<int, int, int, int>(i % 2, dX * 2 - deltaMap[i, 1], dY, i));
}

int countSides(List<Tuple<int, int, int, int>> sections)
{
    var sides = 0;
    var sortedSections = sections.OrderBy(t => t.Item1).ThenBy(t => t.Item2).ThenBy(t => t.Item3).ThenBy(t => t.Item4).ToList();
    int orientation = -1, axis = -2, plot = -2, direction = -1;    
    foreach (var section in sortedSections)
    {
        if ((section.Item1 != orientation || section.Item2 != axis ||
            !(section.Item3 == plot || section.Item3 == plot + 1)) ||
            section.Item1 == orientation && section.Item4 != direction)
            sides++;

        orientation = section.Item1;
        axis = section.Item2;
        plot = section.Item3;
        direction = section.Item4;
    }
    return sides;
}