using Newtonsoft.Json.Linq;
using PaPl.SKS.Package.Services.DTOs.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaPl.SKS.Package.Services.DTOs
{
    [ExcludeFromCodeCoverage]
    public class HopJsonConverter : JsonCreationConverter<Hop>
    {
        protected override Hop Create(Type objectType, JObject jObject)
        {
            if (jObject == null) throw new ArgumentNullException("jObject");

            if (jObject["level"] != null && jObject["nextHops"] != null)
            {
                return new Warehouse();
            }
            else if (jObject["regionGeoJson"] != null && jObject["logisticsPartner"] != null && jObject["logisticsPartnerUrl"] != null)
            {
                return new Transferwarehouse();
            }
            else if (jObject["numberPlate"] != null && jObject["regionGeoJson"] != null)
            {
                return new Truck();
            }
            else
            {
                return new Hop();
            }
        }
    }
}
