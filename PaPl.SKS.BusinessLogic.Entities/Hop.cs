﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaPl.SKS.BusinessLogic.Entities
{
    [ExcludeFromCodeCoverage]
    public class Hop
    {
        public Warehouse Parent { get; set; }
        public string HopType { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int? ProcessingDelayMins { get; set; }
        public string LocationName { get; set; }
        public GeoCoordinate LocationCoordinates { get; set; }
    }
}
