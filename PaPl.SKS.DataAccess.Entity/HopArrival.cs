using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaPl.SKS.DataAccess.Entities
{
    [ExcludeFromCodeCoverage]

    public class HopArrival
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public DateTime? DateTime { get; set; }
    }
}
