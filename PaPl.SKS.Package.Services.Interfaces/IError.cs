using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaPl.SKS.Package.Services.Interfaces
{
    interface IError
    {
        public string ErrorMessage { get; set; }
        public string ToString();
        public string ToJson();
        public bool Equals(object obj);
        public bool Equals(IError other);
        public int GetHashCode();
    }
}
