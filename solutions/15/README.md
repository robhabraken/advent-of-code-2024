# Solutions to Day 15: Warehouse Woes

*For the puzzle description, see [Advent of Code 2024 - Day 15](https://adventofcode.com/2024/day/15).*

Here are my solutions to the puzzles of today. Written chronologically so you can follow both my code and line of thought.

## Part 1

This was another fun challenge! Not super difficult, but a little more work than the other days so far. I love those grid and real coding puzzles though, so I really enjoyed this one.

Part 1 didn't need recursion, and although I planned to use it, I kind of already solved it before getting to that stage. Because every element in the warehouse has the same size, and we could only move in one direction, it was easy to solve by just shifting `char` elements around. What I did basically, was loop through the map in the given direction until I found an empty spot (`.`). If I found one, I moved over every character one place to where that spot was, including the robot itself. When I didn't find one, I just did nothing, because I would've hit a wall before finding an empty spot, so this move wasn't possible. That's why my function is named `attemptMove()`.

Also, they made it quite easy for us, enclosing the map in a rectangle of walls, because for once we didn't have to do any boundary checks at all! Instead of boundary checks, you can also enlarge the map yourself, but even that wasn't needed given the way the input was shaped. This also meant parsing was easy: every line that starts with a `#` can go into the list of map lines, all other non-empty lines go into the list of moves.

I then transformed the `string[]` into a two-dimensional `char[,]`, because although a string array can be read like a character array, the characters in it are read-only, so we cannot move the characters around in the input.

After that, I went over all moves and called `attemptMove()` for each movement. I again used my trusty `deltaMap` to avoid if-statements for each direction. I also used a trick to translate the input of movements to my delta map: a string named `directions` with the direction characters in the same order as my `deltaMap`, like this: `^>v<`. This way I can get the `.IndexOf()` the given character from the input file and that translate to the `int` index of the corresponding direction in my delta map.

Lastly, I go over the map once more to calculate the GPS coordinates of all boxes, by adding the product of `100` and the y-coordinate and the x-coordinate to the answer for each box character (`O`) in the map.

The only think left, was to actually do the move in the desired direction. I did that as the last step this time. My `attemptMove()` function does the following things:
- Add the delta coordinates of the corresponding direction to the coordinates of the robot, giving me `dY` and `dX`.
- If the character add that location is a `.`, indicating an empty space, I just move over the robot to that position, setting the previous location to empty and that's it.
- If the character in the target location represents a box (`O`), I need to do some work. I then start looping indefinitely until I either hit a wall or an empty space, continously adding the same delta coordinates from my delta map in that direction, counting the steps I took along th eway.
- When I hit a wall I don't have to do anything anymore, but If I found and empty spot first I need to continue with my attempt.
- I then move back from the location of the empty space all the way to where the robot is (one more step than the steps I took to also include the robot itself), and just move over the character from the previous position. When I arrive at the robot, I set the original location to empty (`.`) and update the robot's coordinates.

This is actually all that's needed, shifting over characters in a single line in either direction. No recursion was needed and it is super fast, sub half a millisecond for the real input.

## Part 2

I optimized for speed with the first part, but no gigantic proportions for the second part, so that wasn't really necessary. But since we're not moving in a straight line anymore (or not solely), and because each consecutive box can push more than one box at a time, we *do* need recursion this time. So I deleted most of my code (the `char[,]` map and the contents of the `attemptMove()` function) and started over.

I did a few things differently this time:
- I didn't use an actual grid or map to work with, but a list of objects instead. I've created an `Obstacle` class with members for the x,y-coordinates, a public `TryMove()` function and a private `Move()` function. Why? Because a box should be able to move itself, but only could request other boxes to move over *if possible*. I'll get to how that works later. I also added an enumeration named `ObstacleType` so I can reuse this object for the `Robot` and any `Wall` elements too next to the main `Box` type. Of course I don't call the move functions for Walls, but I least I now can store all elements in a single `List<Obstacle>` collection and test whether they are a `Wall` or a `Box`.
- Because I now have an obstacle collection instead of a map, I don't store spaces anymore. That means my input parsing also changed, as I now only respond to `#` and `O` characters in the map. I also can, given the object approach, quite easily take into account the double width: by just setting the coordinates to `x * 2, y` everything is twice as wide. No other changes are needed anymore.

Going over all obstacles and calling `attemptMove()` is still the same, as is the computation of the GPS coordinates.

### Obstacle.TryMove()
Then I impleted the `TryMove()` function. In my mind it's easier to call than *on* an object so I am already thinking from the perspective of that obstacle. Same thing as before, I start with calculating the new coordinates `dY, dX`. But instead of looking at the map, I now loop over *all* obstacles to check if they have coordinates that correspond with the new position trying to move this obstacle to. This if-statement is a bit funky then, given the 2-width obstacle dimension and the blown up map, so I will explain:
```
foreach (var obstacle in obstacles)
    if (obstacle.y == dY)
        if (direction % 2 == 0 && (obstacle.x == dX || obstacle.x == dX - 1 || obstacle.x == dX + 1) ||
            direction % 2 == 1 && ((direction == 1 && obstacle.x == dX + 1) || (direction == 3 && obstacle.x == dX - 1)))
            if (obstacle.type == ObstacleType.Wall)
                return false;
            else
                boxes.Add(obstacle);
```
So we start with looping over the obstacles. Any obstacle that has the y-coordinate matching my target position could be in my way. This goes for both horizontal and vertical movements, as the y-axis is still unchanged. Then, we need to split our if-statement between horizontal and vertical movements, to check the x-coordinates:
- For vertical movements (up and down, or `0` or `2` in my `deltaMap`, or in other words `direction % 2 == 0`) I check who is above or under me. This function is only used for boxes, and boxes are two positions wide. And every obstacle is only stored by its leftmost coordinate! So I need to check for coordinates on both positions above or below me (`dX` and `dX + 1`), but also for the coordinate one step to the left diagonally (`dX - 1`) as that box is also above or below me with its right half. So there could actually be three different positions here: a box diagonally to my left (`dX - 1`), one exactly above or below me (`dX`), and one diagonally to the right (`dX + 1`).
- For horizontal movements (right and left, or `1` or `3` in my `deltaMap`, or in other words `direction % 2 == 1`) I check who is at my left or at my right, but that's again different between left and right, as to my right it's one more than the target location (`dX + 1`) and to the left it's actually one less than the target location (`dX - 1`) as I only store the leftmost coordinate. This all is because my objects all have a width of two, but I always only move by one position.
- Then, if I've found an obstacle over there, I can check if it's a wall (`obstacle.type == ObstacleType.Wall`) and if so, abort because this move isn't possible. If not, then it is a box and I can add it to my temporary obstacle list with boxes I want to move. This list is always going to contain either no, one, or two boxes.

Okay, so I've found what's in my way, if anything, and can continue acting upon that. I still don't know what's after that, but recursion will tackle that.

If there are no boxes in front of me (and also no walls, or else the function would've already been aborted!), I move in that direction, and return `true`, indicating I have been able to move, which is important for the recursion.

If there are boxes in front of me, I call `TryMove()` on all of those boxes. There's one more thing though: if I knew it would always be possible to move, or if things are in a single line, I could always just perform the move and return true to know we can move ourselves as well. But that's not possible, because it could be that further down the chain, a box is obstructed by the wall, making anything hold position. Like this:
```
.........
......##.
..[].[]..
...[][]..
....[]...
....@....
.........
```
In this case, the boxes on the left would be able to move upwards, but the boxes on the right are obstructed by a section of walls. This means, the box closest to the robot cannot move, and thus the boxes on the left also will not be pushed upwards.

I came up with the following solution to that: I have included a `doMove` boolean that tells the `TryMove()` function whether to actually perform the move. If it is set to `false`, the move is only validated, but not performed. This way, I can let the whole recursive chain run without changing anything, just checking if this move would be possible. And only when I now that it actually *is* possible, I call `TryMove()` again on all objects recursively, but now with `doMove` set to `true`.

### Obstacle.Move()
Because all of the validation and logic happens in `TryMove()`, the `Move()` function actually only performs the move, and is only called by the object itself after it received the request to actually perform said move.

### attemptMove()
Last piece of the puzzle, quite literally, the new implementation of the `attemptMove()` function. I did not try to re-use the Obstacle function for that, because it is just too different: a robot is only one position wide, making the if-statement different, and also the follow up is slightly different. So I chose for writing slightly duplicate code in favor of better readability. The general idea is the same though: obtain new position, check if there's anything in my way, being different for vertical movements, left and right - and if that's a wall, just return and do nothing, and if it's a box, try and start moving stuff, but only actually do move when we know it's possible. When everything is moved out of the way, we can move the robot to the new position and that's all.

### Result
As I said I really like this kind of challenges. Not hard, but fun to code. My solution isn't the quickest (117 ms), possibly due to the list approach instead of a map, and also because I am using objects instead of native types. I think it could be done quicker if I use a grid approach, saving me looping through all objects all the time. But I like the solution and it works fast enough I think. If I would've had more time, I would probably make a very cool animation to actually see the robot pushing around the boxes, but I don't, so I just threw out this quick console animation of the example input:

![Animation of solution of AoC Day 15 puzzle using example input](./WarehouseWoes.gif "Warehouse Woes")