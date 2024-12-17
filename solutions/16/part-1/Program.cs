var lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\16\\input.txt");

var deltaMap = new int[4, 2] { { -1, 0 }, { 0, 1 }, { 1, 0 }, { 0, -1 } };

var nodes = new List<Node>();
var start = new Node(new Point(0, 0), true, false);
var end = new Node(new Point(0, 0), false, true);

for (var y = 0; y < lines.Length; y++)
    for (var x = 0; x < lines[0].Length; x++)
        if (!lines[y][x].Equals('#'))
        {
            var node = new Node(new Point(x, y), lines[y][x].Equals('S'), lines[y][x].Equals('E'));
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
            ((Math.Abs(otherNode.p.x - node.p.x) <= 1 && otherNode.p.y == node.p.y) ||
             (Math.Abs(otherNode.p.y - node.p.y) <= 1) && otherNode.p.x == node.p.x))
        {
            var e = new Edge(otherNode, 1);

            for (var i = 0; i < 4; i++)
                if (otherNode.p.y - node.p.y == deltaMap[i, 0] && otherNode.p.x - node.p.x == deltaMap[i, 1])
                    e.direction = i;

            node.connections.Add(e);
        }
    node.totalDistance = StraightLineDistance(node, end);
}

Search();

Console.WriteLine(end.minCostToStart);

double StraightLineDistance(Node from, Node to)
{
    return Math.Sqrt(Math.Pow(from.p.x - to.p.x, 2) + Math.Pow(from.p.y - from.p.y, 2));
}

void Search()
{
    start.minCostToStart = 0;
    var priorityQueue = new List<Tuple<Node, int>> { new(start, 1) };
    do
    {
        priorityQueue = [.. priorityQueue.OrderBy(x => x.Item1.minCostToStart + x.Item1.totalDistance)];
        var nodeTuple = priorityQueue.First();
        priorityQueue.Remove(nodeTuple);
        foreach (var c in nodeTuple.Item1.connections.OrderBy(x => x.Cost))
        {
            var connectedNode = c.ConnectedNode;
            if (connectedNode.visited)
                continue;

            var cost = nodeTuple.Item1.minCostToStart + c.Cost;
            if (c.direction != nodeTuple.Item2)
                cost += 1000;

            if (connectedNode.minCostToStart == null || cost < connectedNode.minCostToStart)
            {
                connectedNode.minCostToStart = cost;
                connectedNode.nearestToStart = nodeTuple.Item1;

                var newTuple = new Tuple<Node, int>(connectedNode, c.direction);
                if (!priorityQueue.Contains(newTuple))
                    priorityQueue.Add(newTuple);
            }

            nodeTuple.Item1.visited = true;
        }
        if (nodeTuple.Item1.end)
            return;
    } while (priorityQueue.Count > 0);
}

internal class Node(Point p, bool start, bool end)
{
    public Point p = p;
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

class Point(int x, int y)
{
    public int x = x;
    public int y = y;
}