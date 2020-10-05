using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Initializer : MonoBehaviour
{
    public GameObject ui;
    public GameObject startPanel;

    public GameObject titelUi;
    public GameObject loadingUi;
    public Slider loadingSlider;
    public TMP_Text loadingText;
    public GameObject startButton;

    private UnityAction init, lateInit;

    public void InitializeGame(UnityAction earlyInit, UnityAction init, UnityAction lateInit)
    {
        Application.targetFrameRate = 9999;

        titelUi.SetActive(true);
        loadingUi.SetActive(true);
        ui.SetActive(false);

        this.init = init;
        this.lateInit = lateInit;

        earlyInit.Invoke();
    }

    public void LoadingProgress(float value)
    {
        loadingSlider.value = value;
        loadingText.text = value.ToString("P");
    }

    public void LoadingProgress()
    {
        GameManager.instance.temperatureMap = new TemperatureMap(-2.0f, 1.0f);

        loadingSlider.value = 1;
        loadingText.text = "Finished!";

        Application.targetFrameRate = -1;

        startButton.SetActive(true);
    }

    public void StartGame()
    {
        GameManager gameManager = GameManager.instance;

        init.Invoke();

        if (gameManager.ui == null || gameManager.player == null || gameManager.inputHandler == null)
            Debug.LogError("Player or UI or Input Handler not initialized!");

        titelUi.SetActive(false);
        loadingUi.SetActive(false);
        ui.SetActive(true);
        startPanel.SetActive(true);

        lateInit.Invoke();

        gameManager.StartGame();
    }
}
