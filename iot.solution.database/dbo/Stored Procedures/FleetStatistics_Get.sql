/*******************************************************************
DECLARE @output INT = 0
		,@fieldName	nvarchar(255)
		,@syncDate	DATETIME
EXEC [dbo].[FleetStatistics_Get]
	 @guid				= 'DC4B1A8B-38D8-4431-83D0-933DE2DD4324'
	,@currentDate	= '2020-07-02 06:47:56.890'
	,@invokingUser  	= '7D31E738-5E24-4EA2-AAEF-47BB0F3CCD41'
	,@version			= 'v1'
	,@output			= @output		OUTPUT
	,@fieldName			= @fieldName	OUTPUT
	,@syncDate		= @syncDate		OUTPUT
               
 SELECT @output status,  @fieldName AS fieldName, @syncDate syncDate    
 

*******************************************************************/

CREATE PROCEDURE [dbo].[FleetStatistics_Get]
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
	DECLARE @dt DATETIME = ISNULL(@currentDate,GETUTCDATE())
	IF (@enableDebugInfo = 1)
	BEGIN
        DECLARE @Param XML
        SELECT @Param =
        (
            SELECT 'FleetStatistics_Get' AS '@procName'
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
  IF OBJECT_ID('tempdb..#tripsFS') IS NOT NULL BEGIN DROP TABLE #tripsFS END  
  CREATE TABLE #tripsFS ([uniqueId] nvarchar(100), [deviceGuid] UNIQUEIDENTIFIER,[fleetGuid] UNIQUEIDENTIFIER,[tripGuid] UNIQUEIDENTIFIER, [startDateTime] DATETIME,[endDateTime] DATETIME,[sourceLatitude] nvarchar(50),[sourceLongitude] nvarchar(50),  
  [destinationLatitude] nvarchar(50),[destinationLongitude] nvarchar(50),[isStarted] BIT,[isCompleted] BIT)   
    
   INSERT INTO #tripsFS ([uniqueId],[deviceGuid],[fleetGuid], [tripGuid],[startDateTime],[endDateTime],[sourceLatitude] ,[sourceLongitude],[destinationLatitude],[destinationLongitude],[isStarted],[isCompleted])   
   SELECT D.[uniqueId],D.[guid],T.[fleetGuid], T.[guid],T.[startDateTime],     
   (SELECT top 1 endDateTime from [dbo].[TripStops] (NOLOCK) WHERE [tripGuid] = T.[Guid] AND [isDeleted] = 0 ORDER BY [endDateTime] DESC) AS [endDateTime]   
   ,[sourceLatitude] ,[sourceLongitude],[destinationLatitude],[destinationLongitude] ,[isStarted],[isCompleted]  
    FROM [dbo].[Trip] T (NOLOCK)   
    INNER JOIN [dbo].[Device] D (NOLOCK) ON T.[fleetGuid]=D.[fleetGuid]   
    WHERE T.[fleetGuid] = @guid AND T.[isDeleted] = 0 AND D.[isDeleted]=0 order by T.[startDateTime] desc
  
  ;WITH   
  CTE_TripStatus  
  AS (   
    SELECT  [fleetGuid],[tripGuid]  
    ,CASE WHEN ([isStarted]=1 AND [isCompleted]=1)  
       THEN CAST('Trip Completed'  as nvarchar(200))
      ELSE CASE WHEN ([isStarted]=1 AND [isCompleted]=0)  
      THEN 'On Duty'  
      ELSE CASE WHEN [endDateTime] < @dt
			THEN 'Overdue' 
      ELSE 'Upcoming'  
      END  
      END END AS [status]   
    FROM #tripsFS    
    WHERE [fleetGuid] = @guid       
    GROUP BY [fleetGuid],[tripGuid],[isStarted],[isCompleted],[endDateTime]  
  )    
  ,CTE_DeviceCount  
  AS ( SELECT [fleetGuid]  
      , COUNT([guid]) [totalDevices]   
    FROM [dbo].[Device] (NOLOCK)   
    WHERE [fleetGuid] = @guid AND [isDeleted] = 0  
    GROUP BY [fleetGuid]  
  )  
  , CTE_Maintenance  
  AS ( SELECT DM.[deviceGuid] AS [fleetGuid]  
     , DM.[guid] AS [guid]  
     ,CASE WHEN (@dt >= [startDateTime] AND @dt <= [endDateTime]) AND ISNULL(DM.[isCompleted],0)=0  
      THEN 'Maintenance'  
      ELSE CASE WHEN ([startDateTime] < @dt AND [endDateTime] < @dt) OR ISNULL(DM.[isCompleted],0)=1 
      THEN 'Maintenance Completed'  
      ELSE 'Maintenance Scheduled'  
      END  
      END AS [status]        
    FROM dbo.[DeviceMaintenance] DM (NOLOCK)   
    INNER JOIN [dbo].[Fleet] E (NOLOCK) ON DM.[DeviceGuid] = E.[guid] AND E.[isDeleted] = 0  
    WHERE DM.[deviceGuid] = @guid   
    AND DM.[IsDeleted]=0   
   )  
   , CTE_NextMaintenance  
  AS ( SELECT TOP 1 DM.[deviceGuid] AS [fleetGuid]  
         , [startDateTime]         
    FROM dbo.[DeviceMaintenance] DM (NOLOCK)   
    INNER JOIN [dbo].[Fleet] E (NOLOCK) ON DM.[DeviceGuid] = E.[guid] AND E.[isDeleted] = 0  
    WHERE DM.[deviceGuid] = @guid   
    AND DM.[IsDeleted]=0  AND [startDateTime]> @dt AND ISNULL([isCompleted],0)=0
   )  
   , CTE_Fuel  
  AS (   
  SELECT E.[Guid] AS [fleetGuid],       
      SUM(T.[sum])   
      AS [totalFuelConsumption]   
   FROM [dbo].[TelemetrySummary_Hourwise] T (NOLOCK)   
   INNER JOIN [dbo].[Device] D (NOLOCK) ON T.[deviceGuid] = D.[guid] AND D.[isDeleted] = 0  
   INNER JOIN [dbo].[Fleet] E (NOLOCK) ON D.[fleetGuid] = E.[guid] AND E.[isDeleted] = 0   
   WHERE E.[guid] = @guid AND [attribute] = 'can_currentin'    
   group by E.[guid]      
   )   
   , CTE_HighestSpeed  
  AS (   
  SELECT E.[Guid] AS [fleetGuid],       
      MAX(T.[attributeValue])   
      AS [highestSpeed]   
   FROM [IOTConnect].[AttributeValue] T (NOLOCK)   
   INNER JOIN [dbo].[Device] D (NOLOCK) ON T.[uniqueId] = D.[uniqueId] AND D.[isDeleted] = 0  
   INNER JOIN [dbo].[Fleet] E (NOLOCK) ON D.[fleetGuid] = E.[guid] AND E.[isDeleted] = 0   
   WHERE E.[guid] = @guid AND T.[localName] = 'can_vehicle_speed'    
   group by E.[guid]      
   )  
  ,CTE_AlertCount  
  AS ( SELECT [fleetGuid] as [fleetGuid]  
      , COUNT(A.[guid]) AS [totalAlerts]  
    FROM [dbo].[IOTConnectAlert] A (NOLOCK)   
    INNER JOIN [dbo].[Device] E (NOLOCK) ON A.[DeviceGuid] = E.[guid] AND E.[isDeleted] = 0  
    WHERE E.[fleetGuid] = @guid  AND CONVERT(Date,A.[eventDate]) = CONVERT(DATE,@dt)   
    GROUP BY [fleetGuid]  
  )  
  SELECT TOP 1 C.[guid]  
    ,C.[fleetId],C.[latitude],C.[longitude],C.[radius]  
    ,ISNULL(L.[uniqueId], DV.[uniqueId]) AS [uniqueId]
	,ISNULL(L.[deviceGuid], DV.[guid]) AS [deviceGuid]
	,L.[sourceLatitude] ,L.[sourceLongitude],L.[destinationLatitude],L.[destinationLongitude]  
    ,L.[tripGuid]  
    ,ISNULL(Z.[Status],CASE WHEN ISNULL(CM.[underMaintenanceCount],0) >0 THEN 'Maintenance' ELSE 'Unassigned' END) as [fleetStatus]  
    , ISNULL(TC.[underTripCount],0) + ISNULL(TC.[scheduleTripCount],0)+ISNULL(TC.[completeTripCount],0) AS [totalTripCount]  
    , ISNULL(TC.[underTripCount],0) AS [totalUnderTripCount]  
    , ISNULL(TC.[scheduleTripCount],0) AS [totalScheduledTripCount]   
    , ISNULL(TC.[completeTripCount],0) AS [totalCompletedTripCount]   
    , ISNULL(D.[totalDevices],0) AS [totalDevices]  
     ,ISNULL(HS.[highestSpeed],0) AS [highestSpeed]
    , ISNULL(CM.[underMaintenanceCount],0) + ISNULL(CM.[scheduleMaintenanceCount],0)+ISNULL(CM.[completeMaintenanceCount],0) AS [totalMaintenanceCount]  
    , ISNULL(CM.[underMaintenanceCount],0) AS [totalUnderMaintenanceCount]  
    , ISNULL(CM.[scheduleMaintenanceCount],0) AS [totalScheduledCount]  
    ,ISNULL (CM.[completeMaintenanceCount],0) AS [totalCompletedMaintenanceCount]   
    , NM.[startDateTime] AS [nextMaintenanceDateTime]  
    , ISNULL(A.[totalAlerts],0) AS [totalAlerts]  
    ,ISNULL(F.[totalFuelConsumption],0) AS [totalFuelConsumption]   
  FROM [dbo].[Fleet] C (NOLOCK)   
  LEFT JOIN [dbo].[Device] DV ON C.[guid] = DV.[fleetGuid]  
  LEFT JOIN #tripsFS L ON C.[guid] = L.[fleetGuid]  
  LEFT JOIN CTE_TripStatus Z ON C.[guid] = Z.[fleetGuid]  AND Z.[tripGuid] =L.[tripGuid] 
  LEFT JOIN CTE_DeviceCount D ON C.[guid] = D.[fleetGuid]  
  LEFT JOIN CTE_HighestSpeed HS ON C.[guid] = HS.[fleetGuid]  
  LEFT JOIN (SELECT S.[fleetGuid]  
      , SUM(CASE WHEN [status] = 'On Duty' THEN 1 ELSE 0 END) AS [underTripCount]  
      , SUM(CASE WHEN [status] = 'Upcoming' THEN 1 ELSE 0 END) AS [scheduleTripCount]  
      , SUM(CASE WHEN [status] = 'Trip Completed' THEN 1 ELSE 0 END) AS [completeTripCount]  
     FROM CTE_TripStatus S   
     GROUP BY S.[fleetGuid]) TC ON C.[guid] = TC.[fleetGuid]  
  LEFT JOIN (SELECT M.[fleetGuid]  
      , SUM(CASE WHEN [status] = 'Maintenance' THEN 1 ELSE 0 END) AS [underMaintenanceCount]  
      , SUM(CASE WHEN [status] = 'Maintenance Scheduled' THEN 1 ELSE 0 END) AS [scheduleMaintenanceCount]  
      , SUM(CASE WHEN [status] = 'Maintenance Completed' THEN 1 ELSE 0 END) AS [completeMaintenanceCount]  
     FROM CTE_Maintenance M   
     GROUP BY M.[fleetGuid]) CM ON C.[guid] = CM.[fleetGuid]  
  LEFT JOIN CTE_AlertCount A ON C.[guid] = A.[fleetGuid]  
  LEFT JOIN CTE_NextMaintenance NM ON C.[guid] = NM.[fleetGuid]  
  LEFT JOIN CTE_Fuel F ON C.[guid] = F.[fleetGuid]  
  WHERE C.[guid]=@guid AND C.[isDeleted]=0  ORDER BY L.[startDateTime] desc
    
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
