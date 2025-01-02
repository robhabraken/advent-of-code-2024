var lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\16\\input.txt");

var deltaMap = new int[4, 2] { { -1, 0 }, { 0, 1 }, { 1, 0 }, { 0, -1 } };

var nodesArray = new Node[lines.Length, lines[0].Length];
var start = new Node(0, 0, true, false);
var end = new Node(0, 0, false, true);

setupGraph();
var minCost = search();

var shortestPath = new List<Node> { end };
buildPath(shortestPath, end);

var seats = new HashSet<int>();
foreach (var node in shortestPath)
{
    seats.Add(node.y * lines.Length + node.x);
    if (!node.start && !node.end && node.connections.Count != 2)
    {
        resetGraph();
        var cost = search(node);
        if (cost == minCost)
        {
            var path = new List<Node>();
            buildPath(path, end);

            foreach (var newNode in path)
                seats.Add(newNode.y * lines.Length + newNode.x);
        }
    }
}

Console.WriteLine(seats.Count);

void setupGraph()
{
    for (var y = 0; y < lines.Length; y++)
        for (var x = 0; x < lines[0].Length; x++)
            if (!lines[y][x].Equals('#'))
            {
                var node = new Node(x, y, lines[y][x].Equals('S'), lines[y][x].Equals('E'));
                nodesArray[y, x] = node;

                if (node.start)
                    start = node;
                else if (node.end)
                    end = node;
            }

    for (var y = 1; y < lines.Length - 1; y++)
        for (var x = 1; x < lines[0].Length - 1; x++)
            if (nodesArray[y, x] != null)
                for (var i = 0; i < 4; i++)
                    if (nodesArray[y + deltaMap[i, 0], x + deltaMap[i, 1]] != null)
                        nodesArray[y, x].connections.Add(new Edge(nodesArray[y + deltaMap[i, 0], x + deltaMap[i, 1]], i));
}

void resetGraph()
{
    foreach (var node in nodesArray)
        node?.Reset();
}

int search(Node? blockedNode = null)
{
    start.minCostToStart = 0;
    var priorityQueue = new List<Tuple<Node, int>> { new(start, 1) };
    do
    {
        priorityQueue = [.. priorityQueue.OrderBy(x => x.Item1.minCostToStart)];
        var nodeWithDirection = priorityQueue.First();
        priorityQueue.Remove(nodeWithDirection);
        foreach (var c in nodeWithDirection.Item1.connections)
        {
            if (c.ConnectedNode.visited)
                continue;

            if (blockedNode != null && c.ConnectedNode == blockedNode)
                continue;

            var cost = nodeWithDirection.Item1.minCostToStart + 1;
            if (c.direction != nodeWithDirection.Item2)
                cost += 1000;

            if (c.ConnectedNode.minCostToStart == null || cost < c.ConnectedNode.minCostToStart)
            {
                c.ConnectedNode.minCostToStart = cost;
                c.ConnectedNode.nearestToStart = nodeWithDirection.Item1;

                var newTuple = new Tuple<Node, int>(c.ConnectedNode, c.direction);
                if (!priorityQueue.Contains(newTuple))
                    priorityQueue.Add(newTuple);
            }

            nodeWithDirection.Item1.visited = true;
        }
        if (nodeWithDirection.Item1.end)
            break;
    }
    while (priorityQueue.Count > 0);

    return end.visited && end.minCostToStart != null ? end.minCostToStart.Value : int.MaxValue;
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
    public bool visited;
    public Node? nearestToStart;

    public void Reset()
    {
        minCostToStart = null;
        visited = false;
        nearestToStart = null;
    }
}

class Edge(Node connectedNode, int direction)
{
    public Node ConnectedNode = connectedNode;
    public int direction = direction;
}