using AdoNetCore.AseClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ClaimRejectionInsert_DOTNET.Models
{
    public class ClaimDataAccessLayer
    {
        /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        ///  <summary> GET: Student/Delete/5 </summary>
        /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public int AddClaims(ClaimsSubmission req)
        {
            req.SetClaims(); // Build claim list from string input

            string connStr = Startup.ConnectionString;

            int claimsInserted = 0;

            using (var conn = new AseConnection(connStr))
            {
                conn.Open();

                string sql = @"
                                INSERT  INTO  guest.EDI_XC_PROV_NAME_REJECTED_CLMS(CLCL_ID, CLCL_CL_SUB_TYPE,CREATED_BY,CREATED_DATE)
                                SELECT  CLCL_ID,  CLCL_CL_SUB_TYPE,  SUSER_NAME(),  GETDATE()
                                FROM    PROD_PORTAL..CMC_CLCL_CLAIM
                                WHERE   CLCL_ID  = @claim";

                foreach (var claim in req.ClaimList)
                {
                    try
                    {
                        AseCommand cmd = new AseCommand(sql, conn);
                        cmd.Parameters.AddWithValue("@claim", claim.ClaimId);
                        claimsInserted += cmd.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        req.ClaimDups.Add(claim.ClaimId);
                        System.Console.WriteLine("**ERROR INSERTING CLAIMS** : {0}", e.ToString());                     
                    }
                }


            }

            return claimsInserted;

        }

        /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        ///  <summary> GET: Student/Delete/5 </summary>
        /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public ClaimsSubmission GetAllClaims(ClaimsSubmission claimSub)
        {

            string connStr = Startup.ConnectionString;

            using (var conn = new AseConnection(connStr))
            {
                conn.Open();

                string sql = @"    SELECT   *
                                   FROM     guest.EDI_XC_PROV_NAME_REJECTED_CLMS
                                   ORDER BY ID ";
                try
                {

                    AseCommand cmd = new AseCommand(sql, conn);
                    AseDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        string id = reader["ID"].ToString();
                        string claimid = reader["CLCL_ID"].ToString();
                        string createdBy = reader["CREATED_BY"].ToString();
                        string createdDate = reader["CREATED_DATE"].ToString();

                        claimSub.ClaimList.Add(
                                new Claim()
                                {
                                    Id = id,
                                    ClaimId = claimid,
                                    CreatedBy = createdBy,
                                    CreatedDate = createdDate
                                });
                    }
                }

                catch (Exception e)
                {
                    System.Console.WriteLine("**ERROR READING CLAIMS** : {0}", e.ToString());
                    throw e;
                }

                return claimSub;
            }
        }

        /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        ///  <summary> GET: Student/Delete/5 </summary>
        /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public Claim GetClaim(string claimId)
        {
            string connStr = Startup.ConnectionString;
            Claim claim = new Claim();

            using (var conn = new AseConnection(connStr))
            {
                conn.Open();

                string sql = @"    SELECT   *
                                   FROM    guest.EDI_XC_PROV_NAME_REJECTED_CLMS
                                   WHERE   CLCL_ID  = @claimId";
                try
                {
                    AseCommand cmd = new AseCommand(sql, conn);
                    cmd.Parameters.Add("@claimId", AseDbType.VarChar, 12);
                    cmd.Parameters["@claimId"].Value = claimId;


                    AseDataReader reader = cmd.ExecuteReader();



                    while (reader.Read())
                    {
                        claim.Id = reader["ID"].ToString();
                        claim.ClaimId = reader["CLCL_ID"].ToString();
                        claim.CreatedBy = reader["CREATED_BY"].ToString();
                        claim.CreatedDate = reader["CREATED_DATE"].ToString();
                    }
                }

                catch (Exception e)
                {
                    System.Console.WriteLine("**ERROR READING CLAIMS** : {0}", e.ToString());
                    throw e;
                }

                return claim;
            }
        }


        /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        ///  <summary> GET: Student/Delete/5 </summary>
        /// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public int DeleteClaims(string claimId)
        {

            int claimsDeleted = 0;
            string connStr = Startup.ConnectionString;


            using (var conn = new AseConnection(connStr))
            {
                conn.Open();

                string sql = @"                              
                                DELETE  
                                FROM    guest.EDI_XC_PROV_NAME_REJECTED_CLMS
                                WHERE   CLCL_ID = @claim";
                try
                {
                    AseCommand cmd = new AseCommand(sql, conn);
                    cmd.Parameters.Add("@claim", AseDbType.VarChar, 12);
                    cmd.Parameters["@claim"].Value = claimId;
                    claimsDeleted += cmd.ExecuteNonQuery();
                }

                catch (Exception e)
                {
                    System.Console.WriteLine("**ERROR INSERTING CLAIMS** : {0}", e.ToString());
                    throw e;
                }
            }
            return claimsDeleted;

        }
    }
}

