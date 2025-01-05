# Solutions to Day 9: Disk Fragmenter

*For the puzzle description, see [Advent of Code 2024 - Day 9](https://adventofcode.com/2024/day/9).*

Here are my solutions to the puzzles of today. Written chronologically so you can follow both my code and line of thought.

## Part 1

Another fun challenge today! It took me a little while to come up with the best data format to store the result in, and while it may seem inefficient to keep track of individual blocks, it is the easiest approach and we also need that information later on when calculating the checksum, so I landed on a `List<int>` to store all the blocks. Then I looped through the input and alternated adding the amount of free blocks or the amount of blocks of the current ID.

I then created a function named `freeSpaceIndex` that found the first free block and return its index. It starts at the position of the last free block (from the previous block move) and does not go beyond the index of the block we're trying to move, because we may only shift blocks to the left and not to the right.

Now I can go over all blocks once starting at the back, and request the index of the first free block, and than move it over. The moment I do not find any free spaces left, I stop.

Lastly, I go over the list of blocks from the left, and calculate the checksum (ID multiplied by index).

### Alternative solution
After submitting my answer, I came up with an alternative solution that I thought would be way faster. Instead of converting the input to the actual blocks, I only go over the actual input once from left to right. Then, when it's a file block, I calculate the checksum for each position and add that to the answer right away. If it's an empty space, I call a function named `takeLast` that takes the last file block from the same list, decreasing the block counter of that last block by one. Then I calculate the checksum based on that new block on my current position and move on. This way, I do not store any blocks in a list and I don't do any list permutations. I build up the answer right away (whereas for the first version I actually wrote out all blocks and then went over them to calculate the checksum). This solution turned out to be three times as fast and one less line of code! I replaced my original solution, and moved the original solution to the file `Original.cs`, excluded from the Build action, as it's only there for reference.

### More speed for part 1!
Part one already was quite fast with ~1 ms runtime. Just for fun, I tested the impact of changing the integer parsing to the new method as mentioned below for part 2: `diskmap[i] - '0'`. And quite to my surprise, part 1 now runs in 0.4987 ms solely because of this small improvement.

## Part 2

I somewhat expected this to be the task for part two, but still, my data format isn't really suitable I think. Maybe keeping track of blocks (ID, index and length) as a unity would've been more efficient, but I decided to stick with what I have for solving the puzzle first. So I changed the `freeSpaceIndex` function to also accept a length, and kept track of the absolute first free block separately, since the leftmost free space that can fit the current file isn't always going to be the first free block in the list. So I will start iterating from the first free block within the entire list, and when I find an empty block, I will test if the length of consecutive empty blocks will accommodate for the whole file length. If the file will fit, I return the start index of the free space, and otherwise, I will continue my search.

Then I changed my algorithm to move over blocks. When I find a block that isn't empty, and with a lower ID than the last block (since I can only move a block once), I now first start looping backwards from that index to find the first block of the file. With that, I also now know the length of the file. And with the length and the start of the file (which will be the right boundary for the space search) I can retrieve the index of a suitable free space, if there is any. If so, I move over all individual blocks.

The checksum logic didn't really change, only that I now go through the entire list instead of stopping at the first free block.

### Alternative solution
And for part 2 as well, I came up with an alternative solution (as mentioned at the start of part 1): only storing and working with files instead of single blocks. This solution turned out to be more elegant and less LoC, though it is in fact a little slower, against my expectations! I now created a class `AmphipodFile` with an `id` and a `size` and go over the input once, storing either a file block, or an empty space (indicated by setting `id = -1`). When I have parsed the input, I  go over all files starting at the back. When I encounter an actual file, I then start search for an empty space from the start that would fit, and move my file into that space. The tricky thing here is that if my file is smaller than the empty space, I now need to insert an extra bit of empty space the size of the delta. This actually works quite well and doesn't take much logic. When finished, I once again iterate over the list of files from left to right, and increment the checksum with each file block and its index. It's a totally different approach, and though it is somewhat slower than my initial version (255ms to 172ms) I found this one interesting enough to add to my repo as an alternative (see `Alternative.cs`, not included in the build).

### Another shot at speeding up the defragmentation
Still not satisfied with the runtime of part 2, I decided to start from scratch once more to see what I could achieve. I tried the following to improve the performance:
- Use as less variable declarations as possible.
- Use a faster integer parsing method: `diskmap[i] - '0'` over `int.Parse($"{diskmap[i]}")`.
- Only use arrays as collections, no `List` objects.
- Try to loop over the entire data set as few times as possible.
- Don't use complex types or objects to keep track of any data.

To achieve this, I changed my parsing a little bit - because we don't know how many fileblocks we will end up with (because that's the sum of all parsed block lengths), I initially used a `List<int>` object so I have a dynamic collection to add to while parsing. This time, I created an `int[]`, but to be able to set it to the correct length, I went over the input once, adding up all block lengths. This looks like it's a penalty and actually takes up more time, but the way I'm parsing the integers now makes this super fast, and it outweighs using a dynamic list by far.

Then, I loop over the diskmap once more, keeping track of the needle on the drive (at which block I'm at, or in other words, the index in the block array), and adding the files and empty spaces while I go. Looking back after building this new version, unsurprisingly, it looks a lot like my original version, but it's a little more efficient - not only due to the data type and the integer conversion, but also because I do not declare an extra variable for the id and the block length.

The main loop to perform the defragmentation looks the same too, although this too is a little more compact. I didn't use an extra function to find the free space and combined everything in one loop, but other than that it is quite similar. My approach is slightly different: first I found a new block, and then searched for the beginning of the file going backwards, determining the length this way. Now I just loop backwards over the blocks continuously until I find the first block of a file, and then quickly walk back forward to determine the length. It's about the same amount of iterations, so that shouldn't differ much. My 'finding the first empty block and setting the absolute first index to avoid having to start over at the beginning of the blocks every time' approach also is a bit similar, though by combining it into the main loop I'm saving up some extra variable declarations.

Finally, when I either reach the first file (will never happen actually) or my first empty space index and my current file index meet, I break out of the loop because there's no free space on the left side of the current file anymore. In my original solution, I think I didn't do this as it just stops finding a free space but it runs all the way down to the first file in the input.

I wrote this new approach a few weeks after the first one, entirely without looking at what I've done before, and focused purely on performance. Funny enough, I don't think there's that much of a difference when looking back - only avoiding the `List` usage, bailing out a little earlier, and using the slightly faster integer conversion. Still, with this new version I was able to shave of almost 50 ms: my original ran for 172 ms, this new approach only takes 124 ms. It's still not super fast, but it's a little bit better nonetheless.