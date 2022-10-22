using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using PaPl.SKS.BusinessLogic;
using PaPl.SKS.BusinessLogic.Interfaces;
using PaPl.SKS.DataAccess.Repository;
using PaPl.SKS.Package.Services.Controllers;
using PaPl.SKS.Package.Services.DTOs.Models;
using PaPl.SKS.Package.Services.Test;
using System;
using System.Collections.Generic;

namespace PaPl.SKS.Package.Services.NUnit
{
    public class Controller_Recipient_API_Test
    {
        private RecipientApiController api;
        private ILogger<RecipientApiController> testLogger;
        private ILogger<TrackParcelLogic> trackingLogger;
        private Mock<IParcelRepository> parcelRepoMoq = new();
        private IMapper mapper;
        private ITrackParcelLogic trackParcelLogic;
        public Controller_Recipient_API_Test()
        {
            parcelRepoMoq
                .Setup(x => x.GetParcelById(It.IsAny<string>()))
                .Returns(new DataAccess.Entities.Parcel()
                {
                    TrackingId = "TRACKING1",
                    State = DataAccess.Entities.Parcel.StateEnum.InTransportEnum,
                    FutureHops = new List<DataAccess.Entities.HopArrival>(),
                    VisitedHops = new List<DataAccess.Entities.HopArrival>()
                }); 

            testLogger = new NullLogger<RecipientApiController>();
            trackingLogger = new NullLogger<TrackParcelLogic>();
            trackParcelLogic = new TrackParcelLogic(parcelRepoMoq.Object, trackingLogger, mapper);
            mapper = new TestMapper().GetTestMapper();
            api = new(testLogger, trackParcelLogic, mapper);
        }


        [Test]
        public void Alpha_TrackParcel_Works()
        {
            IActionResult result = api.TrackParcel("TRACKING1");
            Console.WriteLine(result);
            Assert.NotNull(result);
        }

        [Test]
        public void TrackParcel_ThrowsException()
        {
            parcelRepoMoq
                .Setup(x => x.GetParcelById(It.IsAny<string>()));
            Assert.Throws<Services.Exceptions.ServiceException>(() => api.TrackParcel(null));
        }

        [Test]
        public void TrackParcelWithFutureAndVisitedHops_Works()
        {
            parcelRepoMoq
                .Setup(x => x.GetParcelById(It.IsAny<string>()))
                .Returns(new DataAccess.Entities.Parcel()
                {
                    TrackingId = "TRACKING2",
                    State = DataAccess.Entities.Parcel.StateEnum.InTransportEnum,
                    FutureHops = new List<DataAccess.Entities.HopArrival>()
                    {
                        new DataAccess.Entities.HopArrival(),
                        new DataAccess.Entities.HopArrival(),
                        new DataAccess.Entities.HopArrival()
                    },
                    VisitedHops = new List<DataAccess.Entities.HopArrival>()
                    {
                        new DataAccess.Entities.HopArrival(),
                        new DataAccess.Entities.HopArrival(),
                        new DataAccess.Entities.HopArrival()
                    }
                }); 
            IActionResult result = api.TrackParcel("TRACKING2");
            Console.WriteLine(result);
            Assert.NotNull(result);
        }


    }
}

