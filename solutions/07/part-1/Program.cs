string[] lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\07\\input.txt");

var combinations = new List<List<string>>();
for (var operatorCount = 0; operatorCount < 12; operatorCount++)
{
    combinations.Add([]);
    for (var i = 0; i < (int)Math.Pow(2, operatorCount) && operatorCount > 0; i++)
        combinations[operatorCount].Add(Convert.ToString(i, 2).PadLeft(operatorCount, '0').Replace('0', '*').Replace('1', '+'));
}

long answer = 0;
foreach (var line in lines)
{
    var equation = line.Split(':');

    var testValue = long.Parse(equation[0]);
    var numbers = equation[1].Trim().Split(' ').Select(long.Parse).ToArray();

    foreach (var operators in combinations[numbers.Length - 1])
    {
        long result = numbers[0];
        for (var i = 0; i < operators.Length; i++)
        {
            if (operators[i].Equals('+'))
                result += numbers[i + 1];
            else
                result *= numbers[i + 1];
        }
        if (result == testValue)
        {
            answer += result;
            break;
        }
    }
}

Console.WriteLine(answer);
