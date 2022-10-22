using AutoMapper;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Newtonsoft.Json;
using PaPl.SKS.DataAccess.Entities;
using System.Diagnostics.CodeAnalysis;
using System.IO;
/// Noch Kommetare einfügen welcher Mapper für was da ist.

/// <summary>
///
/// </summary>
[ExcludeFromCodeCoverage]
public class AutoMapperProfile : Profile
{
    /// <summary>
    /// 
    /// </summary>
    public AutoMapperProfile()
    {
        //Hops
        //____BL to DTOs

        CreateMap<PaPl.SKS.BusinessLogic.Entities.GeoCoordinate, PaPl.SKS.Package.Services.DTOs.Models.GeoCoordinate>();
        CreateMap<PaPl.SKS.BusinessLogic.Entities.HopArrival, PaPl.SKS.Package.Services.DTOs.Models.HopArrival>()
            .IncludeAllDerived();
        CreateMap<PaPl.SKS.BusinessLogic.Entities.Hop, PaPl.SKS.Package.Services.DTOs.Models.Hop>()
            .IncludeAllDerived();
        CreateMap<PaPl.SKS.BusinessLogic.Entities.WarehouseNextHops, PaPl.SKS.Package.Services.DTOs.Models.WarehouseNextHops>()
          .IncludeAllDerived();
        CreateMap<PaPl.SKS.BusinessLogic.Entities.Truck, PaPl.SKS.Package.Services.DTOs.Models.Truck>()
             .IncludeAllDerived();
        
        CreateMap<PaPl.SKS.BusinessLogic.Entities.TransferWarehouse, PaPl.SKS.Package.Services.DTOs.Models.Transferwarehouse>()
              .IncludeAllDerived();
        
        CreateMap<PaPl.SKS.BusinessLogic.Entities.Warehouse, PaPl.SKS.Package.Services.DTOs.Models.Warehouse>()
            .IncludeAllDerived();
        

        

        //____Reverse
       CreateMap<PaPl.SKS.Package.Services.DTOs.Models.GeoCoordinate, PaPl.SKS.BusinessLogic.Entities.GeoCoordinate>();
        CreateMap<PaPl.SKS.Package.Services.DTOs.Models.Hop, PaPl.SKS.BusinessLogic.Entities.Hop>()
            .IncludeAllDerived();
        CreateMap<PaPl.SKS.Package.Services.DTOs.Models.WarehouseNextHops, PaPl.SKS.BusinessLogic.Entities.WarehouseNextHops>();

        CreateMap<PaPl.SKS.Package.Services.DTOs.Models.Warehouse, PaPl.SKS.BusinessLogic.Entities.Warehouse>()
            .IncludeAllDerived();


        CreateMap<PaPl.SKS.Package.Services.DTOs.Models.Truck, PaPl.SKS.BusinessLogic.Entities.Truck>();

        CreateMap<PaPl.SKS.Package.Services.DTOs.Models.Transferwarehouse, PaPl.SKS.BusinessLogic.Entities.TransferWarehouse>();

        //____BL to DAL
        CreateMap<PaPl.SKS.BusinessLogic.Entities.HopArrival, PaPl.SKS.DataAccess.Entities.HopArrival>()
            .IncludeAllDerived();
        CreateMap<PaPl.SKS.BusinessLogic.Entities.Hop, PaPl.SKS.DataAccess.Entities.Hop>().IncludeAllDerived()
        .ForMember(dest => dest.GeoLocation, opt => opt.MapFrom(src => new NetTopologySuite.Geometries.Point((double)src.LocationCoordinates.Lon, (double)src.LocationCoordinates.Lat) { SRID = 4326 }));

        CreateMap<PaPl.SKS.BusinessLogic.Entities.WarehouseNextHops, PaPl.SKS.DataAccess.Entities.WarehouseNextHops>()
            .IncludeAllDerived();

        CreateMap<PaPl.SKS.BusinessLogic.Entities.Warehouse, PaPl.SKS.DataAccess.Entities.Warehouse>()
            .IncludeAllDerived()
            .AfterMap((s, d) =>
            {
            foreach (var c in d.NextHops)
            {
                c.Hop.Parent = d;
            }
            });

        

       

        CreateMap<PaPl.SKS.BusinessLogic.Entities.Truck, PaPl.SKS.DataAccess.Entities.Truck>()
        .ForMember(dest => dest.RegionGeoJson, opt => opt.MapFrom<TruckToGeometryResolver>());

        CreateMap<PaPl.SKS.BusinessLogic.Entities.TransferWarehouse, PaPl.SKS.DataAccess.Entities.TransferWarehouse>()
        .ForMember(dest => dest.RegionGeoJson, opt => opt.MapFrom<TransferWarehouseToGeometryResolver>());
        //____Reverse
        CreateMap<PaPl.SKS.DataAccess.Entities.HopArrival, PaPl.SKS.BusinessLogic.Entities.HopArrival>()
            .ReverseMap();

        CreateMap<PaPl.SKS.DataAccess.Entities.Warehouse, PaPl.SKS.BusinessLogic.Entities.Warehouse>()
            .ReverseMap();

        CreateMap<PaPl.SKS.DataAccess.Entities.Hop, PaPl.SKS.BusinessLogic.Entities.Hop>()
        .ReverseMap();

        CreateMap<PaPl.SKS.DataAccess.Entities.WarehouseNextHops, PaPl.SKS.BusinessLogic.Entities.WarehouseNextHops>()
         .ReverseMap();

        CreateMap<PaPl.SKS.DataAccess.Entities.Truck, PaPl.SKS.BusinessLogic.Entities.Truck>()
        .ReverseMap();

        CreateMap<PaPl.SKS.DataAccess.Entities.TransferWarehouse, PaPl.SKS.BusinessLogic.Entities.TransferWarehouse>()
        .ReverseMap();


        //Parcel
        //____DTO to BL
        CreateMap<PaPl.SKS.Package.Services.DTOs.Models.Recipient, PaPl.SKS.BusinessLogic.Entities.Recipient>();

        CreateMap<PaPl.SKS.Package.Services.DTOs.Models.Parcel, PaPl.SKS.BusinessLogic.Entities.Parcel>();
        //____Reverse
        CreateMap<PaPl.SKS.BusinessLogic.Entities.Recipient, PaPl.SKS.Package.Services.DTOs.Models.Recipient>()
            .ReverseMap();

        CreateMap<PaPl.SKS.BusinessLogic.Entities.Parcel, PaPl.SKS.Package.Services.DTOs.Models.Parcel>()
            .ReverseMap();

        //____BL to DAL
        CreateMap<PaPl.SKS.BusinessLogic.Entities.Recipient, PaPl.SKS.DataAccess.Entities.Recipient>();

        CreateMap<PaPl.SKS.BusinessLogic.Entities.Parcel, PaPl.SKS.DataAccess.Entities.Parcel>();

        //____Reverse
        CreateMap<PaPl.SKS.DataAccess.Entities.Recipient, PaPl.SKS.BusinessLogic.Entities.Recipient>()
           .ReverseMap();

        CreateMap<PaPl.SKS.DataAccess.Entities.Parcel, PaPl.SKS.BusinessLogic.Entities.Parcel>()
            .IncludeAllDerived()
            .ReverseMap();

        CreateMap<PaPl.SKS.DataAccess.Entities.HopArrival, PaPl.SKS.Package.Services.DTOs.Models.HopArrival>()
            .IncludeAllDerived();

















    }
}
/// <summary>
/// 
/// </summary>
[ExcludeFromCodeCoverage]
public class TruckToGeometryResolver : IValueResolver<PaPl.SKS.BusinessLogic.Entities.Truck, Truck, Geometry>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    /// <param name="destMember"></param>
    /// <param name="context"></param>
    /// <returns></returns>

    public Geometry Resolve(PaPl.SKS.BusinessLogic.Entities.Truck source, Truck destination, Geometry destMember, ResolutionContext context)
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
    public Geometry Resolve(PaPl.SKS.BusinessLogic.Entities.TransferWarehouse source, TransferWarehouse destination, Geometry destMember, ResolutionContext context)
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

