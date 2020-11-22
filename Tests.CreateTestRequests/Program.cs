using System;
using System.IO;
using Requests.Domain;
using Tests.Helpers;


namespace Tests.CreateTestRequests
{
    class Program
    {
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
                AccountRequest ar = request as AccountRequest;
                var path =  @"C:\Projects\Authorisations\Documentation";
                ar.SaveTo(path);
                Console.WriteLine(request.Id);
            }

            Console.WriteLine(
                $"Created {count} {Enum.GetName(typeof(RequestType), requestType)} requests. Press any key...");
        }

        private static RequestBase CreateRequest(object requestType)
        {
            RequestBase request;

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
        private static RequestBase CreateOrganisationRequest()
        {
            throw new NotImplementedException();
        }

        private static RequestBase CreateAccountRequest()
        {
            // create organisation
            var factory = DomainTypesFactory.Instance;
            var org = factory.CreateOrganisation();
            var applicant = factory.CreateApplicant();
            var contract = factory.CreateContract(org);
            return new AccountRequest(applicant, contract);
        }

        private static RequestBase CreateProductRequest()
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