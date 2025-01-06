# Solutions to Day 24: Crossed Wires

*For the puzzle description, see [Advent of Code 2024 - Day 24](https://adventofcode.com/2024/day/24).*

Here are my solutions to the puzzles of today. Written chronologically so you can follow both my code and line of thought.

## Part 1

Because the given gates wait until they receive a value, I couldn't find the answer by running over the input just once, and thus had to create a data structure to store both the wire and gate data, as well as their current state. So I've created a `Wire` class with a name and a value (but used a `bool?` because the value could also be empty still), and a `Gate` class that references two input wires, an output wire, has an operator type, and a boolean indicating if it is ready processing yet. I also included the code to process the values within this gate in the `Gate` class itself, because I think that's easier and more elegant. This `Process()` function first checks if the gate is already finished processing, and only continues if that's not the case. Then, it checks if both input wires already have a value, and if not, it aborts further processing. This way I can call `Process()` as many times as I want, it won't change it state, won't give an error, and it will only actually perform the gate action once, as soon as the values of both inputs are known (and I'm calling `Process()` of course). An event driven setup would be even better, or at least prettiger, but for the goal of part 1 this suffices.

Then I started reading the input. I first iterate over the *gate* section, not the input values, as the input values or only given for a smaller subset of the actual wires - so if I want to know which wires do exist across all gates, I have to read through the gate section and store every wire mention in both the inputs and the output connections. I then add the three wires to my collection, but also create the gate for those three right away. The wires by the way, are stored in a `SortedDictionary<string, Wire>`, which is super useful, as I can reference my object by the string name of the wire, but also because the dictionary is automatically sorted on the keys, already arranging all `z` wires for the output later on.

After that, when all `Wire` and `Gate` objects are created, I loop over the *first* section, to set the initial values of all wires that are mentioned in this section.

Now the only thing left to do is simulate all gates. I loop over all gates using a do/while-loop as long as any of the gates is still returning me a `false` response, indicating it did not yet receive its input values. For the answer, I concatenate all `z` wires back to front, and convert them from a binary to a decimal (`long`) notation.

## Part 2

I've built two totally different versions for this part: a rather concise version that doesn't even simulate the circuit, and only outputs the answer in a Console app, and quite an elaborate solution that does a full visualization, simulation and animation of the circuit.

### The one I've submitted

I first tried to find a way to reverse engineer the answer starting at the last gates in the chain, but couldn't find a useful clue. As it turns out, there (obviously) is no direct connection between a pair of crossed over wires and an invalid output value of a single bit. Then, I started writing lots of code to output information about the gates. It also helped me a lot to rewrite the gate operators from `AND`, `OR` and `XOR` to `&&`, `||` and `!=` as that made it far easier to see which gates stood out with deviating operators. What I saw in my test output was that there is a certain pattern to how almost all of the gates are built up:
- All input gates are paired `x` and `y` wires with the same number.
- Almost all input gate pairs consist of a `AND` and `XOR` operators, followed by a second grade gate of `OR` and `AND` operators (respectively).
- Almost all output gates (connected to `z` wires) have `XOR` operators.
- Almost all other gates (neither at the front or at the end of the chain) have either `AND` or `OR` operators.

I started scrolling through these gates and their operators, and found a number of deviating gates. I thought I was onto something, but found I think 11 or 12 gates that were 'suspicious'. I tried to come up with an explanation for what I found and also tried a few times to submit an answer that was wrong. I tried to submit all equal pairs of the 4 ones I was sure about were anomalies. That wasn't the answer. I also saw that the first input nodes and the last output node were different, so I included those, but that also wasn't correct. To be honest, by trying it a few times and writing down derivatives based on what I learned, I found my answer. Then I understood what was going on: the first input gates and the last output gate are indeed different, but that's normal apparently. The crossed wires are not crossed between equal pairs per se, but that *could* be the case. Furthermore, there are three set rules:
- Starting gates should be followed by `OR` if `AND`, and by `AND` if `XOR`, except for the first one.
- Gates in the middle should not have `XOR` operators.
- Gates at the end should always have `XOR` operators, except for the last one.

So after finding the answer manually, I've written a coded version to do the selections based on these rules, and it gave me the answer right away. I didn't even need to simulate the device, because these anomalies to the rules automatically revealed the incorrect gates and thus the crossed wires. So I could also remove all code that simulated the gates in part 1. My initial idea, to actually simulate different options after switching wires back, wasn't even necessary to find the answer, but maybe I will add that anyway because I think it would be a cool addition and a fun challenge still.

### The cool one...

After solving today's puzzle, and finishing all puzzles of AoC 2024, I got back to 24.2 and decided to build a new version that includes an elaborate visualization of the circuit. I started over basically from scratch, without using the formerly acquired knowledge about the pattern requirements, and started just building up a visualization from my input using the WPF framework. I wanted to find out if I would also be able to create an advanced visualization of the given circuit, and also if I would be able to find the crossed over wires based on this visualization. This concept is approaching the crossed over wires a little differently, because it draws out the circuit based on the nodes it expects to find when traversing the different connections, and when it finds a different node than expected, it will mark that as `suspicious`. Unsurprisingly it's finding the same spots in the circut, but now it really is doing so by exploring the input and the connections instead of just checking for operator types without building up the graph.

When I did finish the visualization, and was able to find the crossed over wires accordingly, I started thinking about other cool features, like actually animating the values going through the circuit. This eventually lead to a whole set of cool features:
- graphically visualize the circuit;
- mark the crossed over wires (red);
- simulate the functional operation of the device;
- animate the value propagation through the different gates and wires of the circuit;
- output and verify the result of the device;
- mark the faulty bits due to the crossings (red) and the influenced connection and wires (purple);
- demonstrate how far each pair of crossed over wires influence the output of the device for each different set of input values;
- repair the circuit;
- allow you to test different input values for the `x` and `y` wires.

Here's a screenshot of the animated visualization, where you can see the correct wires in green, the crossed over wires in red, and the wires and connections influenced by the crossed over wires in purle:
![Screenshot of the animated visualization of my solution of AoC Day 24 puzzle](./CircuitVisualization "Circuit Visualization")

I won't go and explain the code in detail in this README file (as I normally do for my more elaborate AoC solutions), so for that I would like to refer to the actual code, but I did record a video that demonstrates my solution and explains the different features, which you can find on my [YouTube](https://www.youtube.com/watch?v=A5AJb_34RXc) channel.