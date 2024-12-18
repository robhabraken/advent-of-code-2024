var lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\17\\input.txt");

var register = new long[3];
var programString = lines[4].Replace("Program: ", string.Empty);
var program = programString.Split(',').Select(int.Parse).ToArray();

var output = string.Empty;

long answer = 0;
for (var i = program.Length - 1; i >= 0; i--)
{
    var increment = (long)Math.Pow(8, i);
    var incrementCounter = 0;

    while (string.IsNullOrEmpty(output) || !output[(i * 2)..^1].Equals(programString[(i * 2)..]))
    {
        register[0] = answer + increment * ++incrementCounter;

        var pointer = 0;
        output = string.Empty;

        do pointer = executeInstruction(pointer, program[pointer], program[pointer + 1]);
        while (pointer < program.Length);
    }
    answer += increment * incrementCounter;
}

Console.WriteLine(answer);

int executeInstruction(int pointer, int opcode, int operand)
{
    return opcode switch
    {
        0 => adv(pointer, combine(operand)),
        1 => bxl(pointer, operand),
        2 => bst(pointer, combine(operand)),
        3 => jnz(pointer, operand),
        4 => bxc(pointer),
        5 => ovt(pointer, combine(operand)),
        6 => bdv(pointer, combine(operand)),
        7 => cdv(pointer, combine(operand)),
        _ => -1,
    };
}

long combine(int literalOperand)
{
    var comboOperand = (long)literalOperand;
    if (literalOperand > 3)
        comboOperand = register[comboOperand - 4];
    return comboOperand;
}

int adv(int pointer, long comboOperand)
{
    register[0] /= (int)Math.Pow(2, comboOperand);
    return pointer + 2;
}

int bxl(int pointer, int literalOperand)
{
    register[1] = register[1] ^ literalOperand;
    return pointer + 2;
}
int bst(int pointer, long comboOperand)
{
    register[1] = comboOperand % 8;
    return pointer + 2;
}

int jnz(int pointer, int literalOperand)
{
    if (register[0] == 0)
        return pointer + 2;
    return literalOperand;
}

int bxc(int pointer)
{
    register[1] = register[1] ^ register[2];
    return pointer + 2;
}

int ovt(int pointer, long comboOperand)
{
    output += $"{comboOperand % 8},";
    return pointer + 2;
}

int bdv(int pointer, long comboOperand)
{
    register[1] = register[0] / (int)Math.Pow(2, comboOperand);
    return pointer + 2;
}

int cdv(int pointer, long comboOperand)
{
    register[2] = register[0] / (int)Math.Pow(2, comboOperand);
    return pointer + 2;
}