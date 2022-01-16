using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Perlin3DGrapher : MonoBehaviour
{
    [SerializeField] private Vector3 dimensions = new Vector3(10,10,10);
    [SerializeField] private float perlinHeightScale = 2;
    [SerializeField] private float perlinScale = 0.5f;
    [SerializeField, Range(0f,10f)] private float drawCutOff = 1;
    [SerializeField] private int octaves;
    [SerializeField] private float yHeightOffset;
    [SerializeField] private int seed;
    
    public float PerlinScale => perlinScale;
    public float PerlinHeightScale => perlinHeightScale;
    public float DrawCutOff => drawCutOff;
    public int Octaves => octaves;
    public int Seed => seed;
    public float YHeightOffset => yHeightOffset;

    void CreateCubes()
    {
        for (int z = 0; z < dimensions.z; z++)
        {
            for (int y = 0; y < dimensions.y; y++)
            {
                for (int x = 0; x < dimensions.x; x++)
                {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.name = $"Perlin_cube_({x},{y},{z})";
                    cube.transform.parent = transform;
                    cube.transform.position = new Vector3(x, y, z);
                }
            }
        }
    }

    void Graph()
    {
        //Get the mesh of all the primitives we create in CreateCubes
        MeshRenderer[] cubes = GetComponentsInChildren<MeshRenderer>();

        if (cubes.Length == 0)
        {
            CreateCubes();
        }

        if (cubes.Length == 0) return;
        
        for (int z = 0; z < dimensions.z; z++)
        {
            for (int y = 0; y < dimensions.y; y++)
            {
                for (int x = 0; x < dimensions.x; x++)
                {
                    //Get the perlin value for this specific spot
                    float perlin3D = MeshUtils.FractalBrownianMotion3D(x, y, z, octaves, perlinScale, perlinHeightScale,
                        yHeightOffset, seed);

                    if (perlin3D < drawCutOff)
                    {
                        cubes[x + (int)dimensions.x * (y + (int)dimensions.z * z)].enabled = false;
                    }
                    else
                    {
                        cubes[x + (int)dimensions.x * (y + (int)dimensions.z * z)].enabled = true;
                    }
                }
            }
        }
    }

    private void OnValidate()
    {
        Graph();
    }
}
