using ClaimRejectionInsert.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace ClaimRejectionInsert_DOTNET.Models
{
    class Utility
    {
        public static string GetSybaseConnection(string dataSource, string port, string db, string uid, string pwd)
        {
            return String.Format("DataSource={0};Port={1};Database={2};Uid={3};Pwd={4};", dataSource, port, db, uid, pwd);
        }

        public static string GetUser(string user)
        {          

            if (user == null)
                user = WindowsIdentity.GetCurrent().Name;


            if (user.Contains("\\"))
                user = user.Substring(user.IndexOf("\\") + 1);

            return user;
        }

        public static ConnectionString AssignConnectionStringSybase()// DEV_idb01s_syst
        {
            ConnectionString connStrO = new ConnectionString();

            try
            {
                string connString = Startup.ConnectionString;

                connStrO.ConnectionStringS = connString.Replace(@"\\", @"\");

                string[] connStrArray = connString.Split(';');

                connStrO.DataSource = connStrArray[0].Substring(connStrArray[0].IndexOf('=') + 1);
                connStrO.Port = connStrArray[1].Substring(connStrArray[1].IndexOf('=') + 1);
                connStrO.Database = connStrArray[2].Substring(connStrArray[2].IndexOf('=') + 1);
                connStrO.Uid = connStrArray[3].Substring(connStrArray[3].IndexOf('=') + 1);
                connStrO.Pwd = connStrArray[4].Substring(connStrArray[3].IndexOf('=') + 1);

            }

            catch (Exception ex)
            {
                connStrO.ConnectionStringS = ex.ToString();
            }

            return new ConnectionString()
            {
                ConnectionStringS = connStrO.ConnectionStringS,
                DataSource = connStrO.DataSource,
                Port = connStrO.Port,
                Database = connStrO.Database,
                Uid = connStrO.Uid,
                Pwd = connStrO.Pwd
            };
        }

        public static string GetTableShortName(string tableName)
        {
            if (tableName.ToUpper().Contains("TIN")) tableName = "TIN";
            else if (tableName.ToUpper().Contains("NAME")) tableName = "NAME";
            else { tableName = "NPI"; }

            return tableName;
        }

}
}

/**
public static class Utility
{
    //APP_WORK..EDI_XC_PROV_NPI_CLAIMS_STATUS
    public const string updateResponseDataNPI = @"
    UPDATE  APP_WORK..EDI_XC_PROV_NPI_REJECTED_CLAIMS         
    SET    
        CLCL_BATCH_ID = ISNULL(RTRIM(CLCL.CLCL_BATCH_ID),'FCNY0001'),
        CLCL_ID = CLCL.CLCL_ID,
        CLED_EXT_REF = CLED.CLED_EXT_REF,
        CLCL_CUR_STS = 'QA0',
        CLCL_INPUT_DT = '12/31/2999 12:00:00.000 AM',
        CLCL_TOT_CHG = CLCL.CLCL_TOT_CHG,
        DBASE = 'XCREJ',
        CLED_TRAD_PARTNER = CLED.CLED_TRAD_PARTNER,
        SYS_DT = '12/31/2999 12:00:00.000 AM',
        LAST_NAME = CLME.CLME_LAST_NAME,
        FIRST_NAME = CLME.CLME_FIRST_NAME,

        FROM_DT = (SELECT  MIN(CDML.CDML_FROM_DT)
                   FROM    PROD_FACETSxc..CMC_CDML_CL_LINE CDML
                   WHERE   CLCL.CLCL_ID = CDML.CLCL_ID ),

        TO_DT =   (SELECT   MAX(CDML.CDML_TO_DT)
                   FROM    PROD_FACETSxc..CMC_CDML_CL_LINE CDML
                   WHERE CLCL.CLCL_ID = CDML.CLCL_ID ),

        CLCL_PA_ACCT_NO = CLCL.CLCL_PA_ACCT_NO,
        SBSB_ID = CLME.CLME_SBSB_ID,
        MCTN_ID = ISNULL(RTRIM(CLPR.CLPR_TAX_ID), CLPR.CLPR_SSN),
        PRPR_NPI = CLPR.CLPR_NPI,
        PRPR_NAME = ISNULL(RTRIM(CLPR.CLPR_FAC_NAME), RTRIM(CLPR.CLPR_LAST_NAME) + ',' + RTRIM(CLPR.CLPR_FIRST_NAME))
    
    FROM 
        PROD_FACETSxc..CMC_CLCL_CLAIM CLCL,
        PROD_FACETSxc..CMC_CLED_EDI_DATA CLED,
        PROD_FACETSxc..CMC_CLME_MEMBER CLME,
        PROD_FACETSxc..CMC_CLPR_PROVIDER CLPR,
        APP_WORK..EDI_XC_PROV_NPI_REJECTED_CLAIMS TMP

    WHERE
        CLCL.CLCL_ID        = CLED.CLCL_ID
        AND CLCL.CLCL_ID    = CLME.CLCL_ID
        AND CLCL.CLCL_ID    = TMP.CLCL_ID
        AND CLCL.CLCL_ID    = CLPR.CLCL_ID
        AND CLPR.CLPR_TYPE  = 'SE'";



    //APP_WORK..EDI_XC_PROV_TIN_CLAIMS_STATUS
    public const string updateResponseDataTIN = @"
UPDATE  APP_WORK..EDI_XC_PROV_TIN_REJECTED_CLAIMS
SET 	 
        CLCL_BATCH_ID =ISNULL(RTRIM(CLCL.CLCL_BATCH_ID),'FCNY0001'),
        CLED_EXT_REF = CLED.CLED_EXT_REF,
        CLCL_CUR_STS = 'BE',
        CLCL_INPUT_DT ='12/31/2999 12:00:00.000 AM',
        CLCL_TOT_CHG = CLCL.CLCL_TOT_CHG,
        DBASE = 'XCREJ',
        CLED_TRAD_PARTNER = CLED.CLED_TRAD_PARTNER,
        SYS_DT = '12/31/2999 12:00:00.000 AM',
        LAST_NAME = CLME.CLME_LAST_NAME,
        FIRST_NAME = CLME.CLME_FIRST_NAME,

        FROM_DT = (SELECT  MIN(CDML.CDML_FROM_DT)
                   FROM    PROD_FACETSxc..CMC_CDML_CL_LINE CDML
                   WHERE   CLCL.CLCL_ID = CDML.CLCL_ID ),

        TO_DT =   (SELECT  MAX(CDML.CDML_TO_DT)
                   FROM    PROD_FACETSxc..CMC_CDML_CL_LINE CDML
                   WHERE   CLCL.CLCL_ID = CDML.CLCL_ID ),

        CLCL_PA_ACCT_NO = CLCL.CLCL_PA_ACCT_NO,
        SBSB_ID = CLME.CLME_SBSB_ID,
        MCTN_ID = ISNULL(RTRIM(CLPR.CLPR_TAX_ID), CLPR.CLPR_SSN),
        PRPR_NPI = CLPR.CLPR_NPI,
        PRPR_NAME = ISNULL(RTRIM(CLPR.CLPR_FAC_NAME), RTRIM(CLPR.CLPR_LAST_NAME) + ',' + RTRIM(CLPR.CLPR_FIRST_NAME))

    FROM
        PROD_FACETSxc..CMC_CLCL_CLAIM CLCL,
        PROD_FACETSxc..CMC_CLED_EDI_DATA CLED,
        PROD_FACETSxc..CMC_CLME_MEMBER CLME,
        PROD_FACETSxc..CMC_CLPR_PROVIDER CLPR,
        APP_WORK..EDI_XC_PROV_TIN_REJECTED_CLAIMS TMP
    WHERE
        CLCL.CLCL_ID    = CLED.CLCL_ID
        AND CLCL.CLCL_ID    = CLME.CLCL_ID
        AND CLCL.CLCL_ID    = TMP.CLCL_ID
        AND CLCL.CLCL_ID    = CLPR.CLCL_ID
        AND CLPR.CLPR_TYPE  = 'SE'";


    //APP_WORK..EDI_XC_PROV_NAME_CLAIMS_STATUS
    public const string updateResponseDataProvName = @"
UPDATE     APP_WORK..EDI_XC_PROV_NAME_REJECTED_CLAIMS   
SET        
    CLCL_BATCH_ID = ISNULL(RTRIM(CLCL.CLCL_BATCH_ID),'FCNY0001'),
    CLCL_ID = CLCL.CLCL_ID,
    CLED_EXT_REF = CLED.CLED_EXT_REF,
    CLCL_CUR_STS = '565',
    CLCL_INPUT_DT = '12/31/2999 12:00:00.000 AM',
    CLCL_TOT_CHG = CLCL.CLCL_TOT_CHG,
    DBASE = 'XCREJ',
    CLED_TRAD_PARTNER = CLED.CLED_TRAD_PARTNER,
    SYS_DT = '12/31/2999 12:00:00.000 AM',
    LAST_NAME = CLME.CLME_LAST_NAME,
    FIRST_NAME = CLME.CLME_FIRST_NAME,

    FROM_DT = (SELECT   MIN(CDML.CDML_FROM_DT)
               FROM     PROD_FACETSxc..CMC_CDML_CL_LINE CDML
               WHERE   CLCL.CLCL_ID = CDML.CLCL_ID ),

    TO_DT =   (SELECT   MAX(CDML.CDML_TO_DT)
               FROM     PROD_FACETSxc..CMC_CDML_CL_LINE CDML
               WHERE   CLCL.CLCL_ID = CDML.CLCL_ID ),

    CLCL_PA_ACCT_NO = CLCL.CLCL_PA_ACCT_NO,
    SBSB_ID = CLME.CLME_SBSB_ID,
    MCTN_ID = ISNULL(RTRIM(CLPR.CLPR_TAX_ID), CLPR.CLPR_SSN),
    PRPR_NPI = CLPR.CLPR_NPI,
    PRPR_NAME = ISNULL(RTRIM(CLPR.CLPR_FAC_NAME), RTRIM(CLPR.CLPR_LAST_NAME) + ',' + RTRIM(CLPR.CLPR_FIRST_NAME))

FROM 
    PROD_FACETSxc..CMC_CLCL_CLAIM CLCL,
    PROD_FACETSxc..CMC_CLED_EDI_DATA CLED,
    PROD_FACETSxc..CMC_CLME_MEMBER CLME,
    PROD_FACETSxc..CMC_CLPR_PROVIDER CLPR,
    APP_WORK..EDI_XC_PROV_NAME_REJECTED_CLAIMS TMP

WHERE 
    CLCL.CLCL_ID    = CLED.CLCL_ID  AND
    CLCL.CLCL_ID    = CLME.CLCL_ID  AND
    CLCL.CLCL_ID    = TMP.CLCL_ID   AND
    CLCL.CLCL_ID    = CLPR.CLCL_ID  AND
    CLPR.CLPR_TYPE  = 'SE'";
}
**/
