using Moq;
using NUnit.Framework;
using PaPl.SKS.DataAccess.Entities;
using PaPl.SKS.BusinessLogic.Interfaces;
using PaPl.SKS.DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Diagnostics.CodeAnalysis;
using PaPl.SKS.BusinessLogic.Interfaces.Exceptions;
using AutoMapper;
using PaPl.SKS.Package.Services.Test;

namespace PaPl.SKS.BusinessLogic.Tests.Logic
{
    [ExcludeFromCodeCoverage]
    class Track_Parcel_Test
    {
        private 
            Mock<IParcelRepository> parcelRepoMoq = new();
        private DataAccess.Entities.Parcel parcel = new() { TrackingId = "TRACKING1", State = Parcel.StateEnum.InTransportEnum, FutureHops = null, Recipient = new Recipient(), Sender = new Recipient(), VisitedHops = null, Weight = 3 };


        private  ITrackParcelLogic trackParcelLogic;
        private  ILogger<TrackParcelLogic> testLogger;
        private IMapper mapper;

        public Track_Parcel_Test()
        {
            mapper = new TestMapper().GetTestMapper();
            testLogger = new NullLogger<TrackParcelLogic>();
            parcelRepoMoq.Setup(x => x.GetParcelById(It.IsAny<string>())).Returns(parcel);
            trackParcelLogic = new TrackParcelLogic(parcelRepoMoq.Object, testLogger, mapper);
        }
        
        
        [Test]
        public void Alpha_TrackParcel_Works()
        {
            var parcel = trackParcelLogic.TrackParcel("TRACKING1");
            Assert.That(parcel.TrackingId == "TRACKING1");
        }

        [Test]
        public void TrackParcel_ThrowsException()
        {
            parcelRepoMoq.Setup(x => x.GetParcelById(It.IsAny<string>()))
                .Throws(new Exception());
            Assert.Throws<LogicException>(() => trackParcelLogic.TrackParcel("TRACKING1"));
        }

       


    }
}
