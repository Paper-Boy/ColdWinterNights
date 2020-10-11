using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;

public abstract class Building : MonoBehaviour, IObject
{
    [Header("Variables")]
    public new string name;
    public int woodNeeded;
    public float range;
    public float hitPoints = 200.0f;
    private SpriteRenderer sprite;

    // UI Elements
    [Header("UI Elements")]
    public Sprite icon;
    public Sprite inConstruction;
    public Sprite built;
    public TMP_Text woodRemainingText;
    public Image woodImage;
    public Slider hitSlider;
    public SpriteRenderer overlay;

    public ObjectType ObjectsType { get; } = ObjectType.Building;

    public virtual void Awake()
    {
        GameManager.instance.update += UpdateC;

        sprite = GetComponent<SpriteRenderer>();
        sprite.sprite = inConstruction;
        gameObject.layer = (int)Layers.Building;

        woodRemainingText.text = woodNeeded.ToString("0", GameManager.instance.culture);
    }

    public virtual void UpdateC()
    {
        if (woodNeeded <= 0)
        {
            hitPoints = Mathf.Clamp(hitPoints - 0.25f * Time.deltaTime, 0, 200);

            if (hitPoints < 100.0f)
            {
                hitSlider.gameObject.SetActive(true);
                hitSlider.value = hitPoints;
            }
            else
            {
                hitSlider.gameObject.SetActive(false);
            }
        }
    }

    public virtual int Build(int wood)
    {
        if (woodNeeded > 0)
        {
            woodNeeded -= wood;

            woodRemainingText.text = woodNeeded.ToString("0", GameManager.instance.culture);

            if (woodNeeded <= 0)
            {
                sprite.sprite = built;
                woodRemainingText.gameObject.SetActive(false);
                woodImage.gameObject.SetActive(false);
                overlay.sprite = built;
                overlay.transform.localScale = new Vector3(1, 1, 1);

                return -woodNeeded;
            }
        }
        else
        {
            hitPoints += wood;
        }

        return 0;
    }

    public virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
