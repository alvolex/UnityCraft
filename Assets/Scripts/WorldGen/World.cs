using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct PerlinSettings
{
    public float heightScale;
    public float heightOffset;
    public float scale;
    public int octaves;
    public int seed;
    public float probability;

    public PerlinSettings(float hs, float s, int o, float ho, int worldSeed, float p)
    {
        heightScale = hs;
        scale =s;
        octaves = o;
        heightOffset = ho;
        seed = worldSeed;
        probability = p;
    }
}

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
    
    [Header("Layers")] 
    [SerializeField] private PerlinGrapher surface;
    [SerializeField] private PerlinGrapher stone;
    [SerializeField] private PerlinGrapher diamondTop;
    [SerializeField] private PerlinGrapher diamondBot;
    [SerializeField] private Perlin3DGrapher caves;

    //
    public static PerlinSettings surfaceSettings;
    public static PerlinSettings stoneSettings;
    public static PerlinSettings diamondTopSettings;
    public static PerlinSettings diamondBotSettings;
    public static PerlinSettings caveSettings;


    private void Start()
    {
        loadingBar.maxValue = worldSize.x * worldSize.y * worldSize.z;
        
        surfaceSettings = new PerlinSettings(surface.PerlinHeightScale, surface.PerlinScale, surface.Octaves,
            surface.YHeightOffset, surface.Seed, surface.Probability);
        
        stoneSettings = new PerlinSettings(stone.PerlinHeightScale, stone.PerlinScale, stone.Octaves,
            stone.YHeightOffset, stone.Seed, stone.Probability);
        
        diamondTopSettings = new PerlinSettings(diamondTop.PerlinHeightScale, diamondTop.PerlinScale, diamondTop.Octaves,
            diamondTop.YHeightOffset, diamondTop.Seed, stone.Probability);
        
        diamondBotSettings = new PerlinSettings(diamondBot.PerlinHeightScale, diamondBot.PerlinScale, diamondBot.Octaves,
            diamondBot.YHeightOffset, diamondBot.Seed, diamondBot.Probability);
        
        caveSettings = new PerlinSettings(caves.PerlinHeightScale, caves.PerlinScale, caves.Octaves,
            caves.YHeightOffset, caves.Seed, caves.DrawCutOff);
            
        StartCoroutine(BuildWorld());
    }

    IEnumerator BuildWorld()
    {
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
        float ypos = MeshUtils.FractalBrownianMotion(xpos, zpos, surfaceSettings.octaves, surfaceSettings.scale, surfaceSettings.heightScale,
            surfaceSettings.heightOffset, surfaceSettings.seed) + 10f;
        fpc.transform.position = new Vector3(xpos, ypos, zpos);
    }

    private void SwitchToFirstPersonController()
    {
        mainCam.SetActive(false);
        fpc.SetActive(true);
    }
}
