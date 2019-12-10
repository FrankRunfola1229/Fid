/********************************************* 
**  Declare And Initialize Local Variables  ** 
*********************************************/
DECLARE @vStepTime DATETIME

SELECT @vStepTime = Getdate()

/****************************************************** 
**  Display Starting Date/Time In The Execution Log  ** 
******************************************************/

SELECT 
	'Start Execution Of Deleting XC Claims For Prov NPI Rejects:  Start Date/Time - ' 
      + CONVERT(CHAR(10), @vStepTime, 101) + ' @ ' + CONVERT(CHAR(2), Datepart(hh, @vStepTime))  + ':' 
      + CONVERT(CHAR(2), Datepart(mi, @vStepTime))  + ':' + CONVERT(CHAR(2), Datepart(ss, @vStepTime)) 
GO

/********************************************************************** 
**  Display Data In The Tempory EDI_XC_PROV_NPI_CLAIMS_STATUS Table  ** 
**********************************************************************/
SELECT* FROM  APP_WORK..EDI_XC_PROV_TIN_REJECTED_CLAIMS
GO

/********************************************* 
**  Declare And Initialize Local Variables  ** 
*********************************************/
DECLARE @vStepTime DATETIME

SELECT @vStepTime = Getdate()

/****************************************************** 
**  Display Starting Date/Time In The Execution Log  ** 
******************************************************/
SELECT ' '
SELECT 'Deleting XC Claims For Prov NPI Rejects:  Start Date/Time - ' 
       + CONVERT(CHAR(10), @vStepTime, 101) + ' @ ' + CONVERT(CHAR(2), Datepart(hh, @vStepTime)) + ':' 
       + CONVERT(CHAR(2), Datepart(mi, @vStepTime)) + ':' + CONVERT(CHAR(2), Datepart(ss, @vStepTime)) 
GO

/************************************************** 
**  Declare A Cursor For The CLCL Tempory Table  ** 
**************************************************/
DECLARE clmsdelcurs CURSOR FOR
  SELECT CLCL_ID, CLCL_CL_SUB_TYPE
  FROM   APP_WORK..EDI_XC_PROV_TIN_REJECTED_CLAIMS
  ORDER  BY CLCL_ID, CLCL_CL_SUB_TYPE
GO


/************************************************* 
**  Declare Parameters To Hold The Cursor Data  ** 
*************************************************/
DECLARE @iCLCL_ID CHAR(12), 
        @iCLCL_CL_SUB_TYPE CHAR(1)

/********************** 
**  Open The Cursor  ** 
**********************/
OPEN clmsdelcurs

/********************************************* 
**  Fetch First Temp Table Row Into Cursor  ** 
*********************************************/
FETCH clmsdelcurs INTO @iCLCL_ID, @iCLCL_CL_SUB_TYPE

/*******************************/
/**  Declare Local Variables  **/
/*******************************/
DECLARE @vErrNum           INT, 
        @vErrMsg VARCHAR,
        @pCLCL_ID          CHAR(12), 
        @pCLCL_CL_SUB_TYPE CHAR(1), 
        @lnRetCd INT

/**************************************** 
**  Set Variable Values For First Row  ** 
****************************************/
SELECT @vErrNum = 0,
       @vErrMsg = ' ',
       @pCLCL_ID = @iCLCL_ID,
       @pCLCL_CL_SUB_TYPE = @iCLCL_CL_SUB_TYPE,
       @lnRetCd = 0

/********************************************************** 
**  Loop Thru Temp Table With Cursor To Proces All Rows  **
**  ---------------------------------------------------- 
**  @@sqlstatus = 0  -  Means Successful Fetch           ** 
**  @@sqlstatus = 1  -  Means Error On Previous Fetch    ** 
**  @@sqlstatus = 2  -  Means End Of Result Set Reached  ** 
**********************************************************/
WHILE( @@sqlstatus != 2 )
  BEGIN
      /* Check For Errors */
      IF( @@sqlstatus = 1)
        BEGIN
            PRINT "Error In Getting CLCL_DELETE Data From Temp Table" 
            RETURN
        END

      BEGIN TRANSACTION

      /************************************************************ 
      **  Execute CLCL DELALL_XC Store Procedure To Remove Rows  ** 
      ************************************************************/
      EXECUTE @lnRetCd = PROD_FACETSxc..CMCSP_CLCL_DELALL_XC
            @pCLCL_ID = @iCLCL_ID, 
            @pCLCL_CL_SUB_TYPE = @iCLCL_CL_SUB_TYPE

      IF @lnRetCd != 0 
        BEGIN
            SELECT
              '*** ERROR *** Unable To Execute CMCSP_CLCL_DELALL_XC For CLCL_ID:  '        
              + @iCLCL_ID + '   CLCL_CL_SUB_TYPE:  ' 
              + @iCLCL_CL_SUB_TYPE + '   RetCd:  ' 
              + CONVERT(CHAR(3), @lnRetCd) 

            ROLLBACK TRANSACTION
        END
      ELSE
        BEGIN
            COMMIT TRANSACTION
        END

      /******************************************** 
      **  Fetch Next Temp Table Row Into Cursor  ** 
      ********************************************/
      FETCH clmsdelcurs INTO @iCLCL_ID, @iCLCL_CL_SUB_TYPE

      /*************************************** 
      **  Set Variable Values For Next Row  ** 
      ***************************************/
      SELECT @pCLCL_ID = @iCLCL_ID,
             @pCLCL_CL_SUB_TYPE = @iCLCL_CL_SUB_TYPE
  END

go

/********************************** 
**  Close And Delete The Cursor  ** 
**********************************/
CLOSE clmsdelcurs

DEALLOCATE clmsdelcurs

go

/********************************************* 
**  Declare And Initialize Local Variables  ** 
*********************************************/
DECLARE @vStepTime DATETIME

SELECT @vStepTime = Getdate()

/****************************************************** 
**  Display Starting Date/Time In The Execution Log  ** 
******************************************************/
SELECT ' ' 

SELECT 
	'Completed Execution Of Deleting XC Claims For Prov NPI Rejects:  Start Date/Time - ' 
	+ CONVERT(CHAR(10), @vStepTime, 101) + ' @ ' 
	+ CONVERT(CHAR(2), Datepart(hh, @vStepTime)) 
	+ ':' 
	+ CONVERT(CHAR(2), Datepart(mi, @vStepTime)) 
	+ ':' 
	+ CONVERT(CHAR(2), Datepart(ss, @vStepTime)) 

go 
--T:\DataSets_Private\EDI_Portal\837_Claims_Inbound\WebMD\Claim_Status\Production 
--bcp APP_WORK..EDI_XC_PROV_NPI_CLAIMS_STATUS out YYYYMMDD_NPI_REJECTS.txt -c -t"|" -U lkenny -S FACETS_PROD 