
using UnityEngine;

[ExecuteInEditMode]
public class PerlinGrapher : MonoBehaviour
{
    private LineRenderer lr;
    [SerializeField] private float perlinHeightScale = 2;
    [SerializeField] private float perlinScale = 0.5f;
    [SerializeField, Range(0f,1f)] private float probability = 1;

    //For Fractal Brownian
    [SerializeField] private int octaves;
    [SerializeField] private float yHeightOffset;
    [SerializeField] private int seed;

    public LineRenderer Lr => lr;
    public float PerlinScale => perlinScale;
    public float PerlinHeightScale => perlinHeightScale;
    public float Probability => probability;
    public int Octaves => octaves;
    public int Seed => seed;
    public float YHeightOffset => yHeightOffset;

    private void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 100;

        Graph();
    }

    private void OnValidate()
    {
        Graph();
    }

    private void Graph()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 100;
        int z = 0; //Chunk size +1 in this case
        
        Vector3[] positions = new Vector3[lr.positionCount];

        for (int x = 0; x < lr.positionCount; x++)
        {
            //float y = Mathf.PerlinNoise(x * perlinScale, z *perlinScale); //Normal perlin
            float y = MeshUtils.FractalBrownianMotion(x,z, octaves, perlinScale, perlinHeightScale, yHeightOffset, seed); //Fractal Brownian Motion
            positions[x] = new Vector3(x, y , z); //Draw a straight line that will runs along the X axis
        }
        lr.SetPositions(positions);
    }

    /*float FractalBrownianMotion(float x, float z)
    {
        float total = 0; //Total height
        float frequency = 1;
        for (int i = 0; i < octaves; i++)
        {
            //PerlinScale = how often we sample, lower value gives smoother noise  - perlingHeightScale = Height of the peaks
            total += Mathf.PerlinNoise(x * perlinScale * frequency + seed, z * perlinScale * frequency + seed)  * perlingHeightScale;
            frequency *= 2; //FBM, this makes it so that we get more variation in our noise
        }

        return total;
    }*/
}
