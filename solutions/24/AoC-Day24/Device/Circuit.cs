using AoC_Day24.Visualization;
using System.IO;

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

            SortAndPositionWires();
        }

        private void AddWire(string wireName)
        {
            if (!wires.ContainsKey(wireName))
                wires.Add(wireName, new Wire(wireName, null));
        }

        public void SortAndPositionWires()
        {
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
                                gate.MarkSuspicious(offset);

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
                                            gate.MarkSuspicious(offset);

                                        // expected connection to end wire here
                                        if (!endWires.Contains(gate2.output))
                                        {
                                            gate2.MarkSuspicious(offset);
                                            foreach (var gate3 in gates)
                                            {
                                                if (gate3.inputs[0] == gate2.output || gate3.inputs[1] == gate2.output)
                                                {
                                                    gate3.position = new Coordinate(6, 2, offset);
                                                    gate3.output.position = new Coordinate(8, 2, offset);
                                                }
                                            }
                                        }
                                    }
                                    else if (offset > 0 && gate2.op.Equals("OR"))
                                    {
                                        gate2.position = new Coordinate(6, 2, offset);
                                        gate2.output.position = new Coordinate(8, 2, offset);

                                        // expected different preceding operator
                                        if (!gate.op.Equals("AND"))
                                            gate.MarkSuspicious(offset);
                                    }

                                    // expected different outgoing operator to end wire
                                    if (offset < 44 && endWires.Contains(gate2.output) && !gate2.op.Equals("XOR"))
                                        gate2.MarkSuspicious(offset);
                                }
                            }
                        }
                    }
                }
            }

            MarkInfluenceOfCrossedWires();
        }

        private void MarkInfluenceOfCrossedWires()
        {
            // first simulate the circuit to determine the values of all wires
            SimulateGates();

            // for all suspicious sets of wires, check the effect of uncrossing the wires
            foreach (var wire1 in wires.Values)
            {
                if (wire1.suspicious)
                {
                    foreach (var wire2 in wires.Values)
                    {
                        if (wire1 != wire2 && wire2.suspicious && wire1.group == wire2.group)
                        {
                            if (wire1.value.Value != wire2.value.Value)
                            {
                                WhatIf(wire1, wire2.value.Value);
                                WhatIf(wire2, wire1.value.Value);
                            }
                        }
                    }
                }
            }

            // reset back to to initial state after marking influenced wires
            foreach (var wire in wires.Values)
                wire.ResetValue();

            foreach (var gate in gates)
                gate.ready = false;
        }

        private void WhatIf(Wire wire, bool fictional)
        {
            wire.influenced = true;
            foreach (var gate in gates)
            {
                var otherWire = -1;
                if (gate.inputs[0] == wire)
                    otherWire = 1;
                else if (gate.inputs[1] == wire)
                    otherWire = 0;

                if (otherWire > -1)
                {
                    // an AND gate is only affected when the other input of that gate is true
                    // otherwise it cannot produce a 'true' output anyways
                    if (gate.op.Equals("AND") && gate.inputs[otherWire].value.Value)
                        WhatIf(gate.output, fictional && gate.inputs[otherWire].value.Value);

                    // an OR gate is only affected when the other input of that gate is not true
                    // otherwise the output is 'true' anyways
                    else if (gate.op.Equals("OR") && !gate.inputs[otherWire].value.Value)
                        WhatIf(gate.output, fictional || gate.inputs[otherWire].value.Value);

                    // a XOR gate is always affected by a chance of one of the input values
                    else if (gate.op.Equals("XOR"))
                        WhatIf(gate.output, fictional != gate.inputs[otherWire].value.Value);
                }
            }
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

        public void RepairCrossedWires()
        {
            foreach (var wire1 in wires.Values)
            {
                if (wire1.suspicious)
                {
                    foreach (var wire2 in wires.Values)
                    {
                        if (wire1 != wire2 && wire1.group == wire2.group)
                        {
                            foreach (var gate in gates)
                            {
                                if (gate.output == wire1)
                                    gate.output = wire2;
                                else if (gate.output == wire2)
                                    gate.output = wire1;
                            }
                            wire1.suspicious = false;
                            wire2.suspicious = false;
                        }
                    }
                }
            }

            foreach (var wire in wires.Values)
                wire.HardReset();

            foreach (var gate in gates)
                gate.Reset();

            SortAndPositionWires();
        }

        public long? ProduceNumberFor(string wireType)
        {
            var result = string.Empty;
            foreach (var wire in wires.Values)
                if (wire.name.StartsWith(wireType) && wire.value.HasValue)
                    result = $"{(wire.value.Value ? 1 : 0)}{result}";

            return !result.Equals(string.Empty) ? Convert.ToInt64(result, 2) : null;
        }
    }
}


