using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaPl.SKS.BusinessLogic.Interfaces.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class BLValidationException : LogicException
    {
        public BLValidationException(string logic, string operation) : base(logic, operation)
        {
        }

        public BLValidationException(string logic, string operation, string message) : base(logic, operation, message)
        {
        }

        public BLValidationException(string logic, string operation, string message, Exception innerException) : base(logic, operation, message, innerException)
        {
        }
    }

}
