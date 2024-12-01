string[] lines = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\01\\input.txt");

var answer = 0;
var lists = new List<int>[2];
for (var i = 0; i < 2; i++)
    lists[i] = new List<int>();

foreach (var line in lines)
{
    var numbers = line.Split("   ");
    for (var i = 0; i < 2; i++)
        lists[i].Add(int.Parse(numbers[i]));
}

for (var i = 0; i < 2; i++)
    lists[i].Sort();

for (var i = 0; i < lists[0].Count; i++)
{
    var occurence = 0;
    for (var j = 0; j < lists[1].Count; j++)
    {
        if (lists[0][i] == lists[1][j])
            occurence++;

        if (lists[1][j] > lists[0][i])
            break;
    }

    answer += lists[0][i] * occurence;
}

Console.WriteLine(answer);