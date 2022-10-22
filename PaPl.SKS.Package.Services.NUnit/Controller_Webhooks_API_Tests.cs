using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using PaPl.SKS.DataAccess.Webhook.Interfaces;
using PaPl.SKS.Package.Services.Controllers;
using PaPl.SKS.Package.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaPl.SKS.Package.Services.Test
{
    public class Controller_Webhooks_API_Tests
    {
        private Mock<IWebhookLogic> webhookLogicMoq = new();

        private ParcelWebhookApiController api;

        private ILogger<ParcelWebhookApiController> testLogger;



        public Controller_Webhooks_API_Tests()
        {
            testLogger = new NullLogger<ParcelWebhookApiController>();
            api = new ParcelWebhookApiController(webhookLogicMoq.Object, testLogger);
        }

        [Test]
        public void ListParcelWebhooks_Works()
        {
            webhookLogicMoq
                .Setup(x => x.GetWebhooksWithTrackingId(It.IsAny<string>()))
                .Returns(new List<DataAccess.Entities.Webhook>()
                {
                    new DataAccess.Entities.Webhook()
                    {
                        TrackingId = "TRACKING1",
                        CustomerUrl = "url.de",
                        Id = 1,
                        CreatedAt = DateTime.Now
                    },
                    new DataAccess.Entities.Webhook()
                    {
                        TrackingId = "TRACKING1",
                        CustomerUrl = "url.at",
                        Id = 2,
                        CreatedAt = DateTime.Now.AddDays(-1)
                    },
                    new DataAccess.Entities.Webhook()
                    {
                        TrackingId = "TRACKING^1",
                        CustomerUrl = "url.com",
                        Id = 3,
                        CreatedAt = DateTime.Now.AddDays(-3)
                    }
                });

            IActionResult result = api.ListParcelWebhooks("TRACKING1");
            Assert.NotNull(result);
        }

        [Test]
        public void ListParcelWebhooks_ThrowsException()
        {
            webhookLogicMoq
                .Setup(x => x.GetWebhooksWithTrackingId(It.IsAny<string>()))
                .Throws(new Exception());

            Assert.Throws<ServiceException>(() => api.ListParcelWebhooks("TRACKING1"));
        }

        [Test]
        public void SubscribeWebhooks_Works()
        {
            webhookLogicMoq
                .Setup(x => x.Subscribe(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(200);

            IActionResult result = api.SubscribeParcelWebhookAsync("TRACKING1", "url.de");
            Assert.NotNull(result);
        }

        [Test]
        public void SubscribeWebhooks_ThrowsException()
        {
            webhookLogicMoq
                .Setup(x => x.Subscribe(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception());   

            Assert.Throws<ServiceException>(() => api.SubscribeParcelWebhookAsync("TRACKING1", "url.de"));
        }

        [Test]
        public void UnSubscribeWebhooks_Works()
        {
            webhookLogicMoq
                .Setup(x => x.Unsubscribe(It.IsAny<long>()))
                .Returns(200);

            IActionResult result = api.UnsubscribeParcelWebhook(1);
            Assert.NotNull(result);
        }

        [Test]
        public void UnSubscribeWebhooks_ThrowsException()
        {
            webhookLogicMoq
                .Setup(x => x.Unsubscribe(It.IsAny<long>()))
                .Throws(new Exception());

            Assert.Throws<ServiceException>(() => api.UnsubscribeParcelWebhook(1));
        }


    }
}
