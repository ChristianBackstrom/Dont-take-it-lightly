using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*  all serializable fields in this class are saved */
[System.Serializable] public class SaveData
{
    public bool autosave = false;
    public string p1guid;
    public string p2guid;

    [System.NonSerialized]public Texture2D saveImage = new Texture2D(255, 255);

    public SerialDictionary<string, SerialTransform> positions = new SerialDictionary<string, SerialTransform>();
    public SerialDictionary<string, float> floats = new SerialDictionary<string, float>();
    public SerialDictionary<string, int> ints = new SerialDictionary<string, int>();

    public void SetTransform(string key, SerialTransform value)
    {
        if (positions.ContainsKey(key))
        {
            positions[key] = value;
            return;
        }
        positions.Add(key, value);
    }
    
    public SerialTransform GetTransform(string key)
    {
        if (positions.ContainsKey(key))
        {
            return positions[key];
        }
        return null;
    }

    public void SetInt(string key, int value)
    {
        if (ints.ContainsKey(key))
        {
            ints[key] = value;
            return;
        }
        ints.Add(key, value);
    }
    
    public int GetInt(string key)
    {
        if (ints.ContainsKey(key))
        {
            return ints[key];
        }
        return -1;
    }

    public void SetFloat(string key, float value)
    {
        if (floats.ContainsKey(key))
        {
            floats[key] = value;
            return;
        }
        floats.Add(key, value);
    }
    
    public float GetFloat(string key, float value = 0f)
    {
        if (floats.ContainsKey(key))
        {
            return floats[key];
        }
        return value;
    }

    public void SetBool(string key, bool? value)
    {
        int stored = 0;
        if (value == true) stored = 1; 
        if (value == null) stored = -1;
        if (ints.ContainsKey(key))
        {
            ints[key] = stored;
            return;
        }
        ints.Add(key, stored);
    }
    
    public bool? GetBool(string key)
    {
        if (ints.ContainsKey(key))
        {
            if (ints[key] == 0) return false;
            return true;
        }
        return null;
    }
}
