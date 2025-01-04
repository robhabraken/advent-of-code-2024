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

### The solution

With the concept clear and the sneaky catch discovered, it still took me a few hours to build a decent solution. It requires a bit of code (it is by far my longest source code of AoC 2024), but also the concept just is quite complicated and it takes a lot of focus to constantly make the right checks, loops and if-statements to cope with this level of repeating interwoven patterns. To get things clear in my head I coded up a base class for a `Keypad` definition, that holds the x,y-coordinates over that keypad (of the arm actually), and some functions to calculate the distance to move towards a certain button, and to produce the list of symbols to describe that move (`<`, `^`, `v` or `>`). Then I made two inheriting classes `NumericKeypad` and `DirectionalKeypad`, each containing a `char[]` to describe the layout (referenced by the x,y-coordinate system), a starting position (the coordinates of the `A` button) and a `MoveTo()` function that calculates the distance between the current and the desired location over the keypad, and builds up the required sequence for that move. These classes are actually the largest part of my solution and also the heart of the logic.

Now we need a few more things: create a numpad object, an array of directional keypads, a function `typeCode()` that processes a code on the numpad and fires up the recursion to start iterating over the different robot arms, and a `pressButtons()` function that recursively loops over the robots in the directional keypad array and does a DFS to the end of the chain to find each consecutive sequence of buttons to press for each robot following the previous one's movements (or actually directing those movements, but I approached it from the perspective of the numeric keypad) . Actually though, since there's only one path, we don't actually need recursion, but it was the only thing I could come up with to keep things easy to read and clean. And it's very fast as well.

## Part 2

That's quite a chain of robots! And while the number isn't super high on itself, the length of the required sequence to steer the first robot's arm will grow exponentially. So we need to speed things upa a bit!

With the removal of the workaround mentioned above and a few other fixes, I was able to increase the performance of my solution considerably. I changed the counters from an `int` to a `long`, and I removed concatenating the output sequence (which I used to retrieve the length after each code), but instead now just retrieve the length of each single move (like `3` instead of `<^A`) and add that to my counter while I go. That saved a whole lot of string concatenation and builds up the answer right away pretty much (apart from multiplying it with the numeric part of the code afterwards). Now, a chain of 10 robots took only 78 ms, 15 robots took 2 seconds, 17 robots took 12 seconds, and 20 robots took 3 minutes. So I was getting close, but obviously after that the runtime explodes without further optimization.

Before I continued with any optimizations, I tried to hunt for a pattern. Could I extrapolate anything? Is there a recurring pattern that could predict the answer? Can I use math? I think I'm quite confident that the answer to all of those questions is no. So there's only one thing left: memoization. The thing is though, my recursive solution doesn't really build up any answers longer than a few characters, so memoization isn't going to do a lot there (I thought) - but it's also very hard to implement (I think) as I don't have anything to store other than a single button, move or character. What also should be taken into account, the resulting sequence for a specific move is not only determined by the target button, but also by the current location of the arm (or the originating button if you like). I tried a few things, but couldn't make a big difference by storing output for my current solution.

Then I came up with the idea to rewrite my code, moving away from recursion, and building up a chain: each directional keypad has a reference to the next keypad in line, and I will build up the entire resulting sequence for a single robot, and pass that through to the next as a whole. Now I had a lot more and more importantly longer values to store in my robots memories! But funny enough, this solution was *way* slower! The reason for this was simple: I was doing a lot more string concatenation, and the deep looping requires a lot more memory allocation I think.

After spending a whole day of trying, I had to leave for a while and decided to just give it a shot. Extrapolating `T(15) = 2s`, `T(17) = 12s` and `T(20) = 180s` gave me an estimate of at least four hours. So I let it run while I was afk, and sure enough, after 6.5 hours, I got my answer. So, the code works, I can configure any number of robots and the logic stands, but I did not yet succeed in finding the proper way to implement memoization into my solution. So that's still on my to-do list!

### Memoization: 158,733,678 times faster!
As mentioned, I *knew* I needed to implement memoization, and I did a few attempts, but initially couldn't figure out *how*.  Turns out I was very close, but a little off too. Initially I tried to cache the sequences, not the count of button presses. And I also didn't understand at first how caching individual robot movements made a big enough impact. But then I saw that if a robot at a certain point would cache the number of presses, it would actually do so cumulatively for all robots behind it. And that would only be possible (or work optimally) if memoizing the count and not the sequence itself. Another characteristic of this conundrum, is that there are only five buttons on the keypad, and no matter what the move is you're doing on the numpad, there is only a limited number of possible moves for each robot over the keypads (20 to be precise). So there is a lot of repetition going on in the pattern.

What I did do right in my earlier attempt was to store the cache specifically for a move (from button `<` to button `^` for example) and not for the target button only, as the sequence differs depending on where the arm is coming from. And also that I needed to cache the result per robot. So I kept those but changed the implementation in a few areas.

First of all, I need a super lightweight cache object. Since I am only going to story counters, `long` values would do. In my first attempt I created a tuple that stored the current location (x,y-coordinates in integer values) of a robot arm, the intended button to press (a `char`) and the resulting sequence. Apart from the fact that caching the sequence instead of the count was wrong, this also created a too complex type to my likings. So I changed that to at three-dimensional long array:
```
var cache = new long[5, 5, nboRobots];
```
The first dimension indicates the current location of the robot arm (but in a single integer value instead of x,y-coordinates). The second dimension represents the target location of the robot arm. And the third dimension identifies the robot, in which I will store the number of presses. So this cache, when filled, can keep track of the number of presses for each robot moving from one to another button.

Then I changed my `pressButtons()` function. First, I transform the current position of the current robot's arm from an x,y-coordinate to a single integer, and the `char` value of the button also to an integer index (same one-dimensional index):
```
var current = robots[robotIndex].y * 3 + robots[robotIndex].x - 1;
var target = button switch
{
    '^' => 0,
    'A' => 1,
    '<' => 2,
    'v' => 3,
    _ => 4
};
```
My original function only added the length of each individual sequence to the total length, but didn't keep track of the added length at the location of each robot, so I now store the length before I compute the sequence so I can subtract that later to determine what is added at this stage (at the location of this robot in the chain). And after computing the sequence, I store that into the correct cache location:
```
var lengthBefore = length;

var sequence = robots[robotIndex].MoveTo(button);
if (robotIndex == nboRobots - 1)
    length += sequence.Length;
else
    foreach (var move in sequence)
        pressButtons(move, robotIndex + 1, ref length);

cache[current, target, robotIndex] = length - lengthBefore;
```
Lastly, *before* computing the sequence, I first check if there's already a count know for this move for this robot, and if so, I just add the length from the cache to the total length instead of actually computing the sequence (and all the robots after that point!).
```
if (cache[current, target, robotIndex] > 0)
{
    length += cache[current, target, robotIndex];
    robots[robotIndex].MoveTo(button);
    return;
}
```
This algorithm will actually move all the way to the back of the chain of robots to build up the cache, because for the first few moves, it doesn't have any cache. And then the cache starts to fill up from the end. The result will be passed all the way to the front, adding up the count for all robots towards the front. But once a robot earlier in the line (closer to the keypad) knows what the count will be, the chain stops - it doesn't need to request info from robots further down the line, nor does it need to access the cache at that level anymore!

There's one more important thing to do by the way. Before returning out of the recursion when we utilized our cached data. And that is to actually move the robot arm to the new position anyway! If we woulnd't do that, the next move would start from the wrong position (which would only go wrong if it wasn't in the cache already). The amount of cache request is very low, opposed to what you might expect given the huge impact of memoization (because when a robot knows the answer the chain isn't completed for a button press), so it isn't really expensive to still move the robot arm when retrieving the cache anyway.

And that's it! This change wasn't all that difficult, and once I figured it out in my head it only took a few minutes to build. The performance improvement is absolutely astonishing though: without memoization, 25 robots took me 6,5 hours to run (which I did while being afk and I just wanted to collect the star before continuing to the next day of AoC) - now, with this relatively small change of implementing memoization, it only takes **0.1498 milliseconds**!

The total size of my cache object is `5 * 5 * 25 = 625` for 25 robots. And with a simple test I learned that my cache was queried `591` times. So it is almost fully utilized. But also, because of the memoization building up towards the front of the chain, it doesn't take much more calls to `pressButtons()` to come up with the answer: this function now is only called `1012` times in total, `421` of which are actually gathering data, computing the actual sequences and building up the cache. Which clearly shows how effective this memoization is, and also why it is so fast now.