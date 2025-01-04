# Solutions to Day 18: RAM Run

*For the puzzle description, see [Advent of Code 2024 - Day 18](https://adventofcode.com/2024/day/18).*

Here are my solutions to the puzzles of today. Written chronologically so you can follow both my code and line of thought.

## Part 1

Today felt way more doable after last two days. It seems like it's just a regular path finding algorithm in-between a number of obstacles (falling bytes this time). So I re-used the A* algorithm from day 16, but removed all direction and weight related code, as we can navigate through this memory space without any restrictions other than just avoiding the fallen bytes. So `Node` now only has a collection of connecting nodes instead of 'edges' with a certain direction and weight. And our `search()` algorithm cna be quite straight forward.

Then onto the parsing. I created a two-dimensional `bool[,]` to represent the memory space and read `1024` of bytes from the input, marking each location in the memory space as `true` when a byte fell onto that location. After that I can create my list of nodes, building up all possible connections between all of the nodes (looping over my `deltaMap` and checking if there's an obstacle in each direction and if not add that node to the list of connections).

When the bytes are read and the map is built, I can execute a search. After searching I use `buildPath()` to loop over my nodes, starting at the end node, finding the cheapest (`nearestToStart`) node each time. This gives me path, of which the length is the answer to part 1 of the puzzle - the number of steps taken when following the shortest route through the memory space while avoiding fallen bytes.

## Part 2

For part 2, which also for once was quite easy, we could use most of the same code. I removed the `buildPath()` section because we do not need to know the path anymore, only _if_ we are able to arrive at our destination. Then I moved the code that reads the bytes from the input and builds up the memory space into a function named `endReachable()`. The only argument is the number of bytes to read from the input (which was first set to a fixed value of `1024`) and it returns `end.visited`, which is a simple and easy to check if the search algorithm was able to make it to the end.

Now we _could_ just run that function for every additional byte starting from `1025` and that certainly works, but it's not very fast, not very pretty, and also not necessary. What I did was build a *binary search* over that, testing if the end is reachable or not. So I start halfway through the input file, and if the end is reachable at this byte, I narrow my search to the right half, otherwise to the left. And then I put that into a loop, and do as many tests as needed until the starting index of my search equals the number of bytes I put in, which means I've found the first byte in the list that makes the end unreachable. The average runtime of this binary search doing multiple A* searches is only 525 ms.

The answer then is the line at the number of bytes I put in minus one (as the number of bytes read isn't zero based but the index of my lines `string[]` is).

### Speeding up the run
After optimizing my solution to day 18, I came back to this puzzle, as it is based on the same algorithm. I then applied the same learnings and was able to achieve quite a considerable improvement:
- I changed the nodes `List<Node>` to an `Node[73, 73]` array. Mind that I made it two rows and two columns bigger, of which I will only use the index within the outer border, to avoid any boundary checks (less code, better performance). I now story all newly created nodes in the array at `y + 1, x + 1` instead of adding it to a list.
- I removed the A* total distance optimizations, as due to having equal distances between all nodes this doesn't improve but slightly deteriorates performance.
- I removed the `connections` attribute from the `Node` class as well the entire connection graph building section. This isn't necessary as we can always just look at our direct neighbors to discover any connections.
- I introduced my `deltaMap` once more, and made the Dijkstra algorithm loop over the directions instead of over the connections. I don't have to check for array boundaries, only if a neighbor is null - in that case, the location on the map is occupied by a 'byte' and we can't go there.

With these changes, the runtime for part 1 went from 141 ms to 6.16 ms, and for part 2 it went from 525 ms to 9.14 ms! Same logic, same concept, only way more efficient and a better overall data structure.