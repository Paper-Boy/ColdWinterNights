using UnityEngine;

public class Sheep : MonoBehaviour
{
    // Footsteps
    [Header("Footsteps")]
    public GameObject footstepPrefab;
    public Transform footstepsParent;
    private float deltaPos = 0.0f;
    private float lastStep = 0.0f;
    private bool reversed = false;

    // Movement
    [Header("Movement")]
    [Range(0.1f, 10)]
    public float speed = 1.0f;
    private Vector2 targetPos;
    private Vector2 mapSize;
    private Animator animator;
    private float cooldown = 0.0f;

    private void Awake()
    {
        GameManager.instance.earlyInit += EarlyInit;
        GameManager.instance.init += Init;
    }

    private void EarlyInit()
    {
        do 
        {
            targetPos.x = Random.Range(-10.0f, 10.0f);
            targetPos.y = Random.Range(-10.0f, 10.0f);
        }
        while (Vector2.Distance(Vector2.zero, targetPos) < 4.0f);

        transform.position = targetPos;
    }

    private void Init()
    {
        animator = GetComponent<Animator>();
        mapSize = GameManager.instance.mapGenerator.mapSize;

        GameManager.instance.update += UpdateC;
    }

    private void UpdateC()
    {
        if (cooldown < GameManager.instance.GameTime && Vector2.Distance(targetPos, transform.position) >= 0.25f)
        {
            animator.SetBool("walking", true);

            Vector2 oldPos = transform.position;

            transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

            if (transform.position.x > targetPos.x)
                transform.localScale = new Vector3(0.2f, 0.2f, 1.0f);
            else
                transform.localScale = new Vector3(-0.2f, 0.2f, 1.0f);

            deltaPos += Vector2.Distance(oldPos, transform.position);

            // Draw Footsteps
            if (deltaPos >= 0.1f || (deltaPos >= 0.025f && GameManager.instance.GameTime - lastStep >= 1.0f))
            {
                Vector3 rotatedVectorToTarget;

                if (reversed)
                    rotatedVectorToTarget = Quaternion.Euler(0, 0, 180) * ((Vector3)targetPos - transform.position);
                else
                    rotatedVectorToTarget = Quaternion.Euler(0, 0, 0) * ((Vector3)targetPos - transform.position);

                Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);

                Instantiate(footstepPrefab, transform.position, targetRotation, footstepsParent);

                lastStep = GameManager.instance.GameTime;
                deltaPos = 0.0f;
                reversed = !reversed;
            }
        }
        else if(cooldown < GameManager.instance.GameTime)
        {
            animator.SetBool("walking", false);
            targetPos.x = Random.Range(-mapSize.x / 2, mapSize.x / 2);
            targetPos.y = Random.Range(-mapSize.y / 2, mapSize.y / 2);
            cooldown = GameManager.instance.GameTime + 5.0f;
        }

        if(Vector2.Distance(GameManager.instance.player.transform.position, transform.position) <= 1.5f)
            GameManager.instance.player.GetComponent<Player>().AddHealth(1.5f * Time.fixedDeltaTime);
    }

    // Only in Editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, 1.5f);
    }
}
