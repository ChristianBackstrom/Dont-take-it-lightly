using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cinemachine;

public class CameraManager : MonoBehaviour, IDataObject, IGUIDObject
{
    [SerializeField] CinemachineBrain cinemachineBrain;
    [SerializeField] CameraRoom[] cameras;
    [SerializeField] public UnityEvent[] OnRoomActivate;
    [SerializeField] public UnityEvent[] OnRoomDeactivate;
    [SerializeField] string guid = System.Guid.NewGuid().ToString();
    
    static CameraRoom currentRoom;

    void Awake()
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].cameraManager = this;
            cameras[i].index = i;
            cameras[i].gameObject.SetActive(false); 
        }
        if (SaveDataHandler.saveData == null)
        {
            cameras[0].gameObject.SetActive(true);
            currentRoom = cameras[0];
        }
        else
        {
            int i = SaveDataHandler.saveData.GetInt(guid);
            if (i == -1) i = 0;
            cameras[i].gameObject.SetActive(true);
            currentRoom = cameras[i];
        }
        
    }

    public static void ChangeRoom(CameraRoom cameraRoom)
    {
        if (cameraRoom == currentRoom) return;
        cameraRoom.gameObject.SetActive(true);
        currentRoom.gameObject.SetActive(false);
        currentRoom = cameraRoom;
    }

    public void Save(SaveData saveData)
    {
        //if (!saveData.autosave) return;
        for (int i = 0; i < cameras.Length; i++)
        {
            if (!cameras[i].gameObject.activeInHierarchy) continue;
            saveData.SetInt(guid, i);
        }
    }

    public void Load(SaveData saveData, bool reset = false)
    {
        CinemachineBlendDefinition oldSetting = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.EaseInOut, 1f);
        if (cinemachineBrain != null)
        {
            oldSetting = cinemachineBrain.m_DefaultBlend;
            cinemachineBrain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.Cut, 0f);
        }
        int i = saveData.GetInt(guid);
        if (i == -1) i = 0;
        ChangeRoom(cameras[i]);
        if (cinemachineBrain != null)
        {
            revertCamera(oldSetting);
        }
    }

    async void revertCamera(CinemachineBlendDefinition oldSetting)
    {
        await System.Threading.Tasks.Task.Delay(60);
        cinemachineBrain.m_DefaultBlend = oldSetting;
    }

    public void RegenerateGUID()
    {
        guid = System.Guid.NewGuid().ToString();
    }
    public Object GetObject()
    {
        return this;
    }
}
