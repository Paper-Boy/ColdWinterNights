using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private InputMode inputMode = InputMode.Movement;
    private Builder builder = null;
    private new Camera camera;

    #region Initialization

    private void Awake()
    {
        GameManager.instance.init += Init;
    }

    private void Init()
    {
        GameManager.instance.update += UpdateC;
        GameManager.instance.inputHandler = this;

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

                if (lastTouch <= -999f)
                {
                    lastTouch = GameManager.instance.GameTime;
                }
                else if (GameManager.instance.GameTime - lastTouch >= 0.25f)
                {
                    GameManager.instance.player.SetRotation(lastTouchPos);
                }
            }
            else if (lastTouch > -999f)
            {
                if (GameManager.instance.GameTime - lastTouch < 0.25f)
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
            if (GameManager.instance.touchControls)
            {
                if (Input.touchCount == 1)
                {
                    if (lastTouch > -999f)
                    {
                        if (GameManager.instance.GameTime - lastTouch < 0.25f)
                        {
                            if (builder != null)
                                builder.Move(camera.ScreenToWorldPoint(Input.GetTouch(0).position));
                        }
                        else
                        {
                            lastTouch = GameManager.instance.GameTime;
                        }
                    }
                    else
                    {
                        lastTouch = GameManager.instance.GameTime;
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

        // Sort List by layer (get "Navigation" to front)
        hits.Sort((x, y) => x.collider.gameObject.layer.CompareTo(y.collider.gameObject.layer));

        if (hits.Count > 0)
        {
            bool nav = false;

            foreach (RaycastHit2D hit in hits)
            {
                // Navigation Mesh is hit
                if (!nav && hit.collider.gameObject.layer == (int)Layers.Navigation)
                {
                    nav = true;
                }
                // Navigation and an Object is hit: Move to Object
                else if (nav)
                {
                    switch (hit.collider.gameObject.layer)
                    {
                        case (int)Layers.Tree:
                            GameManager.instance.player.SetTarget(hit.collider.gameObject);
                            break;
                        case (int)Layers.Building:
                            GameManager.instance.player.SetTarget(hit.collider.gameObject);
                            break;
                        default:
                            break;
                    }

                    nav = false;
                    break;
                }
                // Navigation is not hit
                else
                {
                    nav = false;
                    break;
                }
            }

            // If only Navigation was hit: Move to click position
            if (nav == true)
                GameManager.instance.player.SetTarget(position);
        }
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
