using AutoMapper;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using PaPl.SKS.BusinessLogic.Entities;
using PaPl.SKS.BusinessLogic.Interfaces;
using PaPl.SKS.BusinessLogic.Interfaces.Exceptions;
using PaPl.SKS.BusinessLogic.Validator;
using PaPl.SKS.DataAccess.Interfaces.Exceptions;
using PaPl.SKS.DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaPl.SKS.BusinessLogic
{
    public class TrackParcelLogic : ITrackParcelLogic
    {
        private IParcelRepository repo;
        private ILogger<TrackParcelLogic> logger;
        private readonly IMapper mapper;

        public TrackParcelLogic(IParcelRepository _repo, ILogger<TrackParcelLogic> _logger, IMapper _mapper)
        {
            repo = _repo;
            logger = _logger;
            mapper = _mapper;
        }


        public DataAccess.Entities.Parcel TrackParcel(string trackingId)
        {
            logger.LogDebug("TrackParcelLogic TrackParcel started");
            try
            {
                //Validate Tracking Id
                TrackParcelValidation(trackingId);

                var daoParcel = repo.GetParcelById(trackingId);
                
                return daoParcel;
            }
            catch (DataException ex)
            {
                logger.LogError(ex.Message);
                throw new LogicException(nameof(TrackParcelLogic),
                                        nameof(TrackParcel),
                                        "An error occured while tracking parcel",
                                        ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw new LogicException(nameof(TrackParcelLogic),
                                        nameof(TrackParcel),
                                        "An unknown error occured while tracking parcel",
                                        ex);
            }

        }
        [ExcludeFromCodeCoverage]
        private static void TrackParcelValidation(string trackingId)
        {
            Parcel validationParcel = new();
            validationParcel.TrackingId = trackingId;

            TrackingIdValidator trackingIdValidator = new();
            TrackingIdValidator validationRules = trackingIdValidator;

            ValidationResult result = validationRules.Validate(validationParcel);
        }
    }
}
