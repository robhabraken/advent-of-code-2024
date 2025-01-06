# Solutions to Day 19: Linen Layout

*For the puzzle description, see [Advent of Code 2024 - Day 19](https://adventofcode.com/2024/day/19).*

Here are my solutions to the puzzles of today. Written chronologically so you can follow both my code and line of thought.

## Part 1

This was a difficult puzzle for me, I eventually was able to solve both parts myself, but it took me quite a while. I could easily see how you would recurisvely discover all possible arrangements, but that would only work for the example input, as the real puzzle input is just way too big. I tried multiple things, and eventually landed on something of which I wouldn't think it would work, but it did - maybe given the vast amount of different towel patterns available (though it worked also on the input). What I did was create a new string of the same length as the desired design, but with only `.` characters. Then, I'd go over each available towel and place that into the string at the positions where that pattern occured in the desired design, basically just merging all towels patterns into one big string, not looking at which should go where or if towels would overlap. When finished, all designs that still contain a `.` aren't possible. I am well aware that if you for example would have a pattern of `rrgb` and a towel of `rrg` and one of `rgb` than my method would falsely claim that that design is possible, but in fact it isn't - but there are many different lengths so this situation actually never occurs. The solution for part 2 I ended up with would know the difference, but it took some time for me till the penny dropped...

## Part 2

For the second part I partly use the same code to find where the design matches, but now I have created a `List<string>[]` with the same length of the intended design, storing all towels available that would match the pattern in the respective array index, so now for each position in the intended design pattern I know which towels would be available at that position.

Then I start looping from the back, counting the amount of possibilities per towel and per position (or column if you like). Like so:
```
rrbgbr
.....r > 1
.....1
....br > 1
....b. > 1
....2
...gb. > 1
...g.. > 2
...3
..b... > 3
..3
.rb... > 3
.r.... > 3
.6
r..... > 6
6
```
So the intended pattern is `rrbgbr` and the available towels that fit in this pattern are `r`, `rb`, `b`, `g`, `gb` and `br`. To determine the number of possible arrangements, I start at the back, counting the number of possibilities for each position. At the last position, only `r` will fit, so there is only 1 possible arrangement for that position. On the 5th position, we could place either `br` or `b`. Since `br` is as long as the rest of the pattern, there is only one possibility. For `b`, we look at the column to the right, and use the sum of all possibilities of that column to determine the possible arrangment for this position. `r` has 1 possible arrangment at the sixth position, so if you will start with a `b` at the fifth position, there is only one way to complete the pattern. That makes the total count for that column `2` (`br` and `b`,`r`). At the 4th position we could place either `gb`or `g`. For `gb`, we will look at the 6th position (the position that is empty after the pattern itself), giving 1 option. For `g` we look at the 5th position, and from there we have two possibilities. So at the 4th column, we have a total of 3 possibilties. At the 3rd position, we have only one option: `b`. And that logically has the same possible arrangements as the total at the 4rd position: 3. At the 2nd position, we can place either `rb` or `r`, both given us 3 possible arrangements (starting from either the 4th or the 3rd positions, both those columns have a sum of 3), making the total possible arrangements starting at the second position 6. And for the first column, we have 1 option: `r`. That means that for the first position the total number of possible arrangement also is six. And also is the total possible number of arrangement of this entire combination.

We can do this in a single loop and we don't need recursion, that would only overcomplicate things:
```
for (var i = towels.Length - 1; i >= 0; i--)
    foreach (var towel in towels[i])
        if (i + towel.Length < design.Length)
            possibilities[i] += possibilities[i + towel.Length];
        else
            possibilities[i]++;
```
Where `i` is the position or index in the intended design pattern. And `towels[]` holds a list of possible towels for each of those positions.

### A little faster
Retrospecitively, changed a few lines of code, as I spotted that I was calling the same `indexOf()` function multiple times, which is more expensive that assigning the outcome to a local variable. So I now changed these lines:
```
indexes.Add(design.IndexOf(pattern, lastIndex));
towels[design.IndexOf(pattern, lastIndex)].Add(pattern);
lastIndex = design.IndexOf(pattern, lastIndex) + 1;
```
into:
```
var index = design.IndexOf(pattern, lastIndex);
indexes.Add(index);
towels[index].Add(pattern);
lastIndex = index + 1;
```
Which lowers the runtime from 24.8 to 17.7 ms. So it has quite some impact.