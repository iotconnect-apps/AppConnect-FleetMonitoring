/*******************************************************************
DECLARE @output INT = 0
		,@fieldName	nvarchar(255)
		,@syncDate	DATETIME
EXEC [dbo].[CompanyStatistics_Get]
	 @guid				= 'DC4B1A8B-38D8-4431-83D0-933DE2DD4324'
	,@currentDate	= '2020-07-02 06:47:56.890'
	,@invokingUser  	= '7D31E738-5E24-4EA2-AAEF-47BB0F3CCD41'
	,@version			= 'v1'
	,@output			= @output		OUTPUT
	,@fieldName			= @fieldName	OUTPUT
	,@syncDate		= @syncDate		OUTPUT
               
 SELECT @output status,  @fieldName AS fieldName, @syncDate syncDate    
 
001	SAR-138 11-05-2020 [Nishit Khakhi]	Added Initial Version to Get Company Statistics
*******************************************************************/

CREATE PROCEDURE [dbo].[CompanyStatistics_Get]
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
	DECLARE @dt DATETIME = ISNULL(@currentDate,GETUTCDATE() )
	IF (@enableDebugInfo = 1)
	BEGIN
        DECLARE @Param XML
        SELECT @Param =
        (
            SELECT 'CompanyStatistics_Get' AS '@procName'
			, CONVERT(nvarchar(MAX),@guid) AS '@guid'			
	        , CONVERT(VARCHAR(50),@currentDate) as '@currentDate'
			, CONVERT(nvarchar(MAX),@invokingUser) AS '@invokingUser'
			, CONVERT(nvarchar(MAX),@version) AS '@version'
			, CONVERT(nvarchar(MAX),@output) AS '@output'
            , CONVERT(nvarchar(MAX),@fieldName) AS '@fieldName'
            FOR XML PATH('Params')
	    )
	    INSERT INTO DebugInfo(data, dt) VALUES(Convert(nvarchar(MAX), @Param), @dt)
    END
    Set @output = 1
    SET @fieldName = 'Success'
  BEGIN TRY
		SET @syncDate = (SELECT TOP 1 CONVERT(DATETIME,[value]) FROM dbo.[Configuration] (NOLOCK) WHERE [configKey] = 'telemetry-last-exectime')
		IF OBJECT_ID('tempdb..#tripsCS') IS NOT NULL BEGIN DROP TABLE #tripsCS END
		IF OBJECT_ID ('tempdb..#idsCS') IS NOT NULL DROP TABLE #idsCS
		CREATE TABLE #tripsCS ([companyGuid] UNIQUEIDENTIFIER, [fleetGuid] UNIQUEIDENTIFIER, [startDateTime] DATETIME,[endDateTime] DATETIME,[isStarted] BIT,[isCompleted] BIT) 
		
			INSERT INTO #tripsCS ([companyGuid],[fleetGuid], [startDateTime],[endDateTime],[isStarted] ,[isCompleted]) 
			SELECT distinct T.[companyGuid],T.[fleetGuid], [startDateTime],			
			(SELECT top 1 [endDateTime] from [dbo].[TripStops] (NOLOCK) WHERE [tripGuid] = T.[Guid] AND [isDeleted] = 0 ORDER BY [endDateTime] DESC) AS [endDateTime] 			
				,[isStarted] ,[isCompleted] FROM [dbo].[Trip] T (NOLOCK) 
				 INNER JOIN [dbo].[Fleet] AS f WITH (NOLOCK) ON f.[guid] = T.[fleetGuid] AND f.[isDeleted] = 0          
				INNER JOIN [dbo].[Driver] AS d WITH (NOLOCK) ON d.[fleetGuid] = T.[fleetGuid] AND d.[isDeleted] = 0   
				INNER JOIN [dbo].[Device] AS dv WITH (NOLOCK) ON dv.[fleetGuid] = T.[fleetGuid] AND dv.[isDeleted] = 0 
				WHERE T.[companyGuid] = @guid AND T.[isDeleted] = 0 		
		
		SELECT E.[guid]  
		INTO #idsCS
		FROM [dbo].[Device] E (NOLOCK) 
		WHERE E.[companyGuid] = @guid AND E.[isDeleted] = 0 

		;WITH 
		CTE_TripStatus
		AS (	
				SELECT @guid as [guid] 
				,CASE WHEN (@dt >= DM.[startDateTime] AND @dt <= DM.[endDateTime]) AND ISNULL(DM.[isCompleted],0)=0
					 THEN 'InGarage'
					 ELSE 'InTransit'
					 END
					 AS [status] 
				FROM dbo.[DeviceMaintenance] DM (NOLOCK) 
				WHERE [companyGuid] = @guid AND [isDeleted] = 0 				
		)		
		, CTE_Fleet
		AS (	SELECT [companyGuid], COUNT(1) [totalCount] 
				FROM [dbo].[Fleet] (NOLOCK) 
				WHERE [companyGuid] = @guid AND [isDeleted] = 0 
				GROUP BY [companyGuid]
		)
		, CTE_Trip
		AS (	
				--SELECT [companyGuid], COUNT(1) [totalCount] 
				--FROM #tripsCS
				--WHERE [companyGuid] = @guid 
				--GROUP BY [companyGuid]
				SELECT DM.[companyGuid]  
				,SUM(CASE WHEN (DM.[isStarted]=1 AND DM.[isCompleted]=0) THEN 1 ELSE 0 END) [totalRunningCount]  
				,COUNT(1) AS [totalCount]					 
				FROM #tripsCS DM (NOLOCK) 
				WHERE [companyGuid] = @guid 
				GROUP BY [companyGuid]
		)	
		,CTE_DeviceCount
		AS (	SELECT [companyGuid]
						, COUNT([guid]) [totalDevices] 
				FROM [dbo].[Device] (NOLOCK) 
				WHERE [companyGuid] = @guid AND [isDeleted] = 0
				GROUP BY [companyGuid]
		)
		,CTE_Drivers
		AS (	SELECT [companyGuid]
						, COUNT([guid]) [totalDrivers] 
				FROM [dbo].[Driver] (NOLOCK) 
				WHERE [companyGuid] = @guid AND [isDeleted] = 0
				GROUP BY [companyGuid]
		)
		,CTE_Users
		AS (	SELECT [companyGuid]
						, SUM(CASE WHEN [isActive]=1 THEN 1 ELSE 0 END) AS [activeUserCount],COUNT([guid]) AS [totalUserCount]
				FROM [dbo].[User] (NOLOCK) 
				WHERE [companyGuid] = @guid AND [isDeleted]=0  
				GROUP BY [companyGuid]
		)
		,CTE_Energy
		AS (
			SELECT @guid AS [companyGuid],SUM([sum]) AS [value] 
				FROM #idsCS I 
				LEFT JOIN [dbo].[TelemetrySummary_Hourwise] T (NOLOCK) ON T.[deviceGuid] = I.[guid]
				WHERE [attribute] = 'can_currentin' 
				GROUP BY [attribute]
			
		)
		,CTE_AlertCount
		AS (	SELECT [companyGuid]
						, COUNT([guid]) AS [totalAlerts]
				FROM [dbo].[IOTConnectAlert] (NOLOCK) 
				WHERE [companyGuid] = @guid  AND CONVERT(Date,[eventDate]) = CONVERT(DATE,@dt)  
				GROUP BY [companyGuid]
		)
		
		SELECT C.[guid]
				, ISNULL(L.[totalCount],0) - ISNULL(TS.[inGarageFleetCount],0) AS [inTransitFleetCount]
				, ISNULL(U.[totalUserCount],0) AS [totalUserCount]
				, ISNULL(U.[activeUserCount],0) AS [activeUserCount]
				, ISNULL(U.[totalUserCount],0) - ISNULL(U.[activeUserCount],0) AS [inActiveUserCount]
				, ISNULL(TS.[inGarageFleetCount],0) AS [inGarageFleetCount]	
				, CASE WHEN ISNULL(Dr.[totalDrivers],0)>0 THEN ISNULL(Tr.[totalRunningCount],0)*100/ISNULL(Dr.[totalDrivers],0) ELSE 0 END AS [driverUtilizationPer]		
				, CASE WHEN ISNULL(L.[totalCount],0)>0 THEN ISNULL(Tr.[totalRunningCount],0)*100/ISNULL(L.[totalCount],0) ELSE 0 END AS [fleetUtilizationPer]		
				,ISNULL(E.[value],0) AS [totalFuelConsumption]
				, ISNULL(L.[totalCount],0) AS [totalFleetCount]
				, ISNULL(D.[totalDevices],0) AS [totalDevices]
				, ISNULL(A.[totalAlerts],0) AS [totalAlerts]
				,ISNULL(Tr.[totalRunningCount],0) as [totalRunningCount]				
		FROM [dbo].[Company] C (NOLOCK) 
		LEFT JOIN CTE_Users U ON C.[guid]= U.[companyGuid] 
		LEFT JOIN CTE_Fleet L ON C.[guid] = L.[companyGuid]
		LEFT JOIN CTE_Drivers Dr ON C.[guid] = Dr.[companyGuid]
		LEFT JOIN CTE_Trip Tr ON C.[guid]=Tr.[companyGuid]
		LEFT JOIN CTE_DeviceCount D ON C.[guid] = D.[companyGuid]
		LEFT JOIN CTE_Energy E ON C.[guid]=E.[companyGuid]
		LEFT JOIN (SELECT _TS.[guid]
						, SUM(CASE WHEN [status] = 'InTransit' THEN 1 ELSE 0 END) AS [inTransitFleetCount]
						, SUM(CASE WHEN [status] = 'InGarage' THEN 1 ELSE 0 END) AS [inGarageFleetCount]
						
					FROM CTE_TripStatus _TS 
					GROUP BY _TS.[guid]) TS ON C.[guid]=TS.[guid] 
		
		LEFT JOIN CTE_AlertCount A ON C.[guid] = A.[companyGuid]
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