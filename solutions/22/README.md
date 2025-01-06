# Solutions to Day 22: Monkey Market

*For the puzzle description, see [Advent of Code 2024 - Day 22](https://adventofcode.com/2024/day/22).*

Here are my solutions to the puzzles of today. Written chronologically so you can follow both my code and line of thought.

## Part 1

Today was a breeze compared to yesterday. The first part really wasn't anything else than just careful reading and doing what it said. I've created a `mix()` and `prune()` function that did the bitwise XOR and modulo operation as instructed, and created a `pseudo()` function to do the three calculations followed by a mix and a prune, to cycle the secret to its next value. Then it was just a matter of looping over the input and for each secret looping over the `pseudo()` function 2000 times.

## Part 2

The second part was a bit more challenging. We need to find any sequence of four changes that would buy us the most amount of bananas over all the secrets in the input. The trick here is that a sequence can occur multiple times for a single secret, but we may only use the value of the first occurence (as that would be the sell), and the fact that the best sequence in total could not occur at all for some secrets. That's a challenging factor because you cannot simply define the best sequence within the range of changes within one secret.

I first built a few small tests to see if I could reproduce the given example, so I knew my method and understanding was correct. And then I quickly came up with an idea on how to do this in a very simple way. It may not be the fastest, but it's the least amount of code, and quick enough for me to be acceptable. What I do is the following:
- I've added an `int[]` with a length of 2000 to hold all `changes` (the difference between each consecutive price).
- I then store each sequence of four changes in a `List<string>` *per secret* named `occurrences`, in a string notation, exactly as in the example `"-2,1,-1,3"`, but only if that sequence isn't already in that list for the current secret. This way, I know when each sequence of four changes occurs for the first time, so I can ignore occurences that are already in my list and aren't valid to use anyway.
- Lastly, I've added a dictionary named `bananas` of the type `Dictionary<string, int>` that holds each unique sequence as a string notation as its key, and the corresponding amount of bananas that would get you as the value. For each sequence that occurs for the first time *over all secrets* I add the amount of bananas (or price) to that dictionary (of the current secret), and for every occurence after that over all other secrets I increase that value with the price for the respective secret.

So instead of actually searching for a common best sequence, I simply store the amount of bananas for each unqiue sequence over all secrets. Now all I have to do is get the maximum value out of the dictionary and I have my answer!

### Speeding up the market
After submitting my answer, I came up with a huge performance improvement: instead of using the string representation of the sequence as the key for that sequence (like `"-2,1,-1,3"`), I switched to an integer value, as string concatenation and string comparison is very slow. A trick I used more often last year was to store multiple integer values in one single value by multiplying the individual values by their own multitude of 10 each (or in other words bitshifting them to more significant bits). I initially didn't think this was possible as we also have negative numbers - the range of differences between the prices goes from `-9` tot `9`. But since we don't need to retrieve (or decode) the numbers from our key, that actually doesn't matter - it will produce a unique integer value nonetheless and that's all we need! The only thing we need to do to make this work is use factors of 100 instead of 10, to ensure enough separation between the different values.

So instead of:
```
var sequence = $"{changes[j - 3]},{changes[j - 2]},{changes[j - 1]},{changes[j]}"
```
I now produce a unique key for a sequence like this:
```
var sequence = changes[j - 3] * 1000000 + changes[j - 2] * 10000 + changes[j - 1] * 100 + changes[j];
```
And of course I needed to change my `occurrences` collection to a `List<int>`, and the `bananas` collection to a `Dictionary<int, int>`. This small change drops the runtime of part 2 from 13 seconds to 441 ms!

### A little bit faster
When optimizing day 9 I learned about a new way to parse a string into an integer or long value, so I decided to try this too for day 22. I have added my own `parseLong()` function and replace the original `long.Parse` with that:
```
long parseLong(string s)
{
    var result = 0L;
    for (var i = 0; i < s.Length; i++)
        result = result * 10 + (s[i] - '0');
    return result;
}
```
Surprisingly, this shaves off another 40 ms, bringing the runtime down to 400 ms.

### Even faster trading!
Still, I needed a bit more speed to achieve my subsecond goal over all part 2 solutions of 2024 collectively. So I continued experimenting with this solution, as it was the slowest of all, so potentially the also the most to be gained.

My major improvement as described above, generating an integer value as a key of each unique sequence, still could be a little faster. As I was multiplying all values by factors of 10, but my range of changes is only 18 long (-9 to 9), so I decided to try changing this to factors of 18, so the integer value to work with would be way lower:
```
var sequence = changes[j - 3] * 5832 + changes[j - 2] * 324 + changes[j - 1] * 18 + changes[j];
```
I expected this to make a little bit of a difference, but surprisingly, it saved me 20 ms on average!

Now I had a taste for it, so I didn't stop there. I thought of changing the divisions and multiplications to bitshifting operations. Since the given operations are working with powers of 2, it's quite easy to do so:
- multiplying by 64 (= 2^6) can be achieved by shifting 6 bits to the left: `<< 6`.
- dividing by 32 (= 2^5) can be achieved by shifting 5 bits to the right: `>> 5`.
- and then multiplying by 2048 (= 2^11) can be achieved by shifting 11 bits to the left again: `<< 11`.

This gained another few ms, but not all that much. But then, changing the modulo operation to a bitwise operation did improve the runtim much more. We have to take the module of `16777216`, which might *seem* to be an odd number, but it really isn't. At first I really didn't pay attention and just used the common modulo approach, as it was the easiest thing to do and also fast enough for me. But, `16777216` is actually the equivalent of `2^24` and thus also a power of 2. A modulo operation is the remainder of a division by a certain number, and using this specific number (`16777216`) actually isolates the lower `24` for bits of given `value`. The bitwise equivalent of this is `value & (2^n - 1)`, because isolating bits can also be achieved by a bitwise `AND` operator: you can supply the bits that you want to keep, since each bit has to be true on both sides of the opreator. And since in `2^n` the `n` for us is `24`, making `2^24 = 16777216`, and `2^n - 1` thus is `16777216 - 1 = 16777215`, which is the number we can use for the bitwise operation. And that latter number can be then also simplified to the hexadecimal number `0xFFFFFF` (of which the binary representation is `111111111111111111111111` (the 24 bits we'd like to keep)). Long story short, instead of `secret % 16777216` we can also do `secret & 0xFFFFFF`, which is *way* faster!

So, adding up these new changes, a lower factor for the integer key generation and using bitwise operators only, I got my solution down from 400 ms to 362 ms! That's 10% off again, and was the last improvement I needed to achieve my subsecond goal over all solutions!

