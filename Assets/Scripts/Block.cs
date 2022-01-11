using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    //Enum to keep track which side of our block/cube we're currently building. The Quad class is setup to create FRONT first.
    public enum BlockSide
    {
        BOTTOM, TOP, LEFT, RIGHT, FRONT, BACK
    }

    [SerializeField] private Material atlas;
    [SerializeField] private Vector3 posToBuildBlock = new Vector3(0,0,0);
    [SerializeField] private MeshUtils.BlockType blockType;
    

    void Start()
    {
        MeshFilter mf = gameObject.AddComponent<MeshFilter>();
        MeshRenderer mr = gameObject.AddComponent<MeshRenderer>();
        mr.material = atlas;

        //Use our quad class to build all the sides of our block and put them in an array
        Quad[] quads = new Quad[6];
        quads[0] = new Quad(BlockSide.BOTTOM, posToBuildBlock, blockType);
        quads[1] = new Quad(BlockSide.TOP, posToBuildBlock, blockType);
        quads[2] = new Quad(BlockSide.LEFT, posToBuildBlock, blockType);
        quads[3] = new Quad(BlockSide.RIGHT, posToBuildBlock, blockType);
        quads[4] = new Quad(BlockSide.FRONT, posToBuildBlock, blockType);
        quads[5] = new Quad(BlockSide.BACK, posToBuildBlock, blockType);

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
        mf.mesh = MeshUtils.MergeMeshes(sideMeshes);
        mf.mesh.name = "Cube_0_0_0";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
