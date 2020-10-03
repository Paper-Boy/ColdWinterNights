using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Builder : MonoBehaviour
{
    private SpriteRenderer sprite;
    private new BoxCollider2D collider;
    private GameObject building;

    private Vector2 targetPos = new Vector2(-999, -999);
    private float time;
    private bool update = true;

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
        if (update == true)
        {
            UpdateBuildability();
            update = false;
        }

        if (targetPos != new Vector2(-999, -999) && time <= GameManager.instance.GameTime)
        {
            transform.position = targetPos;
            targetPos = new Vector2(-999, -999);

            update = true;
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
        time = GameManager.instance.GameTime + 0.1f;
    }
}
