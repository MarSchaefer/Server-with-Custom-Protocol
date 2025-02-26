using Server.Source.Interfaces;
using System.Text.Json;

namespace Server.Source.Classes
{
    public class Serializer<TData> : ISerializable<TData>
    {
        private readonly string _filePath;
        private readonly string _uniqueKey;
        private readonly TData _defaultValue;

        private readonly static JsonSerializerOptions _jsonSerializeOptions = new JsonSerializerOptions() { 
            IncludeFields = true 
        };

        public Serializer(string relativeFilePath, string uniqueKey, TData defaultValue)
        {
            _filePath = relativeFilePath;
            _uniqueKey = uniqueKey;
            _defaultValue = defaultValue;
        }

        public TData LoadValue()
        {
            if (File.Exists(_filePath) == false)
            {
                CreateDefaultFile("File with default values created: " + this._filePath );
                return _defaultValue;
            }

            using (var fileStram = new FileStream(_filePath, FileMode.Open, FileAccess.Read))
            {
                using (var streamReader = new StreamReader(fileStram))
                {
                    return DeserializeValue(streamReader.ReadToEnd());
                }
            }
        }
        public void SaveValue(TData value)
        {
            string serializedValue = JsonSerializer.Serialize(ValueToDictionary(value), _jsonSerializeOptions);

            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }

            using (var fileStream = new FileStream(_filePath, FileMode.Create, FileAccess.Write))
            {
                using (var streamWriter = new StreamWriter(fileStream))
                {
                    streamWriter.Write(serializedValue);
                }
            }
        }

        public void CreateDefaultFile(string fileDoesNotExistWaring)
        {
            SaveValue(_defaultValue);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(fileDoesNotExistWaring);
            Console.ResetColor();
        }

        private Dictionary<string, TData> ValueToDictionary(TData value)
        {
            return new Dictionary<string, TData>() {
                { this._uniqueKey, value }
            };
        }

        private TData DeserializeValue(string valueToDeserialize)
        {
            Dictionary<string, TData> dictionaryWithValueToDeserialize = 
                JsonSerializer.Deserialize<Dictionary<string, TData>>(
                    valueToDeserialize,
                    Serializer<TData>._jsonSerializeOptions
                );

            if (dictionaryWithValueToDeserialize == null)
            {
                return _defaultValue;
            }

            if (dictionaryWithValueToDeserialize.TryGetValue(this._uniqueKey, out TData value))
            {
                return value;
            }

            return _defaultValue;
        }

        public string GetUniqueKey()
        {
            return this._uniqueKey;
        }

        public TData GetDefaultValue()
        {
            return _defaultValue;
        }
    }
}
