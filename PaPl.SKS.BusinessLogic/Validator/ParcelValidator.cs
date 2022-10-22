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

    public class ParcelValidator : AbstractValidator<Parcel>
    {

        public ParcelValidator()
        {
            RuleFor(x => x.Weight)
               .GreaterThan(0)
               .NotNull();
            RuleFor(x => x.State)
               .NotNull();
            RuleFor(x => x.Sender)
              .NotNull();
            RuleFor(x => x.Recipient)
              .NotNull();
            RuleFor(x => x.TrackingId)
              .Matches("^[A-Z0-9]{9}$-")
              .NotNull();
            RuleFor(x => x.VisitedHops)
              .NotNull();
            RuleFor(x => x.FutureHops)
              .NotNull();
        }
    }
}
