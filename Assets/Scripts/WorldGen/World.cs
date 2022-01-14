using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class World : MonoBehaviour
{
    [Header("Cameras and loading")]
    [SerializeField] private Slider loadingBar;
    [SerializeField] private GameObject mainCam;
    [SerializeField] private GameObject fpc;
   
    [Header("World building")]
    [SerializeField] private Vector3 worldSize = new Vector3(5,3,2);
    public static Vector3 worldDimensions = new Vector3(3, 3, 3);
    public static Vector3 chunkDimensions = new Vector3(10, 10, 10);
    public GameObject chunkPrefab;


    private void Start()
    {
        loadingBar.maxValue = worldSize.x * worldSize.y * worldSize.z; 
        StartCoroutine(BuildWorld());
    }

    IEnumerator BuildWorld()
    {
        Debug.Log(loadingBar.maxValue);
        Debug.Log(worldDimensions.x);
        
        worldDimensions = worldSize;
        for (int z = 0; z < worldDimensions.z; z++)
        {
            for (int y = 0; y < worldDimensions.y; y++)
            {
                for (int x = 0; x < worldDimensions.x; x++)
                {
                    GameObject chunk = Instantiate(chunkPrefab);
                    Vector3 pos = new Vector3(x * chunkDimensions.x, y * chunkDimensions.y, z * chunkDimensions.z);
                    chunk.GetComponent<Chunk>().CreateChunk(chunkDimensions, pos);
                    loadingBar.value++;
                    yield return null;
                }
            }
        }
        //Disable loading bar and then swap camera
        loadingBar.gameObject.SetActive(false);

        MoveFPCToCenterOfMap();
        SwitchToFirstPersonController();
    }

    private void MoveFPCToCenterOfMap()
    {
        float xpos = (worldDimensions.x * chunkDimensions.x) / 2f;
        float zpos = (worldDimensions.z * chunkDimensions.y) / 2f;
        Chunk c = chunkPrefab.GetComponent<Chunk>();
        float ypos = MeshUtils.FractalBrownianMotion(xpos, zpos, c.Octaves, c.PerlinScale, c.HeightScale,
            c.HeightOffset, c.GenerationSeed) + 10f;
        fpc.transform.position = new Vector3(xpos, ypos, zpos);
    }

    private void SwitchToFirstPersonController()
    {
        mainCam.SetActive(false);
        fpc.SetActive(true);
    }
}
