using System;
using System.Text.Json;
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
        public long TimeStamp { get; private set; }  
        public RequestBase Request { get; private set; }
        // ReSharper disable once ConvertToAutoProperty
        public JsonDocument Document => _document;
        public Byte[] SerializedRequest => _requestSerializedToUTF8Bytes;
        public Commands Command { get;  }

        // ctors
        public RequestDocument(RequestBase request)
        {
            TimeStamp = DateTime.UtcNow.Ticks;
            ID = request.ID;
            Request = request;
            _document = CreateJsonFromRequest(request);
            _requestSerializedToUTF8Bytes = Request.SerializeToJson();
        }
        
        // for use with EF
        private RequestDocument()
        {
            Request = _requestSerializedToUTF8Bytes.DeSerializeFromJson<AccountRequest>();
        }
        
        public RequestDocument(RequestBase request, Commands command) : this( request )
        {
            Command = command;
        }

        // methods
        public override string ToString()
        { 
            throw new  NotImplementedException();
        }
        
        // private helpers
        private JsonDocument CreateJsonFromRequest(RequestBase request)
        {
            byte[] model = request.SerializeToJson();
            return JsonDocument.Parse(model);
        }
    }
}