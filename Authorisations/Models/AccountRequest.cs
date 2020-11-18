using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Authorisations.Models
{
    public class AccountRequest: IRequest
    {
         // properties
         public Guid  RequestID { get; private set; } 
         public string FirstName{ get; private set; } 
         public string LastName { get; private set; } 
         public Contract Contract{ get; private set; } 
         public DateTime DateSubmitted{ get; private set; }
         public RequestStatus Status { get; private set; }

         // contructors
         private AccountRequest()
         {
             RequestID = Guid.NewGuid();
             Status = RequestStatus.New;
         }

         public AccountRequest(string fn, string ln, Contract contract) : this()
         {
             FirstName = fn;
             LastName = ln;
             Contract = contract;
         }

         // methods, actions
         public void Submit()
         {
             DateSubmitted = DateTime.Today;
             Status = RequestStatus.Submitted;
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

         public void SaveTo(string directoryPath)
         {
             var  fullPath = Path.Combine(directoryPath,$"{RequestID}.json");
             // note the using declaration
             using StreamWriter outputFile = new StreamWriter(fullPath);
             outputFile.WriteLine(ToJson());
         }
    }
}