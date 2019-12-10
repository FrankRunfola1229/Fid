using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ClaimRejectionInsert_DOTNET.Models
{
    public class Claim
    {
        public string Id { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        [Required]
        public string ClaimId { get; set; }


    }
}
