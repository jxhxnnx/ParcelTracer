using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaPl.SKS.Package.Services.Interfaces
{
    interface IRecipient
    {
        public string Name { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string ToString();
        public string ToJson();

        public bool Equals(object obj);

        public bool Equals(IRecipient other);

        public int GetHashCode();
    }
}
