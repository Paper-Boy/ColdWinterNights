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
    public UnityAction fixedUpdate;

    private void Update()
    {
        if (Running)
        {
            GameTime += Time.deltaTime;

            if (update != null && GameTime >= 0.2f)
                update.Invoke();
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

    public void Death()
    {
        Running = false;

        ui.Death();
    }

    // Options
    public bool debug;
    public bool touchControls;
    public new bool light;

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
}

internal enum Layers
{
    Navigation = 8,
    Tree = 9,
    Building = 10
}