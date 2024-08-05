using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{

    private PlayerInput input;

    public static InputManager instance { get; private set; }

    public static Vector2 MoveInput;
    public static Vector2 LookInput;

    public UIManager uiManager;


    public void SwitchToGameplayMap()
    {
        Debug.Log("Switching to Gameplay Map");
        input.SwitchCurrentActionMap("Gameplay");
    }


    public void SwitchToOverview()
    {
        Debug.Log("Switching to Overview Map");
        input.SwitchCurrentActionMap("Overview");
    }



    void Start()
    {
        input = GetComponent<PlayerInput>();
    }


    // Gameplay Map

    public void Move(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            MoveInput = context.ReadValue<Vector2>();
        }

        else
        {
            MoveInput = Vector2.zero;
        }
    }

    public void Look(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            LookInput = context.ReadValue<Vector2>();
        }
    }

    public void OpenOverview(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (uiManager.CanSwitchToOverview())
            {
                uiManager.SwitchToOverviewMap();
                SwitchToOverview();
            }
            
        }
    }





    // Select Tile Map
    public void MoveUp(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Utils.Log("Up Pressed");
            TileManager.instance.MoveSelection(TileManager.MoveDirection.UP);
        }
    }

    public void MoveDown(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Utils.Log("Down Pressed");
            TileManager.instance.MoveSelection(TileManager.MoveDirection.DOWN);
        }
    }

    public void MoveLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Utils.Log("Left Pressed");
            TileManager.instance.MoveSelection(TileManager.MoveDirection.LEFT);
        }
    }

    public void MoveRight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Utils.Log("Right Pressed");
            TileManager.instance.MoveSelection(TileManager.MoveDirection.RIGHT);
        }
    }

    public void SwitchToGameplay(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            uiManager.SwitchToGameplayMap();
            if(TileManager.instance.GetSelectedTile().isActive) SwitchToGameplayMap();
        }
    }
}
