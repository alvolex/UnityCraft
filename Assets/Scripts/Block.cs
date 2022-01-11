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

    [SerializeField] private BlockSide sideToBuild = BlockSide.FRONT;

    void Start()
    {
        MeshFilter mf = gameObject.AddComponent<MeshFilter>();
        MeshRenderer mr = gameObject.AddComponent<MeshRenderer>();

        //Use our quad class to build a quad and assign the returned mesh to out filter
        Quad q = new Quad();
        mf.mesh = q.Build(sideToBuild);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
