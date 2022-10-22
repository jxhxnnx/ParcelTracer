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

    public class Webhook
    {
        [Key]
        public int Id { get; set; }
        public string TrackingId { get; set; }
        public string CustomerUrl { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
