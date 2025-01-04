# Solutions to Day 20: Race Condition

*For the puzzle description, see [Advent of Code 2024 - Day 20](https://adventofcode.com/2024/day/20).*

Here are my solutions to the puzzles of today. Written chronologically so you can follow both my code and line of thought.

## Part 1

Some puzzles are deceptively easy, this one actually turned out to be the opposite. It looks difficult, but it really isn't. They just wanted to know for each cut off through a wall, how many steps that would save you, and how many of those 'cheats' would save 100 steps or more. Since there's only a single path from start to finish, we don't need to do any path finding either.

What I did was set up a map with integers, storing two things: where my walls are, and how many steps it took up to that point. I then iterate over the input and set each corresponding tile to `-1` where I find a `#` in my input to represent a wall. Then, I start walking over the path, again using my `deltaMap` for looping over the possible directions, and also storing the previous tile I was on, to prevent the algorithm from walking backwards (and thus start looping). For each step I take I set the value of that tile to one more than the previous tile, until I reach the finish. Now I have a two-dimensional `int[,]` with walls (`-1`) and a path through it with step counts (`0, 1, 2, 3, 4, etc.`). This is enough preparation to start looking for the answer to the first part of the puzzle.

I then loop over the map once more, but from `1` to `length - 1` to avoid boundary checks, and because it doesn't make any sense to check those walls either. Then, for each tile that represents a wall (`-1`) I check if either the tiles to the top and bottom, or to the left and right are part of the track. If so, this tile is a candidate for taking a shortcut (other cross sections of the wall wouldn't allow for a cheat path that only takes 2 picoseconds). Like so:
```
..#    ##.
###    .#.
#..    .##
```
In both cases only the center wall sections are sandwiched in-between sections of the racetrack, either vertically or horizontally. And if that's the case, I can try to cheat here.

Calculating how many time I save by cheating is easy: the step count of the furthest tile on the path minus the step count of the closest tile on the path, minus the time of the cheat itself which is 2 picoseconds. If the result is equal to or greater than 100, this is a valid cheating route and I can increment the answer by one. So, if I start cheating after 18 steps, and pass through a wall, and arrive at the tile that would otherwise take 131 steps if I follow the original single track path, than I would save 131 - 18 - 2 = 111 picoseconds.

So, to summarize: from every tile on the track to any other tile on the track, if the difference between the steps of those tiles is 102 or more, count it and we have our answer.

## Part 2

The second part of the puzzle was a bit of a challenge to read and understand. I initially thought that you would need to find any route that goes entirely through walls only, so I thought about adding a Dijkstra algorithm to search from one tile to another on the track while passing through wall sections only, but then I saw one of the examples crossing both path sections and walls, straight to the end destination of the cheat! Luckily, because doing path finding from every section of the racetrack to another would be incredibly slow. Let's take another look at one the these examples (where the `2` actually is on a section of the racetrack and not a wall):
```
###############
#...#...#.....#
#.#.#.#.#.###.#
#S12..#.#.#...#
###3###.#.#.###
###4###.#.#...#
###5###.#.###.#
###6.E#...#...#
###.#######.###
#...###...#...#
#.#####.#.###.#
#.#...#.#.#...#
#.#.#.#.#.#.###
#...#...#...###
###############
```
We don't need any path finding or difficult calculations! The shortest distance from one tile to another is always the sum of the difference on the x- and y-axis. So, this example starts at the starting point (at `0` picoseconds) and moves to the second before last tile on the track (at `82` out of `84` picoseconds for the entire track). That means it would save `82 - 0 = 82` picoseconds. But the distance travelled while cheating is `6` picoseconds, so effectively our cheating route would save us `82 - 6 = 76` picoseconds. And the difference in x-coordinates is `2`, in y-coordinates that's `4`, so there we have our shortest distance.

Meaning we now need to go over our grid twice, for every section of the track (tile that isn't a wall (`-1`)) we go over all other sections of the track. If the distance between those two tiles (completely through walls and track sections no matter what) is 20 or less, and if the difference in original steps between both tiles minus the distance travelled is equal to or greater than 100, then we can increment our answer by one because we have found a valid shortcut.

There's one more thing: this algorithm finds the same shortcut twice, once in each direction. I could've checked to only test one way (going up the path), but I chose to just divide my answer by two, as that was a little leess code and also seemed to be marginally quicker.

### Speeding up the race
Initially, I've written a solution that looped over the entire map and then for every tile that isn't a wall, it looped over the entire map again, looking for all other sections of the racetrack. This solution, which I've used to submit my answer, was quite fast (1.9 seconds), but not subsecond. So after finishing all AoC puzzles of the 2024 edition, I came back to this one to see if I could speed it up. And then it occurred to me that a lot of tiles are actually walls, and that I was looping over way too many positions in my array. So I added a new collection named `singletrack`, which is a `List<Point>` collection, to actually only store the tiles of the racetrack itself, in the order you would also travel across the entire track.

So instead of keeping track of an array that visually represents the entire map including its walls (like the puzzle input itself), and storing the step count to each tile of the racetrack in that same array, I now store a list of steps with their respective x,y-coordinate on the map. And thus the index of each coordinate in the list actually is the step count to that point. This way, I only have to loop over actual track tiles, going from a 141x141 grid of which I was only looping over the inner tiles without the border, so that would be 139x139, to a single list of 9,357 individual steps. That means I go from `19,321 * 19,321 = 373,301,041` iterations to only `9357 x 9357 = 87,553,449` checks.

But, while initially it didn't make much of a difference to check for each pair of tiles if it was going up the track (as they were all over the place when you scan the grid from top to bottom, left to right), now it suddenly became very easy to only check the tiles going forward on the racetrack, meaning I could also skip the division by 2 at the end and I only have to check `43,772,046` pairs now: from each tile of the track, I check all the tiles that are left on the track after my current position. So that's only 12% of the checks compared to my initial solution! And also, the code improved a lot as well. I went from:
```
var answer = 0;
for (var y = 1; y < lines.Length - 1; y++)
    for (var x = 1; x < lines[0].Length - 1; x++)                           // loop over the grid for all starting coords
        if (racetrack[y, x] != -1)                                          // if it's on the track, continue
            for (var dY = 1; dY < lines.Length - 1; dY++)
                for (var dX = 1; dX < lines[0].Length - 1; dX++)            // loop over the grid again, for the end coords
                    if (racetrack[dY, dX] != -1 && !(y == dY && x == dX))   // if that's on the track, and not the same as the start, continue
                        cheat(x, y, dX, dY);                                // try to cheat from each start to end point on the track
```
To:
```
var answer = 0;
for (var from = 0; from < singletrack.Count; from++)
    for (var to = from + 1; to < singletrack.Count; to++)
        cheat(from, to);
```
My new solution runs in 565 ms, which is over 3 times faster, saves 2 LoC in total, and is much easier to read I think.