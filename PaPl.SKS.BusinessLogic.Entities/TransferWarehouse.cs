using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaPl.SKS.BusinessLogic.Entities
{
    [ExcludeFromCodeCoverage]
    public class TransferWarehouse : Hop
    {
        public string RegionGeoJson { get; set; }
        public string LogisticsPartner { get; set; }
        public string LogisticsPartnerUrl { get; set; }
    }
}
