using System;
using System.Collections.Generic;
using System.Text;

namespace ShipRate.Common.Exceptions
{
    public class GeneralAppException:Exception
    {
        public GeneralAppException(string message):base(message)
        {}
    }
}
