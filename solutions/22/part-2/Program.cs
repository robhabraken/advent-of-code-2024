var secrets = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\22\\input.txt").Select(long.Parse).ToArray();

var bananas = new Dictionary<string, int>();
for (var i = 0; i < secrets.Length; i++)
{
    var changes = new int[2000];
    var occurrences = new List<string>();
    for (var j = 0; j < 2000; j++)
    {
        var newSecret = pseudo(secrets[i]);
        changes[j] = price(newSecret) - price(secrets[i]);
        secrets[i] = newSecret;

        if (j >= 3)
        {
            var sequence = $"{changes[j - 3]},{changes[j - 2]},{changes[j - 1]},{changes[j]}";
            if (!occurrences.Contains(sequence))
            {
                if (bananas.ContainsKey(sequence))
                    bananas[sequence] += price(secrets[i]);
                else
                    bananas.Add(sequence, price(secrets[i]));
                occurrences.Add(sequence);
            }
        }
    }
}

Console.WriteLine(bananas.Values.Max());

int price(long secret) => (int)secret % 10;

long pseudo(long secret)
{
    secret = mix(secret, secret * 64);
    secret = prune(secret);

    secret = mix(secret, secret / 32);
    secret = prune(secret);

    secret = mix(secret, secret * 2048);
    secret = prune(secret);

    return secret;
}

long mix(long secret, long value) => secret ^ value;

long prune(long secret) => secret % 16777216;