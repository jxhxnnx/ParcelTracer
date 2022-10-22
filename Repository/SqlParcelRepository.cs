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
    public class SqlParcelRepository : IParcelRepository
    {
        private
            SQLDataContext context;

        //private bool disposed = false;
        private ILogger<SqlParcelRepository> logger;

        public SqlParcelRepository(DbContext _context, ILogger<SqlParcelRepository> _logger)
        {
            context = (SQLDataContext)_context;
            logger = _logger;
        }


        public void Create(Parcel parcel)
        {
            logger.LogDebug("SQLParcelRepository Create started");
            try
            {
                context.Parcel.Add(parcel);
                context.SaveChanges();
            }

            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw new DataException(nameof(SqlParcelRepository),
                                        nameof(Create),
                                        "An unknown error occured while creating a new parcel",
                                        ex);
            }
            finally
            {
                logger.LogDebug("SQLParcelRepository Create executed");
            }

        }

        public void Delete(string id)
        {
            logger.LogDebug("SQLParcelRepository Delete executed");
            try
            {
                Parcel parcel = context.Parcel.Find(id);
                if (parcel == null)
                {
                    throw new DataNotFoundException(nameof(SqlParcelRepository),
                                        nameof(Delete),
                                        "An  error occured while deleting a new parcel, parcel not found");
                }
                context.Parcel.Remove(parcel);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw new DataException(nameof(SqlParcelRepository),
                                        nameof(Delete),
                                        "An unknown error occured while deleting a new parcel",
                                        ex);
            }


        }

        public IEnumerable<Parcel> GetAllParcels()
        {
            List<Parcel> parcels;
            try
            {
                logger.LogDebug("SQLParcelRepository GetAllParcels started");
                parcels = context.Parcel.ToList();
                if (parcels.Count == 0)
                {
                    throw new DataNotFoundException(nameof(SqlParcelRepository),
                                        nameof(GetAllParcels),
                                        "No data found while trying to get all parcels");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw new DataException(nameof(SqlParcelRepository),
                                        nameof(GetAllParcels),
                                        "An unknown error occured while getting all parcels",
                                        ex);
            }

            return parcels;
        }

        public Parcel GetParcelById(string id)
        {
            Parcel parcel;
            try
            {
                logger.LogDebug("SQLParcelRepository GetParcelById started");
                //parcel = context.Parcel.Find(id);

                parcel = context.Parcel.Single(prop => prop.TrackingId == id);
                GetHopArrivalsToCode(parcel);
                if (parcel == null)
                {
                    throw new DataNotFoundException(nameof(SqlParcelRepository),
                                        nameof(GetParcelById),
                                        $"No data found while getting parcel with ID {id}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw new DataException(nameof(SqlParcelRepository),
                                        nameof(GetParcelById),
                                        $"An unknown error occured while getting parcel with ID {id}",
                                        ex);
            }


            return parcel;
        }

        public void Update(Parcel toUpdateParcel, HopArrival deletArrival)
        {
            try
            {  
                logger.LogInformation("start of Update Parcel");
                var trackedParcel = context.Parcel.Find(toUpdateParcel.TrackingId);
                if(toUpdateParcel.State != Parcel.StateEnum.DeliveredEnum)
                {
                    context.RemoveRange(trackedParcel.VisitedHops);
                    context.RemoveRange(trackedParcel.FutureHops);
                    context.SaveChanges();

                    List<HopArrival> tempList = new();

                    foreach (var hopArrival in toUpdateParcel.VisitedHops)
                    {
                        var tempArrival = new HopArrival()
                        {
                            DateTime = hopArrival.DateTime,
                            Code = hopArrival.Code,
                            Description = hopArrival.Description,
                        };
                        tempList.Add(tempArrival);
                    }
                    trackedParcel.VisitedHops = tempList;

                    List<HopArrival> tempList2 = new();
                    foreach (var hopArrival in toUpdateParcel.FutureHops)
                    {
                        var tempArrival2 = new HopArrival()
                        {
                            DateTime = hopArrival.DateTime,
                            Code = hopArrival.Code,
                            Description = hopArrival.Description
                        };
                        tempList2.Add(tempArrival2);
                    }
                    trackedParcel.FutureHops = tempList2;
                }
                

                trackedParcel.State = toUpdateParcel.State;

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw new DataException(nameof(SqlParcelRepository),
                                        nameof(Update),
                                        $"An unknown error occured while updating parcel with ID {toUpdateParcel.TrackingId}",
                                        ex);
            }
        }

        public void GetHopArrivalsToCode(Parcel parcel)
        {
            logger.LogDebug("SQLParcelRepository GetHopArrivalsToCode started");
            context.Entry(parcel).Collection(c => c.FutureHops).Load();
            context.Entry(parcel).Collection(c => c.VisitedHops).Load();
            
        }
        
    }
}
