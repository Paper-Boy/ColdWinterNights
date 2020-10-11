using System.Globalization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Singleton

    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    #endregion

    #region Initialization

    public Initializer initializer;

    public UnityAction earlyInit;
    public UnityAction init;
    public UnityAction lateInit;

    private void Start()
    {
        initializer.InitializeGame(earlyInit, init, lateInit);
    }

    public void StartGame()
    {
        Running = true;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    #endregion

    #region Updates and GameTime

    public bool Running { get; private set; }
    public float GameTime { get; private set; } = 0.0f;

    public UnityAction update;
    public UnityAction lateUpdate;
    public UnityAction fixedUpdate;

    private void Update()
    {
        if (Running)
        {
            GameTime += Time.deltaTime;

            if (update != null && GameTime >= 0.2f)
                update.Invoke();

            if (lateUpdate != null)
                lateUpdate.Invoke();
        }
    }

    private void FixedUpdate()
    {
        if (Running)
        {
            if (fixedUpdate != null)
                fixedUpdate.Invoke();
        }
    }

    #endregion

    public void Pause(bool pause)
    {
        Running = !pause;

        if (pause)
            Time.timeScale = 0.0f;
        else
            Time.timeScale = 1.0f;
    }

    public void ChangeTimeScale(float multiplier)
    {
        Time.timeScale = Mathf.Clamp(Time.timeScale * multiplier, 0.5f, 2.0f);
    }

    public void Death()
    {
        Running = false;
        Time.timeScale = 1.0f;

        ui.Death();
    }

    // Options
    public bool debug = false;
    public bool touchControls = false;

    public CultureInfo culture = CultureInfo.InvariantCulture;

    public Transform footstepsParent;
    public Transform treesParent;

    [HideInInspector]
    public UI ui;
    [HideInInspector]
    public Player player;
    [HideInInspector]
    public InputHandler inputHandler;
    [HideInInspector]
    public MapGenerator mapGenerator;
    [HideInInspector]
    public TemperatureMap temperatureMap;
}

internal enum Layers
{
    Navigation = 8,
    Tree = 9,
    Building = 10,
    Builder = 11
}