using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ClaimRejectionInsert_DOTNET.Models
{
    public class ClaimsSubmission
    {
        [Required]
        public string ClaimsString { get; set; }

        public List<string> ClaimDups { get; set; }

        public List<Claim> ClaimList { get; set; }

        public ClaimsSubmission()
        {
            ClaimList = new List<Claim>();
            ClaimDups = new List<string>();
        }

        public void SetClaims()
        {
            string[] claimsString = ClaimsString.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var claim in claimsString)
            {
                ClaimList.Add(new Claim() { ClaimId = claim });
            }
        }



    }

}