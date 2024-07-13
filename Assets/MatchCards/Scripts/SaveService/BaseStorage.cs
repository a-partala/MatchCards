public abstract class BaseStorage
{
    public abstract void SetInt(string key, int value);
    public abstract int GetInt(string key, int defaultValue = 0);
}
