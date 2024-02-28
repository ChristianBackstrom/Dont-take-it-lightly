using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorRoomTarget : MonoBehaviour
{
    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position, new Vector3(1,1,1));
    }
}
