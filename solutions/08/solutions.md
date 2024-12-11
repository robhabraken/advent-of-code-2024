# Solutions to Day 8: Resonant Collinearity

*For the puzzle description, see [Advent of Code 2024 - Day 8](https://adventofcode.com/2024/day/8).*

Here are my solutions to the puzzles of today. Written chronologically so you can follow both my code and line of thought.

## Part 1

The hard part of today's puzzle was reading and understanding what they wanted. Summarized, they just wanted to know all locations that are in line with each pair of equal characters (frequencies) and twice as far apart. In other words, you have to go over each pair of the same values and then add the distance between them to each of their locations in opposite ways.

To make things easy, I didn't use the map (or grid) in my logic or for counting, but created a `Dictionary<char, List<Tuple<int, int>>>` variable named `antennas`. It's a bit of a train wreck as a variable type, but it's quite handy! It's a dictionary of antenna frequences (thus all frequences grouped by their name) and for each frequency a list of coordinates where we can find them. Using a `Tuple` for a `x, y` coordinate is easier than creating a `Struct` or a `Class` to keep track of a location, but also much faster since it's a built in type. I then just iterated over the map, storing each location of an antenna in that list.

Now, to find the antinodes, I iterated over the list of frequences, and then within that over the list of antennas twice, so I can find each unique pair. I then calculate the distance between those, and place an antinode on either side using that same distance. The function `PlaceAntinode` is used to check if it's out of bounds and to prevent juggling with my long variable names for doing this check twice (also less duplicate code).

Because there could be multiple antinodes at one location, but we may only count that location once, I use a `HashSet<Tuple<int, int>>` to store the antinodes, which automatically filters out duplicates when adding them, so I don't have to build any logic to check if there's already an antinode at a certain position.

Then my answer is the length of the list of antinodes I've collected - no need to actually place them in a grid and go over that grid again.

## Part 2

The second part is easier than it seems. You just have to iterate over placing the antinodes at equal distance, only stopping when you hit the bounds of the map. This time around, we may also add the antennas themselves whenever they are in pairs, because they are an antinode for eachother. So basically, I've changed my `PlaceAntinode` method to accept the original location of an antenna and the delta with the distance, so I could loop over adding the distnace until I'm outside of the bounds of the map. And instead of adding the delta for the first antinode, I place one first before incrementing the distance with the delta, so I add the antenna itself as well. Roughly four lines more than part one, and equally fast.

*Edit: I optimized my code based on the suggestion of a colleague also participating in AoC, to do everything in one single pass over the input. Now, I check all the previous antennas of the same frequency the moment I find one, instead of first collecting all antennas and then looping over them. This way, you also automatically filter out all duplicate combinations, as it is always a one-way perspective. It did get a bit faster too, but not a lot. Saved me some lines of code and some characters though, and I like the single pass solution!*