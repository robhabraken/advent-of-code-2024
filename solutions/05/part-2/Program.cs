string[] lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\05\\input.txt");

var rules = new Dictionary<int, List<int>>();

var answer = 0;
foreach (var line in lines)
    if (line.Contains('|'))
        AddRule(line.Split('|').Select(int.Parse).ToArray());
    else if (!string.IsNullOrEmpty(line))
        CheckUpdate(line);

Console.WriteLine(answer);

void AddRule(int[] numbers)
{
    if (!rules.ContainsKey(numbers[0]))
        rules.Add(numbers[0], []);

    rules[numbers[0]].Add(numbers[1]);
}

void CheckUpdate(string numbers)
{
    var list = numbers.Split(',').Select(int.Parse).ToList();
    list.Sort(CompareNumbers);
    if (!numbers.Equals(string.Join(",", list)))
        answer += list[list.Count / 2];
}

int CompareNumbers(int a, int b)
{
    if (rules.TryGetValue(a, out List<int>? value) && value.Contains(b))
        return -1;

    return 1;
}