using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using PaPl.SKS.BusinessLogic.Interfaces;
using PaPl.SKS.Package.Services.Controllers;
using PaPl.SKS.Package.Services.Exceptions;
using System;

namespace PaPl.SKS.Package.Services.NUnit
{
    public class Controller_Staff_API_Test
    {
        private  Mock<IReportParcelLogic> reportLogicMock = new Mock<IReportParcelLogic>();

        StaffApiController hopApi;
        StaffApiController deliveryApi;
        private  ILogger<StaffApiController> testLogger;

        public Controller_Staff_API_Test()
        {
            
            testLogger = new NullLogger<StaffApiController>();
            hopApi = new StaffApiController(reportLogicMock.Object, testLogger);

            

            deliveryApi = new StaffApiController(reportLogicMock.Object, testLogger);

        }

        [Test]
        public void ReportParcelHop_ThrowsException()
        {
            reportLogicMock
                .Setup(x => x.ReportParcelHop(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception());

            Assert.Throws<ServiceException>(() => hopApi.ReportParcelHop("TRACKING1", "CODE1234"));
        }
        [Test]
        public void ReportParcelDelivery_ThrowsException()
        {
            reportLogicMock
                .Setup(x => x.ReportParcelDelivery(It.IsAny<string>()))
                .Throws(new Exception());

            Assert.Throws<ServiceException>(() => hopApi.ReportParcelDelivery("TRACKING1"));
        }

        [Test]
        public void ReportParcelHop_idIsGOODCASE_codeIsGOODCODE_StatusCode200()
        {
            reportLogicMock.Setup(x => x.ReportParcelHop(It.IsAny<string>(), It.IsAny<string>()));
            IActionResult code = new StatusCodeResult(200);
            IActionResult res = hopApi.ReportParcelHop("GOODCASE", "GOODCODE");
            Assert.That(code.ToString() == res.ToString());
        }

       
        
        
        [Test]
        public void ReportParcelHop_idIsNull_codeIsNull_StatusCode400()
        {
            reportLogicMock.Setup(x => x.ReportParcelHop(It.IsAny<string>(), It.IsAny<string>()));
            IActionResult code = new StatusCodeResult(400);
            IActionResult res = hopApi.ReportParcelHop(null, null);
            Assert.That(code.ToString() == res.ToString());
        }

        

        [Test]
        public void ReportParcelDelivery_idIsGOODCASE_StatusCode200()
        {
            reportLogicMock.Setup(x => x.ReportParcelDelivery(It.IsAny<string>()));
            IActionResult code = new StatusCodeResult(200);
            IActionResult res = deliveryApi.ReportParcelDelivery("GOODCASE");
            Assert.That(code.ToString() == res.ToString());
        }

        
        [Test]
        public void ReportParcelDelivery_idIsNull_StatusCode400()
        {
            reportLogicMock.Setup(x => x.ReportParcelDelivery(It.IsAny<string>()));
            IActionResult code = new StatusCodeResult(400);
            IActionResult res = deliveryApi.ReportParcelDelivery(null);
            Assert.That(code.ToString() == res.ToString());
        }

    }
}
