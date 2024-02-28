using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveObjectPosition : MonoBehaviour, IDataObject, IGUIDObject
{
    [SerializeField] string guid;
    [SerializeField] SerialTransform initialPosition;
    Movement movement;
    public enum ObjectType{
        generic,
        player1,
        player2
    }

    [SerializeField] public ObjectType objectType = ObjectType.generic;
    
    [ContextMenu("Regenerate GUID")]public void RegenerateGUID()
    {
        guid = System.Guid.NewGuid().ToString();
    }

    public Object GetObject()
    {
        return this;
    }

    public void OnValidate()
    {
        initialPosition.position = transform.position;
        initialPosition.rotation = transform.rotation;
    }

    void Start()
    {

        if (SaveDataHandler.saveData == null) return;
        Load(SaveDataHandler.saveData);
    }
    
    public void Save(SaveData saveData)
    {
        switch(objectType)
        {
            case ObjectType.player1:
            {
                saveData.p1guid = guid;
                break;
            } 
            case ObjectType.player2:
            {
                saveData.p2guid = guid;
                break;
            }
        }

        if (objectType != ObjectType.generic && saveData.autosave) return;

        saveData.SetTransform(guid, new SerialTransform(transform));
    }

    public void Load(SaveData saveData, bool reset = false)
    {
        switch(objectType)
        {
            case ObjectType.player1:
            {
                saveData.p1guid = guid;
                break;
            } 
            case ObjectType.player2:
            {
                saveData.p2guid = guid;
                break;
            }
        }

        if (objectType != ObjectType.generic)
        {
            GetComponent<Movement>().Reset();
        }
        SetToInitialPosition();
        if ((reset && objectType == ObjectType.generic))
        {
            return;
        }
        SerialTransform st = saveData.GetTransform(guid);
        if (st == null) return;
        
        transform.position = st.position;
        transform.rotation = st.rotation;
        
    }

    void SetToInitialPosition()
    {
        transform.position = initialPosition.position;
        transform.rotation = initialPosition.rotation;
    }
}
