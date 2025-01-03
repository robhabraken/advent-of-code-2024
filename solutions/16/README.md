# Solutions to Day 16: Reindeer Maze

*For the puzzle description, see [Advent of Code 2024 - Day 16](https://adventofcode.com/2024/day/16).*

Here are my solutions to the puzzles of today. Written chronologically so you can follow both my code and line of thought.

## Part 1

I built a weighted and direction-aware A* search algorithm, with a default cost of `1` and a penalty of `1000` for each directional change. I again used my `deltaMap` for indicating, storing and detecting directions. The directional change is detected by changing the default priority queue that would contain a list of nodes, to a `List<Tuple<Node, int>>` collection, so I could store each node for each direction towards it. After completing the search, the `minCostToStart` of the end node is the answer to part 1 of today's puzzle.

## Part 2

While I very quickly solved part one, it took me a few days to solve the second part of today's puzzle. A* is great for finding *a* shortest path, but finding *all* shortest paths is a lot more difficult. If all steps would've been equally expensive, I could've probably just also stored nodes with the same cost instead of only lower ones (`cost <= connectedNode.minCostToStart` instead of `cost < connectedNode.minCostToStart`), but the fact that there's a penalty on turning makes that impossible: the algorithm will always choose going straight ahead over turning. Every alternative path would in the end be equally expensive, but will have a turn before a straight section instead of the other way around, given the nature of A* and this 'turning penalty' requirement. So after a few tries I gave up on modifying my search algorithm and started thinking about adding additional searches. I've tried three ideas:
- optimized BFS, but that's way too slow with 10.000 nodes and so many connections;
- picking unvisited nodes and doing two searches using my A* method (one to the start, one to the end) and adding the costs of both paths to see if that is a node on a path that's equally expensive.. but although that kind of worked, I had to factor in an off-by-one on turns in both ways, so that got too complex, and also too slow since I have to check many nodes twice;
- doing an A* search from each node of the shortest path to another node on the shortest path, looping over all nodes of the shortest path nested, and see if the distance in-between was the same as their current distance - this actually came close, but gave some unexpected results and was quite complex, so I also dropped that idea.

And then came day 18 part two, with the task to check if adding an extra obstacle would still make a path possible.. and that got me thinking about using that technique for this puzzle as well (I used a simpler A* version for that day which worked great). Then, a colleague also mentioned such solution, which encouraged me to try this idea. And that actually worked quite well! And I got my answer straight away.

What I did was the following:
- I added a `buildPath()` function to build up a path based on a completed search, starting at the end and adding each nearest (cheapest) next node to the list from there.
- I moved my original code to build up the nodes and their connections from the map into a function named `initSearch()` and also perform the search in there, returning the total cost of the shortest path found (or `int.MaxValue` if the end isn't reached and the path isn't possible, to rule out this path as an option, with the highest cost possible).
- I've added x,y-coordinates as arguments to the `initSearch()` function so I could block of a specific location on the map. This places a `#` on the given position, so I can test a full search on the map with that change in place and check the outcome.
- Then I perform a search with negative blocking coordinates (in other words, not blocking any node), and build the actual shortest path first, storing the minimum cost (the answer to part 1) for later reference.
- Lastly, I've created a `HashSet<Tuple<int, int>>` to store all unique locations where we can place our seat (the answer to this part of the puzzle) and start looping over the original shortest path. For each node I check if it is not the start, not the end, and if it doesn't have only two connecting nodes: in this case it is part of a corridor and blocking this wouldn't change anything to the outcome, since blocking crossings only would be sufficient to reroute the algorithm. This brings down the possible search locations from 400 nodes (including start and end nodes) to 92 actual nodes to test and run my A* on. For each of these crossings I call `initSearch()` while blocking that exact location. If the cost of the path that is found is equal to the minimum cost, it is an alternative, and I add all locations of this new path to my list.

It is the only solution yet that doesn't run in under a second, but given the challenge this puzzle was for me, and the initial failed brute-force attempts, a runtime of 82 seconds is acceptable for now. Maybe after AoC I will look up solutions of others and see how this could be done faster!

### Speeding up the search
Update: as promised, I would get back to this solution to see if I can get if faster. My goal is to be able to solve all puzles subsecond, and I'm almost there! I first just analyzed my own code, and found a huge improvement. Then, I started measuring the runtime of each individual function and each part of the algorithm, to find the weak spots in terms of performance. This way, I was able to improve the following things, in the following order:
- As described above, for part 2, I moved the initialization of the graph into a method and made it configurable so I could block a single node on the shortest path. This actaully was quite lazy and far from the best solution, as I just mostly reused my part 1 code and also built up the entire graph over 90 times. Reviewing my solution with fresh eyes, I saw that it was a far better idea to just skip the node to block in the search itself. So I removed the addition of blocking a node in the `initSearch()` function and renamed it to `setupGraph()`, because I now only need to do that once. But since I'm not creating new objects for each new search, I need to reset values like `minCostToStart` and `visited` between evey search, so I also added a `resetGraph()` function that replaced the initalization of the graph between every new search. Then, I added an extra parameter to the `search()` function, a nullable `Node` object named `blockedNode`, so I can let the Dijkstra algorithm skip a certain node. From a functional perspective, the concept behind the solution is still exactly the same, but it cut down the execution time from 82 seconds to 2.7 seconds for part 2!
- I then also changed the storage of seats from a `HashSet<Tuple<int, int>>` to a much simpler `HashSet<int>` collection, storing the unique location of a seat as a one-dimensional array index: `node.y * lines.Length + node.x`. This saves me creating new tuples for every seat I've found (multiple) times, and is just a little bit faster.
- Then, I decided to remove the A* optimalization to the Dijkstra algorithm, and remove the calculation of the absolute distance and the component in the priority queue that sorts on that value. While this normally should speed up the Dijkstra search, it turned out to actually be just a little bit slower for this map. This saved me some more time and I was now down to roughly 2.38 seconds for part 2.
- I now realized that that is pretty fast, as it does multiple searches and part 1 alone already took 840 ms. And that's strange, because if the weight of the algorithm is in the search, searching exactly 92 times instead of just once should be just 3 times slower, but a lot slower! That's why I also initially didn't think the 82 seconds runtime of part 2 were strange, or easy to improve. But now I got that down to under 3 seconds, I realized the weight in part 1 wasn't in the search itself, so I switched my attention to part 1. After some measurements, it turned out my graph building algorithm was super inefficient. I store my nodes in a `List<Node>` and loop over all nodes, for al nodes, and for each pair of nodes I check if they are neighbors. But the majority of the other nodes aren't neighbors, and the graph is just an array, a simple map. And neighbors only can occur right next to another (this graph doesn't have distances). So it would be far more efficient (and logical) to store the nodes in a two-dimensional `Node[,]` and only check for neighbors in the adjacent cells of each node. So I changed this part of the `setupGraph()` function and funny enough, as it's the case with most performance optimizations (up to a certain point) the code came out smaller and less complex.

I went from:
```
foreach (var node in nodes)
{
    foreach (var otherNode in nodes)
        if (node != otherNode &&
            ((Math.Abs(otherNode.x - node.x) <= 1 && otherNode.y == node.y) ||
                (Math.Abs(otherNode.y - node.y) <= 1) && otherNode.x == node.x))
        {
            var e = new Edge(otherNode, 1);

            for (var i = 0; i < 4; i++)
                if (otherNode.y - node.y == deltaMap[i, 0] && otherNode.x - node.x == deltaMap[i, 1])
                    e.direction = i;

            node.connections.Add(e);
        }
    node.totalDistance = Math.Sqrt(Math.Pow(node.x - end.x, 2) + Math.Pow(node.y - end.y, 2));
}
```
To: 
```
for (var y = 1; y < lines.Length - 1; y++)
    for (var x = 1; x < lines[0].Length - 1; x++)
        if (nodesArray[y, x] != null)
            for (var i = 0; i < 4; i++)
                if (nodesArray[y + deltaMap[i, 0], x + deltaMap[i, 1]] != null)
                    nodesArray[y, x].connections.Add(new Edge(nodesArray[y + deltaMap[i, 0], x + deltaMap[i, 1]], 1, i));
```
This turned out to be an amazing performance improvement, my part 1 solution went down from 840 ms to 19.1 ms! So the majority of the runtime went into building the graph in a super inefficient way (which I also repeated lots of times in my initial part 2 solution).
- Back to part 2, applying the same improvement of setting up the graph, my runtime now is only 1.24 seconds!
- The steps are getting smaller, but we still need to find a little bit of time. I went over the Dijkstra algorithm one more time and got the idea to remove the `OrderBy(x => x.Cost)` when looping over the edges of a node, since all neighbors are at an equal distance, so the edge cost actaully is always the same. Then I removed the `cost` attribute from the `Edge` object altogether and just increased the cost by `1` for each step. And I also removed the temporary local variable `var connectedNode = c.ConnectedNode;` as this was just copying over the reference to the connected node into a variable, so it wasn't necessary as I could instead just reference `c.ConnectedNode` all the time. It's a small improvement, but as this happens for all edges of all nodes that the algorithm checks, and then for all crossings to check for alternative routes, it *does* make a difference. With these optimizations of the Dijkstra algorithm part 1 went from 19.1 to 18.8 ms, and part 2 went from 1.24 seconds to 1022 ms.
- And that last step made me realize that I could maybe drop the `Edge` object entirely, also removing the initial linking of the connected nodes, and removing the `List<Edge>` attribute named `connections` from the `Node` class. This saves me storing duplicate references to nodes, and also the initial loop of setting up the connections. I renamed those functions to `initNodes()` and `resetNodes()` as I actually do not set up a graph anymore, and changed my Dijkstra algorithm one more time: instead of looping over the connections in the eponymous collection, I now iterate over the integer values of the different directions (0 to 3) and just retrieve a neighbor from the `nodesArray` if present. It's a little less pretty I think, but it wokrs equally well. Except the fact that I checked the number of connections to determine which tiles of the path are on a crossing, which isn't possible anymore, so I introduced a new function `countConnections()`. With these changes, my runtime dropped from 1022 to 988 ms, reaching my subsecond goal!

### Part 2 revisited
And then, after speeding up my part 2 solution from 82 seconds to 988 ms purely by performance optimizations, I decided to start over with a new concept. This new concept should be way quicker, and is based on the characteristic of the found shortcuts being only 'other ways around a square block`:

```
.###.#.#.
OOOOOOO#.
.#v###O##
.#v#.#O#.
##v#.#O#.
.#>>>>OOO
.#####.#.
.........
```
This is something that isn't the case for the example input, but for the real puzzle input (and I've seen multiple) this seems to be true for any shortcut. Which makes sense, because an entirely different route would almost always need a different amount of turns in such a large graph, or a different step count. So the deviations to the shortest path are actually quite minimal.

So I reverted back to my (new and optimized) part 1 solution, and added a few new functions:
- `findShortcut()` checks a crossing on the shortest path. If there is a possibility to escape the shortest route, it will start walking in each possible direction other than the shortest path itself. This is easily done by looping over the directions: if an adjacent tile is null, it's a wall, and if the adjacent tile is already contained by the shortest path, it's not what we're looking for. In any other case, we can start walking in that new direction. But before I do, I look at the tile on the opposite side of the originating node, because if that's on the track, we didn't change direction - otherwise, we did. This is important to know, because shortcuts that go off and on the track should have the same amount of direction changes. Because it's a corner, one in the middle of the shortcut is allowed (this is what the bool `cornered` is for), but for leaving and joining the shortest path, only one of those should include a direction change to have the same cost in the end.
- `walk()` is a recursive function that is initiated by `findShortcut()` when it's a plausible shortcut. This function will walk one node at a time and at each new crossing, it will call itself again. But once it corners to do so, `cornered` is set to true, to prohibit further direction changes on that new path. When `walk()` runs into a wall (`node == null`) it stops. When it runs into the shortest path again, it checks if the found shortcut is of the same length as the part it cut off, and if it found a path in the same direction (we don't want to count shortcuts going backwards). One last check is to see if either the initial `findShortcut()` changed direction, or joining the track again changes direction (same check, but now look *past* the node on the path where we're joining it): one is allowed to be true, never both (and neither being true cannot occur since that wouldn't be leaving the track in the first place) - hence our `XOR` check. Now if we found a valid shortcut, we will add those nodes to the `seat` collection and we're done.
- The function `onPath()` is used by the two functions above to determine if a certain node lies on the shortest path.

With this new approach, I only have to do one single Dijkstra search, and I only have to traverse the path once, with a few recursive BFS searches with a very limited reach (reaching a wall or doing more than one turn stops a search). This means it's way way quicker than my original approach. This new solution runs in only 17.2 ms!

#### A word of notice
I realize this approach is purely focussed on the characteristics of the puzzle input, but I think that is valid because AoC does focus on a very specific problem anyhow. What might be confusing, is that example 2 in the puzzle description wouldn't correctly be solved by this approach, but example 2 also isn't representative looking at the real puzzle input! On the other hand, my original solution felt more generic initially, and was actually able to solve example 2, but.. it didn't solve example 1! And that's because this contains a very specific edge case of a center route between the original path and an alternative shortcut. Weighted Dijkstra wouldn't be able to find this path (because it always prefers going forward instead of turning because of the extra cost, and blocking either corner on the outside ring doesn't force it to go through the center). And as it occurs, although my personal puzzle didn't include this 'edge' case, I've seen input from others that did. So I think that example 1 is more representative, and thus my new solution actually is more generic looking at the puzzle input than my original approach (of course, only based on the input variations I have seen).