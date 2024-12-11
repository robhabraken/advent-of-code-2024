# Solutions to Day 2: Red-Nosed Reports

*For the puzzle description, see [Advent of Code 2024 - Day 2](https://adventofcode.com/2024/day/2).*

Here are my solutions to the puzzles of today. Written chronologically so you can follow both my code and line of thought.

## Part 1

Another relatively straight forward puzzle. I started parsing the input, converting it into an integer array per report (line), and then looped over the levels within that report, keeping track of the direction while checking the interval for each step. If the delta or interval between two values is zero or larger than 3, this report isn't safe. If the direction changes opposed to the direction of the previous step between to levels, the report isn't safe. As soon as I found an invalid interval (either direction or size), I stopped checking this report and didn't count it as being safe. All other reports add one to the answer.

## Part 2

This is actually quite a bit more tricky than it seems! First of all, I needed to change my approach, as I wanted to count the number of infringements within a report instead of bailing out the moment I found an invalid step. So Instead of check each individual pair of levels, I looped through all levels regardless of the outcome, and counted the number of increasing steps, decreasing steps, and steps that were either zero or too large. If all steps are in the same direction (no steps counted for either going up or going down) and there are no zero sized intervals, or steps that are too big, than this report is safe from the start. If there is only one step in the other direction (either going down or up), or only one step that's wrong (zero or too big), than this report might be safe given the presence of the problem dampener.

So then I stored the index of the faulty level (both from the 'polarity map' and the 'interval map') and removed that to check again if this report is safe now. This increased the number of safe reports, but it didn't solve all of them. I then quickly saw when analyzing the input, that sometimes an adjacent level to the faulty one can be changed too to make the report valid. So I needed to check at least two levels on breaking the pattern if only one of the patterns broke (polarity or interval) and even at least four when both the polarity or the interval pattern wasn't linear. Given the fact that most reports contained only 5 to 8 levels on average, in most cases I needed to test almost all levels within that report. Turned out, the code and logic became rather complex and the optimization negligible.

Thus, I ended up just checking if a report was safe, and if it wasn't, remove each level one by one and recursively test for it to be safe. Way less code, and equally quick, and most of all, much more reliable. This felt like a brute-force solution initially, but given the amount of edge cases I think it's the best and most elegant solution for now.