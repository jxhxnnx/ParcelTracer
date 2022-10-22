using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using PaPl.SKS.BusinessLogic.Entities;

namespace PaPl.SKS.BusinessLogic.Validator
{
    [ExcludeFromCodeCoverage]

    public class TransferWarehouseValidator : AbstractValidator<TransferWarehouse>
    {
        public TransferWarehouseValidator()
        {
            RuleFor(x => x.RegionGeoJson)
              .NotNull();
            RuleFor(x => x.LogisticsPartner)
                .NotNull();
            RuleFor(x => x.LogisticsPartnerUrl)
                .NotNull();
        }
    }
}
