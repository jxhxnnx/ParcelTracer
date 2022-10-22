using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PaPl.SKS.DataAccess.Entities;
using PaPl.SKS.DataAccess.Interfaces;
using PaPl.SKS.DataAccess.Interfaces.Exceptions;
using PaPl.SKS.DataAccess.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaPl.SKS.DataAccess.Repository
{
    public class SqlWebhookRepository : IWebhookRepository
    {
        private ILogger<SqlWebhookRepository> logger;
        private SQLDataContext context;

        public SqlWebhookRepository(DbContext context, ILogger<SqlWebhookRepository> _logger)
        {
            this.context = (SQLDataContext)context;
            logger = _logger;
        }

        public Webhook GetWebhookByTime(DateTime time)
        {
            logger.LogDebug("SQLWebhookRepository GetWebhookByTime started");
            try
            {
                IEnumerable<Webhook> allWebhook = GetAllWebhooks();
                Webhook webhook = new();
                foreach(var hook in allWebhook)
                {
                    if(hook.CreatedAt == time)
                    {
                        webhook = hook;
                    }
                }

                if (webhook.CreatedAt == null)
                {
                    throw new DataException(nameof(SqlHopRepository),
                                            nameof(Delete),
                                            "An error occured while getting this webhook, no webhook for this time");
                }
                return webhook;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw new DataException(nameof(SqlWebhookRepository),
                                        nameof(GetWebhookByTime),
                                        "An unknown error occured while creating a new webhook",
                                        ex);
            }
        }

        public void Create(Webhook webhook)
        {
            logger.LogDebug("SQLWebhookRepository Create started");
            try
            {
                context.Webhook.Add(webhook);
                logger.LogDebug("SQLWebhookRepository Create add hop to db");
                context.SaveChanges();
                logger.LogDebug("SQLWebhookRepository Create save changes");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw new DataException(nameof(SqlWebhookRepository),
                                        nameof(Create),
                                        "An unknown error occured while creating a new webhook",
                                        ex);
            }
            finally
            {
                logger.LogDebug("SQLHopRepository Create executed");
            }
        }

        public void Delete(long hookId)
        {
            int webhookid = (int)hookId;
            logger.LogDebug("SQLHopRepository Delete started");
            try
            {
                Webhook webhook = context.Webhook.Find(webhookid);
                if (webhook == null)
                {
                    throw new DataException(nameof(SqlHopRepository),
                                            nameof(Delete),
                                            "An error occured while deleting this webhook, no webhook for this tracking id");
                }
                context.Webhook.Remove(webhook);
                logger.LogDebug("SQLWebhookRepository remove webhook triggered");
                context.SaveChanges();
                logger.LogDebug("SQLWebhookRepository save changes triggered");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw new DataException(nameof(SqlHopRepository),
                                        nameof(Delete),
                                        "An unknown error occured while deleting a webhook",
                                        ex);
            }
            finally
            {
                logger.LogDebug("SQLWebhookRepository Delete executed");
            }

        }

        public IEnumerable<Webhook> GetAllWebhooks()
        {
            logger.LogDebug("SQLWebhookRepository GetAllWebhooks started");
            List<Webhook> webhooks;
            try
            {
                webhooks = context.Webhook.ToList();
                logger.LogDebug("SQLHopRepository GetAllHops get gops to list");
                if (webhooks.Count == 0)
                {
                    throw new DataNotFoundException(nameof(SqlHopRepository),
                                            nameof(GetAllWebhooks),
                                            "An error occured while getting all hops, no data found");
                }

            }
            catch (SqlException ex)
            {
                logger.LogError(ex.Message);
                throw new DataException(nameof(SqlHopRepository),
                                        nameof(GetAllWebhooks),
                                        "An SQL server error occured while getting all hops",
                                        ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw new DataException(nameof(SqlHopRepository),
                                        nameof(GetAllWebhooks),
                                        "An unknown error occured while deleting a new hop",
                                        ex);
            }

            return webhooks;

        }


        public Webhook GetCustomersByTrackingId(string trackingId)
        {
            logger.LogDebug("SQLWebhookRepository GetCustomersByTrackingId started");
            Webhook webhook = new();
            try
            {
                webhook = context.Webhook.Find(trackingId);
                logger.LogDebug("SQLWebhookRepository GetCustomersByTrackingId get webhook");
                if (webhook == null)
                {
                    throw new DataException(nameof(SqlHopRepository),
                                            nameof(GetCustomersByTrackingId),
                                            $"An error occured while getting webhook, no data with trackingId {trackingId}");
                }
            }

            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw new DataException(nameof(SqlHopRepository),
                                        nameof(GetCustomersByTrackingId),
                                        $"An unknown error occured while getting webhook with trackingId {trackingId}",
                                        ex);
            }

            return webhook;

        }
    }
}
