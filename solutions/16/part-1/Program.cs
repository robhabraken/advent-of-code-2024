var lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\16\\input.txt");

var deltaMap = new int[4, 2] { { -1, 0 }, { 0, 1 }, { 1, 0 }, { 0, -1 } };

var nodesArray = new Node[lines.Length, lines[0].Length];
var start = new Node(0, 0, true, false);
var end = new Node(0, 0, false, true);

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
        foreach (var c in nodeWithDirection.Item1.connections)
        {
            if (c.ConnectedNode.visited)
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
    public bool visited;
    public Node nearestToStart;
}

class Edge(Node connectedNode, int direction)
{
    public Node ConnectedNode = connectedNode;
    public int direction = direction;
}