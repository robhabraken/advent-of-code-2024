# Solutions to Day 14: Restroom Redoubt

*For the puzzle description, see [Advent of Code 2024 - Day 14](https://adventofcode.com/2024/day/14).*

Here are my solutions to the puzzles of today. Written chronologically so you can follow both my code and line of thought.

## Part 1

The first part of today's puzzle was good fun and quite easy. I wrote a `Robot` class to easily keep track of the whereabouts of each robot. This class contains two attributes of the type `Tuple<int, int>` to store the x,y-coordinates for both the current position as well as the velocity.

I've also added a function `Move()` to the robot class. This function calculates the new location based on the robots velocity for the given number of steps (seconds) within the given bathroom space dimensions. Mind that I did not calculate the next step simply by adding the velocity to the current position once and looping over that, but I wanted to optimize my solution for whatever part 2 might be, and calculated the position for the number of steps right away, without looping over each new position individually. This is easily done by taking the modulo of dividing the travelled distance by the map dimension:
```
var pX = position.Item1 + (velocity.Item1 * steps) % dimensions.Item1;
```
If the map is `5` tiles wide, you are at index `2` and your `x`-velocity is `2`, and you want to move `4` steps, this will give you an increase of `x` by `2 * 4 = 8` positions. And `8 % 5 = 3`. In other words, it doens't matter how many times you've teleported from left to right, it only matters what the delta from your current position would be, so you can subtract all full map widths, which is exactly what modulo does. Our new position now is `2 + 3 = 5` and that's one tile outside of the boundary of the map (which is `5` wide so the maximum zero-based `x`-index is `4`). So we now only need to correct for stepping outside the boundaries:
```
if (pX < 0)
    pX += dimensions.Item1;
if (pX >= dimensions.Item1)
    pX -= dimensions.Item1;

if (pY < 0)
    pY += dimensions.Item2;
if (pY >= dimensions.Item2)
    pY -= dimensions.Item2;
```
This way, it doesn't matter if you take `2` steps or `100.000`, it'll take just as long to calculate the new position.

Now we only need to parse the input, create our `Robot` objects, iterate over them and all move the `100` steps in one go.

Lastly, I loop over all robots, and check in which quadrant they are by comparing their x,y-coordinate against each halve of the grid for both the width and the height, counting the robots per quadrant. The answer is the product of all quadrants. A fast and clean solution, let's see what part two brings us today.

## Part 2

*todo*