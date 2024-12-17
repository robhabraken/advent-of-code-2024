var lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\17\\input.txt");

var register = new int[3];
register[0] = int.Parse(lines[0].Replace("Register A: ", string.Empty));
register[1] = int.Parse(lines[1].Replace("Register B: ", string.Empty));
register[2] = int.Parse(lines[2].Replace("Register C: ", string.Empty));

var program = lines[4].Replace("Program: ", string.Empty).Split(',').Select(int.Parse).ToArray();

var pointer = 0;
var output = string.Empty;

do pointer = executeInstruction(pointer, program[pointer], program[pointer + 1]);
while (pointer < program.Length);

Console.WriteLine(output[..^1]);

int executeInstruction(int pointer, int opcode, int operand)
{
    return opcode switch
    {
        0 => adv(pointer, operand),
        1 => bxl(pointer, operand),
        2 => bst(pointer, operand),
        3 => jnz(pointer, operand),
        4 => bxc(pointer, operand),
        5 => ovt(pointer, operand),
        6 => bdv(pointer, operand),
        7 => cdv(pointer, operand),
        _ => -1,
    };
}

int adv(int pointer, int comboOperand)
{
    if (comboOperand <= 3)
        register[0] /= (int)Math.Pow(2, comboOperand);
    else
        register[0] /= (int)Math.Pow(2, register[comboOperand - 4]);
    return pointer + 2;
}

int bxl(int pointer, int literalOperand)
{
    register[1] = register[1] ^ literalOperand;
    return pointer + 2;
}
int bst(int pointer, int comboOperand)
{
    if (comboOperand <= 3)
        register[1] = comboOperand % 8;
    else
        register[1] = register[comboOperand - 4] % 8;
    return pointer + 2;
}

int jnz(int pointer, int literalOperand)
{
    if (register[0] == 0)
        return pointer + 2;
    return literalOperand;
}

int bxc(int pointer, int ignoredOperand)
{
    register[1] = register[1] ^ register[2];
    return pointer + 2;
}

int ovt(int pointer, int comboOperand)
{
    if (comboOperand <= 3)
        output += $"{comboOperand % 8},";
    else
        output += $"{register[comboOperand - 4] % 8},";
    return pointer + 2;
}

int bdv(int pointer, int comboOperand)
{
    if (comboOperand <= 3)
        register[1] = register[0] / (int)Math.Pow(2, comboOperand);
    else
        register[1] = register[0] / (int)Math.Pow(2, register[comboOperand - 4]);
    return pointer + 2;
}

int cdv(int pointer, int comboOperand)
{
    if (comboOperand <= 3)
        register[2] = register[0] / (int)Math.Pow(2, comboOperand);
    else
        register[2] = register[0] / (int)Math.Pow(2, register[comboOperand - 4]);
    return pointer + 2;
}