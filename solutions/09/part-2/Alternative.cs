var diskmap = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\09\\input.txt")[0];

var files = new List<AmphipodFile>();
for (var i = 0; i < diskmap.Length; i++)
{
    if (i % 2 == 0)
        files.Add(new AmphipodFile() { id = i / 2, size = int.Parse($"{diskmap[i]}") });
    else
        files.Add(new AmphipodFile() { id = -1, size = int.Parse($"{diskmap[i]}") });
}

for (var i = files.Count - 1; i >= 0; i--)
{
    if (files[i].id >= 0)
    {
        for (var j = 0; j < i; j++)
        {
            if (files[j].id == -1 && files[j].size >= files[i].size)
            {
                var delta = files[j].size - files[i].size;

                files[j].id = files[i].id;
                files[j].size = files[i].size;

                files[i].id = -1;

                if (delta > 0)
                    files.Insert(j + 1, new AmphipodFile() { id = -1, size = delta });

                break;
            }
        }
    }
}

long checksum = 0;
for (int i = 0, index = 0; i < files.Count; i++)
{
    for (var j = 0; j < files[i].size; j++, index++)
        if (files[i].id >= 0)
            checksum += files[i].id * index;
}

Console.WriteLine(checksum);

class AmphipodFile
{
    public int id;
    public int size;
}