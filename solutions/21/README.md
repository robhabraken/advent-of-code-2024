# Solutions to Day 21: Keypad Conundrum

*For the puzzle description, see [Advent of Code 2024 - Day 21](https://adventofcode.com/2024/day/21).*

Here are my solutions to the puzzles of today. Written chronologically so you can follow both my code and line of thought.

## Part 1

### A sneaky catch
At first this may seem like a simple concept that's just a bit complex to build. But there's a sneaky catch:

If you go from a `3` to a `7` on the keypad, you can go either `^^<<A`, `<<^^A` or even `^<^<A`, `<^<^A`, `<^^<A` and `^<<^A`. So there are six options, and they all are of equal length, in amount of steps. But because a robot arm would have to move between the different buttons to push, the first two options are of equal length for the first robot, and the other four options are longer for that first robot (as he has to switch in-between presses between the buttons). For the second robot, those first two scenarios are of equal length too, but just a bit of a different pattern. But for the third robot, steering the second robot, the required number of presses suddenly gets totally different! Let's simulate pressing a `7` coming from the `3` on the keypad for the first two options I mentioned (each indent being another robot in the chain):

```
^^<<A

^
    <A
        v<<A
        >>^A
^
    A
        A
<
    v<A
        v<A
        <A
        >>^A
<
    A
        A    
A
    >>^A
        vA
        A
        <^A
        >A

result: v<<A>>^AAv<A<A>>^AAvAA<^A>A

<
    v<<A
        v<A
        <A
        A
        >>^A
<
    A
        A
^
    >^A
        vA
        ^<A
        >A
^
    A
        A
A
    >A
        vA
        ^A

result: v<A<AA>>^AAvA^<A>AAvA^A

```
So as you can see, the two paths travelled over the keypad initially are of equal length (either `^^<<A`, `<<^^A`), which for the second robot still goes. But for the third robot the required amount of presses is totally different! So as for the shortest route, in *this particular* case, it is shorter to first go to the left and then go up over the keypad. But of course, that would not be possible if you would go from the `A` to the `4`, a path that requires the same amount of steps in the same directions, because you can't go over the gap on the bottom left! So there, despite being longer, going up twice and then left twice is the best option.

So, for determining the shortest path, we have to take into account a few things:
- You cannot go over the gap on the bottom left of the keypad or over the gap on the top left of the numpad.
- Taking an equal amount of steps for the first robot not necessarily means it's also the shortest option for the last robot in the chain.
- An input digit (like `7` in this example) does not relate to a number of steps, because it is greatly influenced by the last position of the arm over the keypad. So what's true for the first `7` you encounter does not apply to the next `7`. Actually, if there would be two `7`s in a row, all robot arms could just stay in position (over the `A`) and press once more and that's it. Beceause pressing `A` on the last presses the `A` on the next robot, presses `A` on the next, pressing the `7` again, because that's still where the arm of the first robot is. So the difference in total sequence between one or two `7`s in a row is the difference between `v<A<AA>>^AvA^<A>AAvA^A` and `v<A<AA>>^AvA^<A>AAvA^AA`.

I initially work around this finding by, for each digit within the given code, try both ways (going up or down first, and then going left or right first), and then just pick the shortest route. To accommodate this, I built in methods to store the position of the robot's arms, try both scenarios, restore the position of all arms back to the position stored before, and then replay only the best scenario. This is needed because if you just try different scenarios the positions of the arms will all get messed up, as they influence the next move. Or I thought so. I didn't really like this workaround but it gave me the right answer, so I left it in.

But after finishing part 1 and finding the correct answer (by trying the different paths I've found each time), I started seeing a pattern in the debug output. When it needed to go left, it always chose to go left first before going up or down. Otherwise, it always chose to go up or down first before going right. So I removed my backup, restore and test code and just built it in like that. This is a much neater approach and way less code. I assume it's just always the case that if you would need to go left, it's cheaper to do that first, and when you need to go right, it's cheaper to do up or down first. And I also learned from looking at my output, that only the first robot's arm will stay in position between codes, but obviously, that isn't true, as each robot has to end with pressing the `A` to make the next robot push it's button. So after a full cycle of the whole chain, all keypad robots are over the `A` again, leaving only the numpad robot hanging over its last pressed button. These observations cleaned up the code a lot and made it 3 times faster, which is very helpful for part 2.

*Solution for part 1 uploaded, yet to explain here*

## Part 2

With the removal of the workaround and a few fixes, I was able to increase the performance of my solution considerably. I changed the counters from an `int` to a `long`, and I removed concatenating the output sequence (which I used to retrieve the length after each code), but instead now just retrieve the length of each single move (like `<^A`) and add that to my counter while I go. That save a whole lot of string concatenation and builds up the answer right away pretty much (apart from multiplying it with the numeric part of the code afterwards). Now, a chain of 10 robots took only 78 ms, 15 robots took 2 seconds, 17 robots took 12 seconds, and 20 robots took 3 minutes. So I was getting close, but obviously after that the runtime explodes without further optimization.

*Still working on part 2*