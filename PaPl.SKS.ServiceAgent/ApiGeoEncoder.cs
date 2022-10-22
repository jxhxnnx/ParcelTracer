using NetTopologySuite.Geometries;
using Nominatim.API.Geocoders;
using Nominatim.API.Models;
using PaPl.SKS.BusinessLogic.Entities;
using PaPl.SKS.DataAccess.ServiceAgent.Interfaces;
using PaPl.SKS.Package.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaPl.SKS.DataAccess.ServiceAgent
{
    public class ApiGeoEncoder : IGeoEncodingAgent
    {
        public Coordinate EncodeAddress(Recipient address)
        {
            try
            {
                var x = new ForwardGeocoder();

                var r = x.Geocode(new ForwardGeocodeRequest
                {
                    StreetAddress = address.Street,
                    City = address.City,
                    PostalCode = address.PostalCode,
                    Country = address.Country,

                    BreakdownAddressElements = true,
                    ShowExtraTags = true,
                    ShowAlternativeNames = true,
                    ShowGeoJSON = true
                });

                r.Wait();
                //Get best rated API response
                GeocodeResponse response = r.Result[0];

                //Map response to GeoCoordinate
                Coordinate re = new();

                re.X = response.Longitude;
                re.Y = response.Latitude;

                return re;
            } catch (Exception ex)
            {
                throw new ServiceException(nameof(ApiGeoEncoder),
                                        nameof(EncodeAddress),
                                        "An unknown error occured while encoding address",
                                        ex);
               
            }
            
        }
    }
}
