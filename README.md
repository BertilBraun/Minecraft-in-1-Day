
# Minecraft-in-1-Day
Creating Minecraft in one day in Unity

## To-Do's
- Noise Worm Terrain cave generation [link](http://libnoise.sourceforge.net/examples/worms/index.html)
- Torches (simple Point lights?)
- Interacting with Blocks (Inventory for Chest and Furnace)
- Block Mesh implementation for chests, stairs, half slaps etc.

## Fixes
- Sometimes the Spawn point is around (0, 0, 0)
- Tree generation -> Leaves are cut off
- Chunks not loading for more than one Player if already loaded before
- Chunks are not loading again after being unloaded
- Jittery Movement
      - Client handles Movement
      - Sends Update to Server
      - Server authenticates
![Client-Side Prediction](https://www.gabrielgambetta.com/img/fpm2-05.png "Client-Side Prediction and Server Reconciliation")
