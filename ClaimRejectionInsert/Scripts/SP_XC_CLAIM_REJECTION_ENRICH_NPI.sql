USE APP_CONFIG
GO

IF  OBJECT_ID('SP_XC_CLAIM_REJECTION_ENRICH_NPI') IS NOT NULL
	BEGIN
		DROP PROCEDURE SP_XC_CLAIM_REJECTION_ENRICH_NPI
   		PRINT '<< Procedure SP_XC_CLAIM_REJECTION_ENRICH_NPI Dropped >>'
   	END
ELSE 	
	PRINT '<< Procedure SP_XC_CLAIM_REJECTION_ENRICH_NPI DOES NOT EXIST >>'   	
GO

/*********************************************************************************
	NAME:			SP_XC_CLAIM_REJECTION_ENRICH_NPI							
    DESCRIPTION:    ENRICH EDI_XC_PROV_NPI_REJECTED_CLAIMS records for response 
                    file to clearing house
	DEVELOPER:      FRANK P. RUNFOLA
	DATE:			5/17/2019
*********************************************************************************/
CREATE PROC SP_XC_CLAIM_REJECTION_ENRICH_NPI
AS
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
            AND CLPR.CLPR_TYPE  = 'SE'
GO

IF  OBJECT_ID('SP_XC_CLAIM_REJECTION_ENRICH_NPI') IS NOT NULL
   	PRINT '<< Procedure [SP_XC_CLAIM_REJECTION_ENRICH_NPI] CREATED! >>'

GRANT EXECUTE ON APP_CONFIG..SP_XC_CLAIM_REJECTION_ENRICH_NPI TO Claims_Rejection_Role GO


/********************************************************
				EXAMPLE SP EXECUTE
********************************************************
EXECUTE  APP_CONFIG..SP_XC_CLAIM_REJECTION_ENRICH_NPI

SELECT *
FROM APP_WORK..EDI_XC_PROV_NPI_REJECTED_CLAIMS
*/