string[] lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\07\\input.txt");

var combinations = new List<List<string>>();

var ops = new char[] {'+', '|', '*'};
for (var operatorCount = 1; operatorCount < 12; operatorCount++)
{
    var operatorList = new List<string>();
    foreach (var op in ops)
        operatorList.Add($"{op}");

    for (var i = 0; i < operatorCount - 1; i++)
    {
        var newList = new List<string>();
        foreach (var product in operatorList)
            foreach (var op in ops)
                newList.Add($"{product}{op}");
        operatorList = newList;
    }

    combinations.Add(operatorList);
}

long answer = 0;
foreach (var line in lines)
{
    var equation = line.Split(':');

    var testValue = long.Parse(equation[0]);
    var numbers = equation[1].Trim().Split(' ').Select(long.Parse).ToArray();

    foreach (var operators in combinations[numbers.Length - 2])
    {
        long result = numbers[0];
        for (var i = 0; i < operators.Length; i++)
        {
            if (operators[i].Equals('+'))
                result += numbers[i + 1];
            else if (operators[i].Equals('|'))
                result = long.Parse($"{result}{numbers[i + 1]}");
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
