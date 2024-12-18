var lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\18\\input.txt");

var deltaMap = new int[4, 2] { { -1, 0 }, { 0, 1 }, { 1, 0 }, { 0, -1 } };

var memorySpace = new bool[71, 71];
var nodes = new List<Node>();
var start = new Node(0, 0, true, false);
var end = new Node(0, 0, false, true);

var left = 0;
var right = lines.Length;
var byteCount = right;
while (byteCount != left)
{
    byteCount = left + (right - left) / 2;
    if (endReachable(byteCount))
        left = byteCount + 1;
    else
        right = byteCount - 1;
}

Console.WriteLine(lines[byteCount - 1]);

bool endReachable(int bytesToRead)
{
    memorySpace = new bool[memorySpace.GetLength(0), memorySpace.GetLength(1)];
    nodes = [];

    for (var i = 0; i < bytesToRead; i++)
    {
        var bytePosition = lines[i].Split(',').Select(int.Parse).ToArray();
        memorySpace[bytePosition[1], bytePosition[0]] = true;
    }

    for (var y = 0; y < memorySpace.GetLength(0); y++)
        for (var x = 0; x < memorySpace.GetLength(1); x++)
            if (!memorySpace[y, x])
            {
                var node = new Node(x, y, y == 0 && x == 0, y == memorySpace.GetLength(0) - 1 && x == memorySpace.GetLength(1) - 1);
                nodes.Add(node);

                if (node.start)
                    start = node;
                else if (node.end)
                    end = node;
            }

    foreach (var node in nodes)
    {
        foreach (var otherNode in nodes)
            if (node != otherNode &&
                ((Math.Abs(otherNode.x - node.x) <= 1 && otherNode.y == node.y) ||
                 (Math.Abs(otherNode.y - node.y) <= 1) && otherNode.x == node.x))
                node.connections.Add(otherNode);
        node.totalDistance = Math.Sqrt(Math.Pow(node.x - end.x, 2) + Math.Pow(node.y - end.y, 2));
    }

    search();

    return end.visited;
}

void search()
{
    start.minCostToStart = 0;
    var priorityQueue = new List<Node> { start };
    do
    {
        priorityQueue = [.. priorityQueue.OrderBy(x => x.minCostToStart + x.totalDistance)];
        var node = priorityQueue.First();
        priorityQueue.Remove(node);
        foreach (var conn in node.connections)
        {
            if (conn.visited)
                continue;

            var cost = node.minCostToStart + 1;
            if (conn.minCostToStart == null || cost < conn.minCostToStart)
            {
                conn.minCostToStart = cost;
                conn.nearestToStart = node;

                if (!priorityQueue.Contains(conn))
                    priorityQueue.Add(conn);
            }

            node.visited = true;
        }
        if (node.end)
            return;
    }
    while (priorityQueue.Count > 0);
}

internal class Node(int x, int y, bool start, bool end)
{
    public int x = x;
    public int y = y;
    public List<Node> connections = [];

    public bool start = start;
    public bool end = end;

    public int? minCostToStart;
    public double totalDistance;
    public bool visited;
    public Node nearestToStart;
}