using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Authorisations.Models
{
    public static class ModelExtensions
    {
        public static byte[] SerializeToJson(this object model)
        {
            var options = new JsonSerializerOptions {WriteIndented = true};
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
            return JsonSerializer.Deserialize<TRequest>(readOnlySpan);
        }
    }
}