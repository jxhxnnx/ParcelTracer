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
    public class RecipientValidator : AbstractValidator<Recipient>
    {
        List<string> validCountries = new List<string>() { "Austria", "Österreich" };
        public RecipientValidator()
        {
            RuleFor(x => x.PostalCode)
                .Matches("A-[0-9]{4}")
                .When(x => validCountries.Contains(x.Country))
                .NotNull();
            RuleFor(x => x.Street)
                //Maybe include " " and "-" with custum BeLike function
                .Matches("[a-zA-ZäÄüÜöÖß]*[ ][a-zA-Z0-9//]*")
                .When(x => validCountries.Contains(x.Country))
                .NotNull();
            RuleFor(x => x.City)
               .Matches("[A-ZÄÜÖ][a-zA-Z- äüöÄÜÖß]*")
               .When(x => validCountries.Contains(x.Country))
               .NotNull();
            RuleFor(x => x.Name)
                .NotNull();
            RuleFor(x => x.Country)
              .NotNull();
        }
    }
}

