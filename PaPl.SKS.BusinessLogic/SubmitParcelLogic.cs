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
using System.Text;
using System.Threading.Tasks;

namespace PaPl.SKS.BusinessLogic
{
    public class SubmitParcelLogic : ISubmitParcelLogic
    {
        private readonly IHopRepository hopRepo;
        private IParcelRepository ParcelRepo;
        private IHopRepository whRepo;
        private List<DataAccess.Entities.Hop> futureHops = new();

        private ILogger<SubmitParcelLogic> logger;
        private readonly IMapper mapper;

        public SubmitParcelLogic(IHopRepository _HopRepo, IParcelRepository _ParcelRepo, ILogger<SubmitParcelLogic> _logger, IMapper _mapper)
        {
            hopRepo = _HopRepo;
            this.ParcelRepo = _ParcelRepo;
            logger = _logger;
            mapper = _mapper;
        }
        private static Random random = new Random();

        public string RandomString(int length)
        {
            logger.LogDebug("SubmitParcelLogic RandomString started");
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public string SubmitParcel(Parcel body)
        {
            try
            {
                if (body == null)
                {
                    return "404";
                }
                logger.LogDebug("SubmitParcelLogic SubmitParcel started");
                //Validate Parcel
                SubmitParcelValidation(body);
                logger.LogDebug("SubmitParcelLogic SubmitParcel Validate parcel executed");

                string uniqueId = RandomString(9);
                body.TrackingId = uniqueId;
                logger.LogDebug("SubmitParcelLogic SubmitParcel set unique ID");

                var allTrucks = GetAllTrucks();
                logger.LogDebug("SubmitParcelLogic SubmitParcel trucks count: " + allTrucks.Count);
                if (allTrucks == null)
                {
                    return null;
                }

                ApiGeoEncoder apiGeoEncoder = new();
                Coordinate geoCoordinateRecipient = apiGeoEncoder.EncodeAddress(body.Recipient);
                Coordinate geoCoordinateSender = apiGeoEncoder.EncodeAddress(body.Sender);
                logger.LogDebug("SubmitParcelLogic SubmitParcel sender and recipient address to coordinates");

                DataAccess.Entities.Truck SenderTruck = new();
                DataAccess.Entities.Truck RecipientTruck = new();

                body.Sender.Id = RandomString(9);
                body.Recipient.Id = RandomString(9);
                logger.LogDebug("SubmitParcelLogic SubmitParcel set ID for Sender and Recipient");

                GetPolygonToTruck(allTrucks, geoCoordinateRecipient, geoCoordinateSender, ref SenderTruck, ref RecipientTruck);
                logger.LogDebug("SubmitParcelLogic SubmitParcel get Polygon to Trucks");
                body.VisitedHops = new List<HopArrival>()
                {
                    SetSenderTruckToVisitedHop(SenderTruck)
                };
                var daoFutureHops = PredictFutureHops(SenderTruck, RecipientTruck);
                if (daoFutureHops == null)
                {
                    throw new LogicException(nameof(SubmitParcelLogic),
                                        nameof(SubmitParcel),
                                        "No Future Hops due to sendertruck or recipientruck is null");
                }
                List<HopArrival> futureHops = new();
                MapManuallyHopToHopArrival(daoFutureHops, futureHops);
                body.FutureHops = futureHops;

                logger.LogDebug("SubmitParcelLogic SubmitParcel predict future hops");

                body.State = Parcel.StateEnum.PickupEnum;
                logger.LogDebug("SubmitParcelLogic SubmitParcel change state to: " + body.State);


                DataAccess.Entities.Parcel daoParcel = mapper.Map<Parcel, DataAccess.Entities.Parcel>(body);
                logger.LogDebug("SubmitParcelLogic SubmitParcel mapped to DAL-Object");
                ParcelRepo.Create(daoParcel);
                logger.LogDebug("SubmitParcelLogic SubmitParcel CreateParcel executed");
                return daoParcel.TrackingId;
            }
            catch (DataException ex)
            {
                logger.LogError(ex.Message);
                throw new LogicException(nameof(SubmitParcelLogic),
                                        nameof(SubmitParcel),
                                        "An error occured while submitting parcel",
                                        ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw new LogicException(nameof(SubmitParcelLogic),
                                        nameof(SubmitParcel),
                                        "An error occured while submitting parcel",
                                        ex);
            }
            //Return Tracking ID



        }

        public HopArrival SetSenderTruckToVisitedHop(DataAccess.Entities.Truck SenderTruck)
        {
            logger.LogDebug("SubmitParcelLogic SetSenderTruckToVisitedHop started");
            var visitedHops = new HopArrival()
            {
                Code = SenderTruck.Code,
                DateTime = DateTime.Now,
                Description = SenderTruck.Description
            };
            return visitedHops;
        }

        [ExcludeFromCodeCoverage]
        private static void SubmitParcelValidation(Parcel body)
        {
            ParcelValidator parcelValidator = new();
            ParcelValidator validationRulesId = parcelValidator;
            ValidationResult IdResult = validationRulesId.Validate(body);
        }



        public void MapManuallyHopToHopArrival(List<DataAccess.Entities.Hop> daoFutureHops, List<HopArrival> futureHops)
        {
            logger.LogDebug("SubmitParcelLogic MapManuallyHopToHopArrival started");
            foreach (var hop in daoFutureHops)
            {
                HopArrival tempHop = new();
                tempHop.Code = hop.Code;
                tempHop.Description = hop.Description;
                futureHops.Add(tempHop);
            }
        }

        public void GetPolygonToTruck(List<DataAccess.Entities.Truck> allTrucks, Coordinate geoCoordinateRecipient, Coordinate geoCoordinateSender, ref DataAccess.Entities.Truck SenderTruck, ref DataAccess.Entities.Truck RecipientTruck)
        {
            logger.LogDebug("SubmitParcelLogic GetPolygonToTruck started");
            if (allTrucks != null && SenderTruck != null && RecipientTruck != null)
            {
                foreach (var truck in allTrucks)
                {
                    if (PointIsInPolygon(geoCoordinateSender, truck.RegionGeoJson.Coordinates))
                    {
                        SenderTruck = truck;
                        logger.LogDebug("GetPolygonToTruck Sendertruck with code " + truck.Code);
                    }

                    if (PointIsInPolygon(geoCoordinateRecipient, truck.RegionGeoJson.Coordinates))
                    {
                        RecipientTruck = truck;
                        logger.LogDebug("GetPolygonToTruck Recipienttruck with code " + truck.Code);
                    }
                }
            }

        }

        public List<DataAccess.Entities.Truck> GetAllTrucks()
        {
            logger.LogDebug("SubmitParcelLogic GetAllTrucks started");
            var allHops = hopRepo.GetAllHops();
            List<DataAccess.Entities.Truck> allTrucks = new();
            if (allHops != null)
            {
                foreach (var hop in allHops)
                {
                    if (hop.HopType == "Truck")
                    {
                        allTrucks.Add(hop as DataAccess.Entities.Truck);
                    }
                }
            }

            return allTrucks;
        }

        public List<DataAccess.Entities.Hop> PredictFutureHops(DataAccess.Entities.Hop SenderTruck, DataAccess.Entities.Hop RecipientTruck)
        {
            logger.LogDebug("SubmitParcelLogic PredictFutureHops started");
            if (SenderTruck != null && RecipientTruck != null)
            {
                if (SenderTruck.Parent == RecipientTruck.Parent)
                {
                    futureHops.Add(SenderTruck.Parent);
                    futureHops.Add(RecipientTruck);
                    return futureHops;
                }
                else
                {
                    futureHops.Add(SenderTruck.Parent);
                    futureHops.Add(RecipientTruck);
                    return PredictFutureHops(SenderTruck.Parent, RecipientTruck.Parent);
                }
            }
            else return null;


        }

        /*
[ExcludeFromCodeCoverage]
{
   DataAccess.Entities.Parcel daoParcel;
   try
   {
       // Submit a new parcel to the logistics service.
       var amConfiguration = new MapperConfiguration(cfg =>
       {
           cfg.CreateMap<Recipient, DataAccess.Entities.Recipient>();
           cfg.CreateMap<Parcel, DataAccess.Entities.Parcel>();
       }
       );

       var mapper = amConfiguration.CreateMapper();
       daoParcel = mapper.Map<Parcel, DataAccess.Entities.Parcel>(body);
       logger.LogDebug("SubmitParcelLogic SubmitParcel Mapping executed");
   }
   catch (AutoMapperMappingException ex)
   {
       logger.LogError(ex.Message);
       throw new BLMappingException(nameof(SubmitParcelLogic),
                               nameof(SubmitParcel),
                               "An error occured while mapping parcel",
                               ex);
   }
   catch (AutoMapperConfigurationException ex)
   {
       logger.LogError(ex.Message);
       throw new BLMappingException(nameof(SubmitParcelLogic),
                               nameof(SubmitParcel),
                               "An error occured while mapping parcel",
                               ex);
   }


   return daoParcel;
}

        private GeoCoordinate[] ParseStringToPolygon(string coordinates)
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
            logger.LogDebug("SubmitParcelLogic PointIsInPolygon started");
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
