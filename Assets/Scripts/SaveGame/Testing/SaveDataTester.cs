using System;
using System.Collections.Generic;
using UnityEngine;

public class SaveDataTester : MonoBehaviour, IDataObject, IGUIDObject
{
    [SerializeField] string filename;
    [SerializeField] int test;
    [SerializeField] string guid = System.Guid.NewGuid().ToString();
    [SerializeField] Texture2D saveImage;
    [SerializeField] SerialDictionary<string, Texture2D> saveFiles = new SerialDictionary<string, Texture2D>();
    [ContextMenu("Save")] public void CSave()
    {
        SaveDataHandler.Save(filename);
    }

    [ContextMenu("Simulate Autosave")] public void CAuto()
    {
        SaveDataHandler.Save(filename, true);
    }

    [ContextMenu("Load")] public void CLoad()
    {
        SaveDataHandler.Load(filename);
    }

    [ContextMenu("Reset")] public void CReset()
    {
        SaveDataHandler.Reset();
    }

    [ContextMenu("update save info")] public void update()
    {
        SaveDataHandler.UpdateSaveInfo();
        saveFiles = SaveDataHandler.saveList;
    }

    public void RegenerateGUID()
    {
        guid = System.Guid.NewGuid().ToString();
    }

    public UnityEngine.Object GetObject()
    {
        return this;
    }

    public void Awake()
    {
        if (SaveDataHandler.saveData == null) SaveDataHandler.NewGame(filename);
        else 
        {
            filename = SaveDataHandler.currentFilename;
            SaveDataHandler.Save(SaveDataHandler.currentFilename);
        }
    }


    public void Save(SaveData saveData)
    {
        saveData.SetTransform(guid, new SerialTransform(transform));
    }

    public void Load(SaveData saveData, bool reset = false)
    {
        
    }


    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) CSave();
        if (Input.GetKeyDown(KeyCode.F2)) CLoad();
        if (Input.GetKeyDown(KeyCode.F3)) CReset();
        if (Input.GetKeyDown(KeyCode.F4)) CAuto();
    }
}
