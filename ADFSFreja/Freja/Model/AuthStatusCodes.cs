using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Freja.Model
{
    public enum AuthStatusCodes
    {
        STARTED,
        DELIVERED_TO_MOBILE,
        APPROVED,
        REJECTED,
        CANCELED,
        EXPIRED,
        RP_CANCELED
    }
    public enum ValidAuthStatusCodes
    {
        APPROVED
    }
    public enum InvalidValidAuthStatusCodes
    {
        REJECTED,
        CANCELED,
        EXPIRED,
        RP_CANCELED
    }
}
