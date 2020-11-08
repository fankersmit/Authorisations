namespace Authorisation.Models
{
    public interface IRequest
    {
        // Confirm
        void Confirm();
        // Cancel
        void Cancel();
        // Approve
        void  Approve();
        // Disapprove
        void Disapprove();
        // Conclude, 
        void Conclude();
         // Status   
         RequestStatus Status { get; }
    }
}