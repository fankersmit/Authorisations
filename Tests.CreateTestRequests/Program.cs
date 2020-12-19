using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Authorisations.Models;
using Tests.Helpers;


namespace Tests.CreateTestRequests
{
    class Program
    {
        private const string  _path =  @"C:\Projects\Authorisations\Documentation";
            
        static void Main(string[] args)
        {
            if (args.Length != 4)
            {
                Usage();
            }

            var requestType = GetReqeustType(args);
            var count = GetNumberOfRequests(args);

            for (var idx = 0; idx < count; idx++)
            {
                var request = CreateRequest(requestType);
                var jsonString = ConvertToJsonString(request);
                SaveToFile( request.ID, jsonString, _path );
                Console.WriteLine(request.ID);
            }

            Console.WriteLine(
                $"Created {count} {Enum.GetName(typeof(RequestType), requestType)} requests. Press any key...");
        }

        private static string ConvertToJsonString(RequestModel model)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            options.Converters.Add(new JsonStringEnumConverter());
            var json = JsonSerializer.SerializeToUtf8Bytes(model, options);
            return Encoding.UTF8.GetString(json);
        }

        private static void SaveToFile( Guid key,  string jsonString, string path )
        {
            var  fullPath = Path.Combine(path,$"{key.ToString()}.json");
            // note the using declaration
            using StreamWriter outputFile = new StreamWriter(fullPath);
            outputFile.WriteLine(jsonString);
            outputFile.Flush();
            outputFile.Close();
        }

        private static RequestModel CreateRequest(object requestType)
        {
            RequestModel request;

            switch (requestType)
            {
                case RequestType.Organisation:
                    request = CreateOrganisationRequest();
                    break;
                case RequestType.Product:
                    request = CreateProductRequest();
                    break;
                case RequestType.Account:
                    request = CreateAccountRequest();
                    break;
                case RequestType.Invalid:
                default:
                    throw new ArgumentException("Fatal error: Unknown request type");
            }

            return request;
        }

        private static RequestType GetReqeustType(string[] args)
        {
            RequestType rt = RequestType.Invalid;
            if (args[0].ToLowerInvariant() != "--type") Usage();
            var arg2 = args[1].ToUpperInvariant();
            switch (arg2)
            {
                case "A":
                    rt = RequestType.Account;
                    break;
                case "O":
                    rt = RequestType.Organisation;
                    break;
                case "P":
                    rt = RequestType.Product;
                    break;
                default:
                    Usage();
                    break;
            }

            return rt;
        }

        private static void Usage()
        {
            Console.WriteLine("Usage:  --type <A|O|P>  --count <positive number>");
            Console.WriteLine("Example:  --type A  --count 3");
            Console.WriteLine("Creates 3  requests  of type Account");
            Console.WriteLine("A  = Account, O = Organisation, P = Product");
            Console.ReadLine();
            Environment.Exit(0);
        }

        // ---------------------------------------------------------
        // helper methods
        // ---------------------------------------------------------
        //
        private static RequestModel CreateOrganisationRequest()
        {
            throw new NotImplementedException();
        }

        private static RequestModel CreateAccountRequest()
        {
            // create organisation
            var factory = ModelTypesFactory.Instance;
            var org = factory.CreateOrganisation();
            var model = new RequestModel
            {
                Contract = factory.CreateContract(org), 
                Applicant = factory.CreateApplicant()
            };

            return model;
        }

        private static RequestModel CreateProductRequest()
        {
            throw new NotImplementedException();
        }

        private static int GetNumberOfRequests(string[] args)
        {
            if (args[2].ToLowerInvariant() != "--count") Usage();
            if (!int.TryParse(args[3], out var numberOfRequests)) Usage();
            if (numberOfRequests < 1) Usage();
            return numberOfRequests;
        }
    }
}