# Solutions to Day 16: Reindeer Maze

*For the puzzle description, see [Advent of Code 2024 - Day 16](https://adventofcode.com/2024/day/16).*

Here are my solutions to the puzzles of today. Written chronologically so you can follow both my code and line of thought.

## Part 1

I built a weighted and direction-aware A* search algorithm, with a default cost of `1` and a penalty of `1000` for each directional change.

It's a regular A* search algorithm, but instead of marking a tile as visited, I mark each tile as visited per direction, so `visited` became a `bool[]`. And I again used my `deltaMap` for indicating, storing and detecting directions. The default cost per step and thus connection is `1`, but I added a penalty of `1000` for each directional change. The directional change is detected by changing the default priority queue that would contain a list of nodes, to a `List<Tuple<Node, int>>` collection, so I could store each node for each direction towards it. After completing the search, the `minCostToStart` of the end node is the answer to part 1 of today's puzzle.