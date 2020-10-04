using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Tree : MonoBehaviour, IObject
{
    public Material litMaterial;
    public Material unlitMaterial;

    public new BoxCollider2D collider2D;

    private float born = 0.0f;
    private float hitPoints = 1.0f;

    private bool alive = true;
    private bool old = false;

    public ObjectType ObjectsType { get; } = ObjectType.Tree;

    private void Start()
    {
        born = GameManager.instance.GameTime;

        if (!GameManager.instance.light)
        {
            Destroy(gameObject.GetComponent<ShadowCaster2D>());
            GetComponent<SpriteRenderer>().material = unlitMaterial;
        }
        else
        {
            GetComponent<SpriteRenderer>().material = litMaterial;
        }

        GameManager.instance.update += UpdateC;
    }

    // Player chops on tree
    // Returns amount of wood if tree is chopped down or -1 if tree is still standing
    public int Chop(float hit)
    {
        hitPoints -= hit;

        if (hitPoints <= 0)
        {
            alive = false;

            if (old)
                return 10;
            else
                return (int)Mathf.Clamp((GameManager.instance.GameTime - born) / 5, 0, 10);
        }
        else
        {
            return -1;
        }
    }

    public void UpdateC()
    {
        if (!alive)
        {
            GameManager.instance.update -= UpdateC;
            Destroy(gameObject);
        }
    }

    public void OldTree()
    {
        old = true;
    }

    public Vector2 ClosestPoint(Vector2 from)
    {
        return collider2D.ClosestPoint(from);
    }
}
