using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Sequentially initializes all (previously registered) scripts and objects
/// Updates the loading progress bar and start button
/// </summary>
public class Initializer : MonoBehaviour
{
    public GameObject ui;
    public GameObject startPanel;

    public GameObject titelUi;
    public GameObject loadingUi;
    public Slider loadingSlider;
    public TMP_Text loadingText;
    public GameObject startButton;
    public TMP_Text versionText;

    private UnityAction init, lateInit;

    public void InitializeGame(UnityAction earlyInit, UnityAction init, UnityAction lateInit)
    {
        Application.targetFrameRate = 9999;
        versionText.text = "v. " + Application.version;

        titelUi.SetActive(true);
        loadingUi.SetActive(true);
        ui.SetActive(false);

        this.init = init;
        this.lateInit = lateInit;

        earlyInit.Invoke();
    }

	// Updates loading progress bar
    public void LoadingProgress(float value)
    {
        loadingSlider.value = value;
        loadingText.text = value.ToString("P", GameManager.instance.culture);
    }

	// "Finishes" loading progress bar
    public void LoadingProgress()
    {
        GameManager.instance.temperatureMap = new TemperatureMap(-1.5f, 1.0f);

        loadingSlider.value = 1;
        loadingText.text = 1.ToString("P", GameManager.instance.culture);

        Application.targetFrameRate = -1;

        versionText.gameObject.SetActive(true);
        startButton.SetActive(true);
    }

	// Finalizes initialization and starts the game
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
