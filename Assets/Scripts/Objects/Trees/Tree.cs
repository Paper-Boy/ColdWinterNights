using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

/// <summary>
/// Handles growing, lifetime and hitPoints of tree
/// Also calculates closest point from colliders to player
/// </summary>
public class Tree : MonoBehaviour, IObject
{
    public new BoxCollider2D collider2D;

    private Animator animator;

    private float born = 0.0f;
    private float hitPoints = 1.0f;
    private int woodContent = 10;

    private bool alive = true;
    private bool old = false;
    private bool chop = false;

    public ObjectType ObjectsType { get; } = ObjectType.Tree;

    private Coroutine growTree;

    private void Start()
    {
        born = GameManager.instance.GameTime;

        animator = GetComponent<Animator>();

        GameManager.instance.lateUpdate += LateUpdateC;

        woodContent = Random.Range(8, 15);

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
                return woodContent;
            else
                return (int)(Mathf.Clamp((GameManager.instance.GameTime - born) / 60, 0.0f, 1.0f) * woodContent);
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


    // Scales Tree acording to woodContent
    private IEnumerator GrowTree()
    {
        float scaleValue;

        do
        {
            scaleValue = Mathf.Clamp((GameManager.instance.GameTime - born) / 6 * 0.045f, 0.02f, 0.45f) / 15;
            scaleValue *= woodContent;
            transform.localScale = new Vector3(scaleValue, scaleValue, 1);

            yield return new WaitForSeconds(0.5f);
        } while (!old && scaleValue < 0.45f);

        transform.localScale = new Vector3(0.45f, 0.45f, 1) / 15 * woodContent;

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
