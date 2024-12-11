# Solutions to Day 1: Historian Hysteria

*For the puzzle description, see [Advent of Code 2024 - Day 1](https://adventofcode.com/2024/day/1).*

Here are my solutions to the puzzles of today. Written chronologically so you can follow both my code and line of thought.

## Part 1

Of for an easy start! Parsing the input is kind of all the work for the first day. First I set up two integer lists in an array, though using two separate list variables would be the same amount coding, but I like that this looks like less duplicate code. Then, I started parsing the file, splitting the input by its three spaces (why three?) and assigning each number to its respective list.

Now that we have two list objects, we need to determine the difference between each subsequent lowest value. In order words, if we'd sort both lists, we could just determine the difference between each pair of equal index between the two lists. So that's exactly what I do: first sort both lists, then go over the items in the first list, subtract the value of the number in the second list and add that to our answer. Since we don't know which one is the highest number, I just pick one order (always substracting the second or right number from the first or left one) and use Math.Abs to get the absolute difference between them. This is where the puzzle example input might trick you, as with that data set, the right number is always equal to or larger than the left. But that's not the case for the actual puzzle input, so you cannot just subtract the left value from the right - that would give you negative values messing up your answer.

Nice warm-up for this year's AoC!

## Part 2

Apart from adding the absolute value of subtracting the two numbers, we can use all of our part 1 code. We might skip the sorting of the array, as it isn't needed functionally, but it makes for a nice optimization.

To find the answer to part two, we need to go over each number in the left list, and count it's occurences in the right list. That's actually quite easy and literally what I do in code. But since our lists are sorted, once the number in the right list is higher than the number we're looking for, we can stop counting. Here it really helps that are lists are sorted. Then, when we've found the number of occurences, we multiply that with the current number and add the outcome to the answer.

*Edit: I did some measurements, and on average, the profit from being able to break out of the loop after the number on the right was higher than on the left, was equal to the penalty of sorting both arrays. So I removed the sorting and breaking out of the loop in favor of less and cleaner code.*