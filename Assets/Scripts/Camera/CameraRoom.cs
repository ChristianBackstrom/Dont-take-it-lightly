using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraRoom : MonoBehaviour
{
    public int index { private get; set; }
    public CameraManager cameraManager { private get; set; }

    void OnDisable()
    {
        if (cameraManager == null) return; 
        if (cameraManager.OnRoomDeactivate[index] == null) return;

        cameraManager.OnRoomDeactivate[index].Invoke();
    }

    void OnEnable()
    {
        if (cameraManager == null) return; 
        if (cameraManager.OnRoomActivate[index] == null) return;

        cameraManager.OnRoomActivate[index].Invoke();
    }

}
