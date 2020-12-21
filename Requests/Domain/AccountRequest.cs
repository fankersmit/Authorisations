using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Requests.Shared.Domain;


namespace Requests.Domain
{
    public class AccountRequest: RequestBase
    {
         // accountRequest properties
         
         // constructors
         public AccountRequest()
         {
         }

         public AccountRequest( Person applicant, Contract contract) 
         {
             Applicant = applicant;
             Contract = contract;
         }

         // methods
         public void SaveTo(string directoryPath)
         {
             var  fullPath = Path.Combine(directoryPath,$"{ID}.json");
             // note the using declaration
             using StreamWriter outputFile = new StreamWriter(fullPath);
             outputFile.WriteLine(Encoding.UTF8.GetString(this.SerializeToJson() ));
             outputFile.Flush();
             outputFile.Close();
         }
    }
}