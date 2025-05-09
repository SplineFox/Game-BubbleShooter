<!--![Social Preview](https://github.com/SplineFox/Game-BugArena/blob/master/ReadmeMedia/Bug%20Arena%20-%20Social%20Preview.png)-->
<img src="https://github.com/SplineFox/Game-BubbleShooter/blob/master/ReadmeMedia/BubbleShooter%20-%20App%20Icon.png" width="128">

## Overview
***Bubble Shooter*** is a simple implementation of the popular arcade game where players aim and launch colored bubbles onto a grid filled with other bubbles. The objective is to clear the entire board by forming groups of matching-colored bubbles and achieving the highest possible score. The core gameplay mechanics include:

+ ***Matching 3+ Bubbles*** - When three or more bubbles of the same color connect, they burst and disappear from the field.
+ ***Floating Bubbles*** - Any bubbles left disconnected from the main cluster will drop and dissappera from the field giving player extra score.
+ ***Scoring System*** - Player earn more points for clearing larger groups of bubbles in a single shot.
+ ***Penalty System*** - Players are penalized for shots that don’t pop any bubbles. There is a counter representing remaining shots before a new row appears. Each missed shot or failed match reduces the counter by one. Once the counter reaches zero after five misses, the game adds a new row of bubbles from the top. The player then gets four additional attempts before the next penalty.

<video src='https://github.com/SplineFox/Game-BubbleShooter/blob/master/ReadmeMedia/BubbleShooter%20-%20Gameplay.mp4' width=256/>
