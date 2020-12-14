using System;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace Requests.Shared.Domain
{
    public static class ModelExtensions
    {
        public static byte[] SerializeToJson<TRequest>(this TRequest model)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters =
                {
                    new JsonStringEnumConverter()
                }
            };
            return JsonSerializer.SerializeToUtf8Bytes<TRequest>( model, options);
        }
        
        public static TRequest DeSerializeFromJson<TRequest>(this byte[] model)
        {
            var readOnlySpan = new ReadOnlySpan<byte>(model);
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters =
                {
                    new JsonStringEnumConverter()
                }
            };
            return JsonSerializer.Deserialize<TRequest>(readOnlySpan, options);
        }

        public static TRequest DeSerializeFromJson<TRequest>(this byte[] model, string propertyName)
            where TRequest : struct, Enum
        {
            var document = JsonDocument.Parse(model);
            var enumValueName = document.RootElement.GetProperty(propertyName).GetString();
            var result = Enum.Parse<TRequest>(enumValueName);
            return result;
        }
    }
}