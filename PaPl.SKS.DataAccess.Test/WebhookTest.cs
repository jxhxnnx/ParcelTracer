using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using PaPl.SKS.DataAccess.Interfaces;
using PaPl.SKS.DataAccess.Interfaces.Exceptions;
using PaPl.SKS.DataAccess.Repository;
using PaPl.SKS.DataAccess.Webhook;
using PaPl.SKS.DataAccess.Webhook.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PaPl.SKS.DataAccess.Test
{
    public class WebhookTest
    {
        private Mock<IWebhookRepository> webhookRepoMoq = new();
        private Mock<IParcelRepository> parcelRepoMoq = new();
        private IWebhookLogic webhookLogic;
        private ILogger<WebhookLogic> testLogger;

        public WebhookTest()
        {
            testLogger = new NullLogger<WebhookLogic>();
            
            webhookLogic = new WebhookLogic(testLogger, webhookRepoMoq.Object, parcelRepoMoq.Object);

        }

        [Test]
        public void GetWebhookWithTrackingId_Works()
        {
            webhookRepoMoq.Setup(x => x.GetAllWebhooks())
                .Returns(new List<Entities.Webhook>()
                {
                    new Entities.Webhook()
                    {
                        TrackingId = "TRACKING1"
                    }
                });
            var webhook = webhookLogic.GetWebhooksWithTrackingId("TRACKING1");
            Assert.NotNull(webhook);
        }

        [Test]
        public void GetWebhookWithTrackingId_ThrowsException()
        {
            webhookRepoMoq.Setup(x => x.GetAllWebhooks())
                .Throws(new Exception());
            Assert.Throws<DataException>(() => webhookLogic.GetWebhooksWithTrackingId("TRACKING1"));
        }

        [Test]
        public void ListWebhooks_Works()
        {
            webhookRepoMoq.Setup(x => x.GetAllWebhooks())
                .Returns(new List<Entities.Webhook>()
                {
                    new Entities.Webhook()
                    {
                        TrackingId = "TRACKING1"
                    }
                });
            var allWebhooks = webhookLogic.ListWebhooks();
            Assert.That(allWebhooks.Count > 0);
        }

        [Test]
        public void ListWebhooks_ThrowsException()
        {
            webhookRepoMoq.Setup(x => x.GetAllWebhooks())
               .Throws(new Exception());
            Assert.Throws<DataException>(() => webhookLogic.ListWebhooks());
        }

        [Test]
        public void Subscribe_Works()
        {
            webhookRepoMoq.Setup(x => x.GetWebhookByTime(It.IsAny<DateTime>()))
                .Returns(new Entities.Webhook() { Id = 1 });
            var webhookId = webhookLogic.Subscribe("TRACKING1", "url.de");
            Console.WriteLine(webhookId);
            Assert.NotNull(webhookId);
        }

        [Test]
        public void Subscribe_ThrowsException()
        {
            webhookRepoMoq.Setup(x => x.GetWebhookByTime(It.IsAny<DateTime>()))
                .Throws(new Exception());
            Assert.Throws<DataException>(() => webhookLogic.Subscribe("TRACKING1", "url.de"));
        }

        [Test]
        public async Task TriggerAsync_ThrowsException()
        {
            parcelRepoMoq
                .Setup(x => x.GetParcelById(It.IsAny<string>()))
                .Throws(new Exception());

            Assert.ThrowsAsync<DataException>(() => webhookLogic.TriggerAsync("TRACKING1"));
        }

        [Test]
        public async Task TriggerAsync_Works()
        {
            parcelRepoMoq
                .Setup(x => x.GetParcelById(It.IsAny<string>()))
                .Returns(new Entities.Parcel()
                {
                    TrackingId = "TRACKING18",
                    State = Entities.Parcel.StateEnum.InTruckDeliveryEnum,
                    FutureHops = new List<Entities.HopArrival>()
                    {
                        new Entities.HopArrival()
                        {
                            Code = "123",
                            DateTime = DateTime.Now,
                            Description = "Description"
                        },
                        new Entities.HopArrival()
                        {
                            Code = "124",
                            DateTime = DateTime.Now,
                            Description = "Description"
                        },
                        new Entities.HopArrival()
                        {
                            Code = "125",
                            DateTime = DateTime.Now,
                            Description = "Description"
                        }
                    },
                    VisitedHops = new List<Entities.HopArrival>()
                    {
                        new Entities.HopArrival()
                        {
                            Code = "126",
                            DateTime = DateTime.Now,
                            Description = "Description"
                        },
                        new Entities.HopArrival()
                        {
                            Code = "127",
                            DateTime = DateTime.Now,
                            Description = "Description"
                        },
                        new Entities.HopArrival()
                        {
                            Code = "128",
                            DateTime = DateTime.Now,
                            Description = "Description"
                        }
                    },
                });

            webhookRepoMoq
                .Setup(x => x.GetAllWebhooks())
                .Returns(new List<Entities.Webhook>()
                {
                    new Entities.Webhook()
                    {
                        Id = 1,
                        CreatedAt = DateTime.Now,
                        CustomerUrl = "http://www.url.de",
                        TrackingId = "TRACKING1"
                    }
                });

            await webhookLogic.TriggerAsync("TRACKING1");

            Assert.Pass();

        }

        [Test]
        public void Unsubscribe_ThrowsException()
        {
            webhookRepoMoq.Setup(x => x.Delete(It.IsAny<long>()))
                .Throws(new Exception());

            Assert.Throws<DataException>(() => webhookLogic.Unsubscribe(1));
        }

        [Test]
        public void Unsubscribe_Works()
        {
            webhookRepoMoq.Setup(x => x.Delete(It.IsAny<long>()));

            Assert.That(webhookLogic.Unsubscribe(1) == 200);
        }
    }
}
