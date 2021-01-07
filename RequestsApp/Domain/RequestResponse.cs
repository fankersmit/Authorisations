using System.IO;
using System.Linq;
using System.Text.Json;
using Requests.Shared.Domain;

namespace RequestsApp.Domain
{
    public class RequestResponse : Response
    {
        // fields
        private readonly string _serializedRequests;

        // properties
        public override byte[] AsUTF8Bytes => ConvertToUTF8ByteArray();

        // ctors
        public RequestResponse(Queries type, string serializedRequest) : base(type)
        {
            _serializedRequests = serializedRequest;
        }

        // private helper methods
        private byte[] ConvertToUTF8ByteArray()
        {
            byte[] jsonResponse;

            var encoded = _serializedRequests.ToArray<char>();
            var jsonDoc = JsonDocument.Parse(encoded);

            var stream = new MemoryStream();
            var options = new JsonWriterOptions
            {
                Indented = false
            };

            using (var writer = new Utf8JsonWriter(stream, options))
            {
                writer.WriteStartObject();
                writer.WriteString("Query", QueryType.ToString());
                foreach (var (key, value) in Arguments)
                {
                    writer.WriteString(key, value);
                }

                writer.WritePropertyName("Request");
                jsonDoc.WriteTo(writer);
                writer.WriteEndObject();
                writer.Flush();
            }

            jsonResponse = stream.ToArray();
            return jsonResponse;
        }
    }
}
