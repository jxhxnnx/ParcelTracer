using System;
using System.Diagnostics.CodeAnalysis;

namespace PaPl.SKS.DataAccess.Interfaces.Exceptions
{
    [ExcludeFromCodeCoverage]

    public class DataException : ApplicationException
    {
        private string Repository;
        private string Operation;
        public DataException(string repository, string operation)
        {
            Repository = repository;
            Operation = operation;
        }
        public DataException(string repository, string operation, string message, Exception innerException) : base(message, innerException)
        {
            Repository = repository;
            Operation = operation;
        }
        public DataException(string repository, string operation, string message) : base(message)
        {
            Repository = repository;
            Operation = operation;
        }

    }
}
