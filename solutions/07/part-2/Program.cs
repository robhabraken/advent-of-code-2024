var lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\07\\input.txt");

var answer = 0L;
foreach (var line in lines)
{
    var equation = line.Split(':');
    var testValue = long.Parse(equation[0]);
    var numbers = equation[1].Trim().Split(' ').Select(long.Parse).ToArray();

    var possiblyTrue = false;
    evaluate(testValue, testValue, numbers, numbers.Length - 1, ref possiblyTrue);

    if (possiblyTrue)
        answer += testValue;
}

Console.WriteLine(answer);

static void evaluate(long testValue, long current, long[] numbers, int index, ref bool possiblyTrue)
{
    if (!possiblyTrue)
        for (var i = 0; i < 3; i++)
        {
            var result = current - numbers[index];
            if (i == 1)
            {
                if ($"{current}".EndsWith($"{numbers[index]}"))
                    result = (current - numbers[index]) / (long)Math.Pow(10, (int)Math.Log10(numbers[index]) + 1);
                else
                    result = -1;
            }
            else if (i == 2)
            {
                if (current % numbers[index] != 0)
                    return;

                result = current / numbers[index];
            }

            if (index == 1 && result == numbers[0])
                possiblyTrue = true;

            if (index > 1 && result >= numbers[0])
                evaluate(testValue, result, numbers, index - 1, ref possiblyTrue);
        }
}