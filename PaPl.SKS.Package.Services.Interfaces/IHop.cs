using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaPl.SKS.Package.Services.Interfaces
{
    interface IHop
    {
        public string HopType { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int? ProcessingDelayMins { get; set; }
        public string LocationName { get; set; }
        public IGeoCoordinate LocationCoordinates { get; set; }
        public string ToString();
        public string ToJson();

        public  bool Equals(object obj);

        public bool Equals(IHop other);

        public int GetHashCode();

    }
}
