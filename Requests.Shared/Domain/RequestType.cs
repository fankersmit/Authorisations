using System.Text.Json.Serialization;

namespace Requests.Shared.Domain
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum RequestType
    {
        Account,
        Organisation,
        Product
    }
}