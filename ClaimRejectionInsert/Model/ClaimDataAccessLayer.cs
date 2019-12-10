using AdoNetCore.AseClient;
using ClaimRejectionInsert.Models;
using ClaimRejectionInsert_DOTNET.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;

namespace ClaimRejectionInsert_DOTNET.Models
{
    public class ClaimDataAccessLayer
    {
        public string SybaseConnString { get; set; }
        public static string TableName { get; set; }


        /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        ///  <summary> </summary>
        /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public static bool IsUserAuthorized(string user, out string group)
        {

            bool result = false;

            PrincipalContext context = new PrincipalContext(ContextType.Domain, "ADS"); // GETS DOMAIN
            UserPrincipal userPrincipal = UserPrincipal.FindByIdentity(context, user); // GETS USER      
            GroupPrincipal groupPrincipal = GroupPrincipal.FindByIdentity(context, Startup.AdGroupAuthenticate); //AA-DEV-USER-XCCLAIMREJECT


            group = groupPrincipal.Name;


            if (userPrincipal != null) // VALIDATE GROUP EXISTS
            {
                if (userPrincipal.IsMemberOf(groupPrincipal)) // VALIDATE USER IS IN GROUP
                {
                    result = true;
                }
            }
            return result;

        }


        /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        ///  <summary>   </summary>
        /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public int AddAndEnrichClaims(ClaimsSubmission req, string tableName, string httpContextUser)
        {
            int claimsInserted = 0;
            TableName = tableName;
            req.SetClaims(); // Build claim list from string input
            SybaseConnString = HomeController.SybaseConnString;

            using (var conn = new AseConnection(SybaseConnString))
            {
                AseTransaction trans = conn.BeginTransaction(); // START LOCKING TABLE HERE!!!                

                try
                {
                    conn.Open();
                    AseCommand cmd;

                    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                    // INSERT NEW CLAIM DELETIONS TO STAGING TABLES
                    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                    foreach (var claim in req.ClaimList)
                    {
                        try
                        {
                            cmd = new AseCommand("SP_XC_CLAIM_REJECTION_INSERT", conn);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new AseParameter("@TABLE_NAME", TableName));
                            cmd.Parameters.Add(new AseParameter("@CLCL_ID", claim.ClaimId));
                            cmd.Parameters.Add(new AseParameter("@CREATED_BY_USER", httpContextUser));

                            if (cmd.ExecuteNonQuery() > 0) claimsInserted++;

                            else req.InvalidClaimList.Add(claim.ClaimId);
                        }
                        catch (Exception e)
                        {
                            if (e.ToString().ToUpper().Contains("DUPLICATE KEY ROW"))
                                req.DupClaimList.Add(claim.ClaimId);

                            System.Console.WriteLine("**ERROR INSERTING CLAIMS** : {0}", e.ToString());
                        }
                    }

                    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                    // NOW ENRICH THE NEWLY INSERTED CLAIM DELETIONS
                    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                    if (TableName.ToUpper().Contains("NPI"))
                    {
                        cmd = new AseCommand("SP_XC_CLAIM_REJECTION_ENRICH_NPI", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.ExecuteNonQuery();
                    }

                    else if (TableName.ToUpper().Contains("NAME"))
                    {
                        cmd = new AseCommand("SP_XC_CLAIM_REJECTION_ENRICH_NAME", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.ExecuteNonQuery();
                    }

                    else if (TableName.ToUpper().Contains("TIN"))
                    {
                        cmd = new AseCommand("SP_XC_CLAIM_REJECTION_ENRICH_TIN", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.ExecuteNonQuery();
                    }

                    trans.Commit(); // COMMIT THE TRANSACTION 
                }

                catch (Exception e)
                {
                    Console.WriteLine("*****************************************");
                    Console.WriteLine("INSERT ERROR!! --ROLLBACK TRANSACTION--");
                    Console.WriteLine("*****************************************");
                    Console.WriteLine(e.ToString());

                    trans.Rollback(); // **** Abort transaction ****
                    throw e;
                }
            }

            return claimsInserted;

        }

        /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        ///  <summary> GET: Student/Delete/5 </summary>
        /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public ClaimsSubmission GetAllClaims(ClaimsSubmission claimSub, string tableName)
        {
            ClaimDataAccessLayer.TableName = tableName;

            using (var conn = new AseConnection(HomeController.SybaseConnString))
            {
                int rowsRead = 0;

                try
                {
                    conn.Open();

                    AseCommand cmd = new AseCommand("SP_XC_CLAIM_REJECTION_READ", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new AseParameter("@TABLE_NAME", TableName));

                    rowsRead = cmd.ExecuteNonQuery();

                    if (rowsRead > 0)
                    {
                        AseDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            string claimid = reader["CLCL_ID"].ToString();
                            string createdByUser = reader["CREATED_BY_USER"].ToString();
                            string createdByService = reader["CREATED_BY_SERVICE"].ToString();
                            string createdDate = reader["CREATED_DATE"].ToString();
                            string prprNpi = reader["PRPR_NPI"].ToString();

                            Claim newClaim = new Claim()
                            {
                                ClaimId = claimid,
                                CreatedByUser = createdByUser,
                                CreatedByService = createdByService,
                                CreatedDate = createdDate,
                                PrprNpi = prprNpi
                            };

                            claimSub.ClaimList.Add(newClaim);
                        }
                    }
                }
                catch (Exception e)
                {
                    System.Console.WriteLine("**ERROR READING CLAIMS** : {0}", e.ToString());
                }

                return claimSub;
            }
        }



        /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        ///  <summary> GET: Student/Delete/5 </summary>
        /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public int DeleteClaims(List<string> claimIdList)
        {
            int claimsDeleted = 0;
            SybaseConnString = HomeController.SybaseConnString;

            using (var conn = new AseConnection(SybaseConnString))
            {
                conn.Open();

                foreach (var claimId in claimIdList)
                {
                    try
                    {
                        AseCommand cmd = new AseCommand("SP_XC_CLAIM_REJECTION_DELETE", conn);

                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new AseParameter("@TABLE_NAME", TableName));
                        cmd.Parameters.Add(new AseParameter("@CLCL_ID", claimId));

                        claimsDeleted += cmd.ExecuteNonQuery();
                    }

                    catch (Exception e)
                    {
                        System.Console.WriteLine("**ERROR INSERTING CLAIMS** : {0}", e.ToString());
                        throw e;
                    }
                }
            }
            return claimsDeleted;

        }
    }
}

