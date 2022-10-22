using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaPl.SKS.DataAccess.Entities
{
    [ExcludeFromCodeCoverage]

    public class TransferWarehouse : Hop
    {
        public int Id { get; set; }
        [Column(TypeName = "geometry")]
        public Geometry RegionGeoJson { get; set; }
        public string LogisticsPartner { get; set; }
        public string LogisticsPartnerUrl { get; set; }
    }
}
