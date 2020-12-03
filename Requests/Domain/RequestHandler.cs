using 

namespace Requests.Domain
{
    public class RequestHandler
    {
        public RequestHandler()
        {
        }

        public bool Command(RequestBase request, Commands command)
        {
            switch (command)
            {
                case Commands.Submit:
                    request.Submit();
                    break;
                
                default:
                    break;
            }
        }
        
        



    }
}