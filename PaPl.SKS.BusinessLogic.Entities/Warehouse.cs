using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaPl.SKS.BusinessLogic.Entities
{
    [ExcludeFromCodeCoverage]
    public class Warehouse : Hop
    {
        public int? Level { get; set; }
        public List<WarehouseNextHops> NextHops { get; set; }
    }
}
