# WindowsFormsAppFifteen
## A Game Fifteen and Helper are developed using WinForms and C#

I have developed a game fifteen with helper that helps the player win the game. Helper uses two methods to solve the puzzle and find the path on the graph. The first is IDA* which based on iterative deepening depth-first search. The second is my own method SEA* (Step evaluation A*), which is based on IDA* but has some modifications.

The project has three main classes: the class Form1 represents field of game, the class Game contains all the logic of the game, the class Helper contains two methods on the graphs.

### Short description SEA*
The main logic of my method SEA* is that it concentrates on the more probable ways to reach the target vertex,  comparisoning  the child vertices of each of the nodes step-by-step. The algorithm compares all child nodes on the value of F(v) at each stage and determines their order of further deployment. The nodes that have a lower value of F(v) among their brothers will be deployed in the first place , which indicates on the fact that the deployment of these nodes is more likely closer to the terminal vertex and this path is shorter, what is preferable for the speed of achieving the goal. This linearity reduces the number of transmitted nodes, in turn reducing the time to find the final vertex.
