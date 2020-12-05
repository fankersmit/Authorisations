using System;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace Requests.Shared.Domain
{
    public static class ModelExtensions
    {
        public static byte[] SerializeToJson(this object model)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
                
            };
            return JsonSerializer.SerializeToUtf8Bytes(model, options);
        }
        
        public static byte[] SerializeToJson<TRequest>(this TRequest model)
        {
            var options = new JsonSerializerOptions {WriteIndented = true};
            return JsonSerializer.SerializeToUtf8Bytes<TRequest>( model, options);
        }
        
        public static TRequest DeSerializeFromJson<TRequest>(this byte[] model)
        {
            var readOnlySpan = new ReadOnlySpan<byte>(model);
            var options = new JsonSerializerOptions
            {
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
            return JsonSerializer.Deserialize<TRequest>(readOnlySpan, options);
        }
    }
}