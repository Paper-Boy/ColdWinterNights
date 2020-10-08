using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This Script generates a Map with the given size by using a perlin noise map and some random values
/// The Script fills the Map with Trees and Stones
/// </summary>
public class MapGenerator : MonoBehaviour
{
    public int width = 128, height = 128;
    public float scale = 5.0f;
    public GameObject treePrefab;
    public Transform treeParent;

    public GameObject stonePrefab;
    public List<Sprite> stoneSprites;

    public Initializer initializer;

    [HideInInspector]
    public Vector2 mapSize;
    [HideInInspector]
    public float[,] noiseMap;
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
                else if (Decision(chance * 0.05f))
                    map[x, y] = 2;
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
                // Spawn a Tree
                if (map[x, y] == 1)
                {
                    Vector2 pos = new Vector2(
                        x: (x - width / 2.0f) / 25.0f,
                        y: (height / 2.0f - y) / 25.0f);

                    Instantiate(treePrefab, pos, Quaternion.identity, treeParent).GetComponent<Tree>().OldTree();
                }
                // Spawn a stone
                else if(map[x, y] == 2)
                {
                    Vector2 pos = new Vector2(
                        x: (x - width / 2.0f) / 25.0f,
                        y: (height / 2.0f - y) / 25.0f);

                    GameObject stone = Instantiate(stonePrefab, pos, Quaternion.identity, treeParent);
                    stone.GetComponent<SpriteRenderer>().sprite = stoneSprites[Random.Range(0, stoneSprites.Count)];
                    stone.transform.localScale *= Random.Range(0.9f, 1.1f);

                    if (Decision(0.5f))
                        stone.transform.localScale = new Vector3(-0.2f, 0.2f, 1.0f);
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