using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public event Action<InputHandler> OnSetup;

    public InputDevice Device;

    private Controls controls;
    private InputAction movement;
    private InputAction interact;
    private InputAction pause;

    public Controls Controls
    {
        set
        {
            controls = value;
            SetInputActions();
        }
        get
        {
            return controls;
        }
    }

    public InputAction Movement
    {
        get
        {
            return movement;
        }
    }

    public InputAction Interact
    {
        get
        {
            return interact;
        }
    }

    public InputAction Pause
    {
        get
        {
            return pause;
        }
    }

    private void SetInputActions()
    {
        movement = controls.Player.Move;
        movement.Enable();

        interact = controls.Player.Interact;
        interact.Enable();

        pause = controls.Player.Pause;
        pause.Enable();

        OnSetup?.Invoke(this);
    }

    private void OnDisable()
    {
        movement.Disable();
        interact.Disable();
        pause.Disable();
    }
}
