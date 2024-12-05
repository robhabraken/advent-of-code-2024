string[] lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\05\\input.txt");

var rules = new List<int[]>();

var answer = 0;
foreach (var line in lines)
    if (line.Contains('|'))
        rules.Add(line.Split('|').Select(int.Parse).ToArray());
    else if (!string.IsNullOrEmpty(line))
        CheckUpdate(line.Split(',').Select(int.Parse).ToList());

Console.WriteLine(answer);

void CheckUpdate(List<int> numbers)
{
    foreach (var rule in rules)
        if (numbers.Contains(rule[0]) && numbers.Contains(rule[1]))
            if (numbers.IndexOf(rule[0]) > numbers.IndexOf(rule[1]))
                CorrectUpdate(numbers);  
}

void CorrectUpdate(List<int> numbers)
{
    numbers.Sort(CompareNumbers);
    answer += numbers[numbers.Count / 2];
}

int CompareNumbers(int a, int b)
{
    foreach (var rule in rules)
        if (rule[0] == a && rule[1] == b)
            return -1;
        else if (rule[1] == a && rule[0] == b)
            return 1;

    return 0;
}