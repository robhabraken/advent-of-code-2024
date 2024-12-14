# Solutions to Day 13: Claw Contraption

*For the puzzle description, see [Advent of Code 2024 - Day 13](https://adventofcode.com/2024/day/13).*

Here are my solutions to the puzzles of today. Written chronologically so you can follow both my code and line of thought.

## Part 1

I knew this was going to require math, and brute-forcing to find the answer would probably only work for part one, but I wanted to collect the first star as soon as possible and also have reliable test data for part two (which I actually ended up using, running my part two code against the part one puzzle to test if the new approach was valid, before scaling it up to the new dimensions).

Parsing is a bit more complicated today, but with a few tricks, it's not that hard. First, I read the input per four lines, so I don't have to build switches and / or group data. Secondly, the location of the `X`-character is at the same index for each line type, so we can just grab a substring of the first, second and third line for each group of four lines. Then I replace the `Y`-character with the `+` or `=` sign with an empty string, but not the `,`, so I can split the input string on the `,` character and parse all values of the list into integer values and return them as an array.

Then, I create a list of outcomes using a `Tuple<int, int>` list, effectively storing the location of the claw for each number of presses, using two separate lists for the two individual buttons.

Now I iterate over the 100 presses (or actually 101, because not pressing one of the buttons also is a valid solution, which it turned out to be for at least one of the claw machines in my input), calculate the position of the claw for each button for that number of presses, and store the outcome in the respective lists.

Lastly, I iterate over the outcome lists in a nested loop, so I test every number of pushes on the `B` button with every number of pushes on the `A` button. When both coordinates add up to the coordinate of the prize, I add the cost to the answer. I could've stepped out of the loop once I found an answer, but it already was this quick that I didn't bother, and continued with part two.

## Part 2

As expected, looping to find the answer isn't going to work for part two, as the prize coordinates are way too high and the possible combinations almost infinite. Also, since not every claw machine has a possible solution, there's no way to bail out once you've tried 'all' possible combinations. Math is our only resort. We need to actually calculate the values, which will be very quick and effective. It took me quite a while though to come up with the right equations. I tried finding a ratio between the button pushes first, but I then realized this isn't a linear formula (the ratio between the number of presses on both buttons), so that's not possible. Then I also messed up the formula simplification a few times so I was working on what I thought was the right solution, but couldn't get the outcome I expected. After a few attemps, I started over from scratch, which made me see the right equation, and from there, it actually was quite an easy puzzle to solve.

We'll start with creating two formulas, where `a` is the number of times button `A` is pressed and `b` is the number of times button `B` is pressed. The numbers `0` and `1` stand for the x- and y-coordinates of the buttons and the prize and `p` for prize, because that nicely matches the index within my arrays of the respective coordinates. So for the x-axis, we need to press button `A` `a` times and button `B` `b` times to retrieve our prices `p`, all for coordinate `0`. And the same goes for the second coordinate (`1`). Or in a formula, this would be:
```
aA0 + bB0 = p0
aA1 + bB1 = p1
```
Because we have two formulas and two variables, we're able to solve these formulas, so first we isolate `b` based on the first formula (subtract `aA0` on botth sides and divide by `B0` frees up the `b` which is the number of times `B` is pressed):
```
b = (p0 - aA0) / B0
```
Then we insert this into the second formula to replace the `b` value:
```
aA1 + B1 * ((p0 - aA0) / B0) = p1
```
Now we have a formula with only one variable (`a`) and all other values are known for each claw machine. Then we need to simplify this formula and collect all therms with `a` on one side and factor out `a`: *(I needed to look up the math rules for this, it's been a while...)*
```
aA1 + (B1 * ((p0 - aA0) / B0)) = p1
aA1 + ((B1 * p0 - B1 * aA0) / B0) = p1
aA1 - ((B1 * aA0) / B0) = p1 - ((B1 * p0) / B0)
a * (A1 - (B1 * A0 / B0)) = p1 - ((B1 * p0) / B0)
a = (p1 - (B1 * p0 / B0)) / (A1 - (B1 * A0 / B0))
```
Or in code, that would be:
```
var a = (prize[1] - (B[1] * prize[0] / B[0])) / (A[1] - (B[1] * A[0] / B[0]));
```
Now that we know the value of `a` we can use the first formula again, where we factored out `b`, to calculate the value of `b`:
```
var b = (prize[0] - (a * A[0])) / B[0];
```
Note that I changed the type from `int` to `double` when parsing the input, to allow for exact numbers in these divisions.

Now that we have two values we can see if winning a prize is possible: buttons can only be pushed a 'round' number of times, but our formula might result in a negative value, or a value with multiple decimal places. That simply means there is no solution to this, because you cannot push button `A` 40.124 times and button `B` 147.8105 times. So if that's what's needed to get to the prize, we skip this claw machine as winning a prize isn't possible here.

If the two numbers are round and positive, we multiply `a` by 3 and add `b` to calculate the cost of this game and add that to our answer.

There's one more thing: the above formula might result in a value of `40.000000000001`. That's just a rounding issue or in other words, a floating-point error, and the best approximate answer the computer can get when storing exact numbers with a limited number of decimals. To be able to dinstiguish such a value from one that is actually wrong, I compare the rounded number with the number rounded to 2 decimal places. If that produces the same value, I assume that this is a round number, round it off and use it to collect my prize!

It's amazing how fast this solution is, which proves this probably is the 'intended', or at least one of the best solutions to this puzzle I think. My code finds the answer to part two in an impressive 0.3873 ms. And it's a lot shorter than the code for part one as well.