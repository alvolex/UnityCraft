using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class Block
{
    public Mesh mesh;
    private Chunk parentChunk;
    

    public Block(Vector3 offset, MeshUtils.BlockType blockType, Chunk chunk)
    {
        parentChunk = chunk;

        //Use our quad class to build all the sides of our block and put them in an array
        List<Quad> quads = new List<Quad>();

        Vector3 blockLocalPos = offset - chunk.chunkLocation;

        if (blockType != MeshUtils.BlockType.AIR)
        {
            //If-statements to check if the neighbor in the direction the quad is facing is solid or not
            /* TOP */
            if (!HasSolidNeighbor((int) blockLocalPos.x, (int) blockLocalPos.y + 1, (int) blockLocalPos.z))
            {
                if (blockType == MeshUtils.BlockType.GRASSSIDE)
                {
                    quads.Add(new Quad(MeshUtils.BlockSide.TOP, offset, MeshUtils.BlockType.GRASSTOP));
                }
                else
                {
                    quads.Add(new Quad(MeshUtils.BlockSide.TOP, offset, blockType));
                }
            }
            /* BOTTOM */
            if (!HasSolidNeighbor((int) blockLocalPos.x, (int) blockLocalPos.y - 1, (int) blockLocalPos.z))
            {
                if (blockType == MeshUtils.BlockType.GRASSSIDE)
                {
                    quads.Add(new Quad(MeshUtils.BlockSide.BOTTOM, offset, MeshUtils.BlockType.DIRT));
                }
                else
                {
                    quads.Add(new Quad(MeshUtils.BlockSide.BOTTOM, offset, blockType));
                }
            }
            /* LEFT */
            if (!HasSolidNeighbor((int) blockLocalPos.x - 1, (int) blockLocalPos.y, (int) blockLocalPos.z))
            {
                quads.Add(new Quad(MeshUtils.BlockSide.LEFT, offset, blockType));
            }
            /* RIGHT */
            if (!HasSolidNeighbor((int) blockLocalPos.x + 1, (int) blockLocalPos.y, (int) blockLocalPos.z))
            {
                quads.Add(new Quad(MeshUtils.BlockSide.RIGHT, offset, blockType));
            }
            /* FRONT */
            if (!HasSolidNeighbor((int) blockLocalPos.x, (int) blockLocalPos.y, (int) blockLocalPos.z + 1))
            {
                quads.Add(new Quad(MeshUtils.BlockSide.FRONT, offset, blockType));
            }
            /* BACK */
            if (!HasSolidNeighbor((int) blockLocalPos.x, (int) blockLocalPos.y, (int) blockLocalPos.z - 1))
            {
                quads.Add(new Quad(MeshUtils.BlockSide.BACK, offset, blockType));
            }
        }

        if (quads.Count == 0) return;

        //Take the quad meshes we create above and put them into a new array that will be sent to our MeshUtils to be
        //merged into a single mesh.
        Mesh[] sideMeshes = new Mesh[quads.Count];

        int indx = 0;
        foreach (Quad quad in quads)
        {
            sideMeshes[indx] = quad.mesh;
            indx++;
        }

        //Use our mesh utils to merge all the sides of our block into a single mesh, then we assign that new mesh
        mesh = MeshUtils.MergeMeshes(sideMeshes);
        mesh.name = "Cube_0_0_0";
    }

    public bool HasSolidNeighbor(int x, int y, int z)
    {
        //Check if we're currently on the "edge" of the chunk
        if (x < 0 || x >= parentChunk.width ||
            y < 0 || y >= parentChunk.height ||
            z < 0 || z >= parentChunk.depth)
        {
            return false;
        }
        if(parentChunk.chunkData[x + parentChunk.width * (y + parentChunk.depth * z)] == MeshUtils.BlockType.AIR
           || parentChunk.chunkData[x + parentChunk.width * (y + parentChunk.depth * z)] == MeshUtils.BlockType.WATER)
            return false;
        
        return true;
    }
}