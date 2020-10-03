using UnityEngine;

public class Footstep : MonoBehaviour
{
    public float lifetime;

    private float endOfLife;
    private SpriteRenderer sprite;

    private void Awake()
    {
        GameManager.instance.update += UpdateC;
        sprite = gameObject.GetComponent<SpriteRenderer>();
        sprite.sortingLayerID = 0;

        endOfLife = GameManager.instance.GameTime + lifetime;
    }

    private void UpdateC()
    {
        if (GameManager.instance.GameTime >= endOfLife)
        {
            GameManager.instance.update -= UpdateC;
            Destroy(gameObject);
        }

        float a = (endOfLife - GameManager.instance.GameTime) / lifetime;
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, a);
    }
}
