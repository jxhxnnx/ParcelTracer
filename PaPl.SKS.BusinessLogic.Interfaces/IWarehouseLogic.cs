using PaPl.SKS.BusinessLogic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaPl.SKS.BusinessLogic.Interfaces
{
    public interface IWarehouseLogic
    {
        public Hop ExportWarehouse();
        public Warehouse GetWarehouse(string code);
        public Hop GetHopByCode(string code);
        public int ImportWarehouse(Hop warehouse);

    }
}
