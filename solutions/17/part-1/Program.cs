var lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\17\\input.txt");

var output = string.Empty;

var register = new int[3];
register[0] = int.Parse(lines[0].Replace("Register A: ", string.Empty));
register[1] = int.Parse(lines[1].Replace("Register B: ", string.Empty));
register[2] = int.Parse(lines[2].Replace("Register C: ", string.Empty));

var program = lines[4].Replace("Program: ", string.Empty).Split(',').Select(int.Parse).ToArray();

var pointer = 0;
do
{
    pointer = executeInstruction(pointer, program[pointer], program[pointer + 1]);
}
while (pointer < program.Length);

Console.WriteLine(output[..^1]);

int executeInstruction(int pointer, int opcode, int operand)
{
    Console.WriteLine($"{opcode}: {opcode},{operand}");
    switch (opcode)
    {
        case 0: return adv(pointer, operand);
        case 1: return bxl(pointer, operand);
        case 2: return bst(pointer, operand);
        case 3: return jnz(pointer, operand);
        case 4: return bxc(pointer, operand);
        case 5: return OUT(pointer, operand);
        case 6: return bdv(pointer, operand);
        case 7: return cdv(pointer, operand);
        default: return -1;
    }
}

int adv(int pointer, int comboOperand) //
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

int jnz(int pointer, int literalOperand) //
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

int OUT(int pointer, int comboOperand) //
{
    var outputValue = 0;
    if (comboOperand <= 3)
        outputValue = comboOperand % 8;
    else
        outputValue = register[comboOperand - 4] % 8;

    output += $"{outputValue},";

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