# Solutions to Day 9: Disk Fragmenter

*For the puzzle description, see [Advent of Code 2024 - Day 9](https://adventofcode.com/2024/day/9).*

Here are my solutions to the puzzles of today. Written chronologically so you can follow both my code and line of thought.

## Part 1

Another fun challenge today! It took me a little while to come up with the best data format to store the result in, and while it may seem inefficient to keep track of individual blocks, it is the easiest approach and we also need that information later on when calculating the checksum, so I landed on a `List<int>` to store all the blocks. Then I looped through the input and alternated adding the amount of free blocks or the amount of blocks of the current ID.

I then created a function named `freeSpaceIndex` that found the first free block and return its index. It starts at the position of the last free block (from the previous block move) and does not go beyond the index of the block we're trying to move, because we may only shift blocks to the left and not to the right.

Now I can go over all blocks once starting at the back, and request the index of the first free block, and than move it over. The moment I do not find any free spaces left, I stop.

Lastly, I go over the list of blocks from the left, and calculate the checksum (ID multiplied by index).

*Edit: I came up with an alternative solution that I thought would be way faster. Instead of converting the input to the actual blocks, I only go over the actual input once from left to right. Then, when it's a file block, I calculate the checksum for each position and add that to the answer right away. If it's an empty space, I call a function named `takeLast` that takes the last file block from the same list, decreasing the block counter of that last block by one. Then I calculate the checksum based on that new block on my current position and move on. This way, I do not store any blocks in a list and I don't do any list permutations. I build up the answer right away (whereas for the first version I actually wrote out all blocks and then went over them to calculate the checksum). This solution turned out to be three times as fast and one less line of code! So I thought it was interesting enough to add this to my repository as well as an alternative approach to my original submission (see file `Alternative.cs`, excluded from the Build action, only for reference).*

## Part 2

I somewhat expected this to be the task for part two, but still, my data format isn't really suitable I think. Maybe keeping track of blocks (ID, index and length) as a unity would've been more efficient, but I decided to stick with what I have for solving the puzzle first. So I changed the `freeSpaceIndex` function to also accept a length, and kept track of the absolute first free block separately, since the leftmost free space that can fit the current file isn't always going to be the first free block in the list. So I will start iterating from the first free block within the entire list, and when I find an empty block, I will test if the length of consecutive empty blocks will accommodate for the whole file length. If the file will fit, I return the start index of the free space, and otherwise, I will continue my search.

Then I changed my algorithm to move over blocks. When I find a block that isn't empty, and with a lower ID than the last block (since I can only move a block once), I now first start looping backwards from that index to find the first block of the file. With that, I also now know the length of the file. And with the length and the start of the file (which will be the right boundary for the space search) I can retrieve the index of a suitable free space, if there is any. If so, I move over all individual blocks.

The checksum logic didn't really change, only that I now go through the entire list instead of stopping at the first free block.

*Edit: And for part 2 as well, I came up with an alternative solution (as mentioned at the start of part 1): only storing and working with files instead of single blocks. This solution turned out to be more elegant and less LoC, though it is in fact a little slower, against my expectations! I now created a class `AmphipodFile` with an `id` and a `size` and go over the input once, storing either a file block, or an empty space (indicated by setting `id = -1`). When I have parsed the input, I  go over all files starting at the back. When I encounter an actual file, I then start search for an empty space from the start that would fit, and move my file into that space. The tricky thing here is that if my file is smaller than the empty space, I now need to insert an extra bit of empty space the size of the delta. This actually works quite well and doesn't take much logic. When finished, I once again iterate over the list of files from left to right, and increment the checksum with each file block and its index. It's a totally different approach, and though it is somewhat slower than my initial version (255ms to 172ms) I found this one interesting enough to add to my repo as an alternative (again, see `Alternative.cs`, not included in the build).*