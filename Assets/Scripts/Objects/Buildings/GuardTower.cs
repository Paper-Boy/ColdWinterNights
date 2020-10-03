using UnityEngine;

public class GuardTower : Building
{
    public SpriteMask mask;

    public override void Awake()
    {
        base.Awake();

        mask.transform.localScale = new Vector3(range * 4, range * 4, 1);
    }

    public override void UpdateC()
    {
        base.UpdateC();

        mask.enabled = (woodNeeded <= 0 && hitPoints > 0);
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
