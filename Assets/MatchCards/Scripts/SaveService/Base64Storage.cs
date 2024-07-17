using System.IO;
using System;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

public class Base64Storage : IStorage
{
    private string GetFilePath(string key)
    {
        return Path.Combine(Application.persistentDataPath, $"{key}.txt");
    }

    public void Save<T>(string key, T data)
    {
        string filePath = GetFilePath(key);
        byte[] byteArray = ObjectToByteArray(data);
        string base64Data = Convert.ToBase64String(byteArray);
        File.WriteAllText(filePath, base64Data);
    }

    public T Load<T>(string key)
    {
        string filePath = GetFilePath(key);
        if (!File.Exists(filePath))
        {
            return default(T);
        }
        string base64Data = File.ReadAllText(filePath);
        byte[] byteArray = Convert.FromBase64String(base64Data);
        return ByteArrayToObject<T>(byteArray);
    }

    private byte[] ObjectToByteArray(object obj)
    {
        if (obj == null)
        {
            return null;
        }
        BinaryFormatter bf = new BinaryFormatter();
        using (MemoryStream ms = new MemoryStream())
        {
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }
    }

    private T ByteArrayToObject<T>(byte[] byteArray)
    {
        using (MemoryStream ms = new MemoryStream(byteArray))
        {
            BinaryFormatter bf = new BinaryFormatter();
            return (T)bf.Deserialize(ms);
        }
    }

    public void Clear()
    {
        string[] base64Files = Directory.GetFiles(Application.persistentDataPath, "*.txt");
        foreach (var file in base64Files)
        {
            File.Delete(file);
        }
        Debug.Log("Base64 save data cleaned.");
    }

    public void SetInt(string key, int value)
    {
        Save(key, value);
    }

    public int GetInt(string key, int defaultValue = 0)
    {
        var value = Load<int>(key);
        if(value == default(int))
        {
            return defaultValue;
        }
        return value;
    }

}
