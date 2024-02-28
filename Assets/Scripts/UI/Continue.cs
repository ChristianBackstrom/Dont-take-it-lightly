using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Continue : MainMenuItem
{
    [SerializeField] Button button;
    SerialDictionary<string, Texture2D> saveFiles = new SerialDictionary<string, Texture2D>();
    [SerializeField] RawImage rawImage;
    [SerializeField] TextMeshProUGUI textMesh;
    [SerializeField] GameObject fadeoutPrefab;
    [SerializeField] Canvas canvas;
    bool set = false;
    string saveName = SaveDataHandler.currentFilename;


    public override void Awake()
    {
        base.Awake();
        button.onClick.AddListener(LoadThis);
    }
    void Start()
    {
        SaveDataHandler.UpdateSaveInfo();
        saveFiles = SaveDataHandler.saveList;
        if (!saveFiles.ContainsKey("save")) return;

        rawImage.texture = saveFiles["save"];
        textMesh.gameObject.SetActive(false);
        set = true;

    }

    async void LoadThis()
    {
        if (!set) return;
        SaveDataHandler.Load(saveName);
        GameObject fo = Instantiate(fadeoutPrefab, canvas.transform);
        LoadingScreen ls = fo.GetComponent<LoadingScreen>();
        await ls.FadeOut();
        Pool.ResetPools();
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(1);
        ls.UpdateLoadingScreen("Please Wait");
    }
}
