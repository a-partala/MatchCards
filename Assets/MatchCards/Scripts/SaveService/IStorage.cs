public interface IStorage
{
    public void SetInt(string key, int value);
    public int GetInt(string key, int defaultValue = 0);

    public void Clear() { }
}
