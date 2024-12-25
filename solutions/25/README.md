# Solutions to Day 25: Code Chronicle

*For the puzzle description, see [Advent of Code 2024 - Day 25](https://adventofcode.com/2024/day/25).*

Here are my solutions to the puzzles of today. Written chronologically so you can follow both my code and line of thought.

## Part 1

Today was super easy, a bit of a Christmas gift I think. It's just mainly a parsing task. Since every lock and every key is exactly a 5x7 grid, I loop over the input per 8 lines to process each separate grid at once. Then I create an `int[5]` for the heights of each column and loop over all lines within that grid, over each x-coordinate, and increase the height counter for that specific column when I find a `#`. Then, if the first line of those 8 doesn't include a `.` I add the result to my list of locks, otherwise I'll add it to my list of keys.

Then, for each lock, I loop over each key (`lock` is a reserved word in C# so I used `l0ck` as a variable name here) and check if the sum of both for each column exceeds the total number of rows (`7`). If I don't find any columns that do not fit, I increase the answer count by one and that's it.

## Part 2

Just deliver the chronicle, this one's for free!