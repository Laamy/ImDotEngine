# ImDotEngine

Super basic engine (not for production use) that I can use to fuck around and learn new things

# Showcase

you can see a basic showcase of the sample scene & engine
[here](https://streamable.com/npwgbz)

# Features

1. Spatial Partitioning using hashes so you dont have to loop over every object when rendering or doing physic steps
2. Exclusive/Borderless fullscreen (No optimized fullscreen support at this point in time)
3. Easy to extend on the game with components that take in all the default game events as overrides
4. Layers for the level so you can have layers for UI foreground and background (they have their own spatial partitioning grids, memory usage warning)

# Planned (Auto)

AUTO GENERATED BY VS TASK VIEWER																											     									 </br>
																																			     									 </br>
Priority	Line	Description																																						 </br>
Normal	117	TODO: i want to handle blocks with angles differently.. for example the angled grass blocks should make the player go up if its the left or right (depending on the type)</br>
Normal	22	TODO: implement this cuz i found it useful asf when developing cheats for MCBE																							 </br>
Normal	57	TODO: check largest/farthest polygon and use as size for both X & Y																										 </br>
Normal	52	TODO: actual size for this																																				 </br>
Normal	8	TODO: move all the properties into components and stack similar ones next to each other in memory (ECS/ENTT)															 </br>
Normal	14	TODO: make a physics/rigid body class for the localplayer to inherit																									 </br>
Normal	23	TODO: add a second velocity vector for speed																															 </br>
Normal	83	TODO: avoid tunneling by stepping through all the steps ig																												 </br>
Normal	121	TODO: add vert & hor collision flags seperately from onground & inair so i can verify this before clearing these flags													 </br>
Low	8	NOTE: use rectangleshape with an angle/rotation for lines allowing for thickness																							 </br>
Low	7	NOTE: add a quick way to update/move objects around without reinserting constantly																							 </br>
Low	75	NOTE: i really dont care about this counter (add a second counter in level that calculates the accurate count)																 </br>
Low	27	NOTE: make these classes inherit a repository class																															 </br>
Low	12	NOTE: port everything to C++																																				 </br>
Low	152	NOTE: call death functions once below this coordinate level (I might not do this cuz i want an infinite world height														 </br>
Low	153	NOTE: I might switch the world to a signed 32bit integer instead of floats, it'll make debugging easier & stop floating point errors										 </br>
Low	164	NOTE: probably better to move this up to where i set onground to false																										 </br>
Low	48	NOTE: dont set until ready