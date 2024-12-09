# Solutions to Day 9: Disk Fragmenter

Here are my solutions to the puzzles of today. Written chronologically so you can follow both my code and line of thought.

## Part 1

Another fun challenge today! It took me a little while to come up with the best data format to store the result in, and while it may seem inefficient to keep track of individual blocks, it is the easiest approach and we also need that information later on when calculating the checksum, so I landed on a `List<int>` to store all the blocks. Then I looped through the input and alternated adding the amount of free blocks or the amount of blocks of the current ID.

I then created a function named `freeSpaceIndex` that found the first free block and return its index. It starts at the position of the last free block (from the previous block move) and does not go beyond the index of the block we're trying to move, because we may only shift blocks to the left and not to the right.

Now I can go over all blocks once starting at the back, and request the index of the first free block, and than move it over. The moment I do not find any free spaces left, I stop.

Lastly, I go over the list of blocks from the left, and calculate the checksum (ID multiplied by index).

## Part 2

I somewhat expected this to be the task for part two, but still, my data format isn't really suitable I think. Maybe keeping track of blocks (ID, index and length) as a unity would've been more efficient, but I decided to stick with what I have for solving the puzzle first. So I changed the `freeSpaceIndex` function to also accept a length, and kept track of the absolute first free block separately, since the leftmost free space that can fit the current file isn't always going to be the first free block in the list. So I will start iterating from the first free block within the entire list, and when I find an empty block, I will test if the length of consecutive empty blocks will accommodate for the whole file length. If the file will fit, I return the start index of the free space, and otherwise, I will continue my search.

Then I changed my algorithm to move over blocks. When I find a block that isn't empty, and with a lower ID than the last block (since I can only move a block once), I now first start looping backwards from that index to find the first block of the file. With that, I also now know the length of the file. And with the length and the start of the file (which will be the right boundary for the space search) I can retrieve the index of a suitable free space, if there is any. If so, I move over all individual blocks.

The checksum logic didn't really change, only that I now go through the entire list instead of stopping at the first free block.