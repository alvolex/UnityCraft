using System.Collections.Generic;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

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
        blocks = new Block[width, height, depth]; // X, Y , Z

        //Setup for unity Jobs / Burst compiler
        var inputMeshes = new List<Mesh>(width * height * depth);
        int vertexStart = 0;
        int triangleStart = 0;
        int meshCount = width * height * depth;
        int m = 0; //Just a counter to see how many blocks we've created

        //Setup our jobs. The "ProcessMeshDataJob" is the struct that can be founder further down in the code.
        var jobs = new ProcessMeshDataJob
        {
            vertexStart = new NativeArray<int>(meshCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory),
            triangleStart = new NativeArray<int>(meshCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory)
        };

        //Populate our chunk with blocks and give them a position in world space. 
        for (int z = 0; z < depth; z++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    blocks[x, y, z] =
                        new Block(new Vector3(x, y, z),
                            MeshUtils.BlockType
                                .DIRT); //Here we set the block that will be on coordinate X Y Z to be a new block of type dirt in this case.
                    inputMeshes.Add(blocks[x, y, z]
                        .mesh); //Add the mesh that we create in the line above into our mesh array
                    var vertCount = blocks[x, y, z].mesh.vertexCount; //Get the amount of vertices in the current block
                    var triCount =
                        (int) blocks[x, y, z].mesh
                            .GetIndexCount(0); //This will return the amount of triangles in the current mesh

                    jobs.vertexStart[m] = vertexStart;
                    jobs.triangleStart[m] = triangleStart;

                    vertexStart += vertCount; //Add the amount of verts to our "index"
                    triangleStart += triCount; //Add the amount of triangles to our "index"
                    m++; //Update m since we now have added a new block.
                }
            }
        }

        jobs.meshData = Mesh.AcquireReadOnlyMeshData(inputMeshes);
        var outputMeshData = Mesh.AllocateWritableMeshData(1);
        jobs.outputMesh = outputMeshData[0];
        jobs.outputMesh.SetIndexBufferParams(triangleStart, IndexFormat.UInt32);
        jobs.outputMesh.SetVertexBufferParams(vertexStart, new VertexAttributeDescriptor(VertexAttribute.Position),
            new VertexAttributeDescriptor(VertexAttribute.Normal, stream: 1),
            new VertexAttributeDescriptor(VertexAttribute.TexCoord0, stream:2));
        var handle = jobs.Schedule(meshCount, 4);

        var newMesh = new Mesh();
        newMesh.name = "Chunk";
        var sm = new SubMeshDescriptor(0, triangleStart, MeshTopology.Triangles);
        sm.firstVertex = 0;
        sm.vertexCount = vertexStart;
        handle.Complete();

        jobs.outputMesh.subMeshCount = 1;
        jobs.outputMesh.SetSubMesh(0, sm);
        
        Mesh.ApplyAndDisposeWritableMeshData(outputMeshData, new []{newMesh});
        
        jobs.meshData.Dispose();
        jobs.vertexStart.Dispose();
        jobs.triangleStart.Dispose();
        
        newMesh.RecalculateBounds();

        mf.mesh = newMesh;
    }

    [BurstCompile]
    struct ProcessMeshDataJob : IJobParallelFor
    {
        [ReadOnly] public Mesh.MeshDataArray meshData;
        public Mesh.MeshData outputMesh;
        public NativeArray<int> vertexStart;
        public NativeArray<int> triangleStart;

        public void Execute(int index)
        {
            var data = meshData[index];
            var vCount = data.vertexCount;
            var vStart = vertexStart[index];

            //Create a new NativeArray that will hold the vertices for the current mesh
            var verts = new NativeArray<float3>(vCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            data.GetVertices(verts
                .Reinterpret<Vector3>()); //"Reinterprets" normal Vector3 array into a NativeArray float3
            //Same as above but for normals
            var normals = new NativeArray<float3>(vCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            data.GetNormals(normals.Reinterpret<Vector3>());
            //Same as above but for UVs
            var uvs = new NativeArray<float3>(vCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            data.GetUVs(0, uvs.Reinterpret<Vector3>());

            var outputVerts = outputMesh.GetVertexData<Vector3>();
            var outputNormals = outputMesh.GetVertexData<Vector3>(stream: 1);
            var outputUVs = outputMesh.GetVertexData<Vector3>(stream: 2);

            //Loop through our verts and put put in the vertex, normal and UV data
            for (int i = 0; i < vCount; i++)
            {
                outputVerts[i + vStart] = verts[i];
                outputNormals[i + vStart] = normals[i];
                outputUVs[i + vStart] = uvs[i];
            }

            //Need to dispose of the native arrays because they're not handled by garbage collection
            verts.Dispose();
            normals.Dispose();
            uvs.Dispose();

            var triStart = triangleStart[index];
            var triCount = data.GetSubMesh(0).indexCount;
            var outputTris = outputMesh.GetIndexData<int>();

            //We have to check format here because the data is sometimes Int16 and sometimes Int32 depending on the operating system.
            if (data.indexFormat == IndexFormat.UInt16) /*If index is int16*/
            {
                var tris = data.GetIndexData<ushort>();
                for (int i = 0; i < triCount; i++)
                {
                    int indx = tris[i];
                    outputTris[i + triStart] = vStart + indx;
                }
            }
            else /*If format is Int32*/
            {
                var tris = data.GetIndexData<int>();
                for (int i = 0; i < triCount; i++)
                {
                    int indx = tris[i];
                    outputTris[i + triStart] = vStart + indx;
                }
            }
        }
    }
}