using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputHandler : MonoBehaviour
{
    public EventSystem eventSystem;

    private InputMode inputMode = InputMode.Movement;
    private Builder builder = null;
    private new Camera camera;

    private GameManager gameManager;

    #region Initialization

    private void Awake()
    {
        gameManager = GameManager.instance;
        gameManager.init += Init;
    }

    private void Init()
    {
        gameManager.update += UpdateC;
        gameManager.inputHandler = this;

        camera = Camera.main;
    }

    #endregion

    private float lastTouch = -999f;
    private Vector2 lastTouchPos;

    private void UpdateC()
    {
        // Handle inputs for Player movement
        if (inputMode == InputMode.Movement)
        {
            if (Input.touchCount == 1 || Input.GetMouseButton(0))
            {
                if (Input.touchCount == 1)
                    lastTouchPos = camera.ScreenToWorldPoint(Input.GetTouch(0).position);
                else if (Input.GetMouseButton(0))
                    lastTouchPos = camera.ScreenToWorldPoint(Input.mousePosition);
                else
                    return;

                if (lastTouch <= -999f && !IsPointerOverUIObject())
                {
                    lastTouch = Time.unscaledTime;
                }
                else if (Time.unscaledTime - lastTouch >= 0.25f && !IsPointerOverUIObject())
                {
                    gameManager.player.SetRotation(lastTouchPos);
                }
                else if (IsPointerOverUIObject())
                {
                    lastTouch = -999f;
                }
            }
            else if (lastTouch > -999f && !IsPointerOverUIObject())
            {
                if (Time.unscaledTime - lastTouch < 0.25f)
                {
                    Raycast(lastTouchPos);
                }

                lastTouch = -999f;
            }
        }
        // Handle inputs to move construction site
        else if (inputMode == InputMode.Construction)
        {
            // Touch Controls
            if (gameManager.touchControls)
            {
                if (Input.touchCount == 1)
                {
                    if (lastTouch > -999f)
                    {
                        if (Time.unscaledTime - lastTouch < 0.25f)
                        {
                            if (builder != null)
                                builder.Move(camera.ScreenToWorldPoint(Input.GetTouch(0).position));
                        }
                        else
                        {
                            lastTouch = Time.unscaledTime;
                        }
                    }
                    else
                    {
                        lastTouch = Time.unscaledTime;
                    }
                }
                else
                {
                    lastTouch = -999f;
                }
            }
            // Mouse Controls
            else if (Input.GetMouseButtonDown(0))
            {
                // Get position of mouse click
                Vector3 mousePos = camera.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

                if (builder != null)
                    builder.Move(mousePos2D);
            }
        }
    }

    private void Raycast(Vector2 position)
    {
        // Create a List of colliders "under" mouse position
        List<RaycastHit2D> hits = new List<RaycastHit2D>(Physics2D.RaycastAll(position, Vector2.zero));

        // Check if nothing (including UI) is hit -> Move towards this point
        if (hits.Count == 0 && !IsPointerOverUIObject())
        {
            gameManager.player.SetTarget(position);
        }
        // If something is hit check if it is a Building, Builder or Tree -> Interact
        else if (!IsPointerOverUIObject())
        {
            if (hits[0].collider.gameObject.layer >= (int)Layers.Tree && hits[0].collider.gameObject.layer <= (int)Layers.Builder)
                gameManager.player.SetTarget(hits[0].collider.gameObject);
        }
    }
    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    public void SetInputMode(InputMode inputMode)
    {
        this.inputMode = inputMode;
        builder = null;
    }

    public void SetInputMode(InputMode inputMode, Builder builder)
    {
        this.inputMode = inputMode;
        this.builder = builder;
    }
}

public enum InputMode
{
    Movement,
    UI,
    Construction
}
