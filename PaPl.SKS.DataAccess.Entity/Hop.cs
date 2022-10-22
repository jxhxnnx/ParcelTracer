using PaPl.SKS.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaPl.SKS.DataAccess.Entities
{
    [ExcludeFromCodeCoverage]

    public class Hop
    {
        public Warehouse Parent { get; set; }

        public string HopType { get; set; }
        [Key]
        public string Code { get; set; }
        public string Description { get; set; }
        public int? ProcessingDelayMins { get; set; }
        public string LocationName { get; set; }

    
        [Column(TypeName = "geometry")]
        public NetTopologySuite.Geometries.Point GeoLocation { get; set; }

    }
}
