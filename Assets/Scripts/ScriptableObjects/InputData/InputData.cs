using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[CreateAssetMenu(fileName = "InputData", menuName = "InputData", order = 0)]
public class InputData : ScriptableObject
{

    [Serializable]
    public class InputDataEntry
    {
        public string key;
        public string prompt;
        public GameObject p1Prefab;
        public GameObject p2Prefab;
    }
    public InputDataEntry[] entries;
    public InputDataEntry GetEntry(string key)
    {
        foreach (var entry in entries)
        {
            if (entry.key == key)
                return entry;
        }
        return null;
    }
}
