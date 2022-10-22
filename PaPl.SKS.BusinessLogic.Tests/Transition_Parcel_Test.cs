using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NetTopologySuite.Geometries;
using NUnit.Framework;
using PaPl.SKS.BusinessLogic.Interfaces;
using PaPl.SKS.BusinessLogic.Interfaces.Exceptions;
using PaPl.SKS.DataAccess.Entities;
using PaPl.SKS.DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaPl.SKS.BusinessLogic.Tests.Logic
{
    [ExcludeFromCodeCoverage]
    class Transition_Parcel_Test
    {
        private Mock<IParcelRepository> parcelRepoMoq = new();
        private Mock<IHopRepository> hopRepoMoq = new();


        private  ITransitionLogic transitionLogic;
        private  ILogger<TransitionLogic> testLogger;
        private IMapper mapper;

        public Transition_Parcel_Test()
        {
            testLogger = new NullLogger<TransitionLogic>();
            parcelRepoMoq.Setup(x => x.Create(It.IsAny<DataAccess.Entities.Parcel>()));
            hopRepoMoq
                .Setup(x => x.GetAllHops())
                .Returns(
                new List<Hop>()
                {
                    new Hop() { HopType = "Truck" },
                    new Hop() { HopType = "Truck" },
                    new Hop() { HopType = "Truck" },
                    new Hop() { HopType = "Truck" },
                    new Hop() { HopType = "Warehouse" },
                    new Hop() { HopType = "Warehouse" },
                    new Hop() { HopType = "Warehouse" },
                    new Hop() { HopType = "TransferWarehouse" },
                    new Hop() { HopType = "TransferWarehouse" }
                }
                );
            transitionLogic = new TransitionLogic(hopRepoMoq.Object, parcelRepoMoq.Object, testLogger, mapper);
        }

        [Test]
        public void MapManuallyHopToHopArrival_Works()
        {
            List<DataAccess.Entities.Hop> daoFutureHops = new();
            List<BusinessLogic.Entities.HopArrival> futureHops = new();

            DataAccess.Entities.Hop hop1 = new()
            {
                Code = "Code1",
                Description = "Description1"
            };
            DataAccess.Entities.Hop hop2 = new()
            {
                Code = "Code2",
                Description = "Description2"
            };
            DataAccess.Entities.Hop hop3 = new()
            {
                Code = "Code3",
                Description = "Description3"
            };
            daoFutureHops.Add(hop1);
            daoFutureHops.Add(hop2);
            daoFutureHops.Add(hop3);

            int oldFutureHopCount = futureHops.Count;
            futureHops = transitionLogic.MapManuallyHopToHopArrival(daoFutureHops);
            int newFutureHopCount = futureHops.Count;

            Console.WriteLine($"old futurehop count: {oldFutureHopCount}");
            Console.WriteLine($"new futurehop count: {newFutureHopCount}");
            Assert.That(oldFutureHopCount == 0);
            Assert.That(newFutureHopCount == 3);
        }

        [Test]
        public void PointIsInPolygon_Works()
        {
            Coordinate point = new Coordinate()
            {
                X = 48.1854445530772,
                Y = 16.3382989723744
            };

            Coordinate[] polygon = {
                new Coordinate() { X = 48.1856789, Y = 16.3442499 },
                new Coordinate() { X = 48.1884662, Y = 16.3417431 },
                new Coordinate() { X = 48.1883088, Y = 16.3390651 },
                new Coordinate() { X = 48.1846561, Y = 16.3343592 },
                new Coordinate() { X = 48.1837565, Y = 16.3307561 },
                new Coordinate() { X = 48.1835799, Y = 16.3322475 },
                new Coordinate() { X = 48.1821835, Y = 16.3331583 }
            };

            bool pointIsInPolygon = transitionLogic.PointIsInPolygon(point, polygon);

            Assert.IsTrue(pointIsInPolygon);
        }

        [Test]
        public void PredictFutureHops_Works()
        {
            DataAccess.Entities.Hop SenderTruck = new();
            DataAccess.Entities.Warehouse SenderTruckDaddy = new();
            DataAccess.Entities.Hop RecipientTruck = new();
            DataAccess.Entities.Warehouse RecipientTruckDaddy = new();
            DataAccess.Entities.Warehouse BothTrucksGranddaddy = new();

            SenderTruck.Parent = SenderTruckDaddy;
            RecipientTruck.Parent = RecipientTruckDaddy;
            SenderTruckDaddy.Parent = BothTrucksGranddaddy;
            RecipientTruckDaddy.Parent = BothTrucksGranddaddy;

            List<DataAccess.Entities.Hop> futureHops = transitionLogic.PredictFutureHops(SenderTruck, RecipientTruck);
            int futureHopCount = futureHops.Count;

            Console.WriteLine(futureHopCount);
            Assert.That(futureHopCount == 3);
        }

        [Test]
        public void GettAllTrucks_Works()
        {
            List<DataAccess.Entities.Truck> trucks = transitionLogic.GetAllTrucks();
            int truckCount = trucks.Count;

            Console.WriteLine(truckCount);
            Assert.That(truckCount == 4);
        }

        [Test]
        public void RandomString_Works()
        {
            string rand = transitionLogic.RandomString(3);
            int length = rand.Length;
            int expected = 3;

            Assert.AreEqual(length, expected);
        }

        [Test]
        public void TransitionParcel_ThrowsException()
        {
            parcelRepoMoq.Setup(x => x.GetParcelById(It.IsAny<string>()))
                .Throws(new Exception());
            Assert.Throws<LogicException>(() => transitionLogic.TransitionParcel(new BusinessLogic.Entities.Parcel(), "TRACKING"));
        }
    }
}
