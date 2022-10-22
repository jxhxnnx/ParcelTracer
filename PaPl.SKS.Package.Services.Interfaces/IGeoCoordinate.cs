using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaPl.SKS.Package.Services.Interfaces
{
    interface IGeoCoordinate
    {
        public double? Lat { get; set; }
        public double? Lon { get; set; }
    }
}
