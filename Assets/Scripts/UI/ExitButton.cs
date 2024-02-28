using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitButton : MainMenuItem
{
    [SerializeField] Button button;


    public override void Awake()
    {
        base.Awake();
        button.onClick.AddListener(Exit);
    }
    void Exit()
    {
        Application.Quit();
    }
}
