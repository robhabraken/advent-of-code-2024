var secrets = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\22\\input.txt").Select(parseLong).ToArray();
var bananas = new Dictionary<int, int>();

for (var i = 0; i < secrets.Length; i++)
{
    var changes = new int[2000];
    var occurrences = new List<int>();
    for (var j = 0; j < 2000; j++)
    {
        var newSecret = pseudo(secrets[i]);
        changes[j] = price(newSecret) - price(secrets[i]);
        secrets[i] = newSecret;

        if (j >= 3)
        {
            var sequence = changes[j - 3] * 5832 + changes[j - 2] * 324 + changes[j - 1] * 18 + changes[j];
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

long parseLong(string s)
{
    var result = 0L;
    for (var i = 0; i < s.Length; i++)
        result = result * 10 + (s[i] - '0');
    return result;
}

int price(long secret) => (int)secret % 10;

long pseudo(long secret)
{
    secret = mix(secret, secret << 6);
    secret = prune(secret);

    secret = mix(secret, secret >> 5);
    secret = prune(secret);

    secret = mix(secret, secret << 11);
    secret = prune(secret);

    return secret;
}

long mix(long secret, long value) => secret ^ value;

long prune(long secret) => secret & 0xFFFFFF;