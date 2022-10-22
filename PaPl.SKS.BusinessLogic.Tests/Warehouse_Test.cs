using Moq;
using NUnit.Framework;
using PaPl.SKS.BusinessLogic.Interfaces;
using PaPl.SKS.BusinessLogic.Entities;
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
    class Warehouse_Test
    {
        private  Mock<IHopRepository> hopRepoMoq = new();


        private  IWarehouseLogic warehouseLogic;
        private  ILogger<WareHouseLogic> testLogger;
        private IMapper mach;
        

        public Warehouse_Test()
        {
            mach = new TestMapper().GetTestMapper();
            testLogger = new NullLogger<WareHouseLogic>();
            hopRepoMoq.Setup(x => x.GetHopByCode(It.IsAny<string>()))
                .Returns(new DataAccess.Entities.Hop()
                {
                    Code = "1",
                    GeoLocation = new NetTopologySuite.Geometries.Point(new NetTopologySuite.Geometries.Coordinate()
                    {
                        X = 16,
                        Y = 48
                    })
                }); 
            warehouseLogic = new WareHouseLogic(hopRepoMoq.Object, testLogger, mach);
        }

        [Test]
        public void ExportWarehouses_Works()
        {
            hopRepoMoq
                .Setup(x => x.GetHops())
                .Returns(new DataAccess.Entities.Hop());
            Hop hop = warehouseLogic.ExportWarehouse();
            Assert.NotNull(hop);
        }

        [Test]
        public void ExportWarehouse_ThrowsException()
        {
            hopRepoMoq.Setup(x => x.GetHops())
                .Throws(new Exception());
            Assert.Throws<LogicException>(() => warehouseLogic.ExportWarehouse());
        }



        [Test]
        public void GetWarehouse_ThrowsException()
        { 
            Assert.Throws<PaPl.SKS.BusinessLogic.Interfaces.Exceptions.LogicException>(() => warehouseLogic.GetWarehouse(null));
        }

        [Test]
        public void GetWarehouse_NotExistingCode_ThrowsException()
        {
            hopRepoMoq.Setup(x => x.GetHopByCode(It.IsAny<string>()));
            Assert.Throws<LogicException>(() => warehouseLogic.GetWarehouse("CODE"));
        }

        [Test]
        public void ImportWarehouse_Works()
        {
            int statuscode = warehouseLogic.ImportWarehouse(new Hop());
            Assert.AreEqual(statuscode, 200);
        }

        [Test]
        public void ImportWarehouse_ThrowsException()
        {
            Assert.Throws<System.NullReferenceException>(() => warehouseLogic.ImportWarehouse(null));
        }

        [Test]
        public void GetHopByCode_Works()
        {
            Hop hop = warehouseLogic.GetHopByCode("1");
            Assert.NotNull(hop);
        }


    }
}

