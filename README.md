This is a project where the goal was to recreate some of the world building aspects of Minecraft within Unity.

It divides the world up into chunks and each chunk then contains all the data needed for what block type exists where inside of it, while only creating a single mesh per chunk as to keep the gameplay incredibly performant even at larger scale levels.

For world gen a series of perlin noise and fractal brownian motion is used to generate which type of block is allowed to spawn where, and also to create complex cave systems within the chunks.
