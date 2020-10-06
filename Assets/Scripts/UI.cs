using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    // UI Elements
    public TMP_Text woodAmountText;
    public Slider healthAmount;
    public Slider thermometer;
    public TMP_Text thermomenterDeltaText;

    public Image whiteOut;
    public GameObject deathPanel;
    public TMP_Text scoreText;

    public GameObject debugParent;
    public TMP_Text timeText;
    public TMP_Text performanceText;

    public GameObject helpOverlay;
    public GameObject buildOverlay;

    public Transform buildingsParent;
    public GameObject builderPrefab;

    public GameObject buildButton;
    public GameObject foresterButton;
    public GameObject guardTowerButton;

    private Player player;

    // Average Values
    private readonly Queue<float> fps = new Queue<float>();
    private readonly Queue<float> health = new Queue<float>();
    private readonly Queue<float> temp = new Queue<float>();

    private void Awake()
    {
        GameManager.instance.init += Init;
        GameManager.instance.lateInit = LateInit;
    }

    private void Init()
    {
        GameManager.instance.ui = this;

        GameManager.instance.lateUpdate += LateUpdateC;

        woodAmountText.text = 0.ToString();
        buildOverlay.SetActive(false);

        player = GameManager.instance.player;

        health.Enqueue(player.Health);
    }

    private void OnEnable()
    {
        StartCoroutine(UpdateHealthAndTemp());
    }

    private void LateInit()
    {
        if (GameManager.instance.debug)
        {
            debugParent.SetActive(true);
            StartCoroutine(DebugUI());
        }
        else
        {
            debugParent.SetActive(false);
        }
    }

    private void LateUpdateC()
    {
        // Wood
        woodAmountText.text = player.Wood.ToString();

        // White Out
        if (player.Health <= 25.0f)
        {
            whiteOut.gameObject.SetActive(true);
            whiteOut.color = new Color(1.0f, 1.0f, 1.0f, 1.0f - (player.Health / 25.0f));
        }
        else
        {
            whiteOut.gameObject.SetActive(false);
        }
    }

    private IEnumerator UpdateHealthAndTemp()
    {
        for (; ; )
        {
            if (GameManager.instance.Running)
            {
                // Health
                health.Enqueue(player.Health);
                while (health.Count > 20)
                    health.Dequeue();
                float healthAverage = 0.0f;
                foreach (float f in health)
                    healthAverage += f;
                healthAverage /= health.Count;

                healthAmount.value = Mathf.CeilToInt(healthAverage);

                // White Out
                if (player.Health <= 25.0f)
                {
                    whiteOut.gameObject.SetActive(true);
                    whiteOut.color = new Color(1.0f, 1.0f, 1.0f, 1.0f - (player.Health / 25.0f));
                }
                else
                {
                    whiteOut.gameObject.SetActive(false);
                }

                // Temperatur
                temp.Enqueue(player.Temperature);
                while (temp.Count > 20)
                    temp.Dequeue();
                float tempAverage = 0.0f;
                foreach (float f in temp)
                    tempAverage += f;
                tempAverage /= temp.Count;

                thermometer.value = tempAverage;

                float diff = player.Temperature - tempAverage;

                if (diff >= 0.001f)
                {
                    thermomenterDeltaText.text = "▲";
                }
                else if (diff <= -0.001f)
                {
                    thermomenterDeltaText.text = "▼";
                }
                else
                {
                    thermomenterDeltaText.text = "";
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator DebugUI()
    {
        int count = 0;

        for (; ; )
        {
            float curFPS = 1.0f / Time.unscaledDeltaTime;

            fps.Enqueue(curFPS);

            while (fps.Count > 50)
                fps.Dequeue();

            float buf = 0.0f;

            foreach (float f in fps)
                buf += f;

            count++;

            if (count >= curFPS * 0.05f)
            {
                timeText.text = GameManager.instance.GameTime.ToString("0.00");
                performanceText.text = (buf / fps.Count).ToString("0.00") + " FPS";
                count = 0;
            }

            yield return null;
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
        Builder builder = Instantiate(builderPrefab, player.transform.position, Quaternion.identity, buildingsParent).GetComponent<Builder>();
        builder.SetUp(building);

        buildOverlay.SetActive(false);
        GameManager.instance.inputHandler.SetInputMode(InputMode.Construction, builder);
    }

    public void UnlockBuildings()
    {
        foresterButton.SetActive(true);
        guardTowerButton.SetActive(true);
    }

    public void Death()
    {
        deathPanel.SetActive(true);
        TimeSpan time = TimeSpan.FromSeconds(GameManager.instance.GameTime);
        StartCoroutine(ScoreText("You survived for " + time.ToString("mm':'ss") + " minutes!"));
    }

    private IEnumerator ScoreText(string text)
    {
        string bufText = "";

        yield return new WaitForSeconds(4.5f);

        for (int i = 0; i < text.Length; i++)
        {
            scoreText.text = bufText + "_";

            yield return new WaitForSeconds(0.1f);

            bufText += text[i];
        }

        scoreText.text = bufText;
    }

    public void HelpOverlay(bool show)
    {
        helpOverlay.SetActive(show);
    }
}
