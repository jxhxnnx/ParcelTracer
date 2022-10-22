using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using PaPl.SKS.BusinessLogic;
using PaPl.SKS.BusinessLogic.Interfaces;
using PaPl.SKS.Package.Services.Controllers;
using PaPl.SKS.Package.Services.DTOs.Models;
using PaPl.SKS.Package.Services.Exceptions;
using PaPl.SKS.Package.Services.Test;
using System;

namespace PaPl.SKS.Package.Services.NUnit
{
    public class Controller_Sender_API_Test
    {
        private SenderApiController api;
        private  Mock<ISubmitParcelLogic> submitLogicMoq = new();
        private  ILogger<SenderApiController> testLogger;
        private IMapper mapper;

        public Controller_Sender_API_Test()
        {
            mapper = new TestMapper().GetTestMapper();
            
            testLogger = new NullLogger<SenderApiController>();
            api = new SenderApiController(submitLogicMoq.Object, testLogger, mapper);
        }
        
        [Test]
        public void SubmitParcel_ThrowsException()
        {
            submitLogicMoq
                .Setup(x => x.SubmitParcel(It.IsAny<BusinessLogic.Entities.Parcel>()))
                .Throws(new Exception());

            Assert.Throws<ServiceException>(() => api.SubmitParcel(new Parcel()));
        }

        [Test]
        public void SubmitParcel_Works()
        {
            submitLogicMoq
                .Setup(x => x.SubmitParcel(It.IsAny<BusinessLogic.Entities.Parcel>()))
                .Returns("TRACKING1");
            IActionResult result = api.SubmitParcel(new Parcel());
            Console.WriteLine(result.ToString());
            Assert.NotNull(result);
        }

    }
}
