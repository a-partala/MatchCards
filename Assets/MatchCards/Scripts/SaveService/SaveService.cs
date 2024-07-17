#if UNITY_EDITOR
using UnityEditor;
#endif
using Zenject;

public class SaveService
{
    private static IStorage storage;

    [Inject]
    public SaveService(IStorage storage)
    {
        SaveService.storage = storage;
    }

    public static void SetInt(string key, int value)
    {
        storage.SetInt(key, value);
    }

    public static int GetInt(string key, int defaultValue = 0)
    {
        return storage.GetInt(key, defaultValue);
    }

#if UNITY_EDITOR
    [MenuItem("Tools/Clear Save Data")]
    public static void ClearJsonData()
    {
        storage.Clear();
    }
#endif
}
