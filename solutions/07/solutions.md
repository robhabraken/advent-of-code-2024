# Solutions to Day 7: Bridge Repair

Here are my solutions to the puzzles of today. Written chronologically so you can follow both my code and line of thought.

## Part 1

Today wasn't that hard from a logic point of view, but it has a few traps. The first one is that the input uses lower numbers, but the actual input is using some big numbers... so when you're using a typed language like I do with C#, you cannot use integers, but have to switch to a `long` for building up the answer, since the total would overflow the `int.MaxValue` value, messing up your answer. The second trap is that you shouldn't test for the result to match the test value on each loop iteration, but only when you've used up all of the numbers. And the third trap is a bit more complex to explain: if you break out of the loop if your result is equal to or larger than the desired test value for optimization (you don't need to continue if your result will end up higher that the test value anyway), don't forget that the last number could be `1`, and multiplying by one doesn't change the result, so don't test for the result to be equal or higher when breaking out of the loop early - only test if it's higher than the desired test value already.

With those issues out of the way, it's quite straight forward. I wrote the first version without using recursion by creating a cartesian product map of all possible operator combinations for each number of operators. Than I looped over the input testing the options and as soon as I found a valid one, break out of the loop and add the test value to my answer. This gave me the right value so I continued with part 2.

*I changed my solution to be the same as the one I ended up with for part 2 after submitting my answer, only with a small change in it to revert back to only using two different operators.*

## Part 2

For part 2, I needed to rewrite my cartesian map generation, as I first used a trick to generate that list by iterating over the power of the number of operations, converting that to a binary notation. Now I needed to do a bit more looping and list operations to get the cartesian product of three values times the number of operations.

The logic worked equally well, but wasn't very fast (2600 ms on average). So after submitting, I decided to rewrite my solution using recursion. This ended up being less code, a bit cleaner, and way faster - now part 2 only took me 315 ms on average!

Also, the recursive function is quite simple:
- if I already found a possible solution, don't continue - this is to make other branches of the recursion stop when one of them found the answer, and to be able to do this, I use a boolean value by reference, so all branches can access that same variable, as well as the code that needs to know the outcome
- then I loop over all possible operators
- for each operator, apply that specific operator - so if it's a `+` add the current number to the current result, for a `*` multiply them and for a `|` string concatenate them (I used a single pipe to be able to use a `char` array)
- if we're at the end of the list of numbers and the outcome equals the test value, toggle the `possiblyTrue` value to true - this stops all recursion branches, we've found a possible solution!
- if we're not at the end of the list of numbers yet, and the outcome still is equal to or lower than the desired test result, a solution is still possible, so increment index of the number we'd like to evaluate, and call ourself, the `Evaluate()` function, for that next number

The trick to a good performance is to cut any recursive branches when you've found a solution, and to abort further evaluation if the current branch (and only the current one), is already past the desired test result, and thus impossible.

A fun little challenge today! And a good lesson to see what recursion can do for such problem types opposed to sequential lists and loops.