/*******************************************************************
DECLARE @output INT = 0
		,@fieldName	nvarchar(255)
		,@syncDate	DATETIME
EXEC [dbo].[EntityStatistics_Get]
	 @guid			= '1D355B70-94CE-44AB-B623-5859DC1BD847'
	,@currentDate	= '2020-07-16 06:47:56.890'
	,@invokingUser  	= '7D31E738-5E24-4EA2-AAEF-47BB0F3CCD41'
	,@version			= 'v1'
	,@output			= @output		OUTPUT
	,@fieldName			= @fieldName	OUTPUT
	,@syncDate		= @syncDate		OUTPUT
               
 SELECT @output status,  @fieldName AS fieldName, @syncDate syncDate    

001	SAR-138 11-05-2020 [Nishit Khakhi]	Added Initial Version to Get Company Statistics
*******************************************************************/

CREATE PROCEDURE [dbo].[EntityStatistics_Get]
(	 @guid				UNIQUEIDENTIFIER	
	,@currentDate		DATETIME			= NULL
	,@invokingUser		UNIQUEIDENTIFIER	= NULL
	,@version			NVARCHAR(10)
	,@output			SMALLINT		  OUTPUT
	,@fieldName			NVARCHAR(255)	  OUTPUT
	,@syncDate			DATETIME		  OUTPUT
	,@culture			NVARCHAR(10)	  = 'en-Us'
	,@enableDebugInfo	CHAR(1)			  = '0'
)
AS
BEGIN
    SET NOCOUNT ON
	IF (@enableDebugInfo = 1)
	BEGIN
        DECLARE @Param XML
        SELECT @Param =
        (
            SELECT 'EntityStatistics_Get' AS '@procName'
			, CONVERT(nvarchar(MAX),@guid) AS '@guid'			
	        , CONVERT(VARCHAR(50),@currentDate) as '@currentDate'
			, CONVERT(nvarchar(MAX),@invokingUser) AS '@invokingUser'
			, CONVERT(nvarchar(MAX),@version) AS '@version'
			, CONVERT(nvarchar(MAX),@output) AS '@output'
            , CONVERT(nvarchar(MAX),@fieldName) AS '@fieldName'
            FOR XML PATH('Params')
	    )
	    INSERT INTO DebugInfo(data, dt) VALUES(Convert(nvarchar(MAX), @Param), GETUTCDATE())
    END
    Set @output = 1
    SET @fieldName = 'Success'

   BEGIN TRY
		SET @syncDate = (SELECT TOP 1 CONVERT(DATETIME,[value]) FROM dbo.[Configuration] (NOLOCK) WHERE [configKey] = 'telemetry-last-exectime')
		declare @dt DATETIME=GETUTCDATE()
		IF OBJECT_ID('tempdb..#trips') IS NOT NULL BEGIN DROP TABLE #trips END
		CREATE TABLE #trips ( [fleetGuid] UNIQUEIDENTIFIER, [startDateTime] DATETIME,[endDateTime] DATETIME) 
		
			INSERT INTO #trips ([fleetGuid], [startDateTime],[endDateTime]) 
			SELECT [fleetGuid], [startDateTime],			
			(SELECT top 1 endDateTime from [dbo].[TripStops] (NOLOCK) WHERE [tripGuid] = T.[Guid] AND [isDeleted] = 0 ORDER BY [endDateTime] DESC) AS [endDateTime] 
			
				FROM [dbo].[Trip] T (NOLOCK) 
				WHERE [fleetGuid] = @guid AND [isDeleted] = 0 		

		;WITH 
		CTE_TripStatus
		AS (	
				SELECT [fleetGuid]
				,CASE WHEN @dt >= [startDateTime] AND @dt <= [endDateTime]
					 THEN 'In Transit'
					 ELSE CASE WHEN [startDateTime] < @dt AND [endDateTime] < @dt 
					 THEN 'Trip Completed'
					 ELSE 'Trip Scheduled'
					 END
					 END AS [status] 
				FROM #trips  
				WHERE [fleetGuid] = @guid 				
				GROUP BY [fleetGuid],[startDateTime],[endDateTime]
		)		
		,CTE_DeviceCount
		AS (	SELECT [fleetGuid]
						, COUNT([guid]) [totalDevices] 
				FROM [dbo].[Device] (NOLOCK) 
				WHERE [fleetGuid] = @guid AND [isDeleted] = 0
				GROUP BY [fleetGuid]
		)
		, CTE_Maintenance
		AS (	SELECT DM.[deviceGuid] AS [fleetGuid]
					, DM.[guid] AS [guid]
					,CASE WHEN (@dt >= [startDateTime] AND @dt <= [endDateTime]) AND ISNULL([isCompleted],0)=0
					 THEN 'In Garage'
					 ELSE CASE WHEN ([startDateTime] < @dt AND [endDateTime] < @dt) OR ISNULL(DM.[isCompleted],0)=1
					 THEN 'Completed'
					 ELSE 'Maintenance Scheduled'
					 END
					 END AS [status]					 
				FROM dbo.[DeviceMaintenance] DM (NOLOCK) 
				INNER JOIN [dbo].[Fleet] E ON DM.[DeviceGuid] = E.[guid] AND E.[isDeleted] = 0
				WHERE DM.[deviceGuid] = @guid 
				AND DM.[IsDeleted]=0 
			)
			, CTE_NextMaintenance
		AS (	SELECT TOP 1 DM.[deviceGuid] AS [fleetGuid]
									, [startDateTime] 					 
				FROM dbo.[DeviceMaintenance] DM (NOLOCK) 
				INNER JOIN [dbo].[Fleet] E ON DM.[DeviceGuid] = E.[guid] AND E.[isDeleted] = 0
				WHERE DM.[deviceGuid] = @guid 
				AND DM.[IsDeleted]=0 
			)
			, CTE_Fuel
		AS (	
		SELECT E.[Guid] AS [fleetGuid],					
						SUM(T.[sum]) 
					 AS [totalFuelConsumption] 
			FROM [dbo].[TelemetrySummary_Hourwise] T (NOLOCK) 
			INNER JOIN [dbo].[Device] D (NOLOCK) ON T.[deviceGuid] = D.[guid] AND D.[isDeleted] = 0
			INNER JOIN [dbo].[Fleet] E ON D.[fleetGuid] = E.[guid] AND E.[isDeleted] = 0 
			WHERE E.[guid] = @guid AND [attribute] = 'can_currentin' 	
			group by E.[guid] 			
			)			
		,CTE_AlertCount
		AS (	SELECT [fleetGuid] as [fleetGuid]
						, COUNT(A.[guid]) AS [totalAlerts]
				FROM [dbo].[IOTConnectAlert] A (NOLOCK) 
				INNER JOIN [dbo].[Device] E ON A.[DeviceGuid] = E.[guid] AND E.[isDeleted] = 0
				WHERE E.[fleetGuid] = @guid AND MONTH([eventDate]) = MONTH(@dt) AND YEAR([eventDate]) = YEAR(@dt)
				GROUP BY [fleetGuid]
		)
		SELECT C.[guid]
				,C.[fleetId],C.[latitude],C.[longitude],C.[radius]
				,ISNULL(Z.[Status],'') as [fleetStatus]
				, ISNULL(TC.[underTripCount],0) + ISNULL(TC.[scheduleTripCount],0) AS [totalTripCount]
				, ISNULL(TC.[underTripCount],0) AS [totalUnderTripCount]
				, ISNULL(TC.[scheduleTripCount],0) AS [totalScheduledTripCount] 
				, ISNULL(TC.[completeTripCount],0) AS [totalCompletedTripCount] 
				, ISNULL(D.[totalDevices],0) AS [totalDevices]
				
				, ISNULL(CM.[underMaintenanceCount],0) + ISNULL(CM.[scheduleMaintenanceCount],0) AS [totalMaintenanceCount]
				, ISNULL(CM.[underMaintenanceCount],0) AS [totalUnderMaintenanceCount]
				, ISNULL(CM.[scheduleMaintenanceCount],0) AS [totalScheduledCount]
				, NM.[startDateTime] AS [nextMaintenanceDateTime]
				, ISNULL(A.[totalAlerts],0) AS [totalAlerts]
				,ISNULL(F.[totalFuelConsumption],0) AS [totalFuelConsumption] 
		FROM [dbo].[Fleet] C (NOLOCK) 
		LEFT JOIN #trips L ON C.[guid] = L.[fleetGuid]
		LEFT JOIN CTE_TripStatus Z ON C.[guid] = Z.[fleetGuid]
		LEFT JOIN CTE_DeviceCount D ON C.[guid] = D.[fleetGuid]
		LEFT JOIN (SELECT S.[fleetGuid]
						, SUM(CASE WHEN [status] = 'In Transit' THEN 1 ELSE 0 END) AS [underTripCount]
						, SUM(CASE WHEN [status] = 'Trip Scheduled' THEN 1 ELSE 0 END) AS [scheduleTripCount]
						, SUM(CASE WHEN [status] = 'Trip Completed' THEN 1 ELSE 0 END) AS [completeTripCount]
					FROM CTE_TripStatus S 
					GROUP BY S.[fleetGuid]) TC ON C.[guid] = TC.[fleetGuid]
		LEFT JOIN (SELECT M.[fleetGuid]
						, SUM(CASE WHEN [status] = 'In Garage' THEN 1 ELSE 0 END) AS [underMaintenanceCount]
						, SUM(CASE WHEN [status] = 'Maintenance Scheduled' THEN 1 ELSE 0 END) AS [scheduleMaintenanceCount]
					FROM CTE_Maintenance M 
					GROUP BY M.[fleetGuid]) CM ON C.[guid] = CM.[fleetGuid]
		LEFT JOIN CTE_AlertCount A ON C.[guid] = A.[fleetGuid]
		LEFT JOIN CTE_NextMaintenance NM ON C.[guid] = NM.[fleetGuid]
		LEFT JOIN CTE_Fuel F ON C.[guid] = F.[fleetGuid]
		WHERE C.[guid]=@guid AND C.[isDeleted]=0
		
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