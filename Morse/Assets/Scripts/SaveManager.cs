using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml.Serialization;

public class SaveManager : MonoBehaviour
{
    private SaveData saveData;
    [SerializeField] private Morse morse;

    private void Awake()
    {
        Load();
    }

    public void Load()
    {
        if (PlayerPrefs.HasKey("save"))
        {
            saveData = StaticFunctions.Deserialize<SaveData>(PlayerPrefs.GetString("save"));
            morse.LoadSave(saveData);
            Debug.Log("Loaded: " + StaticFunctions.Serialize<SaveData>(saveData));
        }
        else
        {
            saveData = new SaveData();
            Save();
            Debug.Log("No save found, Created one...");
        }
    }

    public void Save()
    {
        morse.Save(ref saveData);
        PlayerPrefs.SetString("save", StaticFunctions.Serialize<SaveData>(saveData));
        Debug.Log("Saved: " + StaticFunctions.Serialize<SaveData>(saveData));
    }
}