using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class TreeController : MonoBehaviour
{
    private readonly List<TreeObj> trees = new List<TreeObj>();
    private Vector2 playerPos;
    private Vector2 lastPos = new Vector2(-999, -999);

    private Thread treeThread;
    private bool update = false;

    private void Awake()
    {
        GameManager.instance.lateInit += Init;
    }

    private void Init()
    {
        if (!GameManager.instance.light)
        {
            Destroy(GetComponent<CompositeShadowCaster2D>());
            Destroy(this);
        }
        else
        {
            GameManager.instance.fixedUpdate += FixedUpdateC;

            foreach (ShadowCaster2D tree in GetComponentsInChildren<ShadowCaster2D>())
            {
                trees.Add(new TreeObj(tree, tree.transform.position));
            }
        }
    }

    private void FixedUpdateC()
    {
        if (Vector2.Distance(lastPos, GameManager.instance.player.transform.position) >= 0.2f)
        {
            if (treeThread != null && treeThread.IsAlive)
                treeThread.Join();

            playerPos = GameManager.instance.player.transform.position;

            treeThread = new Thread(new ThreadStart(UpdateTrees));
            treeThread.IsBackground = true;
            treeThread.Start();

            lastPos = playerPos;
        }

        if (update)
        {
            foreach (TreeObj tree in trees)
                tree.shadowCaster.enabled = tree.enabled;

            update = false;
        }
    }

    private void UpdateTrees()
    {
        foreach (TreeObj tree in trees.ToArray())
        {
            if (tree.shadowCaster == null)
            {
                trees.Remove(tree);
                continue;
            }

            tree.enabled = Vector2.Distance(tree.position, playerPos) <= 3.5f;
        }

        update = true;
    }
}

public class TreeObj
{
    public readonly ShadowCaster2D shadowCaster;
    public readonly SpriteRenderer renderer;
    public readonly Vector2 position;
    public bool enabled;

    public TreeObj(ShadowCaster2D shadowCaster, Vector2 position, bool enabled = true)
    {
        this.shadowCaster = shadowCaster;
        renderer = shadowCaster.gameObject.GetComponent<SpriteRenderer>();
        this.position = position;
        this.enabled = enabled;
    }
}
