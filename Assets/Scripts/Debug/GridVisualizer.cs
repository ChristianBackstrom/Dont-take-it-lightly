using System.Collections;
using System.Collections.Generic;
using UnityEditor;
#if (UNITY_EDITOR) 
using UnityEngine;

public class GridVisualizer : MonoBehaviour
{
    [SerializeField]
    private float whyLevel = 0.1f;

    [SerializeField]
    private bool show = true;

    private void OnDrawGizmos()
    {
        if (!show)
        {
            return;
        }

        Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
        Handles.color = Color.gray;
        Vector3 offset = new Vector3(0.5f, 0, 0.5f);

        for (int x = 0; x < 100; x += 2)
        {
            Vector3 size = new Vector3(x, 0, 100);
            Handles.DrawWireCube(offset+ Vector3.up * whyLevel, size);
        }

        for (int y = 0; y < 100; y += 2)
        {
            Vector3 size = new Vector3(100, 0, y);
            Handles.DrawWireCube(offset+ Vector3.up * whyLevel, size);
        }

    }
}
#endif