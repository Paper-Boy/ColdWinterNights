using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapGenerator : MonoBehaviour
{
    public int width = 128, height = 128;
    public float scale = 5.0f;
    public GameObject treePrefab;
    public Transform treeParent;

    public Initializer initializer;

    [HideInInspector]
    public Vector2 mapSize;
    private float[,] noiseMap;
    private int[,] map;

    private void Awake()
    {
        GameManager.instance.earlyInit += EarlyInit;
    }

    private void EarlyInit()
    {
        GameManager.instance.mapGenerator = this;

        mapSize = new Vector2(
            x: width / 25.0f,
            y: height / 25.0f);

        noiseMap = GenerateNoiseMap();
        map = GenerateMap(noiseMap);

        StartCoroutine(DrawMap(map));
    }

    private float[,] GenerateNoiseMap()
    {
        noiseMap = new float[width, height];
        map = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                noiseMap[x, y] = CalculateValue(x, y);
            }
        }

        return noiseMap;
    }

    private float CalculateValue(int x, int y)
    {
        float xCoord = (float)x / width * scale;
        float yCoord = (float)y / height * scale;

        float sample = Mathf.PerlinNoise(xCoord, yCoord);

        return sample;
    }

    private int[,] GenerateMap(float[,] noiseMap)
    {
        int[,] map = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float chance = Mathf.Clamp(noiseMap[x, y], 0.0f, 0.50f) * 0.325f - Random.Range(0.1f, 0.25f);
                if (Decision(chance))
                    map[x, y] = 1;
                else
                    map[x, y] = 0;
            }
        }
        return map;
    }

    private IEnumerator DrawMap(int[,] map)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (map[x, y] == 1)
                {
                    Vector2 pos = new Vector2(
                        (x - width / 2.0f) / 25.0f,
                        (height / 2.0f - y) / 25.0f);
                    Instantiate(treePrefab, pos, Quaternion.identity, treeParent).GetComponent<Tree>().OldTree();
                }
            }

            initializer.LoadingProgress(y / (float)height);
            yield return null;
        }

        initializer.LoadingProgress();
    }

    private bool Decision(float chance)
    {
        if (chance < 0.0f) return false;

        if (Random.value <= chance) return true;
        else return false;
    }
}