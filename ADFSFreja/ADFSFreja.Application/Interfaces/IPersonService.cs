using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADFSFreja.Application.Interfaces
{
    public interface IPersonService
    {
        string GetCivicNumber(string uid);
    }
}
