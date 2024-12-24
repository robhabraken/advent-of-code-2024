var lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\24\\input.txt");

var wires = new SortedDictionary<string, Wire>();
var gates = new List<Gate>();

// read all wires and gates
foreach (var line in lines)
{
    if (line.Contains("->"))
    {
        var elements = line.Split(' ');
        addWire(elements[0]);
        addWire(elements[2]);
        addWire(elements[4]);

        gates.Add(new Gate(wires[elements[0]], wires[elements[2]], wires[elements[4]], elements[1]));
    }
}

// set initial values
foreach (var line in lines)
{
    if (line.Contains(':'))
    {
        var values = line.Split(": ");
        wires[values[0]].value = values[1].Equals("1");
    }
}

// start processing
bool allReady;
do
{
    allReady = true;
    foreach (var gate in gates)
        if (!gate.Process())
            allReady = false;
}
while (!allReady);

// produce number
var answer = string.Empty;
foreach (var wireName in wires.Keys)
    if (wireName.StartsWith("z"))
        answer = $"{(wires[wireName].value.Value ? 1 : 0)}{answer}";

Console.WriteLine(Convert.ToInt64(answer, 2));

Console.WriteLine();

void addWire(string wireName)
{
    if (!wires.ContainsKey(wireName))
        wires.Add(wireName, new Wire(wireName, null));
}

class Gate(Wire in1, Wire in2, Wire output, string op)
{
    public Wire[] inputs = [in1, in2];
    public Wire output = output;
    public string op = op;
    public bool ready = false;

    public bool Process()
    {
        if (ready) return true;

        if (!inputs[0].value.HasValue || !inputs[1].value.HasValue)
            return false;

        switch (op)
        {
            case "AND":
                output.value = inputs[0].value.Value && inputs[1].value.Value;
                break;
            case "OR":
                output.value = inputs[0].value.Value || inputs[1].value.Value;
                break;
            case "XOR":
                output.value = inputs[0].value.Value != inputs[1].value.Value;
                break;
        }

        ready = true;
        return true;
    }
}

class Wire(string name, bool? value)
{
    public string name = name;
    public bool? value = value;
}