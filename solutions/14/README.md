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

Lastly, I loop over all robots, and check in which quadrant they are by comparing their x,y-coordinate against each halve of the grid for both the width and the height, counting the robots per quadrant. The answer is the product of all quadrants. A super fast (0.3793 ms) and clean solution, let's see what part two brings us today.

## Part 2

*After finding the tree and sending in my answer, and also pushing my code to the repository that helped finding it, I decided to rewrite it completely, as I didn't like the hard-coded variables that would only work for my personal input. Hence, I updated my repository with a now way more generic solution that I also tested against other input files.*

Against my initial assumption that the tree would start to form gradually (as you can read below), it turned out it suddenly appeared for only one single frame and then be gone again! Or in other words, the entire pattern of robots as observed from above looks totally scattered, except for the frame that shows our Christmas tree. In other words, the distribution of x- and y-coordinates of all robots should normally be relatively equal between frames, except for that one single frame, where a lot of robots are actually grouped around the center and don't deviate much from each other in terms of x,y-coordinates.

So now, instead of looking for a specific pattern I do not know upfront, I calculate the average x- and y-coordinates of all robots, and then I calculate the deviation from that average for each robot, and then I take the average of that deviation per x- and y-coordinate. Lastly, I store the *average deviation* of the first frame, as a reference for how scattered the coordinates are. Then for each of the following frames, I divide that average deviation by the current deviation of that frame. When the distribution of coordinates is about the same, that division should deliver a value around `1.0`. The bigger the difference between the deviation of the current frame and my calibrated deviation, the more likely it is that the pattern is hugely different and could contain the Christmas tree I'm looking for! I have set the treshold value to `1.5D`, meaning that whenever the difference is greater than `50%` I assume I have found the tree.

It's not super fast (199 ms), but it works very well!

### Speeding up the search for our tree
My second solution, the generic one (as you can read above this section), is based on looking for a deviation in the distribution of robots across the x- and y-axis. But as you can see below (my original partly manual search), I also discovered a pattern. As my distribution deviation search has to test for all 'seconds' with increments of one before finding the answer (for me in the 8-thousands), it takes quite a while (199 ms as stated above). I wanted to see if I can speed this up if I combine both findings. And than it occurred to me: obviously, the pattern of robots start forming _gradually_ with a cycle of _both_ the x- and y-coordinate. The repetitive pattern of `101` seconds is equal to the width. But there should also be an occurrence of a pattern for the y-axis, of which the width is `103`. I immediately felt that this strange grid size was part of the puzzle and part of the answer when reading the puzzle description initially, and now I know why: **we are looking for the least common multiple of the width and height of the grid, when the occurrence of the horizontal and vertical pattern matches**! A square grid (which we normally see for AoC puzzles) doesn't have this characteristic. But there's a catch. The pattern doesn't start at 0, that would be too easy. It starts on different 'seconds' for both axis.

I printed out more details about the deviations and saw the following: most frames have a deviation of 2 to 6%, which is a normal deviations of a lightly scattered pattern. Some frames have a deviation of around 35%. And all of those frames are spaces either `101` or `103` apart. And finally, the frame we're looking for has a deviation of well over 50% (I think close to 100% actually). In my algorithm above, I just tested each frame until I hit this large deviation. But there's a better way to find our tree!

Now, I lowered my treshold to anything above 10% (`1.1`). This means I will find all the frames that are part of the lcm pattern. My first two values are the start of the pattern for both axis, and my next two numbers will tell which one is for which axis! I'll explain: the pattern of 35% deviations occurs at the following values first: `18` and `77`. After that, we will see cycles of either `101` and `103`, but I don't know which number will start which axis (`18` or `77`). Of course, I can see that when looking at the data, but I want my solution to be generic for any input. Thus, what I do is wait for the first **four** occurrences of this 35% and then *stop*! In my case this are `18, 77, 121, 178`. And that means I only have to iterate over the seconds, checking for the deviation in distribution of robots, until I hit second `178` (in my case). That's a far lower iteration count than my algorithm above. And with these four numbers, we know enough. If the difference between number 1 and 3 is the width, the first number is the starting offset for the least common multiple of the width, and if the difference between 1 and 4 is the width, it's the other way around. In my case, `18` starts the cycle on the vertical axis (add `103` makes 121) and `77` starts the cycle on the horizontal axis.

I then wrote an lcm function that works with a different offset for both values:

```
static int leastCommonMultiple(int a, int offsetA, int b, int offsetB)
{
    var lcm = offsetA + a;
    while (true)
    {
        if ((lcm - offsetA) % a == 0 && (lcm - offsetB) % b == 0)
            return lcm;

        lcm += a;
    }
}
```
And I can call this function using my four markers (in the correct order which we've determined above). So, the least common multiple of increments of `101` after starting at `77` combined with increments of `103` starting at `18` gives me my puzzle answer! This now runs in only 5.1111 ms! And it's generic, and very fast.

### Original journey
I expected a lot, but this.. this certainly was the weirdest and most fun challenge yet! No clues at what to find our what that Christmas tree would look like. I initially assumed that it should be fully and easily detectable by code, and since there's no pointers on what to expect, I imagined it should be something like that all robots where positioned in a triangular shape. But after running through lots of frames based on my test output (I just increases by 10, 100 and 1000 to see how the pattern changes) I concluded that they always are somewhat scattered throught the entire map. My second assumption was that the shape would form gradually, and would start to gradually appear, so it should be noticeable in the upcoming seconds to the formation of the tree as well, but that also wasn't the case. Oh, and one more thing, it initially didn't occur to me the puzzle description said that *most of the robots should arrange themselves*... so even not all robots are included in the pattern.

Then I looked at the number of robots (`500`) and realized that's way to low to fill a lot of 'pixels' in a map that is this large, because `101 x 103 = 10,403 tiles`. This made me stop looking for a pattern via code, as I didn't think it was possible to make a correct assumption on how the pattern would look: is it a filled area? Is it a large more pixelated shape with empty spots in it? Or is it merely the contour of a Christmas tree?

So I continued running my animation in the console of increments of `100` (because I thought I would see it gradually appear as I said), and then I hit a frame that looked totally different than others. I wrote down the frame number, and started to do more tests closer to that number. That's when I learned that this anomaly really only occured for one frame and than it was gone again. But I quickly found another that was looking quite the same. And another one. And they were all spaced apart `101` frames (which is the same as the grid width, which makes sense, as that would describe a full cycle for all robots - though not necessarily a full cycle for each robot at the same time). Then I decided to write my output to a file starting at the initial position, and now by increments of `1` to not miss anything. This showed that the pattern deviation I observed occured first at frame `77` and then every other `101` frames after that. So I changed my code to start at `77` and loop with increments of `101` seconds and write that to a file. And that's when I suddenly saw the Christmas tree appearing! It was way more perfectly shaped as a pictogram than I thought, really only occured at one frame exactly every few thousand frames, and was also centered and way smaller than I imagined.

So I got my answer, by visual analysis. Helped with the code from part 1. I submitted my answer, which obviously was correct, and cleaned up my code to browse through the pattern I found based on these findings, hard-coding them as variables to my input. I included the shape of the Christmas tree as a `string[]` in my code, just for the fun, and wrote my variables in such a way that I could write:
```
if (bathroom.Contains(christmasTree))
    break;
```
As an explanation for the hard-coded values, I included this comment section in my code:
```
/**
 * This solution can't do without hard-coded values: I discovered a pattern of positions that stood out from
 * the rest, and this started occuring after 77 seconds for my input, with a steady cycle of 101 seconds after that.
 * I knew I had to observe those states, and those states only - also making my code a bit faster than iterating over
 * every state for each second.
 * 
 * When I found the Christmas tree in the output I knew what to look for and hard-coded it, just for aesthetics!
 * Because I actually only need to check one of the lines (I chose the bottom of the needles), but what fun would that be?
  */
```
That was good fun! Check out the [Christmas tree](./ChristmasTree.md) that I found in the bathroom!