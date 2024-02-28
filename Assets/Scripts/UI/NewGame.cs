using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class NewGame : MainMenuItem
{

    [SerializeField] Button button;
    [SerializeField] GameObject fadeoutPrefab;
    [SerializeField] Canvas canvas;
    bool textEntered;
    string saveName = SaveDataHandler.currentFilename;

    public override void Awake()
    {
        base.Awake();
        button.onClick.AddListener(NameEntered);
    }


    async void ConfirmNewGame()
    {
        if (!textEntered) return;
        SaveDataHandler.NewGame(saveName);
        
        GameObject fo = Instantiate(fadeoutPrefab, canvas.transform);
        LoadingScreen ls = fo.GetComponent<LoadingScreen>();
        await ls.FadeOut();
        Pool.ResetPools();
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(1);
        ls.UpdateLoadingScreen("Please Wait");
        
    }

    void NameEntered()
    {
        //if (sn == null || sn == "") return;
        textEntered = true;

        ConfirmNewGame();
    }
}
