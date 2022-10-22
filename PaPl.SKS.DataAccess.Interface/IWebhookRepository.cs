using PaPl.SKS.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaPl.SKS.DataAccess.Interfaces
{
    public interface IWebhookRepository
    {
        public void Create(Webhook webhook);
        public void Delete(long id);

        IEnumerable<Webhook> GetAllWebhooks();
        Webhook GetCustomersByTrackingId(string trackingId);
        public Webhook GetWebhookByTime(DateTime time);
    }
}
