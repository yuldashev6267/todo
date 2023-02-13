using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Todo.WebAPi.Helpers;

public class ApiDateTimeConverter : JsonConverter<DateTime>
{
    private readonly string _format;

    public ApiDateTimeConverter(string format)
    {
        _format = format;
    }
    
    public override void Write(Utf8JsonWriter writer, DateTime date, JsonSerializerOptions options)
    {
        writer.WriteStringValue(date.ToString(_format));
    }

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateTime.ParseExact(reader.GetString()!, _format, null);
    }
    
}