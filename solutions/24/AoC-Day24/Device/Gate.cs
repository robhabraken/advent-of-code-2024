using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AoC_Day24.Visualization;

namespace AoC_Day24.Device
{
    public class Gate(Wire in1, Wire in2, Wire output, string op) : Element
    {
        public Wire[] inputs = [in1, in2];
        public Wire output = output;
        public string op = op;
        public bool ready = false;

        private bool suspicious;

        public bool Suspicious
        {
            get
            {
                return suspicious;
            }
            set
            {
                suspicious = value;
                output.suspicious = value;
            }
        }

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
}