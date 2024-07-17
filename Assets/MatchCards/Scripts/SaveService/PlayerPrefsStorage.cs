using UnityEngine;

public class PlayerPrefsStorage : IStorage
{
    public void Clear()
    {
        PlayerPrefs.DeleteAll();
    }

    public int GetInt(string key, int defaultValue = 0)
    {
        return PlayerPrefs.GetInt(key, defaultValue);
    }

    public void SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
    }
}
