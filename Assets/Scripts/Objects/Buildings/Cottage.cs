using UnityEngine;

public class Cottage : Building
{
    public override void Awake()
    {
        base.Awake();
    }

    public override void UpdateC()
    {
        base.UpdateC();

        if (woodNeeded > 0 || hitPoints <= 0)
            return;

        if (Vector2.Distance(transform.position, GameManager.instance.player.transform.position) <= range)
            GameManager.instance.player.AddHealth(1.0f * Time.deltaTime);
    }

    public override int Build(int wood)
    {
        int ret = base.Build(wood);

        if (woodNeeded <= 0)
            GameManager.instance.ui.UnlockBuildings();

        return ret;
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
    }
}
