# Advent of Code 2024
My solutions to the Advent of Code puzzles for the 2024 edition, written in C#.

| AoC Puzzle | Part one | Part two | P1 metrics | P2 metrics |
| :-- | --: | --: | :--: | :--: |
| [Day 1: Historian Hysteria](./solutions/01/) |  1.1 <img src="https://www.robhabraken.nl/img/green.png" title="Solved completely by myself without any help or external input"> | 1.2 <img src="https://www.robhabraken.nl/img/green.png" title="Solved completely by myself without any help or external input"> |  <sub>20 LoC / 0.20 ms</sub> |  <sub>24 LoC / 5.16 ms</sub> |
| [Day 2: Red-Nosed Reports](./solutions/02/) | 2.1 <img src="https://www.robhabraken.nl/img/green.png" title="Solved completely by myself without any help or external input"> | 2.2 <img src="https://www.robhabraken.nl/img/green.png" title="Solved completely by myself without any help or external input"> |  <sub>31 LoC / 0.44 ms</sub> |  <sub>43 LoC / 1.24 ms</sub> |
| [Day 3: Mull It Over](./solutions/03/) | 3.1 <img src="https://www.robhabraken.nl/img/green.png" title="Solved completely by myself without any help or external input"> | 3.2 <img src="https://www.robhabraken.nl/img/green.png" title="Solved completely by myself without any help or external input"> |  <sub>14 LoC / 0.62 ms</sub> |  <sub>23 LoC / 0.58 ms</sub> |
| [Day 4: Ceres Search](./solutions/04/) | 4.1 <img src="https://www.robhabraken.nl/img/green.png" title="Solved completely by myself without any help or external input"> | 4.2 <img src="https://www.robhabraken.nl/img/green.png" title="Solved completely by myself without any help or external input"> |  <sub>21 LoC / 3.02 ms</sub> |  <sub>10 LoC / 0.77 ms</sub> |
| [Day 5: Print Queue](./solutions/05/) | 5.1 <img src="https://www.robhabraken.nl/img/green.png" title="Solved completely by myself without any help or external input"> | 5.2 <img src="https://www.robhabraken.nl/img/green.png" title="Solved completely by myself without any help or external input"> |  <sub>21 LoC / 1.93 ms</sub> |  <sub>35 LoC / 0.76 ms</sub> |
| [Day 6: Guard Gallivant](./solutions/06/) | 6.1 <img src="https://www.robhabraken.nl/img/green.png" title="Solved completely by myself without any help or external input"> | 6.2 <img src="https://www.robhabraken.nl/img/green.png" title="Solved completely by myself without any help or external input"> |  <sub>34 LoC / 0.09 ms</sub> |  <sub>50 LoC / **304 ms**</sub> |
| [Day 7: Bridge Repair](./solutions/07/) | 7.1 <img src="https://www.robhabraken.nl/img/green.png" title="Solved completely by myself without any help or external input"> | 7.2 <img src="https://www.robhabraken.nl/img/green.png" title="Solved completely by myself without any help or external input"> |  <sub>35 LoC / 3.59 ms</sub> |  <sub>37 LoC / **207 ms**</sub> |
| [Day 8: Resonant Collinearity](./solutions/08/) | 8.1 <img src="https://www.robhabraken.nl/img/green.png" title="Solved completely by myself without any help or external input"> | 8.2 <img src="https://www.robhabraken.nl/img/green.png" title="Solved completely by myself without any help or external input"> |  <sub>30 LoC / 0.07 ms</sub> |  <sub>38 LoC / 0.26 ms</sub> |
| [Day 9: Disk Fragmenter](./solutions/09/) | 9.1 <img src="https://www.robhabraken.nl/img/green.png" title="Solved completely by myself without any help or external input"> | 9.2 <img src="https://www.robhabraken.nl/img/green.png" title="Solved completely by myself without any help or external input"> |  <sub>44 LoC /  0.96 ms</sub> |  <sub>77 LoC / **172 ms**</sub> |
| [Day 10: Hoof It](./solutions/10/) | 10.1 <img src="https://www.robhabraken.nl/img/green.png" title="Solved completely by myself without any help or external input"> | 10.2 <img src="https://www.robhabraken.nl/img/green.png" title="Solved completely by myself without any help or external input"> |  <sub>40 LoC / 2.41 ms</sub> |  <sub>31 LoC / 0.67 ms</sub> |
| [Day 11: Plutonian Pebbles](./solutions/11/) | 11.1 <img src="https://www.robhabraken.nl/img/green.png" title="Solved completely by myself without any help or external input"> | 11.2 <img src="https://www.robhabraken.nl/img/green.png" title="Solved completely by myself without any help or external input"> |  <sub>36 LoC / 14.9 ms</sub> |  <sub>47 LoC / 14.8 ms</sub> |
| [Day 12: ..](./solutions/12/) |  |  |  |  |
| [Day 13: ..](./solutions/13/) |  |  |  |  |
| [Day 14: ..](./solutions/14/) |  |  |  |  |
| [Day 15: ..](./solutions/15/) |  |  |  |  |
| [Day 16: ..](./solutions/16/) |  |  |  |  |
| [Day 17: ..](./solutions/17/) |  |  |  |  |
| [Day 18: ..](./solutions/18/) |  |  |  |  |
| [Day 19: ..](./solutions/19/) |  |  |  |  |
| [Day 20: ..](./solutions/20/) |  |  |  |  |
| [Day 21: ..](./solutions/21/) |  |  |  |  |
| [Day 22: ..](./solutions/22/) |  |  |  |  |
| [Day 23: ..](./solutions/23/) |  |  |  |  |
| [Day 24: ..](./solutions/24/) |  |  |  |  |
| [Day 25: ..](./solutions/25/) |  |  |  |  |

### Performance metrics

Each puzzle solution is executed 100 times, excluding the time taken for reading the input file from disk and printing the answer to the console. To account for .NET startup time and potential outliers, I calculate the average (mean) execution time after removing the slowest and fastest measurements. In cases where multiple solutions are implemented for a single puzzle, the table displays the metrics for the fastest or most optimal solution.

### Legend

| Star | Description | 
| :-- | :-- |
| <img src="https://www.robhabraken.nl/img/green.png" title="Solved completely by myself without any help or external input"> | Solved completely by myself without any help or external input |
| <img src="https://www.robhabraken.nl/img/yellow.png" title="Searched online for algorithms or inspiration to solve this problem"> | Searched online for algorithms or inspiration to solve this problem |
| <img src="https://www.robhabraken.nl/img/orange.png" title="Own implementation based on approach as seen on AoC subreddit"> | Own implementation based on approach as seen on AoC subreddit |
| <img src="https://www.robhabraken.nl/img/red.png" title="Wasn't able to solve this myself, implemented someone else's logic to learn from"> | Wasn't able to solve this myself, implemented someone else's logic to learn from |