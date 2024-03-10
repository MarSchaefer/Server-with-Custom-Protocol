namespace Server.Source.Interfaces
{
    public interface ISerializable<T>
    {
        void SaveValue(T value);

        T LoadValue();

        void CreateDefaultFile(string fileDoesNotExsistWaring);

        string GetUniqueKey();

        T GetDefaultValue();
    }
}