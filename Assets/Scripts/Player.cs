﻿using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

/// <summary>
/// Keeps track of health, woodAmount and players temperatureMap
/// Handles 
/// - life loss beacause of temperatureMap
/// - player movement and rotation
/// - player interacting with objects (e.g. tree)
/// </summary>
public class Player : MonoBehaviour
{
    // Debug
    [Header("Debug")]
    public bool loseLife = true;

    // Navigation and Movement
    [Header("Navigation and Movement")]
    public float speed = 1.0f;
    public Transform playerRelatedObjects;
    private Vector2 targetPosition = new Vector2(-999, -999);
    private GameObject targetObject = null;
    private float angle = 0.0f;
    private bool rotate = false;
    private Vector2 mapSize;

    // Footstep
    [Header("Footsteps")]
    private float deltaPos = 0.0f;
    private float lastStep = 0.0f;
    private bool reversed = false;
    public GameObject footstepPrefab;
    public Transform footstepParent;

    // Flashlight
    [Header("Flashlight")]
    public Transform flashlight;
    public Transform flashlightCircle;


    // Animation
    [Header("Animation")]
    public SpriteRenderer sprite;
    public Animator animator;

    private GameManager gameManager;

    public int Wood { private set; get; } = 0;
    public float Health { private set; get; } = 100.0f; // Max 100

    public float Temperature { private set; get; } = 0.0f;

    #region Initialization

    private void Awake()
    {
        gameManager = GameManager.instance;
        gameManager.init += Init;
    }

    private void Init()
    {
        gameManager.player = this;
        gameManager.update += UpdateC;

        mapSize = gameManager.mapGenerator.mapSize / 2.0f;
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

        CalculateTemperature();
        AddHealth(Temperature * 1.5f * Time.deltaTime);

        animator.SetBool("axe", false);
        animator.SetBool("walking", false);

        // Interact if in Range of Target Object
        if (targetObject != null)
        {
            if (targetObject.GetComponent<IObject>() == null)
            {
                targetObject = null;
                return;
            }

            switch (targetObject.GetComponent<IObject>().ObjectsType)
            {
                case ObjectType.Tree:
                    Vector2 closestPoint = targetObject.GetComponent<Tree>().ClosestPoint(transform.position);

                    if (Vector2.Distance(transform.position, closestPoint) < 0.15f)
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
                default:
                    targetObject = null;
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

        // Make sure sprite does not get moved independetly
        transform.position = sprite.transform.position;
        sprite.transform.localPosition = Vector3.zero;

        deltaPos += Vector2.Distance(oldPos, transform.position);

        // Draw Footsteps
        if (deltaPos >= 0.1f || (deltaPos >= 0.025f && gameManager.GameTime - lastStep >= 1.0f))
        {
            Vector3 rotatedVectorToTarget;

            if (reversed)
                rotatedVectorToTarget = Quaternion.Euler(0, 0, 180) * ((Vector3)targetPos - transform.position);
            else
                rotatedVectorToTarget = Quaternion.Euler(0, 0, 0) * ((Vector3)targetPos - transform.position);

            Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);

            Instantiate(footstepPrefab, transform.position - new Vector3(0, 0, 0), targetRotation, footstepParent);

            lastStep = gameManager.GameTime;
            deltaPos = 0.0f;
            reversed = !reversed;
        }
    }

    public void AddHealth(float amount)
    {
        if (!loseLife)
            return;

        Health += amount;
        Health = Mathf.Clamp(Health, 0.0f, 100.0f);

        float flashlightScale = Mathf.Clamp(Health / 100, 0.5f, 1.0f);

        flashlight.localScale = new Vector3(4.0f, 5.0f * flashlightScale, 1.0f);
        flashlightCircle.localScale = new Vector3(1.0f, 1.0f, 1.0f) * flashlightScale;

        if (Health <= 0.0f)
        {
            gameManager.Death();
        }
    }

    private void CalculateTemperature()
    {
        float delta = gameManager.temperatureMap.GetTemp(transform.position) - Temperature;
        Temperature = Mathf.Clamp(Temperature + (delta * Time.deltaTime * 0.005f), -1.0f, 1.0f);
    }

    // Only in Editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, 0.25f);
    }
}

