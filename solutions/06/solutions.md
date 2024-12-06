# Solutions to Day 6: Guard Gallivant

Here are my solutions to the puzzles of today. Written chronologically so you can follow both my code and line of thought.

## Part 1

Things are really getting fun now. I enjoyed today's puzzle a lot. And I also like my solution for part 1 a lot, because it's quite efficient, and super fast. Normally, I would keep track of direction and create if statements for moving in different directions, but I came up with a neat trick. I've created a map to store the y and x delta for each direction:
```
var deltaMap = new int[4, 2] {{-1, 0}, {0, 1}, {1, 0}, {0, -1}};
```
And an integer variable that keeps track of the `direction`. We start with going up, which equals direction `0` and delta `(-1, 0)` (decreasing `y` by one and staying on the x-axis).

Now, since we're always turning to the right, I will just increment direction by one, and move over the delta map. But to prevent needing to check if I am out of bounds of my `deltaMap` and create another if-statement to set the index back to `0`, I just always get the direction from the map using a modulo operator, and increase my direction variable indefinitely. So to retrieve the current `y`-delta of my current direction, I can simply do:
```
deltaMap[direction % 4, 0]
```
With this trick, I can cram the whole algorithm in one single while-loop!
- if I didn't already visit this location, increment my answer, as I am now visiting this location
- mark the location on the map as visited for future reference (don't want to count it twice)
- calculate my new location by adding the current direction delta
- if that new location is out of bounds of the map, I'm done, so I'll break out of the while loop
- if I am still in, I will check if there's an obstruction on that new location, and if so, I will change direction by incrementing my `direction` variable (but I don't update the location since that's not a valid one, so for this loop I'll stay put and only change direction)
- else, if there's no obstacle, change the location to the new location

This is now super efficient and runs in 1 ms, so I have a great starting point for part 2.

## Part 2

This part of the puzzle is a bit tricky. So I first tried to come up with a way to determine all possible locations for an obstruction to create a loop. Which in fact, should always be on a location that I previously visited, since any location that I didn't visit in part one also cannot change my direction anyway. So that narrows down the possible locations quite a bit.

Then we should find a way to detect a loop as soon as possible, which is actually quite easy: if I visited a location before *while going in the same direction* I now any possible path after that will be exactly the same, since my map doesn't change.

So I needed to do two things: first run over the map to mark all locations without any obstructions, which gives me a list of locations to test. Then, go over that list of locations and put an obstruction at that location, restart my route and see if I get into a loop.

I moved my while-loop into a function, which also gives the initial map as a reference to mark the locations in, a starting location, and a location for where to put an obstacle. I use that function without an obstracle first, building up my `referenceMap`, and then use that function for each location in a for-loop with a faux map (don't need it) but with a new obstacle location each time.

The only other thing I needed to change, was my map that stores a visited location as a boolean value. I now need a boolean value for each direction, so I turned that map into a threedimensional array. The third dimension can be set to `true` for `direction % 4` when I visit that location. Now I can detect a loop by checking if my current direction is set to true for that location. If so, I return true and count that location as a possible obstacle location.