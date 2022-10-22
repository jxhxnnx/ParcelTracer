using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaPl.SKS.DataAccess.Entities
{
    [ExcludeFromCodeCoverage]

    public class GeoCoordinate
    {
        public double? Lat { get; set; }
        public double? Lon { get; set; }
    }
}
