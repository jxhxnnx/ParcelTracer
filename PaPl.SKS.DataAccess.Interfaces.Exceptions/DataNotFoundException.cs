using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaPl.SKS.DataAccess.Interfaces.Exceptions
{
    [ExcludeFromCodeCoverage]

    public class DataNotFoundException : DataException
    {
        public DataNotFoundException(string repository, string operation) : base(repository, operation)
        {
            
        }

        public DataNotFoundException(string repository, string operation, string message) : base(repository, operation, message)
        {
        }

        public DataNotFoundException(string repository, string operation, string message, Exception innerException) : base(repository, operation, message, innerException)
        {
        }
    }
}
