# Solutions to Day 12: Garden Groups

*For the puzzle description, see [Advent of Code 2024 - Day 12](https://adventofcode.com/2024/day/12).*

Here are my solutions to the puzzles of today. Written chronologically so you can follow both my code and line of thought.

## Part 1

What a fun puzzle today! Not super hard, but a fun challenge. My plan was to build a recursive function that discovered the entire region in a 'flood fill' type of way. Once I land on a plot with a different plant type, I recursively search for all connecting plots. I first built the area function and then added the perimeter count in that same loop / iteration. So it's just one recursive function over the entire farm, that only hits every plot once!

I first created a two-dimensional boolean array named `visited` to keep track of which plots I already visited. I also again used my common `deltaMap` that is a two dimensional array of integer values, containing the delta of x,y-coordinates into each direction, so I can iterate over directions instead of switching directions using multiple if-statements. I like the fact that this is a cleaner and more concise solution.

Then, I loop over the input once, and if I did not already visit a specific plot, I will call `visitRegion()`. Since every region is visited sequentially (from left to right and from top to bottom), all plots of the previous region are automatically marked as visited once I reach the next plot of the farm, so this function is automatically only called for each new plot. And the top-leftmost plot of region always is my starting point for that matter (doesn't really influence the algorithm, but easier to understand when explaining).

### visitRegion()
Called for each new region, and the starting point for my recursion, but not the recursive method itself! It's the function that sets up a new region and takes care of processing the counts for that region. So, this function first marks the current plot as visited (so that my recursion will not go back to my starting point), and then create a new list of plots. The list of plots is of the `List<Tuple<int, int>>` type, as it stores a list of x,y-coordinates of all plots that make up this region. Then it sets the perimeter to `0`, and calls `discover()`. We pass along our own coordinates as the starting point, and the list of plots and perimeter count both by `reference` so the recursive function can update both along the way. When the recursion is finished, I simply add the product of the plot count and perimeter count to the answer.

### discover()
This recursive function searches for plots that make up the current region. I declare two variables `dY` and `dX` to assign the coordinates to of the plots around me and loop over my `deltaMap` to discover the surrounding plots. The `deltaMap` contains the delta coordinates of `up`, `right`, `down` and `left` in that order. The delta for `up` is `-1, 0` (y, x). If my current position is `5, 4` then adding `-1, 0` to that will give me `4, 4` which is the location of the plot above me.

Then I check if the plot in that direction is out of bounds of the farm (the input grid). If it is within bounds, I continue. But if it's not, I know that there should be a fence, because it is on the outer perimeter of my region, and I thus increase the `perimeter` count.

If I am within bounds, I check if the plot I am looking at contains the same character (plant type) as the current plot. If so, that plot is part of this region, and I continue. But it it's not, I know that there should be a fence, because I found another piece of the outer perimeter of my region. So the else statement of this if-statement also increases the `perimeter` count by one.

When I found a plot that has the same plant type, I first check if I didn't already visit this plot. We need to do this, because in a large region, a plot can be visited from multiple directions coming from other plots of that same plant type. And we don't want to count plots more than once, or trigger infite recursion...

So, we now found a plot that hasn't been discovered yet! Let's add that to my list of plots (by the coordinates), set this location to `visited` in the corresponding boolean array, and call `discover()` for that new plot to continue our recursive search.

This logic is quite straight forward, and gradually discovers an entire region, adding plots to the list while it goes, and counting perimeter sections too. All in one go. It won't count a perimeter twice, since every plot is visited only once, and evey perimeter section count is unique to its plot (as in, plots never share perimeter sections).

I think this is quite an elegant solution and rather efficient. It's not a lot of code either, so it's a great start for part two I think.

## Part 2

This certainly is a bit more tricky than it initially looks like. Even more because the shapes of the regions are quite irregular. So first I'd like to think about what actually *defines* a 'side'? I landed on the following definition for myself: *a side is a contiguous collection of perimeter sections with the same orientation (vertical or horizontal)*. So if I can determine which sections are connected, and oriented in the same direction, I would be able to count all sides of the given region.

I used the exact same code from part 1, with two small changes and two additional functions. 

### The first small change, a complex collection
The first modification was to change the integer `perimeter` counter to a list to store all perimeter sections in. And I've made that into quite an exotic type, a `List<Tuple<int, int, int, int>>`. Why? Because for each perimeter section I want to store multiple values:
- First an integer for the *orientation*, a `0` for vertical and a `1` for horizontal, which is easily produce by taking the module of a division by 2 over my direction counter in the `deltaMap`. Remember in there I store the delta coordinates for `up`, `right`, `down` and `left`, with the respective indexes `0`, `1`, `2` and `3`, which by doing `index % 2` gives me `0`, `1`, `0` and `1`, splitting the directions by their orientation!
- Then an integer value for the *axis* of the section. For vertically oriented sides this holds the x-coordinate, and horizontal sides hold the y-coordinate. All perimeter sections that form a single side should have the same axis (though there could be multiple separate section on the same axis too).
- Then an integer value for the *plot* this section lies at. For vertically oriented sides this holds the y-coordinate, and horizontal sides hold the x-coordinate. Why? Because this is a way to determine if perimeter sections on the same axis are connected, or lie adjacent to each other. Horizontal perimeter sections with y-coordinate `1` at x-coordinates `52`, `53`, `54` and `57` show that we have four sections with the same orientation and on the same axis, but three sections are connected (ascending x-coordinates) and one separate section at index `57`. So this tells us that we have two 'sides'.
- Lastly, an integer value to store the *direction* (or maybe vector) of a section. This is to actually catch a difficult edge case. Even if two sections have the same orientation, lie on the same axis, and touch, they are not necessarily part of the same 'side'!

If you have the following region with plant type `E`:
```
XXXXXXX
XXEEEEX
XXEEXEX
XXEXEEX
```
You can see that where the shape has an enclosed plot, the perimeter sections indicated by the arrows are both vertically orientated, lie on the same axis (same x-coordinate), and are connected, but this is counted as two separate sides. The lower section is the left side of the A right next to it, and the upper section is the right side of the A in the middle row on the left of that section:
```
+-------+
|E E E E|
|E E| |E| <
|E| |E E| <
+-+ +---+
    ^
```
For this reason, we also store the direction from which the perimeter section is observed (looking from the inside of the region outwards). The lower perimeter section of those two is discovered while looking to the left (direction `3`) and the upepr section is discover while looking to the right (direction `1`).

At the end of `visitRegion()` I then call `countSides()` to count the sides based on the collected perimeter sections. And of course, instead of passing `perimeter` to `discover()` I now pass this collection by reference.

### The second change
The second change is that, instead of incrementing the `perimeter` count, I now add each separate perimeter section to my collection by calling `addPerimeterSection()`. On the exact same spots as where I first counted the perimeters. Which makes sense, because that already was the right place to count the individual perimeter sections.

### addPerimeterSection()
This function does what I described above when explaining the new perimeter collection. It adds the given section to the `sections` collection with its orientation (`direction index % 2`), axis, plot location (other coordinate), and direction.

We need one more trick though, and that's centered around the second value, the axis. Let's say we have a plot at the first row, y-coordinate `0`, with a fence on the south side. Thus, the orientation is horizontal. To make things easy, I use the y-coordinate next to the plot as the coordinate of the fence. So for a plot at y-coordinate `0` a north bound fence would have the coordinate `-1` and on the south that would be `1`. It doesn't really matter what we use here, I just need to know which fences are on the same line, so which ones have the same coordinates. The issue is the following: this specific region has another 'peninsula' if you like, that lies two plots down, so on y-coordinate `2`. Like this:
```
XXEEEEEE
XXXXEEEE
XXEEEEEE
XXXXEEEE
```
The north bound side of the lower peninsula woud, following this logic, also have the number `1` as its axis. Which in my algorithm would place them on the same line, but they aren't. It would be solved by adding extra grid lines between all lines and columns, but I don't want to do that - too much work, code and performance loss. So I thought, the fence actually lies between coordinates `0` and `1` and the other one between `1` and `2`. So in fact, that would be `0.5` and `1.5`. But I am not going to use decimals for this. My trick here is to multiple the axis by 2, and subtract the delta from the `deltaMap`. For the south bound side the *delta* y-coordinate is `1` and for the north bound side beneath that the *delta* y-coordinate is `-1`. In other words:

- For the north bound side at the top which would have index `-1` we do `-1 * 2 - -1 = -1`.
- For the south bound side which would have index `1` we do `1 * 2 - 1 = 1`.
- For the north bound side which also would have index `1` we do `1 * 2 - -1 = 3`.
- For the sound bound side on the lower side of the lower peninsula that would be `3 * 2 - 1 = 5`.

So it actually pulls apart the fences around the same plot in blowing up the index and setting them off by one. Vertically seen plot `0` is sandwiched by fences with the axis `-1` and `1`. Plot number `2` is sandwiched by axis with the indexes `3` and `5` (as if the y-coordinate of the plot was actually `4`). And the plot with y-coordinate `3` is surrounded by fences with the axis `5` and `7`. So basically, I am imaginary adding extra columns and rows between all plots for computing the axis.

That was a lot of text to describe why I use `dY * 2 - deltaMap[i, 0]` as the value for the axis... let's move on!

### countSides()
Now the real magic happens. This function counts the number of actual sides, based on the given list of perimeter sections: a collection of perimeter sections together with their orientation, axis, plot locations and directions. In order to determine how many sides we have, I order that list on each of those values accordingly, basically grouping all sections on orientation, then direction, then plot (this is important!) and then direction.

Then I start looping over those perimeter sections, keeping track of the values of the previous section, and:
- whenever the orientation or axis changes,
- or if the plot index isn't the same or an increase by one,
- or if the orientation is the same but not the direction (remember or specific edge case),
then we know that we found a new side and I can increment the side counter by one!

So basically, by grouping and ordering this list, I can see which sections lie in the same orientation and on the same axis. And because I sort per plot location, I can spot connected sections. When there are two sides in line with each other but not connected, the increment between plot locations is more than one, and this if-statement counts a new side. And also, when sections *are* connected, but stored from a different direction, I know it are separate sides as well.

Then I simply store the values of the current section in my local variables so I can compare them with the next section in the next iteration of the loop. And when finished counting, I return the result.

### Wrapping up
I found this algorithm a lot harder to explain than to build. Part 1 took me 20 minutes. Part 2 took me 80 minutes, mostly due to solving that sneaky edge case. The code looks quite clean and concise, but it may not be the best in terms of readability due to the complex collection type. Though it's all based on my definition of what I think defines a side as opposed to a section, which made it relatively easy for me to come up with this and solve this puzzle.

  
