using System;
using System.Collections.Generic;
using UnityEngine;
#if (UNITY_EDITOR) 
using UnityEditor;
#endif
using System.IO;
using System.Linq;
using System.Text;
using System.IO.Compression;

public static class SaveDataHandler
{
    public static SaveData saveData;
    public static string currentFilename = "save";

    public static SerialDictionary<string, Texture2D> saveList = new SerialDictionary<string, Texture2D>();


    public static void NewGame(string filename)
    {
        currentFilename = filename;
        saveData = new SaveData();
        List<IDataObject> dataObjects = FindDataObjects();
        foreach (IDataObject dataObject in dataObjects)
        {
            dataObject.Save(saveData);
        }
    }

    /*  save function: calls all data objects for them to add the stuff they want to save to the savedata object
        saves the data to disk along with a checksum */
    public static void Save(string filename, bool auto = false)        
    {
        if (saveData == null)
        {
            saveData = new SaveData();
        }
        saveData.autosave = auto;
        saveData.saveImage = ScreenCapture.CaptureScreenshotAsTexture();
        List<IDataObject> dataObjects = FindDataObjects();
        foreach (IDataObject dataObject in dataObjects)
        {
            dataObject.Save(saveData);
        }
        currentFilename = filename;
        string fn = filename + ".sav";
        string json = JsonUtility.ToJson(saveData);
        using (FileStream fs = new FileStream(fn, FileMode.Create))
        {
            if(saveData.saveImage == null) saveData.saveImage = new Texture2D(100, 100);
            byte[] image = ImageConversion.EncodeToJPG(saveData.saveImage);
            byte[] data = Encoding.UTF8.GetBytes(json);
            int checksum = CRC(data);
            using (DeflateStream ds = new DeflateStream(fs, CompressionMode.Compress))
            {
                using (BinaryWriter w = new BinaryWriter(ds))
                {
                    w.Write(image.Length);
                    w.Write(image);
                    w.Write(checksum);
                    w.Write(data.Length);
                    w.Write(data);
                }
            }
            
            Debug.Log("wrote " + data.Length + " bytes of data with checksum " + checksum + " to save file: " + fn);
            Debug.Log("\n" + saveData.GetTransform(saveData.p1guid).position);
        }
        
    }

    /* load function: loads a saved game from disk and call all data objects to update to loaded variables
    returns true if loadning is successfull */
    public static bool Load(string filename)
    {
        string fn = filename + ".sav";
        using (FileStream fs = new FileStream(fn, FileMode.Open, FileAccess.Read))
        {
            using (DeflateStream ds = new DeflateStream(fs, CompressionMode.Decompress))
            {
                using (BinaryReader r = new BinaryReader(ds))
                {
                    int bytes = r.ReadInt32();
                    byte[] image = r.ReadBytes(bytes);
                    int checksum = r.ReadInt32();
                    bytes = r.ReadInt32();
                    byte[] data = r.ReadBytes(bytes);
                    string json = Encoding.UTF8.GetString(data);

                    Debug.Log(checksum);
                    Debug.Log(json);

                    if (CRC(data) != checksum)
                    {
                        Debug.LogError("Savefile checksum mismatch");
                        return false;
                    }
                    
                    var checkSaveData = JsonUtility.FromJson<SaveData>(json);
                    if (checkSaveData == null)
                    {
                        return false;
                    }

                    saveData = checkSaveData;
                    currentFilename = filename;

                    List<IDataObject> dataObjects = FindDataObjects();
                    foreach (IDataObject dataObject in dataObjects)
                    {
                        dataObject.Load(saveData);
                    }
                    return true;
                }
            }       
        }
    }

    public static void Reset()
    {
        List<IDataObject> dataObjects = FindDataObjects();
        foreach (IDataObject dataObject in dataObjects)
        {
            dataObject.Load(saveData, true);
        }
    }

    public static void UpdateSaveInfo()
    {
        saveList.Clear();
        DirectoryInfo d = new DirectoryInfo(".");
        foreach (var file in d.GetFiles("*.sav"))
        {
            using (FileStream fs = new FileStream(file.Name, FileMode.Open, FileAccess.Read))
            {
                using (DeflateStream ds = new DeflateStream(fs, CompressionMode.Decompress))
                {
                    using (BinaryReader r = new BinaryReader(ds))
                    {
                        int bytes = r.ReadInt32();
                        byte[] data = r.ReadBytes(bytes);
                        Texture2D image = new Texture2D(255, 255);
                        ImageConversion.LoadImage(image, data);
                        saveList.Add(file.Name.Replace(".sav", ""), image);
                    }
                }       
            }
        }
    }


    private static List<IDataObject> FindDataObjects()
    {
        IEnumerable<IDataObject> dataObjects = UnityEngine.Object.FindObjectsOfType<MonoBehaviour>(true).OfType<IDataObject>();
        return new List<IDataObject>(dataObjects);
    }

    #if (UNITY_EDITOR) 
    [MenuItem("Save System/Save")] static void CSave()
    {
        Save(currentFilename);
    }

    [MenuItem("Save System/Simulate Autosave")] static void CAuto()
    {
        Save(currentFilename, true);
    }

    [MenuItem("Save System/Load")] static void CLoad()
    {
        Load(currentFilename);
    }

    [MenuItem("Save System/Reset")] static void CReset()
    {
        Reset();
    }

    [MenuItem("Save System/Regenerate Object GUIDs")] static void CRegenerateGUIDs()
    {
        IEnumerable<IGUIDObject> guidObjects = UnityEngine.Object.FindObjectsOfType<MonoBehaviour>(true).OfType<IGUIDObject>();
        foreach (IGUIDObject go in guidObjects)
        {
            Undo.RecordObject(go.GetObject(), "GUID refresh");
            go.RegenerateGUID();
            PrefabUtility.RecordPrefabInstancePropertyModifications(go.GetObject());
        }
    }
    #endif

    
    public static int CRC(byte[] bytes)      /* i didnt make this so please dont ask how it works */
    {
        int crc = 0; /* CRC value is 16bit */
        foreach (byte b in bytes)
        {
            crc ^= (b << 8); /* move byte into MSB of 16bit CRC */
            for (int i = 0; i < 8; i++)
            {
                if ((crc & 0x8000) != 0) /* test for MSB = bit 15 */
                {
                    crc = ((crc << 1) ^ 0x8005);
                }
                else
                {
                    crc <<= 1;
                }
            }
        }
        return crc;
    }

    
}
