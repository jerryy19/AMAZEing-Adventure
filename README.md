# AMAZEing-Adventure
CS576 Game Programming Project @UMass Amherst


## Game Description
The purpose of this game is to find a way out of the map but the puzzles need
to be solved before leaving the level. There will be obstacles in the level, such as
areas where they slow you down and enemies trying to prevent you from succeeding. 
This game is built using Unity. 

## What are you trying to teach through this game (Educational Component)?
The target audience is elementary school students who are looking to practice their
vocabulary and improve their spatial memory. 

## 3D world and characters
Our world will be procedurally generated. Becuase our world is procedurally generated, 
we have infinite levels. Each level is a closed environment and the game will be played 
in 3rd person.

## Game mechanics
The player will walk through the level and solve all the puzzles, taking advantage of 
the environment provided and avoiding obstacles. 

## Animation
All animations will be keyframe animated.

## User interface and sound
We will have main, pause, settings, tutorial menus. We will have a healthbar for player model
that takes damage from puzzle and from enemies. There will be sounds for certain zones, player 
movement and enemy interaction. 

## Rough breakdown of the tasks each group member focused on
Liam Brandwein
- BananaMan.cs
- Banana.cs
- Robot FSM
- Robot functionality
- SFX
- Blue mystery tile

Jia Hui Yu
- Most of Main.cs
- Level.cs
- Graph.cs
- Puzzle.cs
- Puzzle UIs and game logic scripts
- PriorityQueue.cs
- Timer.cs
  
Alan Zheng
- BigVegas.cs 
- Animation controller for player and its functionality
- Healtbar and its functionality
- The following UIs: Menu, PauseMenu, Guide, Settings, NewGame, Tutorial
- implmented the transitioning and functionality of the UI
- Implemented the NewGame, StartGame, and Tutorial scene
- Config.cs
- SceneNavigator.cs
- implemented the change of speed of player in Speed.cs and Slow.cs
- implemented volume change in settings

## External References
- https://forum.unity.com/threads/how-to-get-access-to-renderer-features-at-runtime.1193572/
- https://forum.unity.com/threads/how-to-calculate-force-needed-to-jump-towards-target-point.372288/
- giraffe asset is from https://www.cgtrader.com/free-3d-models/animals/mammal/giraffa-puzzle
- building assets are from https://assetstore.unity.com/packages/3d/environments/urban/city-voxel-pack-136141
- Timer script is adapted from https://www.youtube.com/watch?v=3ZfwqWl-YI0
- Big vegas model is from https://www.mixamo.com/#/?page=1&query=big&type=Character
- Healthbar asset is inspired by https://www.youtube.com/watch?v=BLfNP4Sc_iA

