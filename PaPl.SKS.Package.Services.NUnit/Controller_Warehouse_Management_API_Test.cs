using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using PaPl.SKS.BusinessLogic.Interfaces;
using PaPl.SKS.Package.Services.Controllers;
using PaPl.SKS.Package.Services.DTOs.Models;
using PaPl.SKS.Package.Services.Exceptions;
using PaPl.SKS.Package.Services.Test;
using System;

namespace PaPl.SKS.Package.Services.NUnit
{
    public class Controller_Warehouse_Management_API_Test
    {
        private  Mock<IWarehouseLogic> importLogicMock = new Mock<IWarehouseLogic>();
        private  ILogger<WarehouseManagementApiController> testLogger;

        WarehouseManagementApiController importWarehouseLogic;
        WarehouseManagementApiController api;
        private IMapper mapper;

        public Controller_Warehouse_Management_API_Test()
        {
            mapper = new TestMapper().GetTestMapper();
            testLogger = new NullLogger<WarehouseManagementApiController>();
            
            
            importWarehouseLogic = new WarehouseManagementApiController(testLogger, importLogicMock.Object, mapper);
            api = new WarehouseManagementApiController(testLogger, importLogicMock.Object, mapper);
        }
        

        [Test]
        public void GetWareHouse_Works()
        {
            importLogicMock
                .Setup(x => x.GetHopByCode(It.IsAny<string>()))
                .Returns(new BusinessLogic.Entities.Warehouse());
            IActionResult result = api.GetWarehouse("CODE");
            Assert.NotNull(result);
        }

        [Test]
        public void GetWarehouse_ThrowsException()
        {
            importLogicMock
                .Setup(x => x.GetHopByCode(It.IsAny<string>()))
                .Throws(new Exception());
            Assert.Throws<ServiceException>(() => api.GetWarehouse("CODE"));

        }

        [Test]
        public void ExportWarehouses_Works()
        {
            importLogicMock
                .Setup(x => x.ExportWarehouse())
                .Returns(new BusinessLogic.Entities.Hop());
            IActionResult result = api.ExportWarehouses();


            Assert.NotNull(result);
        }

        [Test]
        public void ExportWarehouses_ThrowsException()
        {
            importLogicMock
                .Setup(x => x.ExportWarehouse())
                .Throws(new System.Exception());

            Assert.Throws<ServiceException>(() => api.ExportWarehouses());
        }

        [Test]
        public void ImportWarehouses_Works()
        {
            importLogicMock
                .Setup(x => x.ImportWarehouse(It.IsAny<BusinessLogic.Entities.Hop>()))
                .Returns(200);
            IActionResult result = api.ImportWarehouses(new Warehouse());
            Assert.NotNull(result);
        }

        [Test]
        public void ImportWarehouses_ThrowsException()
        {
            importLogicMock
                .Setup(x => x.ImportWarehouse(It.IsAny<BusinessLogic.Entities.Hop>()))
                .Throws(new Exception());

            Assert.Throws<ServiceException>(() => api.ImportWarehouses(new Warehouse()));
        }

    }
}
