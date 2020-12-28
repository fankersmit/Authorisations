using System.Text.Json.Serialization;

namespace Requests.Shared.Domain
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum RequestStatus 
    {
        New = 128,             // 10000000
        Submitted = 64,        // 01000000
        Confirmed = 32,        // 00100000
        Cancelled = 16,        // 00010000
        Approved = 8,          // 00001000
        Rejected = 4,          // 00000100
        Concluded = 2,         // 00000010
        Removed = 1            // 00000001
    }
}
