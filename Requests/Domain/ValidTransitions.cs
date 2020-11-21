using System;

namespace Requests.Domain
{
   
    public enum ValidTransitions : uint
    {
        New = 128,          // 10000000 
        Submit = 192,       // 11000000
        Confirm = 96,       // 01100000
        Cancel = 80,        // 01010000
        Approve = 40,       // 00101000 
        Disapprove =36,     // 00100100
        Conclude = 30,      // 00011110
        Remove = 131        // 10000011 
    }
}