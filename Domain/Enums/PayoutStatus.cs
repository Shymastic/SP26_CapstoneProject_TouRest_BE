using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouRest.Domain.Enums
{
    public enum PayoutStatus
    {
        Pending,     // waiting for admin approval
        Approved,    // admin approved, calling PayOS
        Processing,  // PayOS processing bank transfer
        Completed,   // money sent
        Rejected,    // admin rejected
        Failed       // PayOS transfer failed
    }
}
