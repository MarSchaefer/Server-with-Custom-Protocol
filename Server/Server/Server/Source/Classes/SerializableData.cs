using Server.Source.Interfaces;

namespace Server.Source.Classes
{
    public class SerializableData<TData>: ISerializable<TData>
    {
        private readonly Serializer<TData> _serializer;
        private readonly string _filePath;

        public SerializableData(string filePath, string uniqueKey, TData defaultValue)
        {
            _serializer = new Serializer<TData>(filePath, uniqueKey, defaultValue);
            _filePath = filePath;
        }

        public void CreateDefaultFile(string fileDoesNotExsistWaring)
        {
            _serializer.CreateDefaultFile(fileDoesNotExsistWaring);
        }

        public TData GetDefaultValue()
        {
            return _serializer.GetDefaultValue();
        }

        public TData LoadValue()
        {
            try
            {
                return _serializer.LoadValue();
            }
            catch
            {
                Console.WriteLine("Daten konnten nicht geladen werden: " + this._filePath);
            }

            return GetDefaultValue();
        }

        public void SaveValue(TData value)
        {
            _serializer.SaveValue(value);
        }
    }
}
