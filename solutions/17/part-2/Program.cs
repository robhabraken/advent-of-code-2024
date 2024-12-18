var lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\17\\input.txt");

var output = string.Empty;

var register = new long[3];
var programString = lines[4].Replace("Program: ", string.Empty);
var program = programString.Split(',').Select(int.Parse).ToArray();

var answer = 0L;
for (var i = program.Length - 1; i >= 0; i--)
{
    var increment = (long)Math.Pow(8, i);
    var incrementCounter = 0;
    var target = program[i];
    while (string.IsNullOrEmpty(output) || !output[(i * 2)..^1].Equals(programString[(i * 2)..]))
    {
        incrementCounter++;
        register[0] = answer + increment * incrementCounter; ;
        output = string.Empty;

        var pointer = 0;
        do pointer = executeInstruction(pointer, program[pointer], program[pointer + 1]);
        while (pointer < program.Length);
    }
    answer += increment * incrementCounter;
}

Console.WriteLine(answer);

int executeInstruction(int pointer, int opcode, int operand)
{
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
        register[0] /= (long)Math.Pow(2, comboOperand);
    else
        register[0] /= (long)Math.Pow(2, register[comboOperand - 4]);

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
    var outputValue = 0L;
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