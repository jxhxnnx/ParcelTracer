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
    class CodeValidator : AbstractValidator<Hop>
    {
        public CodeValidator()
        {
            RuleFor(x => x.Code)
                .Matches("")
                .NotNull();  
        }
    }
}
