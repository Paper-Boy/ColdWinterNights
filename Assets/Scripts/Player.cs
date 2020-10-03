using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Player : MonoBehaviour
{
    // Navigation
    private Vector2 targetPosition = new Vector2(-999, -999);
    private GameObject targetObject = null;
    private float angle = 0.0f;
    private bool rotate = false;
    private Vector2 mapSize;

    // Footstep
    private float deltaPos = 0.0f;
    private float lastStep = 0.0f;
    private bool reversed = false;

    // Flashlight
    public Transform flashlight;
    public Light2D flashlightLight;
    public Transform flashlightCircle;

    public SpriteRenderer shadowReceiver;

    public Material unlitMaterial;

    public SpriteRenderer sprite;
    public Animator animator;

    public float speed = 1.0f;

    public Transform playerRelatedObjects;

    public GameObject footstepPrefab;
    public Transform footstepParent;

    public int Wood { private set; get; } = 0;
    public float Health { private set; get; } = 100.0f; // Max 100

    #region Initialization

    private void Awake()
    {
        GameManager.instance.init += Init;
    }

    private void Init()
    {
        GameManager.instance.player = this;
        GameManager.instance.update += UpdateC;

        mapSize = GameManager.instance.mapGenerator.mapSize / 2.0f;

        if (!GameManager.instance.light)
        {
            shadowReceiver.material = unlitMaterial;
            flashlightLight.enabled = false;
            flashlight.GetComponent<SpriteRenderer>().enabled = true;
            flashlightCircle.GetComponent<SpriteRenderer>().enabled = true;
        }
    }

    #endregion

    public void SetTarget(Vector2 targetPosition)
    {
        this.targetPosition = targetPosition;
        this.targetPosition.x = Mathf.Clamp(this.targetPosition.x, -mapSize.x, mapSize.x);
        this.targetPosition.y = Mathf.Clamp(this.targetPosition.y, -mapSize.y, mapSize.y);
        targetObject = null;
        rotate = false;

        CalculateTargetRotation(targetPosition);
    }

    public void SetTarget(GameObject targetObject)
    {
        this.targetObject = targetObject;
        targetPosition = new Vector2(-999, -999);
        rotate = false;

        CalculateTargetRotation(targetObject.transform.position);
    }

    public void SetRotation(Vector2 targetPosition)
    {
        this.targetPosition = targetPosition;
        targetObject = null;
        rotate = true;
    }

    private void CalculateTargetRotation(Vector2 targetPos)
    {
        Vector3 relative = transform.InverseTransformPoint(targetPos);
        angle = Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;
    }

    // Handles Interaction and Movement
    private void UpdateC()
    {
        if (Health <= 0.0f)
            return;

        AddHealth(-0.5f * Time.deltaTime);

        animator.SetBool("axe", false);
        animator.SetBool("walking", false);

        // Interact if in Range of Target Object
        if (targetObject != null)
        {
            switch (targetObject.GetComponent<IObject>().ObjectsType)
            {
                case ObjectType.Tree:
                    Vector2 closestPoint = targetObject.GetComponent<Tree>().ClosestPoint(transform.position);

                    if (Vector2.Distance(transform.position, closestPoint) < 0.25f)
                    {
                        animator.SetBool("axe", true);

                        int wood = targetObject.GetComponent<Tree>().Chop(0.25f * Time.deltaTime);

                        if (wood != -1)
                        {
                            Wood += wood;
                            targetObject = null;
                        }
                    }
                    else
                    {
                        animator.SetBool("axe", true);

                        WalkTowards(targetObject.transform.position);
                    }
                    break;
                case ObjectType.Building:
                    if (Vector2.Distance(transform.position, targetObject.transform.position) < 0.5f)
                    {
                        Wood = targetObject.GetComponent<Building>().Build(Wood);
                        targetObject = null;
                    }
                    else
                    {
                        WalkTowards(targetObject.transform.position);
                    }
                    break;
            }
        }
        else if (targetPosition != new Vector2(-999, -999))
        {
            if (targetPosition == (Vector2)transform.position)
                targetPosition = new Vector2(-999, -999);
            else
                WalkTowards(targetPosition);
        }
    }

    private void WalkTowards(Vector2 targetPos)
    {
        animator.SetBool("walking", true);

        Vector2 oldPos = transform.position;

        if (transform.position.x > targetPos.x)
            sprite.transform.localScale = new Vector3(-0.15f, 0.2f, 1.0f);
        else
            sprite.transform.localScale = new Vector3(0.15f, 0.2f, 1.0f);

        transform.Rotate(0, 0, -angle * speed * 20.0f * Time.deltaTime);
        CalculateTargetRotation(targetPos);

        playerRelatedObjects.rotation = Quaternion.identity;

        if (!rotate)
            transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        else
            animator.SetBool("walking", false);

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

            Instantiate(footstepPrefab, transform.position - new Vector3(0, 0, 0), targetRotation, footstepParent);

            lastStep = GameManager.instance.GameTime;
            deltaPos = 0.0f;
            reversed = !reversed;
        }
    }

    public void AddHealth(float amount)
    {
        Health += amount;
        Health = Mathf.Clamp(Health, 0.0f, 100.0f);

        flashlight.localScale = new Vector3(3.5f, 5.0f * (Health / 100), 1.0f);
        flashlightCircle.localScale = new Vector3(1.0f, 1.0f, 1.0f) * (Health / 100);
    }

    // Only in Editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, 0.25f);
    }
}

