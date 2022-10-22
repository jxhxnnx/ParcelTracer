using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using Formatting = Newtonsoft.Json.Formatting;

namespace PaPl.SKS.BusinessLogic.Entities
{
    [ExcludeFromCodeCoverage]

    public class Parcel
    {
        public float? Weight { get; set; }
        public Recipient Recipient { get; set; }
        public Recipient Sender { get; set; }
        [Key]
        public string TrackingId { get; set; }
        public StateEnum? State { get; set; }
        public List<HopArrival> VisitedHops { get; set; }
        public List<HopArrival> FutureHops { get; set; }

        public enum StateEnum
        {
            PickupEnum = 0,
            InTransportEnum = 1,
            InTruckDeliveryEnum = 2,
            TransferredEnum = 3,
            DeliveredEnum = 4
        }
    }
}
