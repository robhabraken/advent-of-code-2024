# Solutions to Day 11: Plutonian Pebbles

*For the puzzle description, see [Advent of Code 2024 - Day 11](https://adventofcode.com/2024/day/11).*

Here are my solutions to the puzzles of today. Written chronologically so you can follow both my code and line of thought.

## Part 1

Part 1 was worryingly simple. I took the linear approach (or brute-force if you like, although I did use some sort of caching), and knew that that wasn't going to be enough for part 2. So, part 1 could've been done way faster, even with my part 2 solution in the end, but I kept it like I originally used it for submitting my answer to share the process.

I created a function named `changeStones()` that does the stone and engraving change as described in the puzzle description: `0` changes to `1`, even digit numbers are split into two, and everything else is multiplied by `2024`. To avoid calculating the same engraving multiple times I stored the outcome in a cache collection (a `Dictionary<long, List<long>>` that keeps track of the input and output of this function), which saves me a little bit of time.

Then for each blink I loop over the list of stones, change them and store the outcome in a new list and swap the lists when done with a single iteration. I actually was surprised it was fast enough (under 15ms) and that it got me the answer straight away.

## Part 2

### Trial and error...
My first train of thought was finding a pattern and determining the Least Commom Denominator. If there would be a linear increase in the number of stones, I could predict the outcome after a given number of blinks without actually generating all the stones. However, I could not find a pattern in the data. Not in the increment of stones, not in the difference between the increments, and also not in the individual operations (splitting, multiplying etc.).

My next approach was to also store pairs of stones with their outcome. As a first step to optimizing combinations (as the puzzle description stated the stones stay in their respective order). This didn't help a lot though and I couldn't see how this could work over multiple aggregates.

The third attempt was to find patterns in the string representation of the list of stones, and store large chunks of input with their respective output, as I saw that there was a certain form of repetition in the output. And this actually did work, but with all the string operations turned out to be even slower than just brute-forcing my part 1 solution.

After several attemps of optimizing, I threw everyting away and started over. Whatever I did, it took minutes to get to 50 iterations, and at that point I always would hit an out-of-memory exception due to exceeding the supported range of an array...

I knew there had to be a quicker approach, but I couldn't wrap my mind around a dynamic programming approach of calculating all individual outcomes (as I kind of already implemented caching per input and that didn't do much - although I didn't build it recursively, which would be the trick here I guess). And a mathematical approach also didn't seem likely because the peculiar set of operations and its unpredictable outcome.

### The solution
And then it occurred to me, after several hours of fiddling around, that each list of stones actually contained a good amount of identical numbers. And that the order really didn't influence the outcome at all. The puzzle description really threw me off here by stating that `their`**`order is preserved`**, even marked in bold! But a `0` leads to a stone with `1` engraved in it, and that's the same for every `0` in the list. So is every other identical number. And the rules never take into account adjacent stones, so why bother?

So what I came up with, is a `Dictionary<long, long>` collection that keeps track of the stones with their engraving (first element, and the key of the dictionary) and its number of occurences, or multiplier if you like (second element in the dictionary, the value). So I would turn a list of stones with their engravings into (for example): 1 stone with `0` on it, 3 stones with `2024`, 5 stones with a `6` and so on. This way, I only need to compute every number once for each blink. And I can later on count the occurences per stone type (the engraving).

Then, I loop over the list of `stones` (which is such a weighted collection), and compute the new changed stones, putting them in a new collection cold `blink`, which also is such a dictionary. Obviously, the outcome of each computation may produce stones with an engraving I already have in my list. If so, I just increase the multiplier of that number instead of adding an item. Otherwise, when it's a new number (an engraving I do not have in my list yet) I add a new item to the collection with the multiplier from the originating stone (as it will occur as many times as the stone occurs where it is generated from). When finished blinking, I move the `blink` outcome into the `stones` collection so I can iterate over it again for the next blink.

This way, without any recursion or real memoization, I can loop over all numbers, counting their occurrences per engraving instead of exploding my list of stones with lots of duplicates. I am delighted that this approach is very, very fast, and it produced the right answer right away!