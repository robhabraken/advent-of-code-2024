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