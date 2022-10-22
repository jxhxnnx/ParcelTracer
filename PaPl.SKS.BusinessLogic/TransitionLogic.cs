using AutoMapper;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Newtonsoft.Json;
using PaPl.SKS.BusinessLogic.Entities;
using PaPl.SKS.BusinessLogic.Interfaces;
using PaPl.SKS.BusinessLogic.Interfaces.Exceptions;
using PaPl.SKS.BusinessLogic.Validator;
using PaPl.SKS.DataAccess.Interfaces.Exceptions;
using PaPl.SKS.DataAccess.Repository;
using PaPl.SKS.DataAccess.ServiceAgent;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace PaPl.SKS.BusinessLogic
{
    //Transfer an existing parcel into the system from the service of a logistics partner.

    public class TransitionLogic : ITransitionLogic
    {

        private IParcelRepository ParcelRepo;
        private ILogger<TransitionLogic> logger;
        private readonly IMapper mapper;
        private readonly IHopRepository hopRepo;
        private List<DataAccess.Entities.Hop> futureHops = new();


        /// <summary>
        /// Bekommt ein parcelRepo mit DI;
        /// ---> Fehler weil es den parcelRepo CTOR-Weg nicht nachvollziehen kann
        /// </summary>
        public TransitionLogic(IHopRepository _hopRepo, IParcelRepository _repo, ILogger<TransitionLogic> _logger, IMapper _mapper)
        {
            hopRepo = _hopRepo;
            ParcelRepo = _repo;
            logger = _logger;
            mapper = _mapper;
        }

        private static Random random = new Random();

        public string RandomString(int length)
        {
            logger.LogDebug("TransitionLogic RandomString started");
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public string TransitionParcel(Parcel body, string trackingId)
        {
            try
            {
                logger.LogDebug("TransitionLogic TransitionParcel started");
                //Validation of data
                TransitionParcelValidation(body);
                logger.LogDebug("TransitionLogic TransitionParcel validation of parcel");

                body.TrackingId = trackingId;
                logger.LogDebug("TransitionLogic TransitionParcel set tracking id to: " + body.TrackingId);

                var allTrucks = GetAllTrucks();
                logger.LogDebug("TransitionLogic TransitionParcel GetAllTrucks");

                ApiGeoEncoder apiGeoEncoder = new();
                Coordinate geoCoordinateRecipient = apiGeoEncoder.EncodeAddress(body.Recipient);
                Coordinate geoCoordinateSender = apiGeoEncoder.EncodeAddress(body.Sender);
                logger.LogDebug("TransitionLogic TransitionParcel encode addresses of sender and recipient");

                DataAccess.Entities.Truck SenderTruck = new();
                DataAccess.Entities.Truck RecipientTruck = new();

                body.Sender.Id = RandomString(9);
                body.Recipient.Id = RandomString(9);
                logger.LogDebug("TransitionLogic TransitionParcel set unique id of sender and recipient");

                GetPolygonToTruck(allTrucks, geoCoordinateRecipient, geoCoordinateSender, ref SenderTruck, ref RecipientTruck);
                logger.LogDebug("TransitionLogic TransitionParcel get polygon to truck");

                var daoFutureHops = PredictFutureHops(SenderTruck, RecipientTruck);
                List<HopArrival> futureHops = MapManuallyHopToHopArrival(daoFutureHops);
                logger.LogDebug("TransitionLogic TransitionParcel map manually hop to hop arrival");

                body.FutureHops = futureHops;
                logger.LogDebug("TransitionLogic TransitionParcel set future hops of parcel");

                //Set parcel state to “Pickup”
                body.State = Parcel.StateEnum.PickupEnum;
                logger.LogDebug("TransitionLogic TransitionParcel set state to: " + body.State);
                //Similar to Submit new parcel(1a) (except reusing existing tracking ID)

                DataAccess.Entities.Parcel daoParcel = mapper.Map<Parcel, DataAccess.Entities.Parcel>(body);
                logger.LogDebug("TransitionLogic TransitionParcel map to DAL-parcel");

                ParcelRepo.Create(daoParcel);
                logger.LogDebug("TransitionLogic TransitionParcel created");
            }
            catch (DataException ex)
            {
                logger.LogError(ex.Message);
                throw new LogicException(nameof(TransitionLogic),
                                        nameof(TransitionParcel),
                                        "An error occured while transporting parcel",
                                        ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw new LogicException(nameof(TransitionLogic),
                                        nameof(TransitionParcel),
                                        "An error occured while transporting parcel",
                                        ex);
            }
            // Transfer an existing parcel into the system from the service of a logistics partner.

            logger.LogDebug("TransitionLogic TransitionParcel Create parcel executed");

            if (body != null)
            {
                logger.LogDebug("SubmitParcelLogic SubmitParcel successful");
                return body.TrackingId;
            }
            else
            {
                logger.LogDebug("SubmitParcelLogic SubmitParcel failed due to null-Parcel");
                return null;
            }
        }
        [ExcludeFromCodeCoverage]
        private static void TransitionParcelValidation(Parcel body)
        {
            ParcelValidator parcelValidator = new();
            ParcelValidator validationRulesId = parcelValidator;
            ValidationResult IdResult = validationRulesId.Validate(body);
        }

        public List<HopArrival> MapManuallyHopToHopArrival(List<DataAccess.Entities.Hop> daoFutureHops)
        {
            logger.LogDebug("TransitionLogic MapManuallyHopToHopArrival started");
            List<HopArrival> futureHops = new();
            foreach (var hop in daoFutureHops)
            {
                HopArrival tempHop = new();
                tempHop.Code = hop.Code;
                tempHop.Description = hop.Description;
                futureHops.Add(tempHop);
            }

            return futureHops;
        }

        public void GetPolygonToTruck(List<DataAccess.Entities.Truck> allTrucks, Coordinate geoCoordinateRecipient, Coordinate geoCoordinateSender, ref DataAccess.Entities.Truck SenderTruck, ref DataAccess.Entities.Truck RecipientTruck)
        {
            logger.LogDebug("TransitionLogic GetPolygonToTruck started");
            foreach (var truck in allTrucks)
            {
                if (PointIsInPolygon(geoCoordinateSender, truck.RegionGeoJson.Coordinates))
                {
                    SenderTruck = truck;
                }

                if (PointIsInPolygon(geoCoordinateRecipient, truck.RegionGeoJson.Coordinates))
                {
                    RecipientTruck = truck;
                }
            }
        }

        /*
[ExcludeFromCodeCoverage]
private DataAccess.Entities.Parcel MapParcel(Parcel body)
{
   DataAccess.Entities.Parcel daoParcel;
   try
   {
       var amConfiguration = new MapperConfiguration(cfg =>
       {
           cfg.CreateMap<Recipient, DataAccess.Entities.Recipient>();
           cfg.CreateMap<Parcel, DataAccess.Entities.Parcel>();
       }
               );

       var mapper = amConfiguration.CreateMapper();
       daoParcel = mapper.Map<Parcel, DataAccess.Entities.Parcel>(body);
       logger.LogDebug("TransitionLogic TransitionParcel Mapping executed");
   }
   catch (AutoMapperMappingException ex)
   {
       logger.LogError(ex.Message);
       throw new BLMappingException(nameof(TransitionLogic),
                               nameof(TransitionParcel),
                               "An error occured while mapping parcel",
                               ex);
   }
   catch (AutoMapperConfigurationException ex)
   {
       logger.LogError(ex.Message);
       throw new BLMappingException(nameof(TransitionLogic),
                               nameof(TransitionParcel),
                               "An error occured while mapping parcel",
                               ex);
   }
   return daoParcel;
}
*/

        public List<DataAccess.Entities.Truck> GetAllTrucks()
        {
            logger.LogDebug("TransitionLogic GetAllTrucks started");
            var allHops = hopRepo.GetAllHops();
            List<DataAccess.Entities.Truck> allTrucks = new();



            foreach (var hop in allHops)
            {
                if (hop.HopType == "Truck")
                {
                    allTrucks.Add(hop as DataAccess.Entities.Truck);
                }
            }
            return allTrucks;
        }

        public List<DataAccess.Entities.Hop> PredictFutureHops(DataAccess.Entities.Hop SenderTruck, DataAccess.Entities.Hop RecipientTruck)
        {
            logger.LogDebug("TransitionLogic PredictFutureHops started");
            if (SenderTruck.Parent == RecipientTruck.Parent)
            {
                futureHops.Add(SenderTruck.Parent);
                return futureHops;
            }
            else
            {
                futureHops.Add(SenderTruck.Parent);
                futureHops.Add(RecipientTruck);
                return PredictFutureHops(SenderTruck.Parent, RecipientTruck.Parent);
            }

        }

        /*public GeoCoordinate[] ParseStringToPolygon(string coordinates)
        {
            var serializer = GeoJsonSerializer.Create();
            string converter = coordinates.Substring(29);
            using (var stringReader = new StringReader(converter))
            using (var jsonReader = new JsonTextReader(stringReader))
            {
                GeoCoordinate[] coordinateArray = serializer.Deserialize<GeoCoordinate[]>(jsonReader);
                return coordinateArray;
            }

        }*/
        public bool PointIsInPolygon(Coordinate point, Coordinate[] polygon)
        {
            logger.LogDebug("TransitionLogic PointIsInPolygon started");

            bool result = false;
            int j = polygon.Count() - 1;
            for (int i = 0; i < polygon.Count(); i++)
            {
                if (polygon[i].Y < point.Y && polygon[j].Y >= point.Y || polygon[j].Y < point.Y && polygon[i].Y >= point.Y)
                {
                    if (polygon[i].X + (point.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) * (polygon[j].X - polygon[i].X) < point.X)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
        }
    }
}
