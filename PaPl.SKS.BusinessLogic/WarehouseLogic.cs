using AutoMapper;
using PaPl.SKS.BusinessLogic.Entities;
using PaPl.SKS.BusinessLogic.Interfaces;
using System;
using System.Collections.Generic;
using PaPl.SKS.DataAccess.Repository;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PaPl.SKS.DataAccess.Interfaces.Exceptions;
using PaPl.SKS.BusinessLogic.Interfaces.Exceptions;
using PaPl.SKS.BusinessLogic.Validator;
using FluentValidation.Results;
using System.Diagnostics.CodeAnalysis;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using System.IO;
using NetTopologySuite.IO;
using System.Diagnostics;

namespace PaPl.SKS.BusinessLogic
{
    public class WareHouseLogic : IWarehouseLogic
    {
        private IHopRepository repo;
        private ILogger<WareHouseLogic> logger;
        private IMapper mapper;

        public WareHouseLogic(IHopRepository _repo, ILogger<WareHouseLogic> _logger, IMapper _mapper)
        {
            repo = _repo;
            logger = _logger;
            mapper = _mapper;
        }

        /// Exports the hierarchy of Warehouse and Truck objects. 
        /// Get WH + ALL WarehouseNextHops
        /// Related object one to many 
        public Hop ExportWarehouse()
        {
            try
            {
                logger.LogInformation("WarehouseLogic ExportWarehouse started");
                DataAccess.Entities.Hop dataHops = repo.GetHops();
                Hop hop = ReverseMapHop(dataHops);
                return hop;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw new LogicException(nameof(WareHouseLogic),
                                        nameof(ExportWarehouse),
                                        $"An unknown error occured while exporting warehouse hierarchy",
                                        ex);
            }
            
        }
        //Get hop level 0
        //Lade next hop objekte
        //rekursive funktion ->
        // -> solange hop warehouse ist: lade next hops vom hop


        /// Get a certain warehouse or truck by code
        public Warehouse GetWarehouse(string code)
        {
            DataAccess.Entities.Hop daoHop;
            BusinessLogic.Entities.Hop blHop;
            try
            {
                logger.LogInformation("WarehouseLogic GetWarehouse started");


                //----validating code
                GetWarehouseValidation(code);
                logger.LogInformation("WarehouseLogic GetWarehouse valid hop");
                //Get single hop from DB and return it.

                daoHop = repo.GetHopByCode(code);
                logger.LogInformation("WarehouseLogic GetWarehouse get daoHop");

                blHop = mapper.Map<DataAccess.Entities.Hop, BusinessLogic.Entities.Hop>(daoHop);

                Debug.WriteLine(blHop.LocationCoordinates);
                if (daoHop == null)
                {
                    throw new BLDataNotFoundException(nameof(WareHouseLogic),
                                        nameof(GetWarehouse),
                                        $"An error occured while getting warehouse with code {code}, data not found");
                }
                logger.LogDebug("WarehouseLogic GetWarehouse GetHopByCode code executed");
            }
            catch (DataException ex)
            {
                logger.LogError(ex.Message);
                throw new LogicException(nameof(WareHouseLogic),
                                        nameof(GetWarehouse),
                                        $"An error occured while getting warehouse with code {code}",
                                        ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw new LogicException(nameof(WareHouseLogic),
                                        nameof(GetWarehouse),
                                        $"An unknown error occured while getting warehouse with code {code}",
                                        ex);
            }

            if (code == null)
            {
                throw new LogicException(nameof(WareHouseLogic),
                                        nameof(GetWarehouse),
                                        $"An known error occured while getting warehouse with code {code}");
            }
            return (Warehouse)blHop;
        }
        [ExcludeFromCodeCoverage]
        private void GetWarehouseValidation(string code)
        {
            Hop validationHop = new();
            validationHop.Code = code;
            logger.LogInformation("WarehouseLogic GetWarehouse valid hop code");

            CodeValidator codeValidator = new();
            CodeValidator validationRulesCode = codeValidator;
            ValidationResult CodeResult = validationRulesCode.Validate(validationHop);
        }

        //Welche Param werden übergeben?! ---->Swagger
        /// Imports a hierarchy of Warehouse and Truck objects. 
        public int ImportWarehouse(Hop warehouse)
        {
            try
            {
                //Validation of data
                logger.LogInformation("WarehouseLogic ImportWarehouse started");
                ImportWarehouseValidation(warehouse);
                logger.LogInformation("WarehouseLogic ImportWarehouse warehouse valid");

                //Clear the existing DB(the entire one)
                repo.BigRedButton();
                logger.LogInformation("WarehouseLogic ImportWarehouse clear existing db");

                //Write hop hierarchy to DB

                logger.LogDebug("WarehouseLogic ImportWarehouse started");

                //----Map to dal
                DataAccess.Entities.Hop daoHop = MapHop(warehouse);

                //----Save Hop
                repo.Create(daoHop);
                logger.LogInformation("WarehouseLogic ImportWarehouse save hop");

                //----Return status code
            }
            catch (DataException ex)
            {
                logger.LogError(ex.Message);
                throw new LogicException(nameof(WareHouseLogic),
                                        nameof(ImportWarehouse),
                                        $"An error occured while importing warehouse with code {warehouse.Code}",
                                        ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw new LogicException(nameof(WareHouseLogic),
                                        nameof(ImportWarehouse),
                                        $"An unknown error occured while importing warehouse with code {warehouse.Code}",
                                        ex);
            }
            return 200;
        }
        [ExcludeFromCodeCoverage]
        private static void ImportWarehouseValidation(Hop warehouse)
        {
            HopValidator validationRules = new();

            ValidationResult result = validationRules.Validate(warehouse);
        }

        public Hop GetHopByCode(string code)
        {
            DataAccess.Entities.Hop daoHop;
            BusinessLogic.Entities.Hop blHop;
          
            try
            {
                logger.LogInformation("WarehouseLogic GetHopByCode started");
                //Set

                //----validating code
                GetHopByCodeValidation(code);
                logger.LogInformation("WarehouseLogic GetHopByCode valid hop");
                //Get single hop from DB and return it.

                daoHop = repo.GetHopByCode(code);
                logger.LogInformation("WarehouseLogic GetHopByCode get daoHop");
                blHop = mapper.Map<DataAccess.Entities.Hop, BusinessLogic.Entities.Hop>(daoHop);
                blHop.LocationCoordinates = new GeoCoordinate();
                blHop.LocationCoordinates.Lat = daoHop.GeoLocation.X;
                blHop.LocationCoordinates.Lon = daoHop.GeoLocation.X;
                if (daoHop == null)
                {
                    throw new BLDataNotFoundException(nameof(WareHouseLogic),
                                        nameof(GetWarehouse),
                                        $"An error occured while getting hop with code {code}, data not found");
                }
                logger.LogDebug("WarehouseLogic GetHopByCode GetHopByCode code executed");
            }
            catch (DataException ex)
            {
                logger.LogError(ex.Message);
                throw new LogicException(nameof(WareHouseLogic),
                                        nameof(GetWarehouse),
                                        $"An error occured while getting hop with code {code}",
                                        ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw new LogicException(nameof(WareHouseLogic),
                                        nameof(GetWarehouse),
                                        $"An unknown error occured while getting hop with code {code}",
                                        ex);
            }

            if (code == null)
            {
                throw new LogicException(nameof(WareHouseLogic),
                                        nameof(GetWarehouse),
                                        $"An known error occured while getting hop with code {code}");
            }
            return blHop;
        }
        [ExcludeFromCodeCoverage]
        private void GetHopByCodeValidation(string code)
        {
            Hop validationHop = new();
            validationHop.Code = code;
            logger.LogInformation("WarehouseLogic GetHopByCode valid hop code");

            CodeValidator codeValidator = new();
            CodeValidator validationRulesCode = codeValidator;
            ValidationResult CodeResult = validationRulesCode.Validate(validationHop);
        }

        [ExcludeFromCodeCoverage]
        private DataAccess.Entities.Hop MapHop(Hop warehouse)
        {
            logger.LogInformation("WarehouseLogic MapHop started");
            DataAccess.Entities.Hop daoHop;
            try
            {
                var amConfiguration = new MapperConfiguration(cfg =>
                {
                    //Wir haben alle Rules schon mal jetzt für das Profile später definiert
                    cfg.CreateMap<BusinessLogic.Entities.Hop, DataAccess.Entities.Hop>()
                        .ForMember(dest => dest.GeoLocation, opt => opt.MapFrom(src => new NetTopologySuite.Geometries.Point((double)src.LocationCoordinates.Lon, (double)src.LocationCoordinates.Lat) { SRID = 4326 }))
                        .IncludeAllDerived();

                    cfg.CreateMap<BusinessLogic.Entities.WarehouseNextHops, DataAccess.Entities.WarehouseNextHops>();

                    cfg.CreateMap<BusinessLogic.Entities.Truck, DataAccess.Entities.Truck>()
                    .ForMember(dest => dest.RegionGeoJson, opt => opt.MapFrom<TruckToGeometryResolver>());

                    cfg.CreateMap<BusinessLogic.Entities.TransferWarehouse, DataAccess.Entities.TransferWarehouse>()
                    .ForMember(dest => dest.RegionGeoJson, opt => opt.MapFrom<TransferWarehouseToGeometryResolver>());
                    /*
                    cfg.CreateMap<DataAccess.Entities.Hop, Hop>()
                    .ReverseMap();

                    cfg.CreateMap<DataAccess.Entities.Truck, Truck>()
                    .ReverseMap();

                    cfg.CreateMap<DataAccess.Entities.TransferWarehouse, TransferWarehouse>()
                    .ReverseMap();
                    */
                    cfg.CreateMap<BusinessLogic.Entities.Warehouse, DataAccess.Entities.Warehouse>()
                    .AfterMap((s, d) =>
                    {
                        foreach (var c in d.NextHops)
                        {
                            c.Hop.Parent = d;
                        }
                    });
                }
                );

                var mapper = amConfiguration.CreateMapper();
                daoHop = mapper.Map<BusinessLogic.Entities.Hop, DataAccess.Entities.Hop>(warehouse);

                logger.LogDebug("TrackParcelLogic TrackParcel Mapping executed");
            }
            catch (AutoMapperMappingException ex)
            {
                logger.LogError(ex.Message);
                throw new BLMappingException(nameof(WareHouseLogic),
                                        nameof(ImportWarehouse),
                                        "An error occured while mapping parcel",
                                        ex);
            }
            catch (AutoMapperConfigurationException ex)
            {
                logger.LogError(ex.Message);
                throw new BLMappingException(nameof(WareHouseLogic),
                                        nameof(ImportWarehouse),
                                        "An error occured while mapping parcel",
                                        ex);
            }

            return daoHop;
        }

        [ExcludeFromCodeCoverage]
        private Hop ReverseMapHop(DataAccess.Entities.Hop warehouse)
        {
            logger.LogInformation("WarehouseLogic ReverseMapHop started");
            Hop hop;
            try
            {
                var amConfiguration = new MapperConfiguration(cfg =>
                {
                    //Wir haben alle Rules schon mal jetzt für das Profile später definiert
                    cfg.CreateMap<BusinessLogic.Entities.Hop, DataAccess.Entities.Hop>()
                        .ForMember(dest => dest.GeoLocation, opt => opt.MapFrom(src => new NetTopologySuite.Geometries.Point((double)src.LocationCoordinates.Lon, (double)src.LocationCoordinates.Lat) { SRID = 4326 }))
                        .IncludeAllDerived();

                    cfg.CreateMap<BusinessLogic.Entities.Warehouse, DataAccess.Entities.Warehouse>();

                    cfg.CreateMap<BusinessLogic.Entities.WarehouseNextHops, DataAccess.Entities.WarehouseNextHops>();

                    cfg.CreateMap<BusinessLogic.Entities.Truck, DataAccess.Entities.Truck>()
                    .ForMember(dest => dest.RegionGeoJson, opt => opt.MapFrom<TruckToGeometryResolver>());

                    cfg.CreateMap<BusinessLogic.Entities.TransferWarehouse, DataAccess.Entities.TransferWarehouse>()
                    .ForMember(dest => dest.RegionGeoJson, opt => opt.MapFrom<TransferWarehouseToGeometryResolver>());
                    
                    cfg.CreateMap<BusinessLogic.Entities.Warehouse, DataAccess.Entities.Warehouse>()
                    .AfterMap((s, d) =>
                    {
                        foreach (var c in d.NextHops)
                        {
                            c.Hop.Parent = d;
                        }
                    });

                    cfg.CreateMap<DataAccess.Entities.Hop, Hop>()
                    .IncludeAllDerived()
                    .ReverseMap();

                    cfg.CreateMap<DataAccess.Entities.Warehouse, Warehouse>()
                    .ReverseMap();

                    cfg.CreateMap<DataAccess.Entities.WarehouseNextHops, WarehouseNextHops>()
                    .ReverseMap();

                    cfg.CreateMap<DataAccess.Entities.Truck, Truck>()
                    .ReverseMap();

                    cfg.CreateMap<DataAccess.Entities.TransferWarehouse, TransferWarehouse>()
                    .ReverseMap();

                    
                }
                );

                var mapper = amConfiguration.CreateMapper();
                hop = mapper.Map<DataAccess.Entities.Hop, BusinessLogic.Entities.Hop>(warehouse);

                logger.LogDebug("TrackParcelLogic TrackParcel Mapping executed");
            }
            catch (AutoMapperMappingException ex)
            {
                logger.LogError(ex.Message);
                throw new BLMappingException(nameof(WareHouseLogic),
                                        nameof(ImportWarehouse),
                                        "An error occured while mapping parcel",
                                        ex);
            }
            catch (AutoMapperConfigurationException ex)
            {
                logger.LogError(ex.Message);
                throw new BLMappingException(nameof(WareHouseLogic),
                                        nameof(ImportWarehouse),
                                        "An error occured while mapping parcel",
                                        ex);
            }

            return hop;
        }

    }


    [ExcludeFromCodeCoverage]
    public class TruckToGeometryResolver : IValueResolver<PaPl.SKS.BusinessLogic.Entities.Truck, PaPl.SKS.DataAccess.Entities.Truck, Geometry>
    {
        public Geometry Resolve(Truck source, PaPl.SKS.DataAccess.Entities.Truck destination, Geometry destMember, ResolutionContext context)
        {
            var serializer = GeoJsonSerializer.Create();
            string converter = source.RegionGeoJson.Substring(29);
            using (var stringReader = new StringReader(converter))
            using (var jsonReader = new JsonTextReader(stringReader))
            {
                Geometry geometry = serializer.Deserialize<Geometry>(jsonReader);
                return geometry;
            }
        }
    }
    [ExcludeFromCodeCoverage]
    public class TransferWarehouseToGeometryResolver : IValueResolver<PaPl.SKS.BusinessLogic.Entities.TransferWarehouse, PaPl.SKS.DataAccess.Entities.TransferWarehouse, Geometry>
    {
        public Geometry Resolve(TransferWarehouse source, PaPl.SKS.DataAccess.Entities.TransferWarehouse destination, Geometry destMember, ResolutionContext context)
        {
            var serializer = GeoJsonSerializer.Create();
            string converter = source.RegionGeoJson.Substring(29);
            using (var stringReader = new StringReader(converter))
            using (var jsonReader = new JsonTextReader(stringReader))
            {
                Geometry geometry = serializer.Deserialize<Geometry>(jsonReader);
                return geometry;
            }
        }
    }


}





