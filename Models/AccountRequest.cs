using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Authorisation.Models
{
    public class AccountRequest: IRequest
    {
         public Guid  RequestID { get; private set; } 
         public string FirstName{ get; private set; } 
         public string LastName { get; private set; } 
         public Contract Contract{ get; private set; } 
         public DateTime StartDate{ get; private set; }
         public RequestStatus Status { get; private set; }

         private AccountRequest()
         {
             RequestID = Guid.NewGuid();
             Status = RequestStatus.New;
         }

         public AccountRequest(string fn, string ln, Contract contract, DateTime start) : this()
         {
             FirstName = fn;
             LastName = ln;
             Contract = contract;
             StartDate = start;
         }

         public void Confirm()
         {
             throw new NotImplementedException();
         }

         public void Cancel()
         {
             throw new NotImplementedException();
         }

         public void Approve()
         {
             throw new NotImplementedException();
         }

         public void Disapprove()
         {
             throw new NotImplementedException();
         }

         public void Conclude()
         {
             throw new NotImplementedException();
         }

         private void Remove()
         {
             throw new NotImplementedException();
         }
         
         public string ToJson()
         {
             var options = new JsonSerializerOptions();
             options.WriteIndented = true;
             options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
             return JsonSerializer.Serialize<AccountRequest>(this, options);
         }
    }
}