using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using PaPl.SKS.BusinessLogic.Entities;
using PaPl.SKS.BusinessLogic.Interfaces;
using PaPl.SKS.DataAccess.Interfaces;
using PaPl.SKS.DataAccess.Repository;
using PaPl.SKS.DataAccess.Webhook;
using PaPl.SKS.DataAccess.Webhook.Interfaces;
using PaPl.SKS.Package.Services.Test;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaPl.SKS.BusinessLogic.Tests.Logic
{
    [ExcludeFromCodeCoverage]
    class Report_Parcel_Test
    {
        private  ILogger<ReportParcelLogic> testLogger;
        private  ILogger<WareHouseLogic> WHtestLogger;
        private  ILogger<WebhookLogic> WebHooktestLogger;
        
        private IWarehouseLogic whLogic;
        private Mock<IHopRepository> hopRepoMoq = new();
        private Mock<IParcelRepository> parcelRepoMoq = new();
        private Mock<IWarehouseLogic> whLogicMoq = new();
        private IWebhookLogic webhookLogic;
        private Mock<IWebhookRepository> webhookRepoMoq = new();

        private IMapper mapper;
        ReportParcelLogic logic;
        ReportParcelLogic moqLogic;
        public Report_Parcel_Test()
        {
            mapper = new TestMapper().GetTestMapper();
            testLogger = new NullLogger<ReportParcelLogic>();
            WHtestLogger = new NullLogger<WareHouseLogic>();
            WebHooktestLogger = new NullLogger<WebhookLogic>();
            whLogic = new WareHouseLogic(hopRepoMoq.Object, WHtestLogger, mapper);
            webhookLogic = new WebhookLogic(WebHooktestLogger, webhookRepoMoq.Object, parcelRepoMoq.Object);
            logic = new(testLogger, whLogic, parcelRepoMoq.Object, mapper, webhookLogic);
            moqLogic = new(testLogger, whLogicMoq.Object, parcelRepoMoq.Object, mapper, webhookLogic);
            hopRepoMoq
                .Setup(x => x.GetHopByCode(It.IsAny<string>()))
                .Returns(new DataAccess.Entities.Hop(){ Code = "ABCD01" });
            parcelRepoMoq
                .Setup(x => x.GetParcelById(It.IsAny<string>()))
                .Returns(new DataAccess.Entities.Parcel() { TrackingId = "TRACKING1" });
            whLogicMoq
                .Setup(x => x.GetHopByCode(It.IsAny<string>()))
                .Returns(new BusinessLogic.Entities.Hop() { Code = "ABCD01" });
        }

        [Test]
        public void ReportParcelHop_ThrowsException()
        {
            parcelRepoMoq
                .Setup(x => x.GetParcelById(It.IsAny<string>()))
                .Throws(new Exception());

            Assert.Throws<Interfaces.Exceptions.LogicException>(() => logic.ReportParcelHop("TRACKING", "CODE"));


            parcelRepoMoq
                .Setup(x => x.GetParcelById(It.IsAny<string>()))
                .Returns(new DataAccess.Entities.Parcel() { TrackingId = "TRACKING1" });
        }

        [Test]
        public void ReportParcelHop_Works()
        {
            int statuscode = moqLogic.ReportParcelHop("TRACKING1", "ABCD01");
            Assert.AreEqual(statuscode, 200);
        }

        [Test]
        public void ChangeStateOfParcelWorks()
        {
            Hop hop = new Hop()
            {
                HopType = "Warehouse"
            };

            Parcel parcel = new Parcel()
            {
                State = Parcel.StateEnum.InTruckDeliveryEnum
            };

            Parcel.StateEnum oldState = (Parcel.StateEnum)parcel.State;
            logic.ChangeStateOfParcel(hop, parcel);
            Parcel.StateEnum newState = (Parcel.StateEnum)parcel.State;

            Console.WriteLine("old: " + oldState + " | new: " + newState);
            Assert.AreNotEqual(oldState, newState);
        }

        [Test]
        public void ChangeStateOfParcelWorks_WithTruck()
        {
            Hop hop = new Hop()
            {
                HopType = "Truck"
            };

            Parcel parcel = new Parcel()
            {
                State = Parcel.StateEnum.InTransportEnum
            };

            Parcel.StateEnum oldState = (Parcel.StateEnum)parcel.State;
            logic.ChangeStateOfParcel(hop, parcel);
            Parcel.StateEnum newState = (Parcel.StateEnum)parcel.State;

            Console.WriteLine("old: " + oldState + " | new: " + newState);
            Assert.AreNotEqual(oldState, newState);
        }

        [Test]
        public void ChangeHopArrivalFromFutureToVisited_Works()
        {
            HopArrival hopArrival1 = new HopArrival() { Code = "Code1" };
            HopArrival hopArrival2 = new HopArrival() { Code = "Code2" };
            HopArrival hopArrival3 = new HopArrival() { Code = "Code3" };
            HopArrival hopArrival4 = new HopArrival() { Code = "Code4" };
            HopArrival hopArrival5 = new HopArrival() { Code = "Code5" };
            HopArrival hopArrival6 = new HopArrival() { Code = "Code6" };
            HopArrival hopArrival7 = new HopArrival() { Code = "Code7" };
            List<HopArrival> futureHops = new();
            futureHops.Add(hopArrival1);
            futureHops.Add(hopArrival2);
            futureHops.Add(hopArrival3);
            futureHops.Add(hopArrival4);
            futureHops.Add(hopArrival5);

            List<HopArrival> visitedHops = new();
            visitedHops.Add(hopArrival6);
            visitedHops.Add(hopArrival7);

            Parcel parcel = new()
            {
                FutureHops = futureHops,
                VisitedHops = visitedHops
            };
            Hop hop = new Hop()
            {
                Code = "Code2"
            };
            
            int oldFutureCount = parcel.FutureHops.Count;
            int oldVisitedCount = parcel.VisitedHops.Count;
            logic.ChangeHopArrivalFromFutureToVisited(hop, parcel);
            int newFutureCount = parcel.FutureHops.Count;
            int newVisitedCount = parcel.VisitedHops.Count;

            Console.WriteLine("old future count: " + oldFutureCount);
            Console.WriteLine("old visited count: " + oldVisitedCount);
            Console.WriteLine("new future count: " + newFutureCount);
            Console.WriteLine("new visited count: " + newVisitedCount);

            Assert.AreNotEqual(oldFutureCount, newFutureCount);
            Assert.AreNotEqual(oldVisitedCount, newVisitedCount);
        }

        [Test]
        public void ReportParcelHop_IDNull_Returns400()
        {
            int statuscode = logic.ReportParcelHop(null, "Code");

            Assert.AreEqual(statuscode, 400);
        }


        [Test]
        public void ReportParcelDelivery_IDNull_Returns400()
        {
            parcelRepoMoq
                .Setup(x => x.GetParcelById(It.IsAny<String>()))
                .Returns(new DataAccess.Entities.Parcel() { TrackingId = "TRACK" });

            int statuscode = logic.ReportParcelDelivery(null);

            Assert.AreEqual(statuscode, 400);
        }

        [Test]
        public void ReportParcelDelivery_ParcelNull_Returns400()
        {
            parcelRepoMoq
                .Setup(x => x.GetParcelById(It.IsAny<String>()));

            int statuscode = logic.ReportParcelDelivery("TRACK");

            Assert.AreEqual(statuscode, 400);
        }

        [Test]
        public void ReportParcelDelivery_Works()
        {
            parcelRepoMoq
                .Setup(x => x.GetParcelById(It.IsAny<String>()))
                .Returns(new DataAccess.Entities.Parcel() { TrackingId = "TRACKING1" });

            int statuscode = logic.ReportParcelDelivery("TRACKING1");

            Assert.AreEqual(statuscode, 200);
        }
    }
}
