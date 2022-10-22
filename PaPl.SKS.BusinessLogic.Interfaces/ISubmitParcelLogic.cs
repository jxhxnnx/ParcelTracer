using NetTopologySuite.Geometries;
using PaPl.SKS.BusinessLogic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaPl.SKS.BusinessLogic.Interfaces
{
    public interface ISubmitParcelLogic
    {
        public string SubmitParcel(Parcel body);
        public void GetPolygonToTruck(List<DataAccess.Entities.Truck> allTrucks, Coordinate geoCoordinateRecipient, Coordinate geoCoordinateSender, ref DataAccess.Entities.Truck SenderTruck, ref DataAccess.Entities.Truck RecipientTruck);
        public void MapManuallyHopToHopArrival(List<PaPl.SKS.DataAccess.Entities.Hop> daoFutureHops, List<HopArrival> futureHops);
        public bool PointIsInPolygon(Coordinate point, Coordinate[] polygon);
        public List<DataAccess.Entities.Hop> PredictFutureHops(DataAccess.Entities.Hop SenderTruck, DataAccess.Entities.Hop RecipientTruck);
        public List<DataAccess.Entities.Truck> GetAllTrucks();
        public string RandomString(int length);
        public HopArrival SetSenderTruckToVisitedHop(DataAccess.Entities.Truck SenderTruck);

    }
}
