using UnityEngine;

public class PrefsStorage : BaseStorage
{
    public override int GetInt(string key, int defaultValue = 0)
    {
        return PlayerPrefs.GetInt(key, defaultValue);
    }

    public override void SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
    }
}
