using System.Collections.Generic;
using UnityEngine;

/// <summary>
///	Handles
/// - spawn of new monsters (including position)
/// - maximum monster count
/// </summary>
public class MonsterController : MonoBehaviour
{
    public GameObject monsterPrefab;

    [Range(10, 600)]
    public float newMonsterEverySeconds = 60.0f;

    [Range(0, 25)]
    public int maximumAmountOfMonsters = 10;

    private readonly List<Monster> monsters = new List<Monster>();
    private Vector2 mapSize;

    private void Awake()
    {
        GameManager.instance.init += Init;
    }

    private void Init()
    {
        GameManager.instance.update += UpdateC;
        mapSize = GameManager.instance.mapGenerator.mapSize / 2;
    }

    private void UpdateC()
    {
        int maxMonsters = Mathf.Clamp(Mathf.RoundToInt(GameManager.instance.GameTime / newMonsterEverySeconds), 1, maximumAmountOfMonsters);

        while (monsters.Count < maxMonsters)
        {
            Monster monster = Instantiate(monsterPrefab, MonsterPosition(), Quaternion.identity, transform).GetComponent<Monster>();
            monster.Init(this);
            monsters.Add(monster);

            newMonsterEverySeconds = Mathf.Clamp(newMonsterEverySeconds - 10.0f, 60.0f, 600.0f);
        }
    }

    private Vector2 MonsterPosition()
    {
        Vector2 pos;
        float rand = Random.Range(0, 3);

        switch (rand)
        {
            case 0:
                pos.x = -mapSize.x;
                pos.y = Random.Range(-mapSize.y, mapSize.y);
                break;
            case 1:
                pos.x = Random.Range(-mapSize.x, mapSize.x);
                pos.y = -mapSize.y;
                break;
            case 2:
                pos.x = mapSize.x;
                pos.y = Random.Range(-mapSize.y, mapSize.y);
                break;
            case 3:
                pos.x = Random.Range(-mapSize.x, mapSize.x);
                pos.y = mapSize.y;
                break;
            default:
                pos = Vector2.zero;
                break;
        }

        return pos;
    }

    public void Deregister(Monster monster)
    {
        if (monsters.Contains(monster))
            monsters.Remove(monster);
    }
}
