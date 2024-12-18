using System.Diagnostics;

var s = new Stopwatch(); s.Start();

var lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\16\\input.txt");

var deltaMap = new int[4, 2] { { -1, 0 }, { 0, 1 }, { 1, 0 }, { 0, -1 } };

var nodes = new List<Node>();
var start = new Node(0, 0, true, false);
var end = new Node(0, 0, false, true);

var allNodes = new HashSet<Tuple<int, int>>();
var allNodesVis = new List<Node>();

var minCost = buildMap(-1, -1);

var path = new List<Node>();
path.Add(end);
buildPath(path, end);

draw(path);

var counter = 0;

allNodesVis.AddRange(path);
foreach (var rNode in path)
{
    allNodes.Add(new Tuple<int, int>(rNode.x, rNode.y));
    if (!rNode.start && !rNode.end && rNode.connections.Count != 2)
    {
        var thisCost = buildMap(rNode.x, rNode.y);
        if (thisCost == minCost)
        {
            var p = new List<Node>();
            buildPath(p, end);

            allNodesVis.AddRange(p);
            foreach (var n in p)
                allNodes.Add(new Tuple<int, int>(n.x, n.y));
        }
        counter++;
    }
}

draw(allNodesVis);

s.Stop(); Console.WriteLine(s.ElapsedMilliseconds + " ms");

Console.WriteLine(minCost);
Console.WriteLine(allNodes.Count);
Console.WriteLine("tests done: " + counter);


void draw(List<Node> list)
{
    //for (var ry = 0; ry < lines.Length; ry++)
    //{
    //    for (var rx = 0; rx < lines[0].Length; rx++)
    //    {
    //        var found = false;
    //        foreach (var node in list)
    //            if (node.x == rx && node.y == ry)
    //            {
    //                found = true;
    //                break;
    //            }
    //        if (found)
    //            Console.Write("O");
    //        else
    //            Console.Write(lines[ry][rx]);
    //    }
    //    Console.WriteLine();
    //}
}

int buildMap(int blockX, int blockY)
{
    nodes = [];

    for (var y = 0; y < lines.Length; y++)
        for (var x = 0; x < lines[0].Length; x++)
            if (!lines[y][x].Equals('#') && !(y == blockY && x == blockX))
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

    return end.visited && end.minCostToStart != null ? end.minCostToStart.Value : int.MaxValue;
}

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

void buildPath(List<Node> nodes, Node node)
{
    if (node.nearestToStart == null)
        return;

    nodes.Add(node.nearestToStart);
    buildPath(nodes, node.nearestToStart);
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