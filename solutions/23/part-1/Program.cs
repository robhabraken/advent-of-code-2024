var lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\23\\input.txt");

var computers = new Dictionary<string, Computer>(); 

foreach (var line in lines)
{
    var names = line.Split('-');
    foreach (var name in names)
    {
        if (!computers.ContainsKey(name))
        {
            var c = new Computer();
            c.Name = name;
            c.connections = new List<Computer>();
            computers.Add(name, c);
        }
    }

    computers[names[0]].connections.Add(computers[names[1]]);
    computers[names[1]].connections.Add(computers[names[0]]);
}

var sets = new HashSet<string>();
foreach (var c in computers.Keys)
{
    foreach (var conn in computers[c].connections)
    {
        foreach (var conn2 in computers[c].connections)
        {
            if (conn != conn2)
            {
                if (conn2.connections.Contains(computers[c]) && conn2.connections.Contains(conn))
                {
                    if (computers[c].Name.StartsWith("t") || conn.Name.StartsWith("t") || conn2.Name.StartsWith("t"))
                    {
                        var list = new List<string>
                        {
                            computers[c].Name,
                            conn.Name,
                            conn2.Name
                        };
                        list.Sort();
                        sets.Add($"{list[0]},{list[1]},{list[2]}");
                    }
                }
            }
        }
    }
}

Console.WriteLine(sets.Count);

class Computer
{
    public string Name;
    public List<Computer> connections;
}