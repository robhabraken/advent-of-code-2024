string[] lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\07\\input.txt");

var ops = new char[] {'+', '|', '*'};

long answer = 0;
foreach (var line in lines)
{
    var equation = line.Split(':');
    var testValue = long.Parse(equation[0]);
    var numbers = equation[1].Trim().Split(' ').Select(long.Parse).ToArray();

    var possiblyTrue = false;
    Evaluate(testValue, numbers[0], numbers, 1, ref possiblyTrue);

    if (possiblyTrue)
        answer += testValue;
}

Console.WriteLine(answer);

void Evaluate(long testValue, long current, long[] numbers, int index, ref bool possiblyTrue)
{
    if (!possiblyTrue)
        foreach (var op in ops)
        {
            var result = current + numbers[index];
            if (op.Equals('|'))
                result = long.Parse($"{current}{numbers[index]}");
            else if (op.Equals('*'))
                result = current * numbers[index];

            if (index == numbers.Length - 1 && result == testValue)
                possiblyTrue = true;

            if (index < numbers.Length - 1 && result <= testValue)
                Evaluate(testValue, result, numbers, index + 1, ref possiblyTrue);
        }
}