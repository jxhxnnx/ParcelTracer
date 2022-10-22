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

    class HopArrivalValidator : AbstractValidator<HopArrival>
    {
        public HopArrivalValidator()
        {
            RuleFor(x => x.Code)
             .Matches("^[A-Z0-9]{1,20}$")
             .NotNull();
            RuleFor(x => x.DateTime)
                .NotNull();
            //Maximum Character 1500 FOR NOW
            RuleFor(x => x.Description)
                .NotNull()
                .Matches("[a - zA - ZäÄüÜöÖß] *[ ][a - zA - Z0 - 9//]{0,1500}");
        }
    }
}
