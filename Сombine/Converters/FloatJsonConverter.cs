using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Сombine.Converters
{
    public class FloatJsonConverter : JsonConverter<float>
    {
        private int _precision;
        private MidpointRounding _rounding;

        public FloatJsonConverter() : this(2) {}

        public FloatJsonConverter(int precision) : this(precision, MidpointRounding.AwayFromZero) {}

        public FloatJsonConverter(int precision, MidpointRounding rounding)
        {
            _precision = precision;
            _rounding = rounding;
        }

        public override float Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, float value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(Math.Round((decimal) value, _precision, _rounding));
        }
    }
}