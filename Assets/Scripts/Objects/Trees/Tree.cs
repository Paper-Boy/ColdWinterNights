using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Tree : MonoBehaviour, IObject
{
    // Materials
    [Header("Materials")]
    public Material litMaterial;
    public Material unlitMaterial;

    [Space(10)]
    public new BoxCollider2D collider2D;

    private Animator animator;

    private float born = 0.0f;
    private float hitPoints = 1.0f;

    private bool alive = true;
    private bool old = false;
    private bool chop = false;

    public ObjectType ObjectsType { get; } = ObjectType.Tree;

    private Coroutine growTree;

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

        animator = GetComponent<Animator>();

        GameManager.instance.lateUpdate += LateUpdateC;
        growTree = StartCoroutine(GrowTree());
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
            chop = true;
            return -1;
        }
    }

    public void LateUpdateC()
    {
        if (!alive)
        {
            GameManager.instance.lateUpdate -= LateUpdateC;
            if (growTree != null)
                StopCoroutine(growTree);
            Destroy(gameObject);
        }

        animator.SetBool("Chop", chop);
        chop = false;
    }

    private IEnumerator GrowTree()
    {
        float scaleValue;

        do
        {
            scaleValue = Mathf.Clamp((GameManager.instance.GameTime - born) / 6 * 0.045f, 0.02f, 0.45f);
            transform.localScale = new Vector3(scaleValue, scaleValue, 1);

            yield return new WaitForSeconds(0.5f);
        } while (!old && scaleValue < 0.45f);

        transform.localScale = new Vector3(0.45f, 0.45f, 1);

        growTree = null;
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
