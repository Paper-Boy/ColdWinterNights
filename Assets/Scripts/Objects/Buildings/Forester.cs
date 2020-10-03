using UnityEngine;

public class Forester : Building
{
    public GameObject treePrefab;

    public float cooldown = 25.0f;
    private float last = 0.0f;

    public override void Awake()
    {
        base.Awake();
        last = GameManager.instance.GameTime;
    }

    public override void UpdateC()
    {
        base.UpdateC();

        if (woodNeeded > 0 || hitPoints <= 0)
            return;

        if (last + cooldown <= GameManager.instance.GameTime && Vector2.Distance(transform.position, GameManager.instance.player.transform.position) <= range)
        {
            Vector2 treePos = new Vector2();

            do
            {
                treePos.x = Random.Range(-range, range);
                treePos.y = Random.Range(-range, range);
            } while (Vector2.Distance(transform.position, (Vector2)transform.position + treePos) > range);

            Instantiate(treePrefab, transform.position + (Vector3)treePos, Quaternion.identity, GameManager.instance.treesParent).GetComponent<SpriteRenderer>().sortingOrder = 1;

            last = GameManager.instance.GameTime;
        }
    }

    public override int Build(int wood)
    {
        return base.Build(wood);
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
    }
}
