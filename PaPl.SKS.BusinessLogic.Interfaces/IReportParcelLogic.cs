using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaPl.SKS.BusinessLogic.Interfaces
{
    public interface IReportParcelLogic
    {
        public int ReportParcelHop(string trackingId, string code);
        public int ReportParcelDelivery(string trackingId);
    }
}
