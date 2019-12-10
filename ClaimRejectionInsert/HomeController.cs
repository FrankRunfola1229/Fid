using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ClaimRejectionInsert_DOTNET.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols;
using AdoNetCore.AseClient;
using System.Configuration;
using System.Data;
using System.Net;


namespace ClaimRejectionInsert_DOTNET.Controllers
{
    public class HomeController : Controller
    {
        public ClaimDataAccessLayer objClaim = new ClaimDataAccessLayer();
        public ClaimsSubmission claimSub = new ClaimsSubmission();


        /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        ///  <summary>  </summary>
        /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public IActionResult TableSelect()
        {

            return View();
            // return RedirectToAction("GetAllClaims");
        }

        /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        ///  <summary>  GET: Claim/Create  </summary>
        /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public IActionResult CreateClaims(string tableName)
        {
            ViewBag.tableName = tableName;
            return View();
        }

        /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        ///  <summary>  POST: Claim/Create  </summary>
        /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [HttpPost]
        public IActionResult CreateClaims([Bind] ClaimsSubmission req, string tableName)
        {
            ViewBag.tableName = tableName;

            int claimsInserted = 0;

            if (ModelState.IsValid)
            {
                claimsInserted = objClaim.AddClaims(req);

                if (claimsInserted < 1)
                    ViewBag.Message = String.Format("*NO CLAIMS FOUND*", claimsInserted);
                else
                    ViewBag.Message = String.Format("{0} Claims added successfully! ", claimsInserted);

                if (req.ClaimDups.Count > 0)
                {
                    string dupClaimAppend = "\n*WARNING* : The following claim inserts failed as duplicates..\n";

                    foreach (var dupClaim in req.ClaimDups)
                    {
                        dupClaimAppend += dupClaim + "\n";
                    }
                    ViewBag.Message += dupClaimAppend;
                }
            }
            return View();
        }

        /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        ///   <summary> GET: Claim/GetClaims/5 </summary>
        /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public IActionResult GetAllClaims(string tableName)
        {
            ViewBag.tableName = tableName;

            if (ModelState.IsValid)
                claimSub = objClaim.GetAllClaims(claimSub);

            return View(claimSub.ClaimList);
        }

        /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        ///  <summary> GET: Student/Delete/5 </summary>
        /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [HttpGet]
        public IActionResult DeleteClaims(string claimId)
        {
            if (claimId == null)
                return NotFound();

            Claim claim = objClaim.GetClaim(claimId);


            return View(claim);
        }


        /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        ///  <summary> POST: Claim/Delete/5 </summary>
        /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [HttpPost]
        public IActionResult DeleteClaimsConfirm(string claimId)
        {
            int claimsDeleted = 0;
            claimsDeleted = objClaim.DeleteClaims(claimId);

            if (claimsDeleted < 1)
                ViewBag.Message = String.Format("*NO CLAIMS FOUND*", claimsDeleted);
            else
                ViewBag.Message = String.Format("{0} Claims Deleted successfully! ", claimsDeleted);

            return View();
            // return RedirectToAction("GetAllClaims");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
