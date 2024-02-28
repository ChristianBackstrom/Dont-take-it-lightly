using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SavePanel : MonoBehaviour
{
    [SerializeField] RawImage rawImage;
    [SerializeField] TextMeshProUGUI textMesh;
    string saveName;
    bool set = false;
    public void Set(string name, Texture2D image)
    {
        rawImage.texture = image;
        textMesh.SetText(name);
        saveName = name;
        set = true;
    }

    public void LoadThis()
    {
        if (!set) return;
        SaveDataHandler.Load(saveName);
        SceneManager.LoadSceneAsync(1);
    }
}
