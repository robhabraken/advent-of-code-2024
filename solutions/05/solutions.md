# Solutions to Day 5: Print Queue

Here are my solutions to the puzzles of today. Written chronologically so you can follow both my code and line of thought.

## Part 1

A fun puzzle for today! I made the parser as simple as possible: only create a rule list of the type `List<int>()` to story all the rules in an integer array each, push all lines with an `|` into this list, and then just assume that anything else will be an `update` with numbers to validate and get on with it.

Then, for each list of numbers (which is an update) I loop through the rules, check if the list of numbers contains both numbers of that specific rule, and if so, check if they are in the right order - if not, abort further checking and proceed with the next update.

If I make it through the list of rules without finding one that is being violated, I add the middle number to the answer.

## Part 2

Part two is an extension of the first part, as I still need to first check if a list is in the correct order. But instead of breaking out of the validation when I find a faulty update, I call a new function named `CorrectUpdate()`. And if I make it through the list of rules without finding one that is being violated, I do nothing.

Within `CorrectUpdate()` we need to first sort the list and then add the middle number to our answer after sorting. For the sorting, I used a custom Comparison function. This function loops through the rules: if it finds a rule that applies it returns -1 (sorting to the left, because it's already in the right order), and if it finds a rule that matches the opposite order, it returns 1 (sort them the other way around). I just return 0 because my function should always return a value in any case, though this would never occur for our data set.

This comparison function might not be the most optimized solution, as I could've also created a KeyValue pair collection to store the rules whicht wouldn't require a loop in the Comparison function. But this is the simplest solution and it's fast enough for the still relatively limited amount of rules in the puzzle input.