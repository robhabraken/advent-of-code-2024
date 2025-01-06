var lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\07\\input.txt");

var ops = new char[] { '+', '|', '*' };

var answer = 0L;
foreach (var line in lines)
{
    var equation = line.Split(':');
    var testValue = parseLong(equation[0]);
    var numbers = equation[1].Trim().Split(' ').Select(parseLong).ToArray();

    var possiblyTrue = false;
    evaluate(testValue, numbers[0], numbers, 1, ref possiblyTrue);

    if (possiblyTrue)
        answer += testValue;
}

Console.WriteLine(answer);

void evaluate(long testValue, long current, long[] numbers, int index, ref bool possiblyTrue)
{
    if (!possiblyTrue)
        foreach (var op in ops)
        {
            var result = current + numbers[index];
            if (op.Equals('|'))
                result = (long)Math.Pow(10, (int)Math.Log10(numbers[index]) + 1) * current + numbers[index];
            else if (op.Equals('*'))
                result = current * numbers[index];

            if (index == numbers.Length - 1 && result == testValue)
                possiblyTrue = true;

            if (index < numbers.Length - 1 && result <= testValue)
                evaluate(testValue, result, numbers, index + 1, ref possiblyTrue);
        }
}

long parseLong(string s)
{
    var result = 0L;
    for (var i = 0; i < s.Length; i++)
        result = result * 10 + (s[i] - '0');
    return result;
}