# Night Optimization Kit 🌟

Optimize your Unity projects with the Night Optimization Kit. This package contains a set of powerful tools to enhance your game's performance and efficiency.

## Features
- **NightPath Pathfinding System**: Fully customizable optimized pathfinding package.
- **NightJob System**: Boost performance with parallel job scheduling.
- **Nightcull Dynamic Culling**: Optimize scenes using dynamic culling techniques.
- **DotTrail V2**: DotTrail is now for parallel programming!
- **NightPool**: Manage object pooling for better memory usage.

<br>

## Installation

To use the Night Optimization Kit, simply download the package and import it into your Unity project.

Enjoy optimizing your Unity games with the Night Optimization Kit!

<br><br><br>

# 🧪 *What's new? 9/29/24*

### 🧪 NightPath Pathfinding System: <br>

**NightPath** is a fully customizable pathfinding package that allows you to work with different agent models, pathfinding algorithms, and post-processors. You can also create your own agent models, algorithms, and post-processors to integrate seamlessly with NightPath. By default, NightPath includes two pathfinding algorithms: **QGIS** and **Flow Vector**. Algorithms like *A**, *Dijkstra*, or any other custom solutions can be implemented as well.

Quadrant Growth and Intersection Search (QGIS) is a pathfinding algorithm designed to identify the nodes an agent can access or "see" within its environment. The algorithm starts from a central node and expands outward in four quadrants (up, down, left, right), as well as diagonally, forming a growing square. The expansion continues as long as neighboring nodes meet a minimum weight threshold, ensuring that only valid paths are considered. By combining square-based growth with diagonal searches, QGIS identifies all reachable nodes. This makes it particularly effective for agents that need to dynamically detect accessible areas and optimize their movement in complex environments.
