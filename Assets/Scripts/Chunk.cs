using System.Collections.Generic;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Rendering;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    [SerializeField] private Material atlas;
    [SerializeField] private int width = 2;
    [SerializeField] private int height = 2;
    [SerializeField] private int depth = 2;

    private Block[,,] blocks; //Multidimensional array to store the position of the voxel
    public Block[,,] Blocks => blocks;

    void Start()
    {
        MeshFilter mf = gameObject.AddComponent<MeshFilter>();
        MeshRenderer mr = gameObject.AddComponent<MeshRenderer>();
        mr.material = atlas;
        blocks = new Block[width, height, depth];  // X, Y , Z

        //Setup for unity Jobs / Burst compiler
        var inputMeshes = new List<Mesh>(width * height * depth);
        int vertexStart = 0;
        int triangleStart = 0;
        int meshCount = width * height * depth;
        int m = 0; //Just a counter to see how many blocks we've created
        var jobs = new ProcessMeshDataJob();

        jobs.vertexStart = new NativeArray<int>(meshCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
        jobs.triangleStart = new NativeArray<int>(meshCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
        
        //Populate our chunk with blocks and give them a position in world space. 
        for (int z = 0; z < depth; z++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    blocks[x, y, z] = new Block(new Vector3(x, y, z), MeshUtils.BlockType.DIRT); //Here we set the block that will be on coordinate X Y Z to be a new block of type dirt in this case.
                    inputMeshes.Add(blocks[x, y, z].mesh); //Add the mesh that we create in the line above into our mesh array
                    var vertCount = blocks[x, y, z].mesh.vertexCount; //Get the amount of vertices in the current block
                    var triCount = (int) blocks[x, y, z].mesh.GetIndexCount(0); //This will return the amount of triangles in the current mesh

                    jobs.vertexStart[m] = vertexStart;
                    jobs.triangleStart[m] = triangleStart;
                    
                    vertexStart += vertCount; //Add the amount of verts to our "index"
                    triangleStart += triCount; //Add the amount of triangles to our "index"
                    m++; //Update m since we now have added a new block.
                }
            }
        }
    }

}
