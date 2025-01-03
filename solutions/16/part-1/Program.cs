var lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\16\\input.txt");

var deltaMap = new int[4, 2] { { -1, 0 }, { 0, 1 }, { 1, 0 }, { 0, -1 } };

var nodes = new Node[lines.Length, lines[0].Length];
var start = new Node(0, 0, true, false);
var end = new Node(0, 0, false, true);

for (var y = 0; y < lines.Length; y++)
    for (var x = 0; x < lines[0].Length; x++)
        if (!lines[y][x].Equals('#'))
        {
            var node = new Node(x, y, lines[y][x].Equals('S'), lines[y][x].Equals('E'));
            nodes[y, x] = node;

            if (node.start)
                start = node;
            else if (node.end)
                end = node;
        }

search();

Console.WriteLine(end.minCostToStart);

void search()
{
    start.minCostToStart = 0;
    var priorityQueue = new List<Tuple<Node, int>> { new(start, 1) };
    do
    {
        priorityQueue = [.. priorityQueue.OrderBy(x => x.Item1.minCostToStart)];
        var nodeWithDirection = priorityQueue.First();
        priorityQueue.Remove(nodeWithDirection);
        for (var dir = 0; dir < 4; dir++)
        {
            var neighbor = nodes[nodeWithDirection.Item1.y + deltaMap[dir, 0], nodeWithDirection.Item1.x + deltaMap[dir, 1]];

            if (neighbor == null || neighbor.visited)
                continue;

            var cost = nodeWithDirection.Item1.minCostToStart + 1;
            if (dir != nodeWithDirection.Item2)
                cost += 1000;

            if (neighbor.minCostToStart == null || cost < neighbor.minCostToStart)
            {
                neighbor.minCostToStart = cost;
                neighbor.nearestToStart = nodeWithDirection.Item1;

                var newTuple = new Tuple<Node, int>(neighbor, dir);
                if (!priorityQueue.Contains(newTuple))
                    priorityQueue.Add(newTuple);
            }

            nodeWithDirection.Item1.visited = true;
        }
        if (nodeWithDirection.Item1.end)
            return;
    }
    while (priorityQueue.Count > 0);
}

internal class Node(int x, int y, bool start, bool end)
{
    public int x = x;
    public int y = y;

    public bool start = start;
    public bool end = end;

    public int? minCostToStart;
    public bool visited;
    public Node nearestToStart;
}