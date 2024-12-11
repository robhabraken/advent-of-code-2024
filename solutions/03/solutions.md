# Solutions to Day 3: Mull It Over

*For the puzzle description, see [Advent of Code 2024 - Day 3](https://adventofcode.com/2024/day/3).*

Here are my solutions to the puzzles of today. Written chronologically so you can follow both my code and line of thought.

## Part 1

Today's puzzle was a simple one, but a fun one. The first part quite literally described a regex search. I created a Regex object with the specific pattern (could've done compile time generation, but that's longer in terms of LoC), looped over the input and then over the matches to that pattern, followed by some string magic to strip the instruction from the brackets and the `mul` command, splitting the two digits, converting them to integers, and adding the outcome of the multiplication to the answer. 

## Part 2

Part two actually was the fun part of the puzzle. There were no brainteasers or catches in it, but it was fun to find a way to make the code as clean and concise as possible. What I came up with is actually adding the `do()` and `don't()` commands to the regex patterns as an `OR` statement, so it will find any of the three possible instructions. Then I just looped over the matches and depending on the type of instruction, performed the necessary action: multiply if `enabled` using my former logic from part 1, and toggle `enabled` depending on either a `do()` or `don't` command.