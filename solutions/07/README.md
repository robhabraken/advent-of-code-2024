# Solutions to Day 7: Bridge Repair

*For the puzzle description, see [Advent of Code 2024 - Day 7](https://adventofcode.com/2024/day/7).*

Here are my solutions to the puzzles of today. Written chronologically so you can follow both my code and line of thought.

## Part 1

Today wasn't that hard from a logic point of view, but it has a few traps. The first one is that the input uses lower numbers, but the actual input is using some big numbers... so when you're using a typed language like I do with C#, you cannot use integers, but have to switch to a `long` for building up the answer, since the total would overflow the `int.MaxValue` value, messing up your answer. The second trap is that you shouldn't test for the result to match the test value on each loop iteration, but only when you've used up all of the numbers. And the third trap is a bit more complex to explain: if you break out of the loop if your result is equal to or larger than the desired test value for optimization (you don't need to continue if your result will end up higher that the test value anyway), don't forget that the last number could be `1`, and multiplying by one doesn't change the result, so don't test for the result to be equal or higher when breaking out of the loop early - only test if it's higher than the desired test value already.

With those issues out of the way, it's quite straight forward. I wrote the first version without using recursion by creating a cartesian product map of all possible operator combinations for each number of operators. Than I looped over the input testing the options and as soon as I found a valid one, break out of the loop and add the test value to my answer. This gave me the right value so I continued with part 2.

*I changed my solution to be the same as the one I ended up with for part 2 after submitting my answer, only with a small change in it to revert back to only using two different operators.*

## Part 2

For part 2, I needed to rewrite my cartesian map generation, as I first used a trick to generate that list by iterating over the power of the number of operations, converting that to a binary notation. Now I needed to do a bit more looping and list operations to get the cartesian product of three values times the number of operations.

The logic worked equally well, but wasn't very fast (2600 ms on average). So after submitting, I decided to rewrite my solution using recursion. This ended up being less code, a bit cleaner, and way faster - now part 2 only took me 207 ms on average!

Also, the recursive function is quite simple:
- if I already found a possible solution, don't continue - this is to make other branches of the recursion stop when one of them found the answer, and to be able to do this, I use a boolean value by reference, so all branches can access that same variable, as well as the code that needs to know the outcome
- then I loop over all possible operators
- for each operator, apply that specific operator - so if it's a `+` add the current number to the current result, for a `*` multiply them and for a `|` string concatenate them (I used a single pipe to be able to use a `char` array)
- if we're at the end of the list of numbers and the outcome equals the test value, toggle the `possiblyTrue` value to true - this stops all recursion branches, we've found a possible solution!
- if we're not at the end of the list of numbers yet, and the outcome still is equal to or lower than the desired test result, a solution is still possible, so increment index of the number we'd like to evaluate, and call ourself, the `Evaluate()` function, for that next number

The trick to a good performance is to cut any recursive branches when you've found a solution, and to abort further evaluation if the current branch (and only the current one), is already past the desired test result, and thus impossible.

A fun little challenge today! And a good lesson to see what recursion can do for such problem types opposed to sequential lists and loops.

### Speeding up the repair
Like day 22 and day 9, I decided to introduce my own `parseLong()` function here to see if it did speed up things, but it didn't improve things (or even performed a little worse for some reason). So I did stick with `long.Parse()`. But then I spotted the implementation of the `|` operator, which I simply implemented by string concatenating the numbers together and parsing them back to a `long`. Both the parsing and the string operation are quite heavy. So I came up with the idea to do it purely mathematically. I looked up how you can determine the number of digits within a number, which turns out to be `log10(n) + 1`. If I were to take the power of that number to 10, I would have the number of decimal places to shift the left number with to fit next to the number on the right. So I changed this line of code:
```
result = long.Parse($"{current}{numbers[index]}");
```
into:
```
result = (long)Math.Pow(10, (int)Math.Log10(numbers[index]) + 1) * current + numbers[index];
```
Which is a little more cryptical, but a lot faster. My solution now runs in 119 ms.

Then, I decided to have a shot at reversing the algorithm, as I've heard that that would be faster, since you can bail out early for divisions if the current result isn't divisible by the previous number. The implementation though is completely my own, I didn't consult any external source. I used all of the same code from my original submission, only changing the order (back to front), and with that the check if I reached the first number, and of course also the operators. Changing adding to subtracting and multiplication to division is easy, but the splitting instead of concatenating takes a little more effort. Also, I now need to first validate if the current result contains the previous number. I tested a few things, and the 'contains' check is fastest with a straing comparison, while the actually splitting is fastest mathematically:
```
if ($"{current}".EndsWith($"{numbers[index]}"))
    result = (current - numbers[index]) / (long)Math.Pow(10, (int)Math.Log10(numbers[index]) + 1);
else
    result = -1;
```
And while I was at it, I decided to try one more thing: why would I create a `char[]` with operators, doing a char comparison to each operator for each operation? Although it fits the puzzle description well and helps readability, there's no real reason to do so. Hence, I dropped the `op` collection and the for loop over that collection, and now simple iterate from `0` tot `2`, changing the operator per each index.

With these three changes combined (the mathimatical `||` implementation, reversing the algorithm, and dropping the char array) my solution now runs in 4.58 ms!
