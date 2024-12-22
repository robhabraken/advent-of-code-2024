var secrets = File.ReadAllLines("..\\..\\..\\..\\..\\..\\..\\advent-of-code-2024-io\\22\\input.txt").Select(long.Parse).ToArray();

var answer = 0L;
for (var i = 0; i < secrets.Length; i++)
{
    for (var j = 0; j < 2000; j++)
        secrets[i] = pseudo(secrets[i]);
    answer += secrets[i];
}

Console.WriteLine(answer);

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