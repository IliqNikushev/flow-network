Flow network is an application that is for Year 2 in Fontys during the OOD2 subject.
The goal is to be able to create a pipeline network with various components and set of rules.

Pump - Element that provides an in flow for the network
Sink - Element that is the end point of the flow within the network
Pipe - A connection(path) between two elements that allows flow from 1 to the other. The flow is always OUT-IN
Splitter - Element that takes 1 flow and splits it 50% 50% into two different streams
Adjustable Splitter - Element that takes 1 flow at splits it in two different streams depending on the user preference
Merger - Element that takes 2 different streams and combines them into 1

The network is able to be regulated by the user to set the maximum capacity of each pipe; the maximum flow over the network;
the current flow of each pump;

The user is able to manipulate the network's layout by dragging elements with the select tool.
The pipe tool also allows the user to specify mid points on a pipe.

Extras:
Undo/Redo - The user is provided with the option to undo/redo their actions, untill the very beginning of their interaction with
the current network
