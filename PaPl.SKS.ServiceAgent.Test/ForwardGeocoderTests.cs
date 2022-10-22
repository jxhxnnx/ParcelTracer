using Nominatim.API.Geocoders;
using Nominatim.API.Models;
using NUnit.Framework;
using System;

namespace Nominatim.API.Tests
{
    public class GeocoderTests
    {
        /// <summary>
        /// First test from documentation to see if it works
        /// </summary>
        [Test]
        public void TestSuccessfulForwardGeocode()
        {
            var x = new ForwardGeocoder();

            var r = x.Geocode(new ForwardGeocodeRequest
            {
                /*
                queryString = "1600 Pennsylvania Avenue, Washington, DC",
                */
                StreetAddress = "Pennsylvania Avenue",
                City = "Washington",
                PostalCode = "1600",

                BreakdownAddressElements = true,
                ShowExtraTags = true,
                ShowAlternativeNames = true,
                ShowGeoJSON = true
            });
            r.Wait();
            GeocodeResponse response = r.Result[1];
            Console.WriteLine(response.Latitude);
            Assert.IsTrue(r.Result.Length > 0);
        }

        [Test]
        public void TestSuccessfulReverseGeocodeBuilding()
        {
            var y = new ReverseGeocoder();

            var r2 = y.ReverseGeocode(new ReverseGeocodeRequest
            {
                Longitude = -77.0365298,
                Latitude = 38.8976763,

                BreakdownAddressElements = true,
                ShowExtraTags = true,
                ShowAlternativeNames = true,
                ShowGeoJSON = true
            });
            r2.Wait();

            Assert.IsTrue(r2.Result.PlaceID > 0);
        }


        [Test]
        public void TestSuccessfulReverseGeocodeRoad()
        {
            var z = new ReverseGeocoder();

            var r3 = z.ReverseGeocode(new ReverseGeocodeRequest
            {
                Longitude = -58.7051622809683,
                Latitude = -34.440723129053,

                BreakdownAddressElements = true,
                ShowExtraTags = true,
                ShowAlternativeNames = true,
                ShowGeoJSON = true
            });
            r3.Wait();

            Assert.IsTrue((r3.Result.PlaceID > 0) && (r3.Result.Category == "highway") && (r3.Result.ClassType == "milestone"));
        }
    }
}