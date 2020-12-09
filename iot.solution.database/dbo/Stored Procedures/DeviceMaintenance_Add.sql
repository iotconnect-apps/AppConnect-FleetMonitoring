/*******************************************************************
DECLARE @output INT = 0
	,@fieldName	nvarchar(255)	
	,@newid		UNIQUEIDENTIFIER
EXEC [dbo].[DeviceMaintenance_Add]	
	@companyGuid	= 'F29BE816-76B7-4E90-A9BC-20BE896A8C81'
	,@entityGuid	= '6A783803-3172-449B-94CE-CF10C2B95F23'
	,@DeviceGuid	= '3141C24F-81E5-48D2-BD55-2488117C0FDC'
	,@description	= 'Test 1'
	,@startDateTime		= '2020-05-19 15:10:04.620'
	,@endDateTime		= '2020-05-19 23:10:04.620'
	,@invokingUser	= 'C1596B8C-7065-4D63-BFD0-4B835B93DFF2'              
	,@version		= 'v1'              
	,@newid			= @newid		OUTPUT
	,@output		= @output		OUTPUT
	,@fieldName		= @fieldName	OUTPUT	

SELECT @output status, @fieldName fieldName, @newid newid

001	23-04-2020 [Nishit Khakhi]	Added Initial Version to Add Device Maintenance
*******************************************************************/
CREATE PROCEDURE [dbo].[DeviceMaintenance_Add]
(	@companyGuid	UNIQUEIDENTIFIER
	,@entityGuid	UNIQUEIDENTIFIER
	,@DeviceGuid	UNIQUEIDENTIFIER	
	,@description	NVARCHAR(1000)		= NULL
	,@startDateTime		DATETIME			= NULL
	,@endDateTime		DATETIME			= NULL
	,@invokingUser	UNIQUEIDENTIFIER	= NULL
	,@version		nvarchar(10)    
	,@newid			UNIQUEIDENTIFIER	OUTPUT
	,@output		SMALLINT			OUTPUT    
	,@fieldName		nvarchar(100)		OUTPUT   
	,@culture		nvarchar(10)		= 'en-Us'
	,@enableDebugInfo	CHAR(1)			= '0'
)	
AS
BEGIN
	SET NOCOUNT ON

    IF (@enableDebugInfo = 1)
	BEGIN
        DECLARE @Param XML 
        SELECT @Param = 
        (
            SELECT 'DeviceMaintenance_Add' AS '@procName'             
            	, CONVERT(nvarchar(50),@companyGuid) AS '@companyGuid' 
            	, CONVERT(nvarchar(50),@entityGuid) AS '@entityGuid' 
				, CONVERT(nvarchar(50),@DeviceGuid) AS '@DeviceGuid' 				
				, @description AS '@description' 
				, CONVERT(nvarchar(50),@startDateTime) AS '@startDateTime'
				, CONVERT(nvarchar(50),@endDateTime) AS '@endDateTime'
				, CONVERT(nvarchar(50),@invokingUser) AS '@invokingUser'
            	, CONVERT(nvarchar(MAX),@version) AS '@version' 
            	, CONVERT(nvarchar(MAX),@output) AS '@output' 
            	, @fieldName AS '@fieldName'   
            FOR XML PATH('Params')
	    ) 
	    INSERT INTO DebugInfo(data, dt) VALUES(Convert(nvarchar(MAX), @Param), GETUTCDATE())
    END       
	
	SET @newid = NEWID()

	SET @output = 1
	SET @fieldName = 'Success'

	BEGIN TRY
		IF NOT EXISTS(SELECT TOP 1 1 FROM [dbo].[Fleet] where [companyGuid] = @companyGuid AND [guid] = @deviceGuid AND [isDeleted] = 0 AND [isActive] = 1)
		BEGIN
			SET @output = -3
			SET @fieldname = 'FleetNotExists!'	
			RETURN;
		END	

		IF EXISTS(SELECT TOP 1 1 FROM [dbo].[DeviceMaintenance] where [companyGuid] = @companyGuid AND [DeviceGuid] = @DeviceGuid 
		AND ISNULL([isCompleted],0)=0 
		AND ((@startDateTime BETWEEN [startDateTime] AND [endDateTime]) OR (@endDateTime BETWEEN [startDateTime] AND [endDateTime])) AND [isDeleted] = 0)
		BEGIN
			SET @output = -3
			SET @fieldname = 'Device Maintenance already exists!'	
			RETURN;
		END		
		IF OBJECT_ID('tempdb..#tripsDM') IS NOT NULL BEGIN DROP TABLE #tripsDM END
		CREATE TABLE #tripsDM ( [fleetGuid] UNIQUEIDENTIFIER, [startDateTime] DATETIME,[endDateTime] DATETIME) 
		
			INSERT INTO #tripsDM ([fleetGuid], [startDateTime],[endDateTime]) 
			SELECT [fleetGuid], [startDateTime],			
			(SELECT top 1 endDateTime from [dbo].[TripStops] (NOLOCK) WHERE [tripGuid] = T.[Guid] AND [isDeleted] = 0 ORDER BY [endDateTime] DESC) AS [endDateTime] 			
				FROM [dbo].[Trip] T (NOLOCK) 
				WHERE [fleetGuid] = @DeviceGuid AND [isDeleted] = 0 		
		
		IF EXISTS(SELECT TOP 1 1 FROM #tripsDM where [fleetGuid] = @DeviceGuid 
		AND ((@startDateTime BETWEEN [startDateTime] AND [endDateTime]) OR (@endDateTime BETWEEN [startDateTime] AND [endDateTime])))
		BEGIN
			SET @output = -2
			SET @fieldname = 'Trip already scheduled for this duration!'	
			RETURN;
		END		
	BEGIN TRAN	
		
			INSERT INTO [dbo].[DeviceMaintenance](
				[guid]		
				,[companyGuid]
				,[entityGuid]
				,[DeviceGuid]
				,[description]
				,[startDateTime]
				,[endDateTime]
				,[createddate]
				,[isDeleted]
				)
			VALUES(@newid
				,@companyGuid
				,@entityGuid
				,@DeviceGuid
				,@description
				,@startDateTime
				,@endDateTime
				,GETUTCDATE()
				,0
				)	
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
