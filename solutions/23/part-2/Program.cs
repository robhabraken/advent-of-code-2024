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

var answer = string.Empty;
foreach (var computerName in computers.Keys)
{
    var connections = findMutualConnections(computers[computerName]);
    removeComputersThatAreNotConnectedToAllOthers(connections);
    setAnswer(computerName, connections);
}

Console.WriteLine(answer);

HashSet<string> findMutualConnections(Computer computer)
{
    var mutualConnections = new HashSet<string>();
    foreach (var computerConnection in computer.connections)
    {
        foreach (var secondGradeConnection in computerConnection.connections)
        {
            if (computer.connections.Contains(secondGradeConnection))
            {
                mutualConnections.Add(computerConnection.name);
                mutualConnections.Add(secondGradeConnection.name);
            }
        }
    }
    return mutualConnections;
}

void removeComputersThatAreNotConnectedToAllOthers(HashSet<string> computerNames)
{
    if (computerNames.Count == 0) return;
    
    var toRemove = new List<string>();
    foreach (var name1 in computerNames)
        foreach (var name2 in computerNames)
            if (name1 != name2)
                if (!computers[name1].connections.Contains(computers[name2]))
                    toRemove.Add(name1);

    foreach (var name in toRemove)
        computerNames.Remove(name);
}

void setAnswer(string computerName, HashSet<string> connections)
{
    if (connections.Count == 0) return;

    var list = connections.ToList();
    list.Add(computerName);
    list.Sort();

    var password = string.Empty;
    foreach (var name in list)
        password += $"{name},";

    if (password.Length > answer.Length + 1)
        answer = password[..^1];
}

class Computer(string name)
{
    public string name = name;
    public List<Computer> connections = [];
}