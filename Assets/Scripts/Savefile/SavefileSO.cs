using System;
using System.IO;
using UnityEngine;

public struct TowerResumen
{
    public Vector3 position { get; }
    public TowerType tipus { get; }
    public int tier { get; }

    public override string ToString() => $"({position}, {tipus}, {tier})";
}

[CreateAssetMenu(fileName = "SavefileSO", menuName = "Scriptable Objects/SavefileSO")]
public class SavefileSO : ScriptableObject
{
    public TowerResumen[] placedTowers;
    public int level;
    public int xp;
    public int round;

    private string ToJson()
    {
        return JsonUtility.ToJson(this, true);
    }

    private void FromJson(string json)
    {
        JsonUtility.FromJsonOverwrite(json, this);
    }

    public void Save()
    {
        string filepath = GetFilePath();

        try
        {
            File.WriteAllText(filepath, ToJson());
        }
        catch (Exception e)
        {
            Debug.LogError($"Error: Could not save {filepath} : {e.Message}");
        }
    }

    public void Load()
    {
        string filepath = GetFilePath();

        try
        {
            FromJson(File.ReadAllText(filepath));
        }
        catch (Exception e)
        {
            Debug.LogError($"Error: Could not load {filepath} : {e.Message}");
        }
    }

    private string GetFilePath()
    {
        #if UNITY_EDITOR
            return "savefile";
        #else
            return Path.Combine(Application.persistentDataPath, SaveFile);
        #endif
    }
}
