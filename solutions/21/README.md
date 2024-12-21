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
<
    A
        >>^A
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

result: v<A<AA>>^AvA^<A>AAvA^A

```
So as you can see, the two paths travelled over the keypad that initially are of equal length (either `^^<<A`, `<<^^A`), which for the second robot still goes. But for the third robot the required amount of presses is totally different! So as for the shortest route, in *this particular* case, it is shorter to first go to the left and then go up over the keypad. But of course, that would not be possible if you would go from the `A` to the `4`, a path that requires the same amount of steps in the same directions, because you can't go over the gap on the bottom left! So there, despite being longer, going up twice and then left twice is the best option.

So, for determining the shortest path, we have to take into account a few things:
- You cannot go over the gap on the bottom left of the keypad or over the gap on the top left of the numpad.
- Taking an equal amount of steps for the first robot not necessarily means it's also the shortest option for the last robot in the chain.
- An input digit (like `7` in this example) does not relate to a number of steps, because it is greatly influenced by the last position of the arm over the keypad. So what's true for the first `7` you encounter does not apply to the next `7`. Actually, if there would be two `7`s in a row, all robot arms could just stay in position (over the `A`) and press once more and that's it. Beceause pressing `A` on the last presses the `A` on the next robot, presses `A` on the next, pressing the `7` again, because that's still where the arm of the first robot is. So the difference in total sequence between one or two `7`s in a row is the difference between `v<A<AA>>^AvA^<A>AAvA^A` and `v<A<AA>>^AvA^<A>AAvA^AA`.

*Solution for part 1 uploaded, yet to explain here*

## Part 2

*Still working on part 2*