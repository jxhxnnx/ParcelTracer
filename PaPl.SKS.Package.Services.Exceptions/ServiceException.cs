using System;
using System.Diagnostics.CodeAnalysis;

namespace PaPl.SKS.Package.Services.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class ServiceException : ApplicationException
    {
        private string Service;
        private string Operation;
        public ServiceException(string service, string operation)
        {
            Service = service;
            Operation = operation;
        }
        public ServiceException(string service, string operation, string message, Exception innerException) : base(message, innerException)
        {
            Service = service;
            Operation = operation;
        }
        public ServiceException(string service, string operation, string message) : base(message)
        {
            Service = service;
            Operation = operation;
        }
    }
}
