using System;
using System.Diagnostics.CodeAnalysis;

namespace PaPl.SKS.BusinessLogic.Interfaces.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class LogicException : ApplicationException
    {
        private string Logic;
        private string Operation;
        public LogicException(string logic, string operation)
        {
            Logic = logic;
            Operation = operation;
        }
        public LogicException(string logic, string operation, string message, Exception innerException) : base(message, innerException)
        {
            Logic = logic;
            Operation = operation;
        }
        public LogicException(string logic, string operation, string message) : base(message)
        {
            Logic = logic;
            Operation = operation;
        }
    }
}
