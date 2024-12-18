var lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\18\\input.txt");

var deltaMap = new int[4, 2] { { -1, 0 }, { 0, 1 }, { 1, 0 }, { 0, -1 } };

var nodes = new List<Node>();
var start = new Node(new Point(0, 0), true, false);
var end = new Node(new Point(0, 0), false, true);

var map = new char[71, 71];
for (var i = 0; i < 1024; i++)
{
    var bytePosition = lines[i].Split(',').Select(int.Parse).ToArray();
    map[bytePosition[1], bytePosition[0]] = '#';
}

for (var y = 0; y < map.GetLength(0); y++)
    for (var x = 0; x < map.GetLength(1); x++)
        if (!map[y, x].Equals('#'))
        {
            var node = new Node(new Point(x, y), y == 0 && x == 0, y == map.GetLength(0) - 1 && x == map.GetLength(1) - 1);
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
    node.totalDistance = straightLineDistance(node, end);
}

search();

// draw(nodes);

var path = new List<Node>();
buildPath(path, end);

Console.WriteLine(path.Count);

void draw(List<Node> list)
{
    for (var ry = 0; ry < map.GetLength(0); ry++)
    {
        for (var rx = 0; rx < map.GetLength(1); rx++)
        {
            var found = false;
            foreach (var node in list)
                if (node.p.x == rx && node.p.y == ry)
                {
                    found = true;
                    break;
                }
            if (found)
                Console.Write("O");
            else if (map[ry, rx].Equals('#'))
                Console.Write("#");
            else
                Console.Write('.');
        }
        Console.WriteLine();
    }
}

double straightLineDistance(Node from, Node to)
{
    return Math.Sqrt(Math.Pow(from.p.x - to.p.x, 2) + Math.Pow(from.p.y - from.p.y, 2));
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
        foreach (var c in node.connections.OrderBy(x => x.Cost))
        {
            var connectedNode = c.ConnectedNode;
            if (connectedNode.visited)
                continue;

            var cost = node.minCostToStart + c.Cost;
            if (connectedNode.minCostToStart == null || cost < connectedNode.minCostToStart)
            {
                connectedNode.minCostToStart = cost;
                connectedNode.nearestToStart = node;

                if (!priorityQueue.Contains(connectedNode))
                    priorityQueue.Add(connectedNode);
            }

            node.visited = true;
        }
        if (node.end)
            return;
    } while (priorityQueue.Count > 0);
}

void buildPath(List<Node> nodes, Node node)
{
    if (node.nearestToStart == null)
        return;

    nodes.Add(node.nearestToStart);
    buildPath(nodes, node.nearestToStart);
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