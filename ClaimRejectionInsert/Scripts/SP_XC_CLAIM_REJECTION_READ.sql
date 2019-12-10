USE APP_CONFIG
GO

IF  OBJECT_ID('SP_XC_CLAIM_REJECTION_READ') IS NOT NULL
	BEGIN
		DROP PROCEDURE SP_XC_CLAIM_REJECTION_READ
   		PRINT '<< Procedure SP_XC_CLAIM_REJECTION_READ Dropped >>'
   	END
ELSE 	
	PRINT '<< Procedure SP_XC_CLAIM_REJECTION_READ DOES NOT EXIST >>'   	
GO

/*********************************************************************************
	NAME:			SP_XC_CLAIM_REJECTION_READ							
	DESCRIPTION:    Reads staging tables PROV,NPI,TIN 
	DEVELOPER:      FRANK P. RUNFOLA
	DATE:			6/12/2019
*********************************************************************************/

CREATE PROC SP_XC_CLAIM_REJECTION_READ
   	   	@TABLE_NAME varchar(100)
AS  
    EXEC('SELECT  CLCL_ID, CREATED_BY_USER, CREATED_BY_SERVICE, CREATED_DATE, PRPR_NPI
	 	  FROM    APP_WORK..' + @TABLE_NAME + '
		  ORDER   BY CLCL_ID')
GO

IF  OBJECT_ID('SP_XC_CLAIM_REJECTION_READ') IS NOT NULL
   	PRINT '<< Procedure [SP_XC_CLAIM_REJECTION_READ] CREATED! >>'

GRANT EXECUTE ON APP_CONFIG..SP_XC_CLAIM_REJECTION_READ TO srvc_rejclaims GO

/********************************************************
				EXAMPLE SP EXECUTE
********************************************************
EXECUTE  APP_CONFIG..SP_XC_CLAIM_REJECTION_READ 'EDI_XC_PROV_NAME_REJECTED_CLAIMS'
*/