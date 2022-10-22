using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using PaPl.SKS.DataAccess.Entities;
using PaPl.SKS.DataAccess.Interfaces.Exceptions;
using PaPl.SKS.DataAccess.Repository;
using PaPl.SKS.DataAccess.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaPl.SKS.DataAccess.Test
{
    public class WebhookRepositoryTest
    {
        public SqlWebhookRepository webhookRepo;
        private ILogger<SqlWebhookRepository> testLogger;
        private SQLDataContext inMemoryDBContext;
        public WebhookRepositoryTest()
        {
            testLogger = new NullLogger<SqlWebhookRepository>();
            var options = new DbContextOptionsBuilder<SQLDataContext>()
           .UseInMemoryDatabase(databaseName: "TestingDatabase")
           .Options;
            inMemoryDBContext = new SQLDataContext(options);

            webhookRepo = new SqlWebhookRepository(inMemoryDBContext, testLogger);
        }

        [Test]
        public void Alpha_GetAllWebhooks_ListIsNull_Exception_Test()
        {
            Assert.Throws<DataException>(() =>webhookRepo.GetAllWebhooks());
        }

        [Test]
        public void GetAllWebhooks_Works()
        {
            Entities.Webhook webhook = new()
            {
                CreatedAt = DateTime.Now,
                CustomerUrl = "url.de",
                Id = 0,
                TrackingId = "TRACKING1"
            };
            webhookRepo.Create(webhook);
            var allwebHooks = webhookRepo.GetAllWebhooks();
            Assert.That(allwebHooks.ToList().Count > 0);
        }

        [Test]
        public void Create_Works()
        {
            Entities.Webhook webhook = new()
            {
                CreatedAt = DateTime.Now,
                CustomerUrl = "url.de",
                Id = 1,
                TrackingId = "TRACKING2"
            };
            webhookRepo.Create(webhook);
            var resultWebhook = inMemoryDBContext.Webhook
                .Single(b => b.TrackingId == "TRACKING2");

            Assert.IsNotNull(resultWebhook);
        }

        [Test]
        public void Delete_Works()
        {
            Entities.Webhook webhook = new()
            {
                CreatedAt = DateTime.Now,
                CustomerUrl = "url.de",
                Id = 2,
                TrackingId = "TRACKING3"
            };
            webhookRepo.Create(webhook);
            Entities.Webhook webhook2 = new()
            {
                CreatedAt = DateTime.Now,
                CustomerUrl = "url.de",
                Id = 5,
                TrackingId = "TRACKING4"
            };
            webhookRepo.Create(webhook2);

            var webhookCounterBefore = webhookRepo.GetAllWebhooks().ToList().Count;
            webhookRepo.Delete(2);
            var webhookCounterAfter = webhookRepo.GetAllWebhooks().ToList().Count;

            Assert.That(webhookCounterBefore - webhookCounterAfter == 1);
        }

        [Test]
        public void Delete_WebhookNull_Exception()
        {
            Assert.Throws<DataException>(() => webhookRepo.Delete(20));
        }

       [Test]
       public void GetWebhookByTime_Works()
        {
            var time = DateTime.Now;

            Entities.Webhook webhook = new()
            {
                CreatedAt = time,
                CustomerUrl = "url.de",
                Id = 2,
                TrackingId = "TRACKING3"
            };

            webhookRepo.Create(webhook);

            var resultWebhook = webhookRepo.GetWebhookByTime(time);
            Assert.NotNull(resultWebhook);
        }

        [Test]
        public void GetWebhookByTime_Exception()
        {
            var time = DateTime.Now;
            var webhookTime = DateTime.Now.AddDays(-2);

            Entities.Webhook webhook = new()
            {
                CreatedAt = webhookTime,
                CustomerUrl = "url.de",
                Id = 33,
                TrackingId = "TRACKING33"
            };

            webhookRepo.Create(webhook);
            Console.WriteLine("webhook created at: " + webhookRepo.GetWebhookByTime(webhookTime).CreatedAt);

            Assert.Throws<DataException>(() => webhookRepo.GetWebhookByTime(time));
        }
            
    }
}
