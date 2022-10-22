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
    public class Controller_Logistics_Partner_API_Test
    {
        private  Mock<ITransitionLogic> transLogicMoq = new();

        private  LogisticsPartnerApiController api;

        private  Parcel parcel = new();

        private  ILogger<LogisticsPartnerApiController> testLogger;
        private IMapper mapper;
     

        public Controller_Logistics_Partner_API_Test()
        {
            mapper = new TestMapper().GetTestMapper();
            
                
            testLogger = new NullLogger<LogisticsPartnerApiController>();
            api = new LogisticsPartnerApiController(testLogger, transLogicMoq.Object, mapper);
        }
        
        [Test]
        public void TransitionParcel_ThrowsExcetion()
        {
            transLogicMoq
                .Setup(x => x.TransitionParcel(It.IsAny<BusinessLogic.Entities.Parcel>(), It.IsAny<string>()))
                .Throws(new Exception());
            
            Assert.Throws<ServiceException>(() => api.TransitionParcel(new Parcel(), "TRACKING1"));
        }

        [Test]
        public void TransitionParcel_Works()
        {
            transLogicMoq
                .Setup(x => x.TransitionParcel(It.IsAny<BusinessLogic.Entities.Parcel>(), "TRACKING1"))
                .Returns("TRACKING1");
            IActionResult result = api.TransitionParcel(new Parcel(), "TRACKING1");

            Console.WriteLine(result.ToString());
            Assert.NotNull(result);
        }

    }
}