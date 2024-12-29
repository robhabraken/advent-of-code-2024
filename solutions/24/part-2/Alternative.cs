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

// store initial values so we can reset to initial state later on
foreach (var wire in wires.Values)
    wire.Set();

// select begin and end wires of circuit
var beginWires = new List<Wire>();
var endWires = new List<Wire>();
foreach (var wire in wires.Values)
{
    bool hasInput = false, hasOutput = false;
    foreach (var gate in gates)
        if (gate.output == wire)
            hasInput = true;
        else if (gate.inputs[0] == wire || gate.inputs[1] == wire)
            hasOutput = true;
    
    if (!hasInput) beginWires.Add(wire);
    if (!hasOutput) endWires.Add(wire);
}

// sort and position wires
var inputsProcessed = new List<Wire>();
foreach (var wire in beginWires)
{
    if (!inputsProcessed.Contains(wire))
    {
        var offset = int.Parse(wire.name[1..]);
        foreach (var gate in gates)
        {
            if (gate.inputs[0] == wire || gate.inputs[1] == wire)
            {
                var inputWires = gate.inputs.OrderBy(x => x.name).ToArray();
                inputWires[0].position = new Coord(0, 1, offset);
                inputWires[1].position = new Coord(1, 3, offset);

                if (gate.inputs[0] == wire)
                    inputsProcessed.Add(gate.inputs[1]);
                else
                    inputsProcessed.Add(gate.inputs[0]);

                if (gate.op.Equals("XOR"))
                {
                    gate.position = new Coord(1, 1, offset);
                    gate.output.position = new Coord(3, 1, offset);
                }
                else
                {
                    gate.position = new Coord(2, 2, offset);
                    gate.output.position = new Coord(4, 2, offset);
                }

                // didn't expect a direct connection to an end wire here
                if (offset > 0 && endWires.Contains(gate.output))
                    gate.suspicious = true;

                foreach (var gate2 in gates)
                {
                    if (gate2.inputs[0] == gate.output || gate2.inputs[1] == gate.output)
                    {
                        if (gate2.op.Equals("AND"))
                        {
                            gate2.position = new Coord(4, 0, offset);
                            gate2.output.position = new Coord(7, 0, offset);
                        }
                        else if (gate2.op.Equals("XOR"))
                        {
                            gate2.position = new Coord(5, 1, offset);
                            gate2.output.position = new Coord(9, 1, offset);

                            // expected different preceding operator
                            if (offset > 0 && !gate.op.Equals("XOR"))
                                gate.suspicious = true;

                            // expected connection to end wire here
                            if (!endWires.Contains(gate2.output))
                                gate2.suspicious = true;
                        }
                        else if (offset > 0 && gate2.op.Equals("OR"))
                        {
                            gate2.position = new Coord(6, 2, offset );
                            gate2.output.position = new Coord(8, 2, offset);

                            // expected different preceding operator
                            if (!gate.op.Equals("AND"))
                                gate.suspicious = true;
                        }

                        // expected different outgoing operator to end wire
                        if (offset < 44 && endWires.Contains(gate2.output) && !gate2.op.Equals("XOR"))
                            gate2.suspicious = true;
                    }
                }
            }
        }
    }
}

foreach (var wire in wires.Values)
    if (wire.position == null)
        Console.WriteLine($"Wire not positioned: {wire.name}");

foreach (var gate in gates)
    if (gate.position == null)
        Console.WriteLine($"Gate not positioned: {gate}");

var answer = string.Empty;
foreach (var gate in gates.OrderBy(x => x.output.name))
    if (gate.suspicious)
        answer += $"{gate.output.name},";

Console.WriteLine(answer[..^1]);


// -- WIP BLOCK

var x = produceNumberFor("x");
var y = produceNumberFor("y");

var zShouldBe = x + y;
var zShouldBeBinary = longToBinary(zShouldBe);

// start processing
simulateGates();            /// WOULD BE ABSOLUTELY SUPER COOL TO ACTUALLY ANIMATE THE FLOW OF TRUE AND FALSE STATES GOING THROUGH THE MACHINE!!!

var zActuallyIs = produceNumberFor("z");
var zActuallyIsBinary = longToBinary(zActuallyIs);

Console.WriteLine(zShouldBeBinary);
Console.WriteLine(zActuallyIsBinary);
Console.WriteLine();

// --


void addWire(string wireName)
{
    if (!wires.ContainsKey(wireName))
        wires.Add(wireName, new Wire(wireName, null));
}

void simulateGates()
{
    bool allReady;
    do
    {
        allReady = true;
        foreach (var gate in gates)
            if (!gate.Process())
                allReady = false;
    }
    while (!allReady);
}

long produceNumberFor(string wireType)
{
    var result = string.Empty;
    foreach (var wireName in wires.Keys)
        if (wireName.StartsWith(wireType))
            result = $"{(wires[wireName].value.Value ? 1 : 0)}{result}";

    return Convert.ToInt64(result, 2);
}

string longToBinary(long number) => Convert.ToString(number, 2);

class Gate(Wire in1, Wire in2, Wire output, string op) : Element
{
    public Wire[] inputs = [in1, in2];
    public Wire output = output;
    public string op = op;
    public bool ready = false;

    public bool suspicious;

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

    public override string ToString()
    {
        var opSymbol = op switch
        {
            "AND" => "&&",
            "OR" => "||",
            _ => "!="
        };

        return $"{inputs[0].name} ({(!inputs[0].value.HasValue ? "null" : inputs[0].value.Value ? 1 : 0)}) {opSymbol} {inputs[1].name} ({(!inputs[1].value.HasValue ? "null" : inputs[1].value.Value ? 1 : 0)}) -> {output.name}";
    }
}

class Wire(string name, bool? value) : Element
{
    public string name = name;
    public bool? value = value;

    public bool? initialValue;

    public void Set()
    {
        initialValue = value;
    }

    public void Reset()
    {
        value = initialValue;
    }
}

class Element
{
    public Coord position;
}

class Coord(int x, int y, int offset)
{
    public int x = x;
    public int y = y;
    public int offset = offset;
}