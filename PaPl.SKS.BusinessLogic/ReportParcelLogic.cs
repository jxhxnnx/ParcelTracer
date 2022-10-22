using AutoMapper;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using PaPl.SKS.BusinessLogic.Entities;
using PaPl.SKS.BusinessLogic.Interfaces;
using PaPl.SKS.BusinessLogic.Interfaces.Exceptions;
using PaPl.SKS.BusinessLogic.Validator;
using PaPl.SKS.DataAccess.Repository;
using PaPl.SKS.DataAccess.Webhook.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PaPl.SKS.BusinessLogic
{
    public class ReportParcelLogic : IReportParcelLogic
    {
        private ILogger<ReportParcelLogic> logger;
        private IWarehouseLogic whLogic;
        private IParcelRepository parcelRepo;
        private IMapper mapper;
        private IWebhookLogic webhookLogic;

        public ReportParcelLogic(ILogger<ReportParcelLogic> _logger, IWarehouseLogic _whLogic, IParcelRepository _parcelRepo, IMapper _mapper, IWebhookLogic _webhookLogic)
        {
            logger = _logger;
            whLogic = _whLogic;
            parcelRepo = _parcelRepo;
            mapper = _mapper;
            webhookLogic = _webhookLogic;
        }


        // Report that a Parcel has arrived at a certain warehouse or Truck. 
        public int ReportParcelHop(string trackingId, string code)
        {
            if (trackingId == null || code == null)
            {
                return 400;
            }

            try
            {
                logger.LogDebug("ReportParcelLogic  ReportParcelHop started");
                //Validation of data
                //----validating tracking id
                ReportParcelValidation(trackingId, code);
                logger.LogDebug("ReportParcelLogic  ReportParcelHop validation of hop code");
                var daoParcel = parcelRepo.GetParcelById(trackingId);
                Hop hop = whLogic.GetHopByCode(code);
                if (daoParcel == null || hop == null)
                {
                    return 404;
                }
                logger.LogDebug("ReportParcelLogic  ReportParcelHop get hop with code " + code);
                Parcel parcel = mapper.Map<DataAccess.Entities.Parcel, BusinessLogic.Entities.Parcel>(daoParcel);
                logger.LogDebug("ReportParcelLogic  ReportParcelHop map to BL-parcel");

                parcel = ChangeHopArrivalFromFutureToVisited(hop, parcel);
                logger.LogDebug("ReportParcelLogic  ReportParcelHop change hop arrival from future to visited");
                ChangeStateOfParcel(hop, parcel);
                logger.LogDebug("ReportParcelLogic  ReportParcelHop change state of parcel to: " + parcel.State);
                daoParcel = mapper.Map<BusinessLogic.Entities.Parcel, DataAccess.Entities.Parcel>(parcel);
                parcelRepo.Update(daoParcel, null);
                logger.LogDebug("ReportParcelLogic  ReportParcelHop map back to DAL-object");
                webhookLogic.TriggerAsync(parcel.TrackingId);

                return 200;
            }
            catch (DataException ex)
            {
                logger.LogError(ex.Message);
                throw new LogicException(nameof(ReportParcelLogic),
                                        nameof(ReportParcelHop),
                                        "An error occured while reporting parcel arrival",
                                        ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw new LogicException(nameof(ReportParcelLogic),
                                        nameof(ReportParcelHop),
                                        "An error occured while reporting parcel arrival",
                                        ex);
            }

        }

        [ExcludeFromCodeCoverage]
        private void ReportParcelValidation(string trackingId, string code)
        {
            Parcel validationParcel = new();
            validationParcel.TrackingId = trackingId;
            logger.LogDebug("ReportParcelLogic  ReportParcelHop validation of parcel");

            TrackingIdValidator trackingIdValidator = new();
            TrackingIdValidator validationRulesId = trackingIdValidator;
            ValidationResult IdResult = validationRulesId.Validate(validationParcel);
            logger.LogDebug("ReportParcelLogic  ReportParcelHop validation of tracking id");

            Hop validationHop = new();
            validationHop.Code = code;
            CodeValidator codeValidator = new();
            CodeValidator validationRulesCode = codeValidator;
            ValidationResult CodeResult = validationRulesCode.Validate(validationHop);
        }

        public void ChangeStateOfParcel(Hop hop, Parcel parcel)
        {
            logger.LogDebug("ReportParcelLogic ChangeStateOfParcel start");
            switch (hop.HopType)
            {
                case "Warehouse":
                    parcel.State = Parcel.StateEnum.InTransportEnum;
                    logger.LogDebug("ReportParcelLogic ChangeStateOfParcel new state: " + parcel.State);
                    break;
                case "Truck":
                    parcel.State = Parcel.StateEnum.InTruckDeliveryEnum;
                    logger.LogDebug("ReportParcelLogic ChangeStateOfParcel new state: " + parcel.State);
                    break;
                case "TransferWarehouse":
                    TransferWarehouse twh = (TransferWarehouse)hop;
                    var url = new Uri(twh.LogisticsPartnerUrl + "/parcel/" + parcel.TrackingId + "/");
                    var request = (HttpWebRequest)WebRequest.Create(url);
                    request.UserAgent = "Other";
                    var response = (HttpWebResponse)request.GetResponse();
                    parcel.State = Parcel.StateEnum.TransferredEnum;
                    logger.LogDebug("ReportParcelLogic ChangeStateOfParcel new state: " + parcel.State);
                    break;
            }



        }


        public Parcel ChangeHopArrivalFromFutureToVisited(Hop hop, Parcel parcel)
        {
            logger.LogDebug("ReportParcelLogic ChangeHopArrivalFromFutureToVisited start");
            if (parcel.FutureHops != null)
            {
                HopArrival tempHopArrival = new();
                foreach (var arrival in parcel.FutureHops)
                {
                    if (hop.Code == arrival.Code)
                    {
                        tempHopArrival.Code = arrival.Code;
                        tempHopArrival.Description = arrival.Description;
                        tempHopArrival.DateTime = DateTime.Now;
                        parcel.FutureHops.Remove(arrival);
                        parcel.VisitedHops.Add(tempHopArrival);
                        logger.LogDebug("ReportParcelLogic ChangeHopArrivalFromFutureToVisited changed arrival with code: " + hop.Code);
                        break;
                    }
                }
            }
            return parcel;


        }

        // Report that a Parcel has been delivered at it&#x27;s final destination address.

        public int ReportParcelDelivery(string trackingId)
        {
            logger.LogDebug("ReportParcelLogic ReportParcelDelivery start");
            if (trackingId == null || parcelRepo.GetParcelById(trackingId) == null)
            {
                return 400;
            }
            Parcel validationParcel = new();
            validationParcel.TrackingId = trackingId;
            logger.LogDebug("ReportParcelLogic ReportParcelDelivery set trackingId to " + trackingId);
            TrackingIdValidator trackingIdValidator = new();
            TrackingIdValidator validationRulesId = trackingIdValidator;
            ValidationResult IdResult = validationRulesId.Validate(validationParcel);
            logger.LogDebug("ReportParcelLogic ReportParcelDelivery validated");
            DataAccess.Entities.Parcel parcel = parcelRepo.GetParcelById(trackingId);
            logger.LogDebug("ReportParcelLogic ReportParcelDelivery get parcel with id");
            parcel.State = DataAccess.Entities.Parcel.StateEnum.DeliveredEnum;
            logger.LogDebug("ReportParcelLogic ReportParcelDelivery changed state");
            parcelRepo.Update(parcel, null);
            webhookLogic.TriggerAsync(parcel.TrackingId);
            logger.LogDebug("ReportParcelLogic ReportParcelDelivery triggered webhook");
            return 200;
        }
    }
}
