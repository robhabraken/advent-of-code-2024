var lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\23\\input.txt");

var computers = new Dictionary<string, Computer>();

foreach (var line in lines)
{
    var names = line.Split('-');
    foreach (var name in names)
        if (!computers.ContainsKey(name))
            computers.Add(name, new Computer(name));

    computers[names[0]].connections.Add(computers[names[1]]);
    computers[names[1]].connections.Add(computers[names[0]]);
}

var sets = new HashSet<string>();
foreach (var computerName in computers.Keys)
    foreach (var computerConnection in computers[computerName].connections)
        foreach (var secondGradeConnection in computers[computerName].connections)
            if (computerConnection != secondGradeConnection &&
                secondGradeConnection.connections.Contains(computers[computerName]) &&
                secondGradeConnection.connections.Contains(computerConnection) &&
                (computerName.StartsWith("t") || computerConnection.name.StartsWith("t") || secondGradeConnection.name.StartsWith("t")))
                {
                    var list = new List<string>
                    {
                        computerName,
                        computerConnection.name,
                        secondGradeConnection.name
                    };
                    list.Sort();
                    sets.Add($"{list[0]},{list[1]},{list[2]}");
                }

Console.WriteLine(sets.Count);

class Computer(string name)
{
    public string name = name;
    public List<Computer> connections = [];
}