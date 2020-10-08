using UnityEngine;

public class Monster : MonoBehaviour
{
    // Footsteps
    [Header("Footsteps")]
    public GameObject footstepPrefab;
    private Transform footstepsParent;
    private float deltaPos = 0.0f;
    private float lastStep = 0.0f;
    private bool reversed = false;

    // Movement
    [Header("Movement")]
    [Range(0.1f, 10)]
    public float speed = 1.0f;
    private Transform player;
    private Vector2 targetPos;
    private bool moveToPlayer = true;
    private Vector2 mapSize;
    private Animator animator;

    // Lifetime
    [Header("Lifetime")]
    [Range(10, 300)]
    public float lifeTime = 60.0f;
    private float born = 0.0f;

    private MonsterController monsterController;

    public void Init(MonsterController monsterController)
    {
        this.monsterController = monsterController;
        footstepsParent = GameManager.instance.footstepsParent;

        GameManager.instance.update += UpdateC;

        born = GameManager.instance.GameTime;
        player = GameManager.instance.player.transform;

        mapSize = GameManager.instance.mapGenerator.mapSize / 2.0f;

        animator = GetComponent<Animator>();
    }

    private void UpdateC()
    {
        if (born + lifeTime <= GameManager.instance.GameTime)
            Destroy(gameObject);

        Movement();
    }

    private void Movement()
    {
        float distance = Vector2.Distance(targetPos, transform.position);
        float speedC = speed;

        if (Mathf.Abs(CalculatePlayerRotation()) >= 55.0f)
        {
            targetPos = player.position;
            moveToPlayer = true;
        }
        else if (moveToPlayer || distance <= 0.25f)
        {
            targetPos = new Vector2(
                x: Random.Range(-mapSize.x, mapSize.x),
                y: Random.Range(-mapSize.y, mapSize.y));
            moveToPlayer = false;
            speedC *= 2.0f;
        }

        if (distance >= 0.25f)
        {
            animator.SetBool("walking", true);

            Vector2 oldPos = transform.position;

            transform.position = Vector2.MoveTowards(transform.position, targetPos, speedC * 0.1f * Time.deltaTime);

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
        else
        {
            animator.SetBool("walking", false);
            player.GetComponent<Player>().AddHealth(-1.5f * Time.fixedDeltaTime);
        }
    }

    private float CalculatePlayerRotation()
    {
        Vector3 relative = player.InverseTransformPoint(transform.position);
        return Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;
    }

    private void OnDestroy()
    {
        monsterController.Deregister(this);

        GameManager.instance.update -= UpdateC;
    }
}
