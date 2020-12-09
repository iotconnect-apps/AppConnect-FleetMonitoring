﻿/*******************************************************************
DECLARE @output INT = 0
	,@fieldName				nvarchar(255)	

EXEC [dbo].[Device_ValidateDelete]		 
	@companyguid			= '895019CF-1D3E-420C-828F-8971253E5784'
	,@guid					= 'E9F77DD4-78BC-4461-9D00-64D927998ABE'
	,@invokinguser			= '200EDCFA-8FF1-4837-91B1-7D5F967F5129'
	,@version				= 'v1'                         
	,@output				= @output									OUTPUT
	,@fieldname				= @fieldName								OUTPUT	

SELECT @output status, @fieldName fieldName


*******************************************************************/
CREATE PROCEDURE [dbo].[Device_ValidateDelete]
(	@companyguid		UNIQUEIDENTIFIER
	,@guid				UNIQUEIDENTIFIER
	,@currentDate DateTime =NULL
	,@invokinguser		UNIQUEIDENTIFIER
	,@version			nvarchar(10)              
	,@output			SMALLINT			OUTPUT
	,@fieldname			nvarchar(255)		OUTPUT
	,@culture			nvarchar(10)		= 'en-Us'	
	,@enabledebuginfo	CHAR(1)				= '0'
)
AS
BEGIN
    SET NOCOUNT ON

    IF (@enabledebuginfo = 1)
	BEGIN
        DECLARE @Param XML 
        SELECT @Param = 
        (
            SELECT 'Device_ValidateDelete' AS '@procName'             
			, CONVERT(nvarchar(MAX),@companyguid) AS '@companyguid'
			, CONVERT(nvarchar(MAX),@guid) AS '@guid'
			, CONVERT(nvarchar(MAX),@invokinguser) AS '@invokinguser'
			, CONVERT(nvarchar(MAX),@version) AS '@version'
            FOR XML PATH('Params')
	    ) 
	    INSERT INTO DebugInfo(data, dt) VALUES(Convert(nvarchar(MAX), @Param), GETDATE())
    END                    
    
   BEGIN TRY            
        SET @output = 1
		SET @fieldname = 'Success'
		DECLARE @fleetGuid UNIQUEIDENTIFIER 
		DECLARE @dt DATETIME = ISNULL(@currentDate,GETUTCDATE())
		SET @fleetGuid=(SELECT TOP 1 [fleetGuid] FROM [Device] (NOLOCK) WHERE [guid]=@guid and [companyguid] = @companyguid AND [isdeleted]=0)
		
		IF @fleetGuid IS NOT NULL 
		BEGIN
				IF OBJECT_ID('tempdb..#tripsDD') IS NOT NULL BEGIN DROP TABLE #tripsDD END
				CREATE TABLE #tripsDD ( [fleetGuid] UNIQUEIDENTIFIER, [startDateTime] DATETIME,[endDateTime] DATETIME,[isStarted] BIT,[isCompleted] BIT) 
				INSERT INTO #tripsDD ([fleetGuid], [startDateTime],[endDateTime],[isStarted] ,[isCompleted]) 
				SELECT [fleetGuid], [startDateTime],			
				(SELECT top 1 endDateTime from [dbo].[TripStops] (NOLOCK) WHERE [tripGuid] = T.[Guid] AND [isDeleted] = 0 ORDER BY [endDateTime] DESC) AS [endDateTime] 			
					,[isStarted] ,[isCompleted] FROM [dbo].[Trip] T (NOLOCK) 
					WHERE [fleetGuid] = @fleetGuid AND [isDeleted] = 0 
				IF EXISTS (SELECT 1 from #tripsDD WHERE ([isStarted]=1 AND [isCompleted]=0) and [fleetGuid]=@fleetGuid)--(@dt >= [startDateTime] AND @dt <= [endDateTime])
				BEGIN
					SET @output = -3
					SET @fieldname = 'OnGoingTripExists'						
					RETURN;
				END
				
				IF EXISTS (SELECT 1 from dbo.[DeviceMaintenance] DM (NOLOCK) WHERE [isDeleted] = 0 AND ISNULL([isCompleted],0)=0 AND @dt >= [startDateTime] AND @dt <= [endDateTime] and [deviceGuid]=@fleetGuid)
				BEGIN
					SET @output = -2
					SET @fieldname = 'OnGoingMaintenanceExists'						
					RETURN;
				END
				IF EXISTS (SELECT 1 from dbo.[Driver] D (NOLOCK) WHERE [isDeleted] = 0 and [fleetGuid]=@fleetGuid)
				BEGIN
					SET @output = -1
					SET @fieldname = 'DriverExists'						
					RETURN;
				END
				IF @fleetGuid IS NOT NULL
				BEGIN
					SET @output = -4
					SET @fieldname = 'FleetAllocatedToDevice'
					RETURN;
				END
			END
		
			
	END TRY        
	
	BEGIN CATCH	
		DECLARE @errorReturnMessage nvarchar(MAX)

		SET @output = 0

		SELECT @errorReturnMessage = 
			ISNULL(@errorReturnMessage, '') +  SPACE(1)   + 
			'ErrorNumber:'  + ISNULL(CAST(ERROR_NUMBER() as nvarchar), '')  + 
			'ErrorSeverity:'  + ISNULL(CAST(ERROR_SEVERITY() as nvarchar), '') + 
			'ErrorState:'  + ISNULL(CAST(ERROR_STATE() as nvarchar), '') + 
			'ErrorLine:'  + ISNULL(CAST(ERROR_LINE () as nvarchar), '') + 
			'ErrorProcedure:'  + ISNULL(CAST(ERROR_PROCEDURE() as nvarchar), '') + 
			'ErrorMessage:'  + ISNULL(CAST(ERROR_MESSAGE() as nvarchar(max)), '')
		RAISERROR (@errorReturnMessage, 11, 1)  
 
		IF (XACT_STATE()) = -1  
		BEGIN   
			ROLLBACK TRANSACTION 
		END   
		IF (XACT_STATE()) = 1  
		BEGIN      
			ROLLBACK TRANSACTION  
		END   
		RAISERROR (@errorReturnMessage, 11, 1)   
	END CATCH
END