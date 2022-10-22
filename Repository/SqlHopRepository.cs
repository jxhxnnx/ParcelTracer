using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PaPl.SKS.DataAccess.Entities;
using PaPl.SKS.DataAccess.Interfaces.Exceptions;
using PaPl.SKS.DataAccess.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaPl.SKS.DataAccess.Repository
{
    public class SqlHopRepository : IHopRepository
    {
        private ILogger<SqlHopRepository> logger;
        private SQLDataContext context;
        //private bool disposed = false;


        public SqlHopRepository(DbContext context, ILogger<SqlHopRepository> _logger)
        {
            this.context = (SQLDataContext)context;
            logger = _logger;
        }

        public void BigRedButton()
        {
            logger.LogDebug("SQLHopRepository BigRedButton started");
            try
            {
                //context.Database.EnsureDeleted();
                context.Database.ExecuteSqlRaw("delete from Webhook");
                context.Database.ExecuteSqlRaw("delete from WarehouseNextHops");
                context.Database.ExecuteSqlRaw("delete from HopArrival");
                context.Database.ExecuteSqlRaw("delete from Parcel");
                context.Database.ExecuteSqlRaw("delete from Recipient");
                context.Database.ExecuteSqlRaw("delete from Hop");
                
                logger.LogDebug("SQLHopRepository BigRedButton DB delete");
                context.Database.EnsureCreated();
                logger.LogDebug("SQLHopRepository BigRedButton DB create");
                context.SaveChanges();
                logger.LogDebug("SQLHopRepository BigRedButton executed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw new DataException(nameof(SqlHopRepository),
                                        nameof(Create),
                                        "An unknown error occured while deleting and recreating db",
                                        ex);
            }


        }

        public void Create(Hop hop)
        {
            logger.LogDebug("SQLHopRepository Create started");
            try
            {
                context.Hop.Add(hop);
                logger.LogDebug("SQLHopRepository Create add hop to db");
                context.SaveChanges();
                logger.LogDebug("SQLHopRepository Create save changes");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw new DataException(nameof(SqlHopRepository),
                                        nameof(Create),
                                        "An unknown error occured while creating a new hop",
                                        ex);
            }
            finally
            {
                logger.LogDebug("SQLHopRepository Create executed");
            }



        }

        public void Delete(string code)
        {
            logger.LogDebug("SQLHopRepository Delete started");
            try
            {
                Hop hop = context.Hop.Find(code);
                if (hop == null)
                {
                    throw new DataException(nameof(SqlHopRepository),
                                            nameof(Delete),
                                            "An error occured while deleting a new hop, hop not found");
                }
                context.Hop.Remove(hop);
                logger.LogDebug("SQLHopRepository Delete remove hop");
                context.SaveChanges();
                logger.LogDebug("SQLHopRepository Delete save changes");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw new DataException(nameof(SqlHopRepository),
                                        nameof(Delete),
                                        "An unknown error occured while deleting a new hop",
                                        ex);
            }
            finally
            {
                logger.LogDebug("SQLHopRepository Delete executed");
            }



        }

        public IEnumerable<Hop> GetAllHops()
        {
            logger.LogDebug("SQLHopRepository GetAllHops started");
            List<Hop> hops;
            try
            {
                hops = context.Hop.ToList();
                logger.LogDebug("SQLHopRepository GetAllHops get gops to list");
                if (hops.Count == 0)
                {
                    throw new DataNotFoundException(nameof(SqlHopRepository),
                                            nameof(GetAllHops),
                                            "An error occured while getting all hops, no data found");
                }

            }
            catch (SqlException ex)
            {
                logger.LogError(ex.Message);
                throw new DataException(nameof(SqlHopRepository),
                                        nameof(GetAllHops),
                                        "An SQL server error occured while getting all hops",
                                        ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw new DataException(nameof(SqlHopRepository),
                                        nameof(GetAllHops),
                                        "An unknown error occured while deleting a new hop",
                                        ex);
            }

            return hops;


        }

        public Hop GetHopByCode(string code)
        {
            logger.LogDebug("SQLHopRepository GetHopByCode started");
            Hop hop = new();
            try
            {
                hop = context.Hop.Find(code);
                logger.LogDebug("SQLHopRepository GetHopByCode get hop");
                if (hop == null)
                {
                    throw new DataException(nameof(SqlHopRepository),
                                            nameof(GetHopByCode),
                                            $"An error occured while getting hops, no data with code {code}");
                }
            }

            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw new DataException(nameof(SqlHopRepository),
                                        nameof(GetHopByCode),
                                        $"An unknown error occured while getting hop with code {code}",
                                        ex);
            }

            return hop;



        }

        public void Update(Hop hop)
        {
            logger.LogDebug("SQLHopRepository Update started");
            try
            {
                context.Entry(hop).State = EntityState.Modified;
                logger.LogDebug("SQLHopRepository Update change state to modified");
                context.SaveChanges();
                logger.LogDebug("SQLHopRepository Update save changes");
            }

            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw new DataException(nameof(SqlHopRepository),
                                        nameof(Update),
                                        $"An unknown error occured while updating hop with code {hop.Code}",
                                        ex);
            }
            finally
            {
                logger.LogDebug("SQLHopRepository Update executed");
            }



        }

        public Hop GetHops()
        {
            logger.LogDebug("SQLHopRepository GetHops started");
            var papaHop = context.Hop.Single(b => (b as Warehouse).Level == 0);

            context.Entry(papaHop)
                .Collection(b => (b as Warehouse).NextHops)
                .Load();
            var hopRoot = GetRootHop(papaHop);

            return hopRoot;
        }


        public Hop GetRootHop(Hop hop)
        {
            logger.LogDebug("SQLHopRepository GetRootHop started");
            while (hop.HopType == "Warehouse")
            {
                foreach (WarehouseNextHops nextHop in (hop as Warehouse).NextHops)
                {
                    context.Entry(nextHop)
                        .Reference(b => b.Hop)
                        .Load();
                    if (nextHop.Hop.HopType == "Warehouse")
                    {
                        context.Entry(nextHop.Hop)
                            .Collection(b => (b as Warehouse).NextHops)
                            .Load();
                        GetRootHop(nextHop.Hop);
                    }
                }
                return hop;
            }
            return hop;
        }

        
       
    }
}
