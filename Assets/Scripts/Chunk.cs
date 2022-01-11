using System.Collections;
using System.Collections.Generic;
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

        //Populate our chunk with blocks and give them a position in world space. 
        for (int z = 0; z < depth; z++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    blocks[x, y, z] = new Block(new Vector3(x, y, z), MeshUtils.BlockType.DIRT); //Here we set the block that will be on coordinate X Y Z to be a new block of type dirt in this case.
                }
            }
        }
    }

}
