using System.Text.Json.Serialization;

namespace Requests.Shared.Domain
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum RequestType
    {
        Account,            // 0
        Organisation,       // 1
        Product             // 2
    }
}
