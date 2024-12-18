# Solutions to Day 17: Chronospatial Computer

*For the puzzle description, see [Advent of Code 2024 - Day 17](https://adventofcode.com/2024/day/17).*

Here are my solutions to the puzzles of today. Written chronologically so you can follow both my code and line of thought.

## Part 1

This was a bit of a read! Lots of very specific instructions, but nothing crazy difficult. It's pretty much just doing exactly what it says to get the answer. It's literally a computer program written out in words. So I started with creating a function for each instruction first, carefully reading to understand what it should do. Note that I named the function for the instruction `out` differently to omit using a reserved word, so my corresponding function is named `ovt()`. Instead of naming the registers `A`, `B` and `C` I created an `int[]` of length `3` to store the registers in, as this is less code and also easier for the combo operand (since `4` represents `A`, `5` represents `B` and `6` represents `C`, operands of the combo type, if higher than 3, indicate the index of my array by calculating `operand - 4`). I also made a `combine()` function to implement the combo operand logic to avoid duplicating that in each operation implementation.

Now that I have the operations I can create an `executeInstruction()` function that switches to the right instruction based on the given `opcode`. And lastly, the heart of the program is:
```
do pointer = executeInstruction(pointer, program[pointer], program[pointer + 1]);
while (pointer < program.Length);
```
With the pointer initially set to `0`, this will start at the beginning of the program (first opcode), read the integer after that as its operand, executes the instruction, and returns the new pointer. This is repeated in a do/while loop until the pointer goes past the end of the program. The output is stored in the eponymous variable and that's all there is too it. For convenience I always write the output with a trailing comma, so I don't need if-statements, and upon writing the output to the console, I just truncate the last character.

## Part 2

Then things got interesting. I right away knew the number we're looking for is going to be very big, so there has to be a pattern. I did a few checks and quickly learned that I needed to change some `int` variables and functions to handle numbers of the type `long` to cope with the size of the numbers we need to work with. So I changed the `combine()` function and all inputs that use that operand type, as well as the register array. Then I started hunting for a pattern, and just wrote a loop over my initial program that starts at `register A = 0`, with increments of `1`:
```
7
5
4
3
2
1
0
3,7
7,7
4,7
6,7
(..)
```
What I saw was that for the first 7 values of input into `A`, I got an output of one instruction. At `A = 8` it switched to 2 instructions in the output. At `A = 64` it switched to three. So, in other words, given it is a 3-bit computer with 8 possible numbers, unsurprisingly, we can state that:
```
0..7 produces 1 character
8..63 produces 2 characters
64..511 produces 3 characters
512..4095 produces 4 characters
(..)
```
So the first second digit appears at 8, which is the same as `8^1`. And the third digit apperas at 64, or `8^2`. The fourth at `8^3`. In other words, if we take the number of digits we want in our output and name that `instructions`, we can state that this range would then start at `8^(instructions-1)`.

Second thing that I saw was that when the second range started, the first range (`7543210`) appeared at the rightmost position, each character in that order appearing exactly 8 times (so 8 times `7`, then 8 times `5` etc.). And when we moved up to three numbers of output, that same range started appearing with 64 occurrences of each of those! So it actually functions like a decimal or binary numeric system, but reversed. Only the numbers aren't in an incremental order, but of a quite unpredictable pattern:
```
1 number output, first position: 7 5 4 3 2 1 0
2 numbers output, first position: 3 7 4 6 3 2 1 0 7 7 7 0 3 3 1 0 3 7 6 2 3 3 1 0 7 7 1 4 2 0 1 0 3 7 0 6 2 0 1 0 7 7 3 0 2 1 1 0 3 7 2 2 2 1 1 0
3 numbers output, first position: 7 7 5 4 1 6 1 1 3 7 4 6 1 6 1 1 7 7 7 0 1 7 1 1 3 7 6 2 1 7 1 1 7 7 1 4 0 4 1 1 3 7 0 6 0 4 1 1 7 7 3 0 0 5 1 1 (..)
(..)
```
So, we cannot predict those numbers (or at least I didn't see how), but we know exactly when a number changes:
- We have an output of 16 numbers, so the first occurrence of 16 numbers in the output would be at `8^(16-1) = 8^15 = 35,184,372,088,832`.
- Then we will have a `7` at the last position for the next `8^15 = 35,184,372,088,832` numbers of input into the `A` register.
- Then we will have a `5` at the last position (still following the range I first got with only one digit output) for another `8^15`.
- And that goes up all the way until we reach `8^16` (which is actually 8 times `8^15` so we've gone through our entire 3-bit range), which will then produce 17 numbers of output; but that's beyond the number we're looking for, we need 16 numbers output.
- In my case, the last number of the `program` I got was a `0`. So, I now know that that will start appearing at `7 * 8^15 = 246,290,604,621,824` (because the `0` is at the 7th position in our `7543210` range).
- Then, we are going to shift one position inward (to the left), because we have our last digit. Now we are looking at increments of `8^14` (for the 15th position).
- My input program wants a `3` at that position. If you look at the list, you can see it starts right away with a `3` (the second range shown above). So actually that's at `7 * 8^15` already (or in other words `7 * 8^15 + 0 * 8^14`). Without any increments. The next number in that list (a `7`) will appear at `7 * 8^15 + 1 * 8^14` though.
- Now there's something interesting, the first range has all unique numbers, so we *know* where we have to be. The second range though, contains multiple duplicates and our number (`3`) appears at an increment of `0 * 8^14`, `4 * 8^14`, `12 * 8^14`, `13 * 8^14`.. But! For the first few ranges, I can check this manually, but our data set is way to large, so we need to come up with a plan to test any of those and compare it with the number we're looking for. And that way is quite simple: increase our input number with increments of `8^14` until we've found our number. The first one is the one we're going to continue with, because they want *the first occurrence* so that's most likely, though not necessarily the one we're looking for. But let's continue.
- Then, we move on to the next digit, which in my case should be a `5`. We will start incrementing our input number with `8^13` until we've found it. At the third position of this range is a `5`, so we absolutely know for sure that the first occurrence of the pattern we're looking for *must* be at a number higher than `7 * 8^15 + 0 * 8^14 + 3 * 8^13` for the input of the register `A`. And there we can see our solution and program taking shape.

The idea then is to:
- Check the desired program length and loop from that minus 1, to 0 (our index range).
- Start incrementing the input value with `8^index`.
- Run my original program code (the do/while loop over the `executeInstruction()` function) for that input.
- Check if the output *at the digit of our index* is equal to the number we're looking for.
- If so, continue with the next index (minus one), if not, do another increment of `8^index` at the same index.

And with this simple loop, we're taking giant but very well planned steps through our input values for the `A` register, in search for our value. And you would say we're there, but alas, there's one more catch...

Sometimes, when we increase the number in search for the digit at position `3`, the digit at position `4` suddenly changes too! But since we're going from back to front, we already set our input value increment for that position! So what do we do? If you would only check the output for the exact digit (index) you are trying to find, then you would get a wrong answer. First I thought I had to go back to the forth digit and increase until I find the next number there, but after a bit of trial and error, I found out that that isn't the case. If we are looking for a `0` at the third position, but the fourth position then changes to another value, than that `0` simply isn't the one we're looking for, and we have to keep increasing our increment at that third position until we found *the correct value at the third position that doesn't change the output value at the fourth position* (or any number after that)!

I did implement that by not checking if my `output[i]` matches my `program[i]` instruction (individually), but by taking a substring of both my output and the program from the current position all the way to the end of the string (because that all has to stay the same since we've already determined those values). So that's why my while loop continues as long as:
```
!output[(i * 2)..^1].Equals(programString[(i * 2)..])
```
The `i * 2` is because we need to skip the commas. So what this does is compare everything from where we are to the end of the string. And only when we found the correct number at the current position `i` that doesn't change all the rest, we've found the correct *first occurrence* of that combination at that position.

So, when we put all of that into a little bit of code, we get:
```
long answer = 0;
for (var i = program.Length - 1; i >= 0; i--)
{
    var increment = (long)Math.Pow(8, i);
    var incrementCounter = 0;

    while (string.IsNullOrEmpty(output) || !output[(i * 2)..^1].Equals(programString[(i * 2)..]))
    {
        register[0] = answer + increment * ++incrementCounter;

        var pointer = 0;
        output = string.Empty;

        do pointer = executeInstruction(pointer, program[pointer], program[pointer + 1]);
        while (pointer < program.Length);
    }
    answer += increment * incrementCounter;
}
```
Which determines the program length, then starts walking from the back to the front, determines the increment for that position, and starts adding that increment until it matches our desired output, simply testing it by running our initial program for that input value. Note that I increment the counter in the loop, and start with an `incrementCounter` of `0`, so if the value at `index` is correct right away because it is already the starting number at that index when we have set the previous input, by the nature of the condition of that while loop, it will skip that index entirely and continue with the next number automatically. When this loop is done, the value of `answer` is the value we're looking for. And this loop is super quick, it only takes `0.0028 ms` to find our answer!