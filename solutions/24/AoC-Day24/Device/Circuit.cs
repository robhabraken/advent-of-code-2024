using AoC_Day24.Visualization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace AoC_Day24.Device
{
    public class Circuit
    {
        public SortedDictionary<string, Wire> wires;
        public List<Gate> gates;

        public Circuit()
        {
            wires = [];
            gates = [];
        }

        public void Import()
        {
            var lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\24\\input.txt");

            // read all wires and gates
            foreach (var line in lines)
            {
                if (line.Contains("->"))
                {
                    var elements = line.Split(' ');
                    AddWire(elements[0]);
                    AddWire(elements[2]);
                    AddWire(elements[4]);

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
                            inputWires[0].position = new Coordinate(0, 1, offset);
                            inputWires[1].position = new Coordinate(1, 3, offset);

                            if (gate.inputs[0] == wire)
                                inputsProcessed.Add(gate.inputs[1]);
                            else
                                inputsProcessed.Add(gate.inputs[0]);

                            if (gate.op.Equals("XOR"))
                            {
                                gate.position = new Coordinate(1, 1, offset);
                                gate.output.position = new Coordinate(3, 1, offset);
                            }
                            else
                            {
                                gate.position = new Coordinate(2, 2, offset);
                                gate.output.position = new Coordinate(4, 2, offset);
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
                                        gate2.position = new Coordinate(4, 0, offset);
                                        gate2.output.position = new Coordinate(7, 0, offset);
                                    }
                                    else if (gate2.op.Equals("XOR"))
                                    {
                                        gate2.position = new Coordinate(5, 1, offset);
                                        gate2.output.position = new Coordinate(9, 1, offset);

                                        // expected different preceding operator
                                        if (offset > 0 && !gate.op.Equals("XOR"))
                                            gate.suspicious = true;

                                        // expected connection to end wire here
                                        if (!endWires.Contains(gate2.output))
                                            gate2.suspicious = true;
                                    }
                                    else if (offset > 0 && gate2.op.Equals("OR"))
                                    {
                                        gate2.position = new Coordinate(6, 2, offset);
                                        gate2.output.position = new Coordinate(8, 2, offset);

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
        }

        private void AddWire(string wireName)
        {
            if (!wires.ContainsKey(wireName))
                wires.Add(wireName, new Wire(wireName, null));
        }

        public void SimulateGates()
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

        private long ProduceNumberFor(string wireType)
        {
            var result = string.Empty;
            foreach (var wireName in wires.Keys)
                if (wireName.StartsWith(wireType))
                    result = $"{(wires[wireName].value.Value ? 1 : 0)}{result}";

            return Convert.ToInt64(result, 2);
        }

        private string LongToBinary(long number) => Convert.ToString(number, 2);
    }
}


