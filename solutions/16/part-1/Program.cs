var lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\16\\input.txt");

var deltaMap = new int[4, 2] { { -1, 0 }, { 0, 1 }, { 1, 0 }, { 0, -1 } };

var nodes = new List<Node>();
var start = new Node(0, 0, true, false);
var end = new Node(0, 0, false, true);

for (var y = 0; y < lines.Length; y++)
    for (var x = 0; x < lines[0].Length; x++)
        if (!lines[y][x].Equals('#'))
        {
            var node = new Node(x, y, lines[y][x].Equals('S'), lines[y][x].Equals('E'));
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
        {
            var e = new Edge(otherNode, 1);

            for (var i = 0; i < 4; i++)
                if (otherNode.y - node.y == deltaMap[i, 0] && otherNode.x - node.x == deltaMap[i, 1])
                    e.direction = i;

            node.connections.Add(e);
        }
    node.totalDistance = Math.Sqrt(Math.Pow(node.x - end.x, 2) + Math.Pow(node.y - end.y, 2));
}

search();

Console.WriteLine(end.minCostToStart);

void search()
{
    start.minCostToStart = 0;
    var priorityQueue = new List<Tuple<Node, int>> { new(start, 1) };
    do
    {
        priorityQueue = [.. priorityQueue.OrderBy(x => x.Item1.minCostToStart + x.Item1.totalDistance)];
        var nodeWithDirection = priorityQueue.First();
        priorityQueue.Remove(nodeWithDirection);
        foreach (var c in nodeWithDirection.Item1.connections.OrderBy(x => x.Cost))
        {
            var connectedNode = c.ConnectedNode;
            if (connectedNode.visited)
                continue;

            var cost = nodeWithDirection.Item1.minCostToStart + c.Cost;
            if (c.direction != nodeWithDirection.Item2)
                cost += 1000;

            if (connectedNode.minCostToStart == null || cost < connectedNode.minCostToStart)
            {
                connectedNode.minCostToStart = cost;
                connectedNode.nearestToStart = nodeWithDirection.Item1;

                var newTuple = new Tuple<Node, int>(connectedNode, c.direction);
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
    public List<Edge> connections = [];

    public bool start = start;
    public bool end = end;

    public int? minCostToStart;
    public double totalDistance;
    public bool visited;
    public Node nearestToStart;
}

class Edge(Node connectedNode, int cost)
{
    public int Cost = cost;
    public Node ConnectedNode = connectedNode;
    public int direction;
}