using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class ControlPage : SettingsPage
{
    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;
    private Controls[] controls = new Controls[2];
    [SerializeField] Color rebindingColor, normalColor;
    [SerializeField] TextMeshProUGUI p1TypeText, p2TypeText;
    [SerializeField] TextMeshProUGUI[] text0, text1;
    TextMeshProUGUI[][] text = new TextMeshProUGUI[2][];
    private TextMeshProUGUI currentRebind;
    private int p1ControlType = 1;
    private int p2ControlType = 0;
    private string[] controltype = {"stick", "WASD"};
    private string[] controlDisplayName = {"Controller", "Keyboard"};

    private bool rebindInProgress = false;
    [SerializeField] Image blocker;
    
    public void OnEnable()
    {
        text[0] = text0;
        text[1] = text1;
        for (int i = 0; i < controls.Length; i++)
        {
            controls[i] = new Controls();
            string rebinds = PlayerPrefs.GetString("rebinds" + i);
            if (!string.IsNullOrEmpty(rebinds))
            {
                controls[i].LoadBindingOverridesFromJson(rebinds);
            }
            UpdateText(text[i][0], controls[i].Player.Interact, i);
            UpdateCompositeText(text[i][1], controls[i].Player.Move, "up", i);
            UpdateCompositeText(text[i][2], controls[i].Player.Move, "down", i);
            UpdateCompositeText(text[i][3], controls[i].Player.Move, "left", i);
            UpdateCompositeText(text[i][4], controls[i].Player.Move, "right", i);
        }
        UpdateControlTypeText();
    }
    public void UpdateText(TextMeshProUGUI text, InputAction action, int player = 0)
    {
        int type = 0;
        if (player == 0)
        {
            type = p1ControlType;
        }
        else if (player == 1)
        {
            type = p2ControlType;
        }
        string output = action.GetBindingDisplayString();
        string[] output2 = output.Split("|");
        text.SetText(output2[type]);
    }
    public void UpdateCompositeText(TextMeshProUGUI text, InputAction action, string direction, int player)
    {
        int type = 0;
        if (player == 0)
        {
            type = p1ControlType;
        }
        else if (player == 1)
        {
            type = p2ControlType;
        }
        InputActionSetupExtensions.BindingSyntax composite = action.ChangeCompositeBinding(controltype[type]);
        InputActionSetupExtensions.BindingSyntax accessor = composite.NextPartBinding(direction);
        string output = accessor.binding.ToDisplayString();
        string[] output2 = output.Split("|");
        text.SetText(output);
    }
    void Rebind(InputAction action, int player)
    {
        rebindInProgress = true;
        blocker.gameObject.SetActive(true);

        currentRebind.color = rebindingColor;
        action.Disable();
        rebindingOperation = action.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse")
            .WithCancelingThrough("<Keyboard>/escape")
            .WithControlsExcluding("*/{Menu}")
            .WithCancelingThrough("<XInputController>/start")
            .OnMatchWaitForAnother(0.1f)
            .OnCancel(operation => RebindCancel(action, player))
            .OnComplete(operation => RebindComplete(action, player));
        rebindingOperation.Start();
    }

    void CompositeRebind(InputAction action, string direction,  int player)
    {
        rebindInProgress = true;
        blocker.gameObject.SetActive(true);
        string binding = "stick";
        if (player == 0)
        {
            binding = controltype[p1ControlType];
        }
        else if (player == 1)
        {
            binding = controltype[p2ControlType];
        }

        InputActionSetupExtensions.BindingSyntax composite = action.ChangeCompositeBinding(binding);
        InputActionSetupExtensions.BindingSyntax accessor = composite.NextPartBinding(direction);


        currentRebind.color = rebindingColor;
        action.Disable();
        rebindingOperation = action.PerformInteractiveRebinding()
            .WithTargetBinding(accessor.bindingIndex)
            .WithControlsExcluding("Mouse")
            .WithControlsExcluding("<Gamepad>/*/x")
            .WithControlsExcluding("<Gamepad>/*/y")
            .WithCancelingThrough("<Keyboard>/escape")
            .WithControlsExcluding("*/{Menu}")
            .WithCancelingThrough("<XInputController>/start")
            .OnMatchWaitForAnother(0.1f)
            .OnCancel(operation => RebindCancel(action, player))
            .OnComplete(operation => RebindComplete(action, player));
        rebindingOperation.Start();
    }

    private void UpdateControlTypeText()
    {
        p1TypeText.SetText(controlDisplayName[p1ControlType]);
        p2TypeText.SetText(controlDisplayName[p2ControlType]);
    }

    


    public void RebindInteract(int player)
    {
        if (rebindInProgress) return;
        currentRebind = text[player][0];
        Rebind(controls[player].Player.Interact, player);
    }

    public void RebindFoward(int player)
    {
        if (rebindInProgress) return;
        currentRebind = text[player][1];
        CompositeRebind(controls[player].Player.Move, "Up", player);
    }

    public void RebindBack(int player)
    {
        if (rebindInProgress) return;
        currentRebind = text[player][2];
        CompositeRebind(controls[player].Player.Move, "Down", player);
    }

    public void RebindLeft(int player)
    {
        if (rebindInProgress) return;
        currentRebind = text[player][3];
        CompositeRebind(controls[player].Player.Move, "Left", player);
    }

    public void RebindRight(int player)
    {
        if (rebindInProgress) return;
        currentRebind = text[player][4];
        CompositeRebind(controls[player].Player.Move, "Right", player);
    }

    public void RebindMove(int player)
    {
        if (rebindInProgress) return;
        currentRebind = text[player][5];
        Rebind(controls[player].Player.Move, player);
    }

    public void ChangeControlType(int player)
    {
        if (rebindInProgress) return;
        if (player == 0)
        {
            if (p1ControlType == 1)
            {
                p1ControlType = 0;
            }
            else
            {
                p1ControlType = 1;
            }
        }
        if (player == 1)
        {
            if (p2ControlType == 1)
            {
                p2ControlType = 0;
            }
            else
            {
                p2ControlType = 1;
            }
        }

        UpdateText(text[player][0], controls[0].Player.Interact, player);
        UpdateCompositeText(text[player][1], controls[player].Player.Move, "up", player);
        UpdateCompositeText(text[player][2], controls[player].Player.Move, "down", player);
        UpdateCompositeText(text[player][3], controls[player].Player.Move, "left", player);
        UpdateCompositeText(text[player][4], controls[player].Player.Move, "right", player);
        UpdateControlTypeText();
    }


    void RebindComplete(InputAction action, int player)
    {
        rebindingOperation.Dispose();
        action.Enable();
        UpdateText(currentRebind, action);
        currentRebind.color = normalColor;
        currentRebind = null;

        string rebinds = controls[player].SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("rebinds" + player, rebinds);
        

        CallRebindObjects(player);
        OnEnable();
        rebindInProgress = false;    
        blocker.gameObject.SetActive(false);
    }

    void RebindCancel(InputAction action, int player)
    {
        rebindingOperation.Dispose();
        action.Enable();
        currentRebind.color = normalColor;
        currentRebind = null;
        OnEnable();
        rebindInProgress = false;  
        blocker.gameObject.SetActive(false);
    }

    void CallRebindObjects(int player)
    {
        List<IRebindOperation> rebindRecievers = FindRebindRecievers();
        foreach (IRebindOperation obj in rebindRecievers)
        {
            obj.UpdateControlRebinds(player);
        }
    }


    private static List<IRebindOperation> FindRebindRecievers()
    {
        IEnumerable<IRebindOperation> output = UnityEngine.Object.FindObjectsOfType<MonoBehaviour>(true).OfType<IRebindOperation>();
        return new List<IRebindOperation>(output);
    }


    public void ResetPressed()
    {
        for (int i = 0; i < controls.Length; i++)
        {
            controls[i].Player.Disable();
            PlayerPrefs.SetString("rebinds" + i, null);
            controls[i].RemoveAllBindingOverrides();
            controls[i].Player.Enable();
            UpdateText(text[i][0], controls[0].Player.Interact, i);
            UpdateCompositeText(text[i][1], controls[i].Player.Move, "up", i);
            UpdateCompositeText(text[i][2], controls[i].Player.Move, "down", i);
            UpdateCompositeText(text[i][3], controls[i].Player.Move, "left", i);
            UpdateCompositeText(text[i][4], controls[i].Player.Move, "right", i);
        }
        UpdateControlTypeText();
    }


    
}
