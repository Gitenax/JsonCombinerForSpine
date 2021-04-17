using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace Сombine
{
    public class JsonWriter
    {
        private JsonSerializerOptions _options;

        public JsonWriter() : this(true) {}

        public JsonWriter(bool compress)
        {
            ExecuteOptions(!compress);
        }

        private void ExecuteOptions(bool compress)
        {
            _options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = compress,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                },
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            };
        }
        
        public string Serialize(SpineDocument document)
        {
            return JsonSerializer.Serialize(document, _options);
        }
    }
}