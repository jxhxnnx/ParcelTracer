using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaPl.SKS.Package.Services.Interfaces
{
    interface ITrackingInformation
    {
        public enum StateEnum
        { PickupEnum = 0, InTransportEnum = 1, InTruckDeliveryEnum = 2, TransferredEnum = 3, DeliveredEnum = 4 };

        public StateEnum? State { get; set; }
        public List<IHopArrival> VisitedHops { get; set; }
        public List<IHopArrival> FutureHops { get; set; }
        public string ToString();
        public string ToJson();

        public bool Equals(object obj);

        public bool Equals(ITrackingInformation other);

        public int GetHashCode();
    }
}
