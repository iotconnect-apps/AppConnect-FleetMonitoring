CREATE  PROCEDURE [dbo].[Fleet_Delete]
(	
@companyguid UNIQUEIDENTIFIER 
	,@guid			UNIQUEIDENTIFIER 		 
	,@currentDate DateTime =NULL
	,@invokingUser	UNIQUEIDENTIFIER
	,@version		nvarchar(10)
	,@output		SMALLINT			OUTPUT    
	,@fieldName		nvarchar(100)		OUTPUT   
	,@culture		nvarchar(10)		= 'en-Us'
	,@enableDebugInfo	CHAR(1)			= '0'
)	
AS
BEGIN

	SET @enableDebugInfo = 1
	SET NOCOUNT ON
	DECLARE @dt DATETIME = GETUTCDATE()
    IF (@enableDebugInfo = 1)
	BEGIN
        DECLARE @Param XML 
        SELECT @Param = 
        (
            SELECT 'Fleet_Delete' AS '@procName'             
            	, CONVERT(nvarchar(MAX),@companyguid) AS '@companyguid' 
            	, CONVERT(nvarchar(MAX),@guid) AS '@guid' 
				 ,@currentDate As '@currentDate'				
				, CONVERT(nvarchar(MAX),@invokingUser) AS '@invokingUser'
            	, CONVERT(nvarchar(MAX),@version) AS '@version' 
            	, CONVERT(nvarchar(MAX),@output) AS '@output' 
            	, @fieldName AS '@fieldName'   
            FOR XML PATH('Params')
	    ) 
	    INSERT INTO DebugInfo(data, dt) VALUES(Convert(nvarchar(MAX), @Param), @dt)
    END       
	
	DECLARE @poutput		SMALLINT
			,@pFieldName	nvarchar(100)
    
	  IF(@poutput!=1)
      BEGIN
        SET @output = @poutput
        SET @fieldName = @pfieldName
        RETURN;
      END


	SET @output = 1
	SET @fieldName = 'Success'

	BEGIN TRY
			IF EXISTS (SELECT TOP 1 1 FROM [Fleet] (NOLOCK) WHERE @guid is not null and [companyguid] = @companyguid AND [isdeleted]=0)
			BEGIN
				IF OBJECT_ID('tempdb..#tripsFD') IS NOT NULL BEGIN DROP TABLE #tripsFD END
				CREATE TABLE #tripsFD ( [fleetGuid] UNIQUEIDENTIFIER, [startDateTime] DATETIME,[endDateTime] DATETIME,[isStarted] BIT,[isCompleted] BIT) 
		
				INSERT INTO #tripsFD ([fleetGuid], [startDateTime],[endDateTime],[isStarted],[isCompleted]) 
				SELECT [fleetGuid], [startDateTime],			
				(SELECT top 1 endDateTime from [dbo].[TripStops] (NOLOCK) WHERE [tripGuid] = T.[Guid] AND [isDeleted] = 0 ORDER BY [endDateTime] DESC) AS [endDateTime] 			
					,[isStarted],[isCompleted] FROM [dbo].[Trip] T (NOLOCK) 
					WHERE [fleetGuid] = @guid AND [isDeleted] = 0 		

				IF EXISTS (SELECT 1 from #tripsFD WHERE [isStarted]= 1 AND [isCompleted]=0 and [fleetGuid]=@guid)
				BEGIN
					SET @output = -1
					SET @fieldname = 'OnGoingTripExists'						
					RETURN;
				END
				IF EXISTS (SELECT 1 from dbo.[DeviceMaintenance] DM (NOLOCK) WHERE [isDeleted] = 0 AND @dt >= [startDateTime] AND @dt <= [endDateTime] AND ISNULL([isCompleted],0)=0 and [deviceGuid]=@guid)
				BEGIN
					SET @output = -1
					SET @fieldname = 'OnGoingMaintenanceExists'						
					RETURN;
				END
				IF EXISTS (SELECT 1 from dbo.[Driver] D (NOLOCK) WHERE [isDeleted] = 0 and [fleetGuid]=@guid)
				BEGIN
					SET @output = -1
					SET @fieldname = 'DriverExists'						
					RETURN;
				END
			END
		
	BEGIN TRAN	
			UPDATE dbo.[Device]
			SET  [fleetGuid]=null
				,[updatedDate]	= @dt
				,[updatedBy]	= @invokingUser			
			WHERE
				[fleetGuid] = @guid
				AND [companyGuid] = @companyGuid
				AND [isDeleted] = 0				
			
			UPDATE dbo.[Driver]
			SET  [fleetGuid]=null
				,[updatedDate]	= @dt
				,[updatedBy]	= @invokingUser			
			WHERE
				[fleetguid] = @guid
				AND [companyGuid] = @companyGuid
				AND [isDeleted] = 0

			UPDATE dbo.[Fleet]
			SET  [isDeleted]=1
				,[updatedDate]	= @dt
				,[updatedBy]	= @invokingUser			
			WHERE
				[guid] = @guid
				AND [companyGuid] = @companyGuid
				AND [isDeleted] = 0
			
	
	COMMIT TRAN	
	END TRY 		
	BEGIN CATCH
	SET @output = 0
	DECLARE @errorReturnMessage nvarchar(MAX)

	SELECT
		@errorReturnMessage = ISNULL(@errorReturnMessage, ' ') + SPACE(1) +
		'ErrorNumber:' + ISNULL(CAST(ERROR_NUMBER() AS nvarchar), ' ') +
		'ErrorSeverity:' + ISNULL(CAST(ERROR_SEVERITY() AS nvarchar), ' ') +
		'ErrorState:' + ISNULL(CAST(ERROR_STATE() AS nvarchar), ' ') +
		'ErrorLine:' + ISNULL(CAST(ERROR_LINE() AS nvarchar), ' ') +
		'ErrorProcedure:' + ISNULL(CAST(ERROR_PROCEDURE() AS nvarchar), ' ') +
		'ErrorMessage:' + ISNULL(CAST(ERROR_MESSAGE() AS nvarchar(MAX)), ' ')

	RAISERROR (@errorReturnMessage
	, 11
	, 1
	)

	IF (XACT_STATE()) = -1 BEGIN
		ROLLBACK TRANSACTION
	END
	IF (XACT_STATE()) = 1 BEGIN
		ROLLBACK TRANSACTION
	END
	END CATCH
END