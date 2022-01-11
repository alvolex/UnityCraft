using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Added a using statement where we create a new datatype that we call VertexData that will store vertices, normals and UVs
using VertexData = System.Tuple<UnityEngine.Vector3, UnityEngine.Vector3, UnityEngine.Vector2>;

/// <summary>
/// We'rem etting all the vertices, uvs and triangles from all meshes and putting them together into a single mesh which
/// means taking all the vertices from multiple arrays and putting them into one array.  The same must happen for uvs and
/// these must be aligned with the appropriate vertices.  Then because there might be multiple vertices that are the same
/// after meshes have been combined, we want to get rid of any duplicates, but we need to make sure the integrity of
/// the triangles remain the same.
///
/// Therefore, for each triangle we might have to change the index of the vertex it refers to.  So a search through all
/// the vertices for the correct one that matches the triangle takes place.
/// </summary>

public static class MeshUtils
{
  
  const float TEX_SIZE = 0.0625f;
  
  //Blocktypes where the int value is it's position in the atlas
  public enum BlockType
  {
    GRASSTOP = 145,
    GRASSSIDE = 3,
    DIRT = 2,
    WATER = 209,
    STONE = 16,
    SAND = 18
  }
  //Enum to keep track which side of our block/cube we're currently building.
  public enum BlockSide
  {
    BOTTOM, TOP, LEFT, RIGHT, FRONT, BACK
  }

  public static Mesh MergeMeshes(Mesh[] meshes)
  {
    Mesh mesh = new Mesh();

    Dictionary<VertexData, int> pointsOrder = new Dictionary<VertexData, int>();
    HashSet<VertexData> pointsHash = new HashSet<VertexData>();
    List<int> triangles = new List<int>();

    int pIndex = 0;

    //Loop through each mesh that should be merged
    for (int i = 0; i < meshes.Length; i++)
    {
      if (meshes[i] == null ) continue;
      
      //Loop through every vertex of the current mesh
      for (int j = 0; j < meshes[i].vertices.Length; j++)
      {
        Vector3 vertices = meshes[i].vertices[j];
        Vector3 normals = meshes[i].normals[j];
        Vector2 uvs = meshes[i].uv[j];

        VertexData point = new VertexData(vertices, normals, uvs);

        //Check if we've already added the current point, if not we add it to our Dictionary with the current index.
        //The hashset is only used because it's way faster to do lookups in a hashset rather than a list/dictionary
        if (!pointsHash.Contains(point))
        {
          pointsHash.Add(point);
          pointsOrder.Add(point, pIndex);
          pIndex++;
        }
      }

      for (int k = 0; k < meshes[i].triangles.Length; k++)
      {
        int triPoint = meshes[i].triangles[k]; //Get the current point from the mesh
        
        //Check which vertex, normal, and UV that corresponds to that specific point in the triangle 
        //Remember that there are 3 points in a triangle, but the same point will be used many times when constructing a quad and a cube.
        Vector3 vertices = meshes[i].vertices[triPoint];
        Vector3 normals = meshes[i].normals[triPoint];
        Vector2 uvs = meshes[i].uv[triPoint];
        
        VertexData point = new VertexData(vertices, normals, uvs);
        
        int index;
        pointsOrder.TryGetValue(point, out index); //Check if the current point already exists in our Dictionary
        triangles.Add(index); //We now add the index so that we know which vertices it's pointing to

      }
      meshes[i] = null;
    }

    //Extract all the data from our VertexData dictionary into arrays and put them into our new mesh
    ExtractArrays(pointsOrder, mesh);
    
    //Add in all our triangles so that we can figure out which points will make up triangles / quads.
    mesh.triangles = triangles.ToArray();
    //Recalcuate the bounds of the new mesh
    mesh.RecalculateBounds();

    return mesh; //Return our final combined mesh
  }

  public static void ExtractArrays(Dictionary<VertexData, int> list, Mesh mesh)
  {
    List<Vector3> vertices = new List<Vector3>();
    List<Vector3> normals = new List<Vector3>();
    List<Vector2> uvs = new List<Vector2>();

    //Get data out of our Tuple
    foreach (var v in list.Keys)
    {
      //Item 1,2,3 is just the syntax on how you access data from a tuple, wher the Tuple in this case is the one
      //we created at the top with the using statement
      vertices.Add(v.Item1);
      normals.Add(v.Item2);
      uvs.Add(v.Item3);
    }

    //Add all the data to our mesh
    mesh.vertices = vertices.ToArray();
    mesh.normals = normals.ToArray();
    mesh.uv = uvs.ToArray();
  }
    
}
