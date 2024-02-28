using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Checkpoint : MonoBehaviour, IDataObject, IGUIDObject
{
    static Checkpoint currentCheckpoint;
    [SerializeField] string guid = System.Guid.NewGuid().ToString();
    [SerializeField] Transform player1Pos;
    [SerializeField] Transform player2Pos;
    [SerializeField] UnityEvent OnCheckpointActivated;
    public static UnityEvent OnReloadFromCheckpoint;
    bool? collected;
    bool loaded = false;
    
    public void RegenerateGUID()
    {
        guid = System.Guid.NewGuid().ToString();
    }

    void Awake()
    {
        if (SaveDataHandler.saveData == null || collected != null) return;
        Load(SaveDataHandler.saveData);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (collected == true) return;
        if (!loaded) return;
        if (other.gameObject.layer != 6) return;
        if (SaveDataHandler.saveData == null) return;
        SaveDataHandler.saveData.SetTransform(SaveDataHandler.saveData.p1guid, new SerialTransform(player1Pos, true));
        SaveDataHandler.saveData.SetTransform(SaveDataHandler.saveData.p2guid, new SerialTransform(player2Pos, true));
        collected = true;
        OnCheckpointActivated.Invoke();
        SaveDataHandler.saveData.SetBool(guid, collected);
        SaveDataHandler.Save(SaveDataHandler.currentFilename, true);
    }


    public void Save(SaveData saveData)
    {
        //if (!collected) return;
        //saveData.SetBool(guid, collected);
        //saveData.SetTransform("checkpoint", new SerialTransform(transform));
    }

    public void Load(SaveData saveData, bool reset = false)
    {
        collected = saveData.GetBool(guid);
        if (collected == true) OnCheckpointActivated.Invoke();
        loaded = true;
    }

    public static void LoadCheckpoint()
    {
        OnReloadFromCheckpoint.Invoke();
    }
    public Object GetObject()
    {
        return this;
    }
}
