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

var shortestPath = new List<Node> { end };
buildPath(shortestPath, end);

var seats = new HashSet<int>();
for (var i = 0; i < shortestPath.Count; i++)
{
    var node = shortestPath[i];
    seats.Add(node.y * lines.Length + node.x);
    if (!node.start && !node.end && countConnections(node) != 2)
        findShortcut(node, i);
}

Console.WriteLine(seats.Count);

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
            break;
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

int countConnections(Node node)
{
    var count = 0;
    for (var i = 0; i < 4; i++)
        if (nodes[node.y + deltaMap[i, 0], node.x + deltaMap[i, 1]] != null)
            count++;

    return count;
}

void findShortcut(Node node, int startIndex)
{
    for (var dir = 0; dir < 4; dir++)
    {
        var neighbor = nodes[node.y + deltaMap[dir, 0], node.x + deltaMap[dir, 1]];
        if (neighbor != null && !shortestPath.Contains(neighbor))
        {
            var opposite = nodes[node.y - deltaMap[dir, 0], node.x - deltaMap[dir, 1]];
            walk([neighbor], neighbor, startIndex, dir, !onPath(opposite), false);
        }
    }
}

void walk(List<Node> path, Node node, int startIndex, int dir, bool dirChange, bool cornered)
{
    while (true)
    {
        node = nodes[node.y + deltaMap[dir, 0], node.x + deltaMap[dir, 1]];
        
        if (node == null)
            return;
                
        path.Add(node);

        if (shortestPath.Contains(node))
        {
            var arrivedAt = shortestPath.IndexOf(node);
            if (arrivedAt > startIndex && path.Count == arrivedAt - startIndex)
            {
                var next = nodes[node.y + deltaMap[dir, 0], node.x + deltaMap[dir, 1]];
                if (dirChange != !onPath(next))
                    foreach (var shortcut in path)
                        seats.Add(shortcut.y * lines.Length + shortcut.x);
            }
            return;
        }

        if (!cornered)
        {
            for (var dir2 = 0; dir2 < 4; dir2++)
            {
                if (dir % 2 != dir2 % 2)
                {
                    var arroundTheCorner = nodes[node.y + deltaMap[dir2, 0], node.x + deltaMap[dir2, 1]];
                    if (arroundTheCorner != null)
                        walk(new List<Node>(path) { arroundTheCorner }, arroundTheCorner, startIndex, dir2, dirChange, true);
                }
            }
        }
    }
}

bool onPath(Node node) => node != null && shortestPath.Contains(node);

internal class Node(int x, int y, bool start, bool end)
{
    public int x = x;
    public int y = y;

    public bool start = start;
    public bool end = end;

    public int? minCostToStart;
    public bool visited;
    public Node? nearestToStart;
}