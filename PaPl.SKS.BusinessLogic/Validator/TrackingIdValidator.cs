using FluentValidation;
using PaPl.SKS.BusinessLogic.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaPl.SKS.BusinessLogic.Validator
{
    [ExcludeFromCodeCoverage]

    public class TrackingIdValidator : AbstractValidator<Parcel>
    {
        public TrackingIdValidator()
        {
            RuleFor(x => x.TrackingId)
             .Matches("[A-Z0-9]")
             .NotNull();
        }
    }
}
