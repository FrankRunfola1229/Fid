﻿USE APP_CONFIG
GO

IF  OBJECT_ID('SP_XC_CLAIM_REJECTION_INSERT') IS NOT NULL
	BEGIN
		DROP PROCEDURE SP_XC_CLAIM_REJECTION_INSERT
   		PRINT '<< Procedure SP_XC_CLAIM_REJECTION_INSERT Dropped >>'
   	END
ELSE 	
	PRINT '<< Procedure SP_XC_CLAIM_REJECTION_INSERT DOES NOT EXIST >>'   	
GO

/*********************************************************************************
	NAME:			SP_XC_CLAIM_REJECTION_INSERT							
	DESCRIPTION:    INSERT CLAIM REJECTS TO STAGING TABLES
	DEVELOPER:      FRANK P. RUNFOLA
	DATE:			5/17/2019
*********************************************************************************/
CREATE PROC SP_XC_CLAIM_REJECTION_INSERT

   	   	@TABLE_NAME varchar(100),
	    @CLCL_ID  varchar(30),
		@CREATED_BY_USER  varchar(20)
AS

    EXEC('INSERT INTO APP_WORK..' + @TABLE_NAME + '(CLCL_ID, CLCL_CL_SUB_TYPE, CREATED_BY_USER, CREATED_BY_SERVICE, CREATED_DATE)
	 	  SELECT   CONVERT(CHAR, @CLCL_ID),  CLCL_CL_SUB_TYPE,  @CREATED_BY_USER, SUSER_NAME(),  GETDATE()
	 	  FROM    PROD_FACETSxc..CMC_CLCL_CLAIM
		  WHERE   CLCL_ID = @CLCL_ID')
GO


IF  OBJECT_ID('SP_XC_CLAIM_REJECTION_INSERT') IS NOT NULL
   	PRINT '<< Procedure [SP_XC_CLAIM_REJECTION_INSERT] CREATED! >>'


GRANT EXECUTE ON APP_CONFIG..SP_XC_CLAIM_REJECTION_INSERT TO srvc_rejclaims GO

/********************************************************
				EXAMPLE SP EXECUTE
********************************************************

EXECUTE  APP_CONFIG..SP_XC_CLAIM_REJECTION_INSERT

		'EDI_XC_PROV_NAME_REJECTED_CLAIMS', 
		'016525268900',
		 'FD179893'

SELECT *
FROM APP_WORK..EDI_XC_PROV_NAME_REJECTED_CLAIMS
WHERE   CLCL_ID = '<>'

SELECT *
FROM APP_WORK..EDI_XC_PROV_NPI_REJECTED_CLAIMS
WHERE   CLCL_ID = '<>'

SELECT *
FROM APP_WORK..EDI_XC_PROV_TIN_REJECTED_CLAIMS
WHERE   CLCL_ID = '<>'
*/