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

var suspiciousGates = new List<Gate>();
var outputWires = wires.Values.Select(w => w).Where(w => w.name.StartsWith('z')).ToList();
foreach (var gate in gates)
{
    // starting gates should be followed by OR if AND, and by AND if XOR, except for the first one
    if ((gate.inputs[0].name.StartsWith('x') || gate.inputs[1].name.StartsWith('x')) &&
        (gate.inputs[0].name.StartsWith('y') || gate.inputs[1].name.StartsWith('y')) &&
        (!gate.inputs[0].name.Contains("00") && !gate.inputs[1].name.Contains("00")))
        foreach (var secondGate in gates)
            if (gate.output == secondGate.inputs[0] || gate.output == secondGate.inputs[1])
                if ((gate.op.Equals("AND") && secondGate.op.Equals("AND")) ||
                    (gate.op.Equals("XOR") && secondGate.op.Equals("OR")))
                    suspiciousGates.Add(gate);

    // gates in the middle should not have XOR operators
    if (!gate.inputs[0].name.StartsWith('x') && !gate.inputs[1].name.StartsWith('x') &&
        !gate.inputs[0].name.StartsWith('y') && !gate.inputs[1].name.StartsWith('y') &&
        !gate.output.name.StartsWith('z') && gate.op.Equals("XOR"))
        suspiciousGates.Add(gate);

    // gates at the end should always have XOR operators, except for the last one
    if (outputWires.Contains(gate.output) && !gate.output.name.Equals($"z{outputWires.Count - 1}") && !gate.op.Equals("XOR"))
        suspiciousGates.Add(gate);
}

var answer = string.Empty;
foreach (var sGate in suspiciousGates.OrderBy(x => x.output.name))
    answer += $"{sGate.output.name},";

Console.WriteLine(answer[..^1]);

void addWire(string wireName)
{
    if (!wires.ContainsKey(wireName))
        wires.Add(wireName, new Wire(wireName));
}

class Gate(Wire in1, Wire in2, Wire output, string op)
{
    public Wire[] inputs = [in1, in2];
    public Wire output = output;
    public string op = op;
}

class Wire(string name)
{
    public string name = name;
}