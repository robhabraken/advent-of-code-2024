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
    answer += Math.Abs(lists[0][i] - lists[1][i]);

Console.WriteLine(answer);