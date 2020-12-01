#nullable enable
using System.Net.Mail;
using System.Text;

namespace Requests.Domain
{
    public class EmailAddress : MailAddress
    {
        public EmailAddress(string emailAddress) 
            : base(emailAddress)
        {
        }

        public EmailAddress(string address, string? displayName) 
            : base(address, displayName)
        {
        }
        
        public EmailAddress(string address, string? displayName, Encoding displayNameEncoding) 
            : base(address, displayName, displayNameEncoding)
        {
        }

        private EmailAddress() : base("ben@contoso.com" )
        {
        }
    }
}