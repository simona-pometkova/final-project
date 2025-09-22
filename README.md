# Procedural Dungeon Generation: A Novel Twist on the Game of Life
Procedural generation has long been a powerful tool in game development, enabling designers and developers to create expansive, unpredictable worlds with relatively compact and straightforward algorithms. It allows the creation of infinite variety from finite code, a principle that underpins some of the most enduring and replayable games in the industry.

This project explores how procedural systems can be used to generate compelling dungeon layouts and emergent agent behaviour. The core concept was to implement a modular simulation in which a 2D top-down dungeon is created using Binary Space Partitioning and Cellular Automata, and then populated with simple AI agents whose behaviour is driven by local rules and environmental conditions.

Deriving from the original concept and as a result of evolving design decisions during development, the final product is a strategic simulation-roguelike hybrid in which each level is procedurally generated. Once created, the dungeon is populated with two opposing types of autonomous agents — players and enemies — whose movement, behaviour and territorial influence are randomized and driven by simple rules heavily inspired by Conway’s Game of Life. 

The player can observe the dungeon as a zero-player simulation or intervene directly by controlling any player agent. The primary objective is to light all torches scattered across the dungeon, while enemy agents attempt to extinguish them, creating a dynamic cycle of conflict and territorial control. The game also introduces the challenge of possible conversion between teams, inviting the player to incorporate strategic thinking. A level is completed when all torches remain lit, after which a bigger, more complex dungeon is generated and filled with smarter, more threatening enemies. 

This design combines the structural clarity of classic roguelikes with the unpredictability of Cellular Automata-inspired ecosystems. Simple interactions — lighting or extinguishing a torch, converting agents through proximity and opponent advantage — produce complex, evolving patterns that can surprise even me as the developer. 

