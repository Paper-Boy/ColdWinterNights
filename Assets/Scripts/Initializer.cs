using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Initializer : MonoBehaviour
{
    public GameObject ui;
    public GameObject startPanel;

    public GameObject loadingUi;
    public Slider loadingSlider;
    public TMP_Text loadingText;
    public GameObject startButton;

    private UnityAction init, lateInit;

    public void InitializeGame(UnityAction earlyInit, UnityAction init, UnityAction lateInit)
    {
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
        loadingSlider.value = 1;
        loadingText.text = "Finished!";

        startButton.SetActive(true);
    }

    public void StartGame()
    {
        GameManager gameManager = GameManager.instance;

        init.Invoke();

        if (gameManager.ui == null || gameManager.player == null || gameManager.inputHandler == null)
            Debug.LogError("Player or UI or Input Handler not initialized!");

        lateInit.Invoke();

        loadingUi.SetActive(false);
        ui.SetActive(true);
        startPanel.SetActive(true);

        gameManager.StartGame();
    }
}
