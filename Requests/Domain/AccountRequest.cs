using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace Requests.Domain
{
    public class AccountRequest: RequestBase
    {
         // accountRequest properties
         
         // constructors
         private AccountRequest()
         {
         }

         public AccountRequest( Person applicant, Contract contract) : this()
         {
             Applicant = applicant;
             Contract = contract;
         }

         // methods
         public void SaveTo(string directoryPath)
         {
             var  fullPath = Path.Combine(directoryPath,$"{Id}.json");
             // note the using declaration
             using StreamWriter outputFile = new StreamWriter(fullPath);
             outputFile.WriteLine(Encoding.UTF8.GetString(ToJson() ));
             outputFile.Flush();
             outputFile.Close();
         }
    }
}