using AoC_Day24.Visualization;

namespace AoC_Day24.Device
{
    public class Gate(Wire in1, Wire in2, Wire output, string op) : Element
    {
        public Wire[] inputs = [in1, in2];
        public Wire output = output;
        public string op = op;
        public bool ready = false;

        public bool suspicious;

        public void MarkSuspicious(int group)
        {
            suspicious = true;
            output.suspicious = true;
            output.group = group;
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

        public void Reset()
        {
            ready = false;
            suspicious = false;
        }
    }
}