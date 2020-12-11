using System.Text.Json.Serialization;

namespace Requests.Shared.Domain
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Commands
    {
        NoOp, 
        Submit,
        Confirm,
        Cancel,
        Approve, 
        Disapprove,
        Conclude,
        Remove      
    }
}