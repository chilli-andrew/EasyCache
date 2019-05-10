namespace EasyCache
{
    public interface IEasyCache
    {
        T GetWithCompression<T>(string key);
        void SetWithCompression<T>(string key, T value);
        T Get<T>(string key);
        void Set<T>(string key, T value);
        void Remove(string key);
    }
}