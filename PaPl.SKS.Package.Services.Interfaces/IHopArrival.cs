using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaPl.SKS.Package.Services.Interfaces
{
    interface IHopArrival
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public DateTime? DateTime { get; set; }

        public string ToString();
        public string ToJson();

        public bool Equals(object obj);

        public bool Equals(IHopArrival other);

        public int GetHashCode();
    }
}
