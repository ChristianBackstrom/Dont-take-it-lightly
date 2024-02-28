using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPortal : MonoBehaviour
{
    [SerializeField] CameraRoom destinationRoom;
    [SerializeField] int numberOfPlayers = 2;
    List<GameObject> playersEntered = new List<GameObject>();


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 6) return;

        playersEntered.Add(other.gameObject);
        if (playersEntered.Count >= numberOfPlayers)
        {
            CameraManager.ChangeRoom(destinationRoom);
            playersEntered.Clear();
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != 6) return;
        playersEntered.Remove(other.gameObject);
    }
}
