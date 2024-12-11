# Solutions to Day 4: Ceres Search

*For the puzzle description, see [Advent of Code 2024 - Day 4](https://adventofcode.com/2024/day/4).*

Here are my solutions to the puzzles of today. Written chronologically so you can follow both my code and line of thought.

## Part 1

Today was quite fun. Especially part 1. A grid search, but not that hard. For me the challenge was to produce a very clean and concise solution, without the need to create function to check each direction, or keep track of the direction. Since the word's always the same (`XMAS`) and not that long, we don't have to check each character, but we can just grab a list of four characters in each direction and than string compare that to the word `XMAS`.

So what I did, straight from reading the input file, is looping through the lines of the input followed by the characters of that line, treating the input as a big two-dimensional string array, which it in fact already is. When I come across any `X` I know that potentially could be the start of the word I'm looking for, so I do a search in each direction using deltas. The deltas indicate the direction (-1 on the y-axis, -1 on the x-asis, then -1 on the y-axis but 0 on the x-axis etc.). I produce these delta with two loops two prevent naming directions or creating functions for each direction. Then, for each direction (or combination of x- and y- deltas actually), I call my function `Search()`.

Within `Search`, I simply take 3 steps in each direction, increasing by the deltas, and concatenate the characters that I found, stopping when I hit the outer bounds of the array. That's only three lines of code!

Then, I have produced a string varying from 1 to 4 characters (depending on the location within the array: 1 character if I start at an X that is at the outer bounds of the array and I'm moving into a direction that is out of bounds). I then can simply string compare the outcome and increment the answer by one if it says `XMAS`.

## Part 2

So while part 1 took me a little while to come up with a smart solution, part 2 only took a few minutes. Since we're only looking for diagonals, the start of our cross is always `A` and that's always in the center. Meaning, the start of the search can never be at an edge, and only goes -1 or +1. So if I start my loop at index 1 and stop at length minus one, I don't even have to do an out of bounds check anymore!

So, I'll use the same code as part one to loop through the two dimensional input array, but with adjusted start and end indexes. And I don't have to loop through delta's, I'll solve that in a different way. So let's just loop through the input and start searching when I find an `A`, which is the starting point of my `X`.

Then, I can just string concatenate two diagonals, using the outer corner indexes, without worrying about boundaries. And those two diagonals should both either read `SAM` or `MAS`, which is a simple string comparison. And that again leads to conditionally incrementing the answer.

*Edit: having to wait for another puzzle at day 8 I had a shot at optimizing my shortest and quickest solution up until now, so I tweaked my solution for day 4 part 2. I removed the variable declaration and compressed the code a bit, without losing to much readability I think. It's now only 9 lines of code running in 0.58 ms!*