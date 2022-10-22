using NetTopologySuite.Geometries;
using PaPl.SKS.BusinessLogic.Entities;
using System;
using System.Collections.Generic;

namespace PaPl.SKS.BusinessLogic.Interfaces
{
    public interface ITransitionLogic
    {
        public string TransitionParcel(Parcel body, string trackingId);
        public void GetPolygonToTruck(List<DataAccess.Entities.Truck> allTrucks, Coordinate geoCoordinateRecipient, Coordinate geoCoordinateSender, ref DataAccess.Entities.Truck SenderTruck, ref DataAccess.Entities.Truck RecipientTruck);
        public List<HopArrival> MapManuallyHopToHopArrival(List<PaPl.SKS.DataAccess.Entities.Hop> daoFutureHops);
        public bool PointIsInPolygon(Coordinate point, Coordinate[] polygon);
        public List<DataAccess.Entities.Hop> PredictFutureHops(DataAccess.Entities.Hop SenderTruck, DataAccess.Entities.Hop RecipientTruck);
        public List<DataAccess.Entities.Truck> GetAllTrucks();
        public string RandomString(int length);

    }
}
