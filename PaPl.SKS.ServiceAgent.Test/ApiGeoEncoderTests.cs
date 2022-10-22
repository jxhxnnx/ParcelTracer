using NUnit.Framework;
using PaPl.SKS.BusinessLogic.Entities;
using PaPl.SKS.DataAccess.ServiceAgent;
using PaPl.SKS.Package.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaPl.SKS.ServiceAgent.Test
{
    public class ApiGeoEncoderTests
    {
        Recipient recipient = new Recipient();
        ApiGeoEncoder encoder = new ApiGeoEncoder();

        [Test]
        public void ApiGeoEncoder_EncodeAdress_Works()
        {

            recipient.Street = "Pennsylvania Avenue";
            recipient.City = "Washington"; 
            recipient.PostalCode = "1600";

            NetTopologySuite.Geometries.Coordinate coordinate = encoder.EncodeAddress(recipient);

            Debug.WriteLine(coordinate.X + " " + coordinate.Y);
            Assert.NotNull(coordinate.X);
            Assert.NotNull(coordinate.Y);
        }

        [Test]
        public void ApiGeoEncoder_EncodeAdress_ThrowsException()
        {
            Assert.Throws<ServiceException>(() => encoder.EncodeAddress(recipient));
        }
    }
}
