public static class SaveService
{
    private static BaseStorage _storage;
    private static BaseStorage storage
    {
        get
        {
            if (_storage == null)
            {
                _storage = new PrefsStorage();
            }
            return _storage;
        }
    }

    public static void SetInt(string key, int value)
    {
        storage.SetInt(key, value);
    }

    public static int GetInt(string key, int defaultValue = 0)
    {
        return storage.GetInt(key, defaultValue);
    }
}
