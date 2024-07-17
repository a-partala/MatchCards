using System.IO;
using UnityEngine;

public class JsonStorage : IStorage
{
    private string GetFilePath(string key)
    {
        return Path.Combine(Application.persistentDataPath, $"{key}.json");
    }

    public void Save<T>(string key, T data)
    {
        string filePath = GetFilePath(key);
        string jsonData = JsonUtility.ToJson(new Wrapper<T>(data));
        File.WriteAllText(filePath, jsonData);
    }

    public T Load<T>(string key)
    {
        string filePath = GetFilePath(key);
        if (!File.Exists(filePath))
        {
            return default(T);
        }
        string jsonData = File.ReadAllText(filePath);
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(jsonData);
        return wrapper.Value;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T Value;

        public Wrapper(T value)
        {
            Value = value;
        }
    }

    public void Clear()
    {
        string[] jsonFiles = Directory.GetFiles(Application.persistentDataPath, "*.json");
        foreach (var file in jsonFiles)
        {
            File.Delete(file);
        }
        Debug.Log("JSON save data cleaned.");
    }

    public void SetInt(string key, int value)
    {
        Save(key, value);
    }

    public int GetInt(string key, int defaultValue = 0)
    {
        var value = Load<int>(key);
        if (value == default(int))
        {
            return defaultValue;
        }
        return value;
    }
}
