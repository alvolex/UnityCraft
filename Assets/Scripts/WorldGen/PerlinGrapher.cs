
using UnityEngine;

[ExecuteInEditMode]
public class PerlinGrapher : MonoBehaviour
{
    private LineRenderer lr;
    [SerializeField] private float perlingHeightScale = 2;
    [SerializeField] private float perlinScale = 0.5f;
    //For Fractal Brownian
    [SerializeField] private int octaves;
    [SerializeField] private float yHeightOffset;
    

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
        int z = 11; //Chunk size +1 in this case
        
        Vector3[] positions = new Vector3[lr.positionCount];

        for (int x = 0; x < lr.positionCount; x++)
        {
            //float y = Mathf.PerlinNoise(x * perlinScale, z *perlinScale); //Normal perlin
            float y = FractalBrownianMotion(x,z); //Fractal Brownian Motion
            positions[x] = new Vector3(x, y - yHeightOffset, z); //Draw a straight line that will runs along the X axis
        }
        lr.SetPositions(positions);
    }

    float FractalBrownianMotion(float x, float z)
    {
        float total = 0; //Total height
        float frequency = 1;
        for (int i = 0; i < octaves; i++)
        {
            //PerlinScale = how often we sample, lower value gives smoother noise  - perlingHeightScale = Height of the peaks
            total += Mathf.PerlinNoise(x * perlinScale * frequency, z * perlinScale * frequency)  * perlingHeightScale;
            frequency *= 2; //FBM, this makes it so that we get more variation in our noise
        }

        return total;
    }
}
