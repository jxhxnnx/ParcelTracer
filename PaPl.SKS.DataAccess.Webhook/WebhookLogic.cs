using PaPl.SKS.DataAccess.Interfaces;
using PaPl.SKS.DataAccess.Webhook.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using PaPl.SKS.DataAccess.Repository;
using Newtonsoft.Json;
using PaPl.SKS.Package.Services.DTOs.Models;
using PaPl.SKS.DataAccess.Interfaces.Exceptions;

namespace PaPl.SKS.DataAccess.Webhook
{
    public class WebhookLogic : IWebhookLogic
    {
        private readonly ILogger<WebhookLogic> logger;
        private readonly IWebhookRepository repo;
        private readonly IParcelRepository parcelRepo;

        public WebhookLogic(ILogger<WebhookLogic> _logger, IWebhookRepository _repo, IParcelRepository _parcelRepo)
        {
            logger = _logger;
            repo = _repo;
            parcelRepo = _parcelRepo;
        }

        public List<Entities.Webhook> GetWebhooksWithTrackingId(string trackingId)
        {
            logger.LogDebug("WebhookLogic GetWebhooksWithTrackingId started");
            try
            {
                List<Entities.Webhook> allWebhooks = ListWebhooks();
                List<Entities.Webhook> subscribers = new();

                foreach (var webhook in allWebhooks)
                {
                    if (trackingId == webhook.TrackingId)
                    {
                        subscribers.Add(webhook);
                    }
                }

                return subscribers;
            }
            catch(Exception ex)
            {
                throw new DataException(nameof(WebhookLogic),
                                        nameof(GetWebhooksWithTrackingId),
                                        "An unknown error occured while getting webhook with trackingid: " + trackingId,
                                        ex);
            }
            
        }

        public List<Entities.Webhook> ListWebhooks()
        {
            logger.LogDebug("WebhookLogic ListWebhooks started");
            try
            {
                return repo.GetAllWebhooks().ToList();
            }
            catch (Exception ex)
            {
                throw new DataException(nameof(WebhookLogic),
                                        nameof(ListWebhooks),
                                        "An unknown error occured while listing all webhooks",
                                        ex);
            }
        }

        public int Subscribe(string trackingId, string urlCustomer)
        {
            logger.LogDebug("WebhookLogic Subscribe started");
            try
            {
                Entities.Webhook tobeAdded = new();
                //maybe add id
                tobeAdded.TrackingId = trackingId;
                tobeAdded.CustomerUrl = urlCustomer;
                tobeAdded.CreatedAt = DateTime.Now;
                logger.LogDebug("WebhookLogic Subscribe data set");
                repo.Create(tobeAdded);
                logger.LogDebug("WebhookLogic Subscribe created");
                Entities.Webhook webhook = repo.GetWebhookByTime((DateTime)tobeAdded.CreatedAt);
                logger.LogDebug("WebhookLogic Subscribe successful with id " + webhook.Id);
                return webhook.Id;
            }
            catch (Exception ex)
            {
                throw new DataException(nameof(WebhookLogic),
                                        nameof(Subscribe),
                                        $"An unknown error occured while subscribing webhook with id {trackingId} and url {urlCustomer}",
                                        ex);
            }
            
        }

        public async Task TriggerAsync(string trackingId)
        {
            logger.LogDebug("WebhookLogic TriggerAsync started");
            try
            {
                var parcel = parcelRepo.GetParcelById(trackingId);
                logger.LogDebug("WebhookLogic TriggerAsync GetParcelById");
                WebhookMessage webhookMessage = new();
                webhookMessage.TrackingId = parcel.TrackingId;
                webhookMessage.State = (TrackingInformation.StateEnum?)parcel.State;
                webhookMessage.FutureHops = new();
                webhookMessage.VisitedHops = new();
                if (parcel.VisitedHops.Count > 0)
                {
                    foreach (var hoparrival in parcel.VisitedHops)
                    {
                        HopArrival arrival = new();
                        arrival.Code = hoparrival.Code;
                        arrival.DateTime = hoparrival.DateTime;
                        arrival.Description = hoparrival.Description;
                        webhookMessage.VisitedHops.Add(arrival);
                    }
                }

                if (parcel.FutureHops.Count > 0)
                {
                    foreach (var hoparrival in parcel.FutureHops)
                    {
                        HopArrival arrival = new();
                        arrival.Code = hoparrival.Code;
                        arrival.DateTime = hoparrival.DateTime;
                        arrival.Description = hoparrival.Description;
                        webhookMessage.FutureHops.Add(arrival);
                    }
                }
                logger.LogDebug("WebhookLogic TriggerAsync data set");
                HttpRequestMessage message = new();
                message.Content = new StringContent(JsonConvert.SerializeObject(webhookMessage));

                using (var client = new HttpClient())
                {
                    foreach (var webhook in GetWebhooksWithTrackingId(trackingId))
                    {
                        var response = await SendNotification(message, client, webhook);
                    }
                }
                logger.LogDebug("WebhookLogic TriggerAsync successful");
            }
            catch (Exception ex)
            {
                throw new DataException(nameof(WebhookLogic),
                                        nameof(TriggerAsync),
                                        "An unknown error occured while triggering webhook",
                                        ex);
            }
            
        }

        private static Task<HttpResponseMessage> SendNotification(HttpRequestMessage content, HttpClient client, Entities.Webhook webhook)
        {
           
            try
            {
                return client.PostAsync(webhook.CustomerUrl, content.Content);
            }
            catch (Exception ex)
            {
                throw new DataException(nameof(WebhookLogic),
                                        nameof(SendNotification),
                                        "An unknown error occured while sending notification",
                                        ex);
            }
            
        }

        public int Unsubscribe(long id)
        {
            logger.LogDebug("WebhookLogic Unsubscribe started");

            try
            {
                repo.Delete(id);

                return 200;
            }
            catch (Exception ex)
            {
                throw new DataException(nameof(WebhookLogic),
                                        nameof(Unsubscribe),
                                        "An unknown error occured while unsubscribing webhook with id" + id,
                                        ex);
            }

            
        }
 
    }
}
