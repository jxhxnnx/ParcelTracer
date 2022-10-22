using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaPl.SKS.DataAccess.Webhook.Interfaces
{
    public interface IWebhookLogic
    {
        public int Subscribe(string trackingId, string urlCustomer);

        public  Task TriggerAsync(string trackingId);

        public int Unsubscribe(long id);

        public List<Entities.Webhook> ListWebhooks();

        public List<Entities.Webhook> GetWebhooksWithTrackingId(string trackingId);

    }
}
