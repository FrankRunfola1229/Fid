using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ClaimRejectionInsert_DOTNET.Models;
using Microsoft.AspNetCore.Http;
using ClaimRejectionInsert.Models;
using System.Security.Principal;

namespace ClaimRejectionInsert_DOTNET.Controllers
{
    public class HomeController : Controller
    {

        public static ClaimDataAccessLayer claimDataAccessLayer = new ClaimDataAccessLayer();
        public static string SybaseConnString { get; set; }
        public ClaimsSubmission claimSub = new ClaimsSubmission();
        public ConnectionString ConnStr { get; set; }


        /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        ///  <summary>  </summary>
        /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// 

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        ///  <summary>  </summary>
        /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [HttpGet]
        public IActionResult Index(string errorMessage, string UserAuthMessage)
        {
            if (UserAuthMessage != null)
            {
                ViewBag.UserAuthMessage = UserAuthMessage;
                ViewBag.Error = errorMessage;
            }

            else
            {
                string httpContextUser = HttpContext.User.Identity.Name;
                string user = Utility.GetUser(httpContextUser);

                if (user == null)
                    user = WindowsIdentity.GetCurrent().Name;

                ViewBag.UserAuthMessage = user;
            }




            return View();
        }

        /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        ///  <summary>  GET: Home/CreateClaims  </summary>
        /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [HttpGet]
        public IActionResult CreateClaims(string tableName)
        {

            string httpContextUser = HttpContext.User.Identity.Name;
            string user = Utility.GetUser(httpContextUser);

            if (user == null)
                user = WindowsIdentity.GetCurrent().Name;

            ViewBag.TableName = tableName;
            ViewBag.GetTableShortName = Utility.GetTableShortName(tableName);
            ViewBag.UserAuthMessage = String.Format("{0} Authenticated", user);// for {1}!!", user, group);
            return View();
        }

        /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        ///  <summary> POST: Home/CreateClaims/req="BLAH" </summary>
        /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [HttpPost]
        public IActionResult CreateClaims([Bind] ClaimsSubmission req, string tableName)
        {
            ViewBag.TableName = tableName;

            string httpContextUser = HttpContext.User.Identity.Name;
            string user = Utility.GetUser(httpContextUser);

            if (user == null)
                user = WindowsIdentity.GetCurrent().Name;

            ViewBag.UserAuthMessage = String.Format("{0} Authenticated", user);// for {1}!!", user, group);
            ViewBag.GetTableShortName = Utility.GetTableShortName(tableName);

            try
            {
                int claimsInserted = claimDataAccessLayer.AddAndEnrichClaims(req, tableName, user);

                if (claimsInserted > 0)
                    ViewBag.MessageInsertedClaims = String.Format("Successful Claim Rejects = {0} ", claimsInserted);

                if (req.InvalidClaimList.Count > 0)
                {
                    string dupClaimAppend = "INVALID CLAIM ERRORS!! =  ";

                    foreach (var invalidClaims in req.InvalidClaimList)
                        dupClaimAppend += invalidClaims + "  ";

                    ViewBag.MessageInvalidClaims += dupClaimAppend;
                }

                if (req.DupClaimList.Count > 0)
                {
                    string dupClaimAppend = "DUPLICATE CLAIM ERRORS!! =  ";

                    foreach (var dupClaim in req.DupClaimList)
                        dupClaimAppend += dupClaim + "  ";

                    ViewBag.MessageDupClaims += dupClaimAppend;
                }
            }

            catch (Exception e)
            {
                ViewBag.Error = e.ToString();
            }


            return View();
        }

        /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        ///   <summary> </summary>
        /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public IActionResult GetAllClaims(string tableName, string message)
        {
            string group = "";

            string httpContextUser = HttpContext.User.Identity.Name;
            string user = Utility.GetUser(httpContextUser);

            // **********************************
            //       ATUTHENTICATE USER
            // **********************************
            try
            {
                if (!ClaimDataAccessLayer.IsUserAuthorized(user, out group))
                {
                    ViewBag.ErrorMessage += String.Format("User {0} is not authorized for {1}!!", user, group);
                    ViewBag.IsAuthenticated = "FALSE";
                    ViewBag.UserAuthMessage = String.Format("{0} not Authenticated", user);// for {1}!!", user, group);

                    return RedirectToAction("Index", new { ViewBag.ErrorMessage, ViewBag.UserAuthMessage });
                }
            }

            catch (Exception e)
            {
                ViewBag.ErrorMessage = string.Format("ERROR DURING AUTHENTICATION FOR {0} ... EXCEPTION = {1}",
                    user, e.ToString());

                return RedirectToAction("Index", new { ViewBag.ErrorMessage });
            }

            // **********************************
            //       CONNECT TO DB
            // **********************************
            try
            {
                if (ConnStr == null)
                {
                    ConnStr = Utility.AssignConnectionStringSybase();
                    SybaseConnString = Utility.GetSybaseConnection(ConnStr.DataSource, ConnStr.Port, ConnStr.Database,
                        Startup.SecretUsername, Startup.SecretPassword);
                }
            }

            catch (Exception e)
            {
                ViewBag.ErrorMessage = string.Format("ERROR WHEN TRYING TO CONNECT TO DATABASE {0} !!  ... EXCEPTION = {1}",
                    ConnStr.Database, e.ToString());

                return RedirectToAction("Index", new { ViewBag.ErrorMessage });
            }

            // **********************************
            //  READ STAGED CLAIMS TO BE DELETED
            // **********************************
            try
            {
                claimSub = claimDataAccessLayer.GetAllClaims(claimSub, tableName);
            }

            catch (Exception e)
            {
                ViewBag.ErrorMessage = string.Format("ERROR WHEN TRYING TO READ FROM {0} !! ... EXCEPTION = {1}",
                    tableName, e.ToString());

                return RedirectToAction("Index", new { ViewBag.ErrorMessage });
            }


            ViewBag.GetTableShortName = Utility.GetTableShortName(tableName);
            ViewBag.TableName = tableName;
            ViewBag.UserAuthMessage = String.Format("{0} Authenticated", user);// for {1}!!", user, group);

            ViewBag.Message = message;
            ViewBag.TableCount = claimSub.ClaimList.Count;

            return View(claimSub.ClaimList);
        }

        /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        ///  <summary> GET: Home/DeleteClaims/5 </summary>
        /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [RequestFormSizeLimit(valueCountLimit: 10000)] // Utility.cs (max number of form deletes per req)
        [HttpPost]

        public IActionResult DeleteClaims(List<string> deleteClaims, string tableName)
        {
            try
            {
                ViewBag.TableName = tableName;

                if (deleteClaims == null)
                    return NotFound();

                int claimsDeleted = claimDataAccessLayer.DeleteClaims(deleteClaims);

                if (claimsDeleted < 1)
                    ViewBag.Message = String.Format("No items selected!!", claimsDeleted);
                else
                    ViewBag.Message = String.Format("{0} Claims removed from staging! ", claimsDeleted);
            }

            catch (Exception e)
            {
                ViewBag.Error = e.ToString();
            }

            return RedirectToAction("GetAllClaims", new { tableName = ViewBag.TableName, ViewBag.Message });
        }

        /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        ///  <summary> GET: Home/Error </summary>
        /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}