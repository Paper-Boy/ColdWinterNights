using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public TMP_Text woodAmountText;
    public Slider healthAmount;
    public TMP_Text healthDeltaText;
    public Image whiteOut;

    public GameObject debugParent;
    public TMP_Text timeText;
    public TMP_Text performanceText;
    private readonly Queue<float> fps = new Queue<float>();

    private readonly Queue<float> health = new Queue<float>();

    public GameObject buildButton;
    public GameObject buildOverlay;
    public Transform buildingsParent;

    public GameObject builderPrefab;

    public GameObject foresterButton;
    public GameObject guardTowerButton;

    private void Awake()
    {
        GameManager.instance.init += Init;
    }

    private void Init()
    {
        GameManager.instance.ui = this;

        GameManager.instance.update += UpdateC;

        woodAmountText.text = 0.ToString();
        buildOverlay.SetActive(false);

        health.Enqueue(GameManager.instance.player.Health);

        debugParent.SetActive(GameManager.instance.debug);
    }

    private void UpdateC()
    {
        woodAmountText.text = GameManager.instance.player.Wood.ToString();
        healthAmount.value = GameManager.instance.player.Health;

        health.Enqueue(GameManager.instance.player.Health);
        while (health.Count > 20)
            health.Dequeue();
        float healthAverage = 0.0f;
        foreach (float f in health)
            healthAverage += f;
        healthAverage /= health.Count;

        float diff = GameManager.instance.player.Health - healthAverage;

        if (Mathf.RoundToInt(GameManager.instance.player.Health) == 100)
            healthDeltaText.text = "";
        else if (diff >= 0.01f)
            healthDeltaText.text = "▲";
        else if (diff <= -0.01f)
            healthDeltaText.text = "▼";
        else
            healthDeltaText.text = "";

        if (GameManager.instance.player.Health <= 10.0f)
        {
            whiteOut.gameObject.SetActive(true);
            whiteOut.color = new Color(1.0f, 1.0f, 1.0f, 1.0f - (GameManager.instance.player.Health / 10.0f));
        }
        else
        {
            whiteOut.gameObject.SetActive(false);
        }

        if (GameManager.instance.debug)
        {
            timeText.text = GameManager.instance.GameTime.ToString("0.00");

            fps.Enqueue(1.0f / Time.unscaledDeltaTime);

            while (fps.Count > 100)
                fps.Dequeue();

            float buf = 0.0f;

            foreach (float f in fps)
                buf += f;

            performanceText.text = (buf / fps.Count).ToString("0.00") + " FPS";
        }
    }

    // Shows or Hides Build Overlay
    public void BuildOverlay(bool show)
    {
        buildOverlay.SetActive(show);
        buildButton.SetActive(!show);

        if (show)
            GameManager.instance.inputHandler.SetInputMode(InputMode.UI);
        else
            GameManager.instance.inputHandler.SetInputMode(InputMode.Movement);
    }

    public void Build(GameObject building)
    {
        Builder builder = Instantiate(builderPrefab, GameManager.instance.player.transform.position, Quaternion.identity, buildingsParent).GetComponent<Builder>();
        builder.SetUp(building);

        buildOverlay.SetActive(false);
        GameManager.instance.inputHandler.SetInputMode(InputMode.Construction, builder);
    }

    public void UnlockBuildings()
    {
        foresterButton.SetActive(true);
        guardTowerButton.SetActive(true);
    }
}
