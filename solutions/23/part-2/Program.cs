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

var answer = string.Empty;
foreach (var c in computers.Keys)
{
    test(computers[c]);
}


Console.WriteLine(answer);

void test(Computer c)
{
    //if (c.Name != "ka") return; // test

    var list = new List<string>();
    foreach (var conn in c.connections)
    {
        //Console.WriteLine(c.Name + "-" + conn.Name);

        foreach (var conn2 in conn.connections)
        {
                
            if (c.connections.Contains(conn2))
            {
                //Console.WriteLine("\t" + conn.Name + "-" + conn2.Name);
                if (!list.Contains(conn.Name))
                    list.Add(conn.Name);
                if (!list.Contains(conn2.Name))
                    list.Add(conn2.Name);
            }
        }
    }

    if (list.Count > 0)
    {
        var remove = new List<string>();
        foreach (var comp in list)
        {
            foreach (var comp2 in list)
            {
                if (comp != comp2)
                {
                    if (!computers[comp].connections.Contains(computers[comp2]))
                    {
                        remove.Add(comp);
                    }
                }
            }
        }

        foreach (var r in remove)
        {
            list.Remove(r);
        }
    }

    if (list.Count > 0)
    {
        list.Add(c.Name);
        list.Sort();
        var result = "";
        foreach (var l in list)
            result += $"{l},";

        if (result.Length > answer.Length + 1)
            answer = result[..^1];
    }
}

class Computer
{
    public string Name;
    public List<Computer> connections;
}