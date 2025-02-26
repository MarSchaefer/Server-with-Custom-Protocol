namespace Server.Source.Interfaces
{
    public interface ISerializable<T>
    {
        void SaveValue(T value);

        T LoadValue();

        void CreateDefaultFile(string fileDoesNotExistWaring);

        T GetDefaultValue();
    }
}