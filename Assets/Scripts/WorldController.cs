using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WorldController : MonoBehaviour
{
    [SerializeField] private GameObject block;
    [SerializeField] private int worldSize = 2;
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 2;
    [SerializeField] private int depth = 10;
    

    IEnumerator BuildWorld()
    {
        for (int z = 0; z < depth; z++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (y >= height - 2 && Random.Range(0,100) > 50)
                    {
                        continue;
                    }

                    Vector3 pos = new Vector3(x, y, z);
                    GameObject cube = Instantiate(block, pos, Quaternion.identity);
                    cube.name = $"{x} - {y} - {z}";
                    cube.GetComponent<Renderer>().material = new Material(Shader.Find("Standard")); //Give every cube a it's own material so that Unity can't batch them together. Tanks the FPS.
                }
                yield return null;
            }
        }
    }

    private void Start()
    {
        StartCoroutine(BuildWorld());
    }
}
