using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles building blueprint positioning and confirmation
/// </summary>
public class Builder : MonoBehaviour
{
    private SpriteRenderer sprite;
    private new BoxCollider2D collider;
    private GameObject building;

    private Vector2 targetPos = new Vector2(-999, -999);
    private float time;
    private int update = 2;

    public Button confirmButton;

    private void Awake()
    {
        GameManager.instance.fixedUpdate += FixedUpdateC;

        sprite = gameObject.GetComponent<SpriteRenderer>();
        collider = gameObject.GetComponent<BoxCollider2D>();

        gameObject.GetComponent<Canvas>().worldCamera = Camera.main;
    }

    private void FixedUpdateC()
    {
        if (update > 0)
        {
            UpdateBuildability();
            update--;
        }

        if (targetPos != new Vector2(-999, -999) && time <= Time.unscaledTime)
        {
            transform.position = targetPos;
            targetPos = new Vector2(-999, -999);

            update++;
        }
    }

    private bool UpdateBuildability()
    {
        List<ContactPoint2D> contacts = new List<ContactPoint2D>();
        Color color;

        if (collider.GetContacts(contacts) > 0)
        {
            color = Color.red;
            color.a = 0.75f;
            sprite.color = color;

            confirmButton.interactable = false;
            return false;
        }
        else
        {
            sprite.color = Color.green;

            confirmButton.interactable = true;
            return true;
        }
    }

    public void SetUp(GameObject building)
    {
        if (sprite == null)
            sprite = gameObject.GetComponent<SpriteRenderer>();

        this.building = building;
        sprite.sprite = building.GetComponent<Building>().built;
    }

    public void Confirm()
    {
        if (UpdateBuildability())
        {
            Instantiate(building, transform.position, Quaternion.identity, transform.parent);
            GameManager.instance.ui.BuildOverlay(false);

            GameManager.instance.fixedUpdate -= FixedUpdateC;
            Destroy(gameObject);
        }
    }

    public void Abort()
    {
        GameManager.instance.ui.BuildOverlay(false);

        GameManager.instance.fixedUpdate -= FixedUpdateC;
        Destroy(gameObject);
    }

    public void Move(Vector2 position)
    {
        targetPos = position;
        time = Time.unscaledTime + 0.1f;
    }
}
