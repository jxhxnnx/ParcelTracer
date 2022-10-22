using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NetTopologySuite.Geometries;
using NUnit.Framework;
using PaPl.SKS.BusinessLogic;
using PaPl.SKS.BusinessLogic.Interfaces;
using PaPl.SKS.DataAccess.Entities;
using PaPl.SKS.DataAccess.Repository;
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
    class Submit_Parcel_Test
    {
        private Mock<IParcelRepository> parcelRepoMoq = new();
        private Mock<IHopRepository> hopRepoMoq = new();


        private ISubmitParcelLogic submitParcelLogic;
        private ILogger<SubmitParcelLogic> testLogger;
        private IMapper mapper;

        public Submit_Parcel_Test()
        {

            testLogger = new NullLogger<SubmitParcelLogic>();
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
            submitParcelLogic = new SubmitParcelLogic(hopRepoMoq.Object, parcelRepoMoq.Object, testLogger, mapper);
        }

        [Test]
        public void SubmitParcel_NullParcel()
        {
            Console.WriteLine(submitParcelLogic.SubmitParcel(null));
            Assert.AreEqual(submitParcelLogic.SubmitParcel(null), "404");

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
            submitParcelLogic.MapManuallyHopToHopArrival(daoFutureHops, futureHops);
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

            bool pointIsInPolygon = submitParcelLogic.PointIsInPolygon(point, polygon);

            Assert.IsTrue(pointIsInPolygon);
        }

        [Test]
        public void PredictFutureHops_TrucksNull()
        {
            Assert.IsNull(submitParcelLogic.PredictFutureHops(null, null));
        }

        [Test]
        public void PredictFutureHops_Works()
        {
            DataAccess.Entities.Hop SenderTruck = new() { Code = "SENDERTRUCK", HopType = "Truck" };
            DataAccess.Entities.Warehouse SenderTruckDaddy = new() { Code = "SenderDaddyWARE", HopType = "Warehouse" };
            DataAccess.Entities.Hop RecipientTruck = new() { Code = "RECTRUCK", HopType = "Truck" };
            DataAccess.Entities.Warehouse RecipientTruckDaddy = new() { Code = "RecDaddyWARE", HopType = "Warehouse" };
            DataAccess.Entities.Warehouse BothTrucksGranddaddy = new() { Code = "Grandpa", HopType = "Warehouse" };

            SenderTruck.Parent = SenderTruckDaddy;
            RecipientTruck.Parent = RecipientTruckDaddy;
            SenderTruckDaddy.Parent = BothTrucksGranddaddy;
            RecipientTruckDaddy.Parent = BothTrucksGranddaddy;

            List<DataAccess.Entities.Hop> futureHops = submitParcelLogic.PredictFutureHops(SenderTruck, RecipientTruck);
            int futureHopCount = futureHops.Count;

            Console.WriteLine(futureHopCount);
            foreach (var hop in futureHops)
            {
                Console.WriteLine(hop.Code + " " + hop.HopType);
            }

            Assert.That(futureHopCount == 4);
        }

        [Test]
        public void GettAllTrucks_Works()
        {
            List<DataAccess.Entities.Truck> trucks = submitParcelLogic.GetAllTrucks();
            int truckCount = trucks.Count;

            Console.WriteLine(truckCount);
            Assert.That(truckCount == 4);
        }

        [Test]
        public void RandomString_Works()
        {
            string rand = submitParcelLogic.RandomString(3);
            int length = rand.Length;
            int expected = 3;

            Assert.AreEqual(length, expected);
        }

        [Test]
        public void SetSenderTruckToVisitedHop_Works()
        {
            BusinessLogic.Entities.Parcel parcel = new();
            DataAccess.Entities.Truck SenderTruck = new()
            {
                Description = "Test Description",
                Code = "TestCode"
            };

            Assert.IsNull(parcel.VisitedHops);
            parcel.VisitedHops = new List<BusinessLogic.Entities.HopArrival>()
            {
                submitParcelLogic.SetSenderTruckToVisitedHop(SenderTruck)
            };

            Console.WriteLine(parcel.VisitedHops[0].Code + parcel.VisitedHops[0].Description);
            Assert.NotNull(parcel.VisitedHops);

        }


    }
}
