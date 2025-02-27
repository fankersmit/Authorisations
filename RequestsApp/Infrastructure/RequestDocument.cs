using System;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging.Abstractions;
using Requests.Domain;
using Requests.Shared.Domain;

namespace RequestsApp.Infrastructure
{
    public class RequestDocument
    {
        // fields, document is not to be changed after creation, ever!
        private readonly JsonDocument _document;
        private readonly byte[] _requestSerializedToUTF8Bytes;
        
        // properties
        public Guid ID  { get; private set; }      
        public int  Version  { get; internal set; }
        public long TimeStamp { get; private set; }
        public Commands Command { get; private set; }
        public RequestBase Request { get; private set; }
        public string SerializedRequest { get; private set; }
        // ReSharper disable once ConvertToAutoProperty
        public JsonDocument Document => _document;
      
        // ctors
        public RequestDocument(RequestBase request, Commands command)
        {
            TimeStamp = DateTime.UtcNow.Ticks;
            Version = request.Version;
            ID = request.ID;
            Request = request;
            Command = command;
            _document = CreateJsonFromRequest(request);
            _requestSerializedToUTF8Bytes = Request.SerializeToJson();
            SerializedRequest = Encoding.UTF8.GetString(_requestSerializedToUTF8Bytes);
        }

        public RequestDocument() {}

        // methods
        public RequestBase GetRequestFromJsonDocument()
        {
            var builder = new RequestFromJsonBuilder(null);
            var jsonDocument = JsonDocument.Parse(SerializedRequest);
            return builder.GetRequest(jsonDocument.RootElement);
        }

        // private helpers
        private JsonDocument CreateJsonFromRequest(RequestBase request)
        {
            byte[] model = request.SerializeToJson();
            return JsonDocument.Parse(model);
        }
    }
}
