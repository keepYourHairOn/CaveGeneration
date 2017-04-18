# CaveGeneration
Cave Levels Generation Using Cellular Automata Based Algorithm described in the paper by Johnson et all *"Cellular automata for real-time generation of infinite cave levels"* for **Procedural Content Generation** (PCG) course.

The algorithm works in such a manner:
1)	Create base grid. 
2)	Assign it’s cells to be of the floor type. 
3)	The next step is to add grids of the same size at the east, west, north and south of the initial grid.
4)	Convert R percent of the cells to rock type from the floor type.
5)	Use iterated cellular automata:
a)	Examine relationship neighbor cells of each cell of the grid.
b)	Change cell’s type to rock if it’s neighborhood value >= T
c)	Change cell’s type to floor if it’s neighborhood value < T
6)	Repeat step 5 N times.
7)	Rock cells with at least one neighbour of the floor type as assigned to be of the wall type.


