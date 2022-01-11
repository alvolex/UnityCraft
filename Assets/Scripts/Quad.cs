using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quad
{
    public Mesh Build(Block.BlockSide side)
    {
        Mesh mesh;

        mesh = new Mesh();
        mesh.name = "ScriptedQuad";
        
        //Data needed to construct a quad (Vertices, normals, UVs, triangles)
        //We beed 4 vertices, normals and uvs since there are 4 corners to a quad. We need 6 ints for the triangles
        //since there are 3 vertices in a triangle and we need 2 triangeles to make a quad..
        Vector3[] vertices = new Vector3[4];
        Vector3[] normals = new Vector3[4];
        Vector2[] uvs = new Vector2[4];
        int[] triangles = new int[6];

        Vector2 uv00 = new Vector2(0, 0);
        Vector2 uv10 = new Vector2(1, 0);
        Vector2 uv01 = new Vector2(0, 1);
        Vector2 uv11 = new Vector2(1, 1);
        
        //Our Vertices / points. We need to define 8 vertices since there are 8 distinct corners in a cube.
        Vector3 p0 = new Vector3(-0.5f, -0.5f, 0.5f);
        Vector3 p1 = new Vector3(0.5f, -0.5f, 0.5f);
        Vector3 p2 = new Vector3(0.5f, -0.5f, -0.5f);
        Vector3 p3 = new Vector3(-0.5f, -0.5f, -0.5f);
        Vector3 p4 = new Vector3(-0.5f, 0.5f, 0.5f);
        Vector3 p5 = new Vector3(0.5f, 0.5f, 0.5f);
        Vector3 p6 = new Vector3(0.5f, 0.5f, -0.5f);
        Vector3 p7 = new Vector3(-0.5f, 0.5f, -0.5f);

        switch (side)
        {
            case Block.BlockSide.FRONT:
            {
                //Pick 4 of these vertices that make up two triangles
                vertices = new Vector3[] {p4, p5, p1, p0};
                normals = new Vector3[] {Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward};
                uvs = new Vector2[] {uv11, uv01, uv00, uv10};
                //Getting the correct points for our triangles. The first 3 will be the first triangle and the last 3 the second triangle
                //These values corresponds to the vertices in our vertices array. eg: 3 represents p0, 1 = p5, 0 = p4 etc..
                //The order of the triangles doesn't matter, but the order of the VERTICES do matter.
                //Ther vertices needs to be picked in clockwise order.
                triangles = new[] {3, 1, 0, 3, 2, 1};
                break;
            }
            case Block.BlockSide.BACK:
            {
                vertices = new Vector3[] {p6, p7, p3, p2};
                normals = new Vector3[] {Vector3.back, Vector3.back, Vector3.back, Vector3.back};
                uvs = new Vector2[] {uv11, uv01, uv00, uv10};
                triangles = new[] {3, 1, 0, 3, 2, 1};
                break;
            }
            case Block.BlockSide.TOP:
            {
                vertices = new Vector3[] {p7, p6, p5, p4};
                normals = new Vector3[] {Vector3.up, Vector3.up, Vector3.up, Vector3.up};
                uvs = new Vector2[] {uv11, uv01, uv00, uv10};
                triangles = new[] {3, 1, 0, 3, 2, 1};
                break;
            }
            case Block.BlockSide.BOTTOM:
            {
                vertices = new Vector3[] {p0, p1, p2, p3};
                normals = new Vector3[] {Vector3.down, Vector3.down, Vector3.down, Vector3.down};
                uvs = new Vector2[] {uv11, uv01, uv00, uv10};
                triangles = new[] {3, 1, 0, 3, 2, 1};
                break;
            }
            case Block.BlockSide.LEFT:
            {
                vertices = new Vector3[] {p5, p6, p2, p1};
                normals = new Vector3[] {Vector3.left, Vector3.left, Vector3.left, Vector3.left};
                uvs = new Vector2[] {uv11, uv01, uv00, uv10};
                triangles = new[] {3, 1, 0, 3, 2, 1};
                break;
            }
            case Block.BlockSide.RIGHT:
            {
                vertices = new Vector3[] {p7, p4, p0, p3};
                normals = new Vector3[] {Vector3.right, Vector3.right, Vector3.right, Vector3.right};
                uvs = new Vector2[] {uv11, uv01, uv00, uv10};
                triangles = new[] {3, 1, 0, 3, 2, 1};
                break;
            }
        }

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        
        mesh.RecalculateBounds(); //Unity calculates the bounds so we can use colliders later.

        return mesh;
    }

}
