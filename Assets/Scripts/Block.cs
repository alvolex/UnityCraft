using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    public Mesh mesh;
    private Chunk parentChunk;

    public Block(Vector3 offset, MeshUtils.BlockType blockType, Chunk chunk)
    {
        parentChunk = chunk;
        
        //Use our quad class to build all the sides of our block and put them in an array
        Quad[] quads = new Quad[6];
        quads[0] = new Quad(MeshUtils.BlockSide.BOTTOM, offset, blockType);
        quads[1] = new Quad(MeshUtils.BlockSide.TOP, offset, blockType);
        quads[2] = new Quad(MeshUtils.BlockSide.LEFT, offset, blockType);
        quads[3] = new Quad(MeshUtils.BlockSide.RIGHT, offset, blockType);
        quads[4] = new Quad(MeshUtils.BlockSide.FRONT, offset, blockType);
        quads[5] = new Quad(MeshUtils.BlockSide.BACK, offset, blockType);

        //Take the quad meshes we create above and put them into a new array that will be sent to our MeshUtils to be
        //merged into a single mesh.
        Mesh[] sideMeshes = new Mesh[6];
        sideMeshes[0] = quads[0].mesh;
        sideMeshes[1] = quads[1].mesh;
        sideMeshes[2] = quads[2].mesh;
        sideMeshes[3] = quads[3].mesh;
        sideMeshes[4] = quads[4].mesh;
        sideMeshes[5] = quads[5].mesh;
      
        //Use our mesh utils to merge all the sides of our block into a single mesh, then we assign that new mesh
        mesh = MeshUtils.MergeMeshes(sideMeshes);
        mesh.name = "Cube_0_0_0";
    }

    public bool HasSolidNeighbor(int x, int y, int z)
    {
        //Check if we're currently on the "edge" of the chunk
        if (x < 0 || x >= parentChunk.width || y < 0 || y > parentChunk.height || z < 0 || z > parentChunk.depth)
        {
            return false;
        }

        if (parentChunk.chunkData[x + parentChunk.width * (y + parentChunk.depth * z)] == MeshUtils.BlockType.AIR || parentChunk.chunkData[x + parentChunk.width * (y + parentChunk.depth * z)] == MeshUtils.BlockType.WATER )
        {
            return false;
        }

        return true;
    }

}
