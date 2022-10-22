using AutoMapper;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaPl.SKS.Package.Services.Test
{
    [ExcludeFromCodeCoverage]
    public class TestMapper : Profile
    {
        public TestMapper()
        {
            
        }
        public IMapper GetTestMapper()
        {
            var configuration = new MapperConfiguration(
                cfg =>
                {
                    cfg.CreateMap<PaPl.SKS.BusinessLogic.Entities.GeoCoordinate, PaPl.SKS.Package.Services.DTOs.Models.GeoCoordinate>()
                    .ReverseMap();
                    
                    cfg.CreateMap<PaPl.SKS.BusinessLogic.Entities.HopArrival, PaPl.SKS.Package.Services.DTOs.Models.HopArrival>()
                        .IncludeAllDerived()
                        .ReverseMap();
                    cfg.CreateMap<PaPl.SKS.BusinessLogic.Entities.Hop, PaPl.SKS.Package.Services.DTOs.Models.Hop>()
                        .IncludeAllDerived()
                        .ReverseMap();
                    cfg.CreateMap<PaPl.SKS.BusinessLogic.Entities.WarehouseNextHops, PaPl.SKS.Package.Services.DTOs.Models.WarehouseNextHops>()
                      .IncludeAllDerived()
                      .ReverseMap();
                    cfg.CreateMap<PaPl.SKS.BusinessLogic.Entities.Truck, PaPl.SKS.Package.Services.DTOs.Models.Truck>()
                         .IncludeAllDerived()
                         .ReverseMap();

                    cfg.CreateMap<PaPl.SKS.BusinessLogic.Entities.TransferWarehouse, PaPl.SKS.Package.Services.DTOs.Models.Transferwarehouse>()
                          .IncludeAllDerived()
                          .ReverseMap();

                    cfg.CreateMap<PaPl.SKS.BusinessLogic.Entities.Warehouse, PaPl.SKS.Package.Services.DTOs.Models.Warehouse>()
                        .IncludeAllDerived()
                        .ReverseMap();

                    //____BL to DAL
                    cfg.CreateMap<PaPl.SKS.BusinessLogic.Entities.HopArrival, PaPl.SKS.DataAccess.Entities.HopArrival>()
                        .IncludeAllDerived();
                    cfg.CreateMap<PaPl.SKS.BusinessLogic.Entities.Hop, PaPl.SKS.DataAccess.Entities.Hop>().IncludeAllDerived()
                    .ForMember(dest => dest.GeoLocation, opt => opt.MapFrom(src => new NetTopologySuite.Geometries.Point((double)src.LocationCoordinates.Lon, (double)src.LocationCoordinates.Lat) { SRID = 4326 }));

                    cfg.CreateMap<PaPl.SKS.BusinessLogic.Entities.WarehouseNextHops, PaPl.SKS.DataAccess.Entities.WarehouseNextHops>()
                        .IncludeAllDerived();

                    cfg.CreateMap<PaPl.SKS.BusinessLogic.Entities.Warehouse, PaPl.SKS.DataAccess.Entities.Warehouse>()
                        .IncludeAllDerived()
                        .AfterMap((s, d) =>
                        {
                            foreach (var c in d.NextHops)
                            {
                                c.Hop.Parent = d;
                            }
                        });

                    cfg.CreateMap<PaPl.SKS.BusinessLogic.Entities.Truck, PaPl.SKS.DataAccess.Entities.Truck>()
                    .ForMember(dest => dest.RegionGeoJson, opt => opt.MapFrom<TruckToGeometryResolver>());

                    cfg.CreateMap<PaPl.SKS.BusinessLogic.Entities.TransferWarehouse, PaPl.SKS.DataAccess.Entities.TransferWarehouse>()
                    .ForMember(dest => dest.RegionGeoJson, opt => opt.MapFrom<TransferWarehouseToGeometryResolver>());
                    //____Reverse
                    cfg.CreateMap<PaPl.SKS.DataAccess.Entities.HopArrival, PaPl.SKS.BusinessLogic.Entities.HopArrival>()
                        .ReverseMap();

                    cfg.CreateMap<PaPl.SKS.DataAccess.Entities.Warehouse, PaPl.SKS.BusinessLogic.Entities.Warehouse>()
                        .ReverseMap();

                    cfg.CreateMap<PaPl.SKS.DataAccess.Entities.Hop, PaPl.SKS.BusinessLogic.Entities.Hop>()
                    .ReverseMap();

                    cfg.CreateMap<PaPl.SKS.DataAccess.Entities.WarehouseNextHops, PaPl.SKS.BusinessLogic.Entities.WarehouseNextHops>()
                     .ReverseMap();

                    cfg.CreateMap<PaPl.SKS.DataAccess.Entities.Truck, PaPl.SKS.BusinessLogic.Entities.Truck>()
                    .ReverseMap();

                    cfg.CreateMap<PaPl.SKS.DataAccess.Entities.TransferWarehouse, PaPl.SKS.BusinessLogic.Entities.TransferWarehouse>()
                    .ReverseMap();


                    //Parcel
                    //____DTO to BL
                    cfg.CreateMap<PaPl.SKS.Package.Services.DTOs.Models.Recipient, PaPl.SKS.BusinessLogic.Entities.Recipient>();

                    cfg.CreateMap<PaPl.SKS.Package.Services.DTOs.Models.Parcel, PaPl.SKS.BusinessLogic.Entities.Parcel>();
                    //____Reverse
                    cfg.CreateMap<PaPl.SKS.BusinessLogic.Entities.Recipient, PaPl.SKS.Package.Services.DTOs.Models.Recipient>()
                        .ReverseMap();

                    cfg.CreateMap<PaPl.SKS.BusinessLogic.Entities.Parcel, PaPl.SKS.Package.Services.DTOs.Models.Parcel>()
                        .ReverseMap();

                    //____BL to DAL
                    cfg.CreateMap<PaPl.SKS.BusinessLogic.Entities.Recipient, PaPl.SKS.DataAccess.Entities.Recipient>();

                    cfg.CreateMap<PaPl.SKS.BusinessLogic.Entities.Parcel, PaPl.SKS.DataAccess.Entities.Parcel>();

                    //____Reverse
                    cfg.CreateMap<PaPl.SKS.DataAccess.Entities.Recipient, PaPl.SKS.BusinessLogic.Entities.Recipient>()
                       .ReverseMap();

                    cfg.CreateMap<PaPl.SKS.DataAccess.Entities.Parcel, PaPl.SKS.BusinessLogic.Entities.Parcel>()
                        .IncludeAllDerived()
                        .ReverseMap();

                    cfg.CreateMap<PaPl.SKS.DataAccess.Entities.HopArrival, PaPl.SKS.Package.Services.DTOs.Models.HopArrival>()
                        .IncludeAllDerived();

                });
            IMapper mapper = configuration.CreateMapper();
            return mapper;
        }
    }
}

[ExcludeFromCodeCoverage]
public class TruckToGeometryResolver : IValueResolver<PaPl.SKS.BusinessLogic.Entities.Truck, PaPl.SKS.DataAccess.Entities.Truck, Geometry>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    /// <param name="destMember"></param>
    /// <param name="context"></param>
    /// <returns></returns>

    public Geometry Resolve(PaPl.SKS.BusinessLogic.Entities.Truck source, PaPl.SKS.DataAccess.Entities.Truck destination, Geometry destMember, ResolutionContext context)
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
/// <summary>
/// 
/// </summary>
[ExcludeFromCodeCoverage]
public class TransferWarehouseToGeometryResolver : IValueResolver<PaPl.SKS.BusinessLogic.Entities.TransferWarehouse, PaPl.SKS.DataAccess.Entities.TransferWarehouse, Geometry>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    /// <param name="destMember"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    [ExcludeFromCodeCoverage]
    public Geometry Resolve(PaPl.SKS.BusinessLogic.Entities.TransferWarehouse source, PaPl.SKS.DataAccess.Entities.TransferWarehouse destination, Geometry destMember, ResolutionContext context)
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
