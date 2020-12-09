/*******************************************************************
DECLARE @output INT = 0
		,@fieldName	nvarchar(255)
		,@syncDate	DATETIME
EXEC [dbo].[DriverStatistics_Get]
	 @guid				= '8E76A334-CB96-4EDE-901E-5A0B982BEAA2'
	,@currentDate	= '2020-07-02 06:47:56.890'
	,@invokingUser  	= '7D31E738-5E24-4EA2-AAEF-47BB0F3CCD41'
	,@version			= 'v1'
	,@output			= @output		OUTPUT
	,@fieldName			= @fieldName	OUTPUT
	,@syncDate		= @syncDate		OUTPUT
               
 SELECT @output status,  @fieldName AS fieldName, @syncDate syncDate    
 

*******************************************************************/

CREATE PROCEDURE [dbo].[DriverStatistics_Get]  
(  @guid    UNIQUEIDENTIFIER   
 ,@currentDate  DATETIME   = NULL  
 ,@invokingUser  UNIQUEIDENTIFIER = NULL  
 ,@version   NVARCHAR(10)  
 ,@output   SMALLINT    OUTPUT  
 ,@fieldName   NVARCHAR(255)   OUTPUT  
 ,@syncDate   DATETIME    OUTPUT  
 ,@culture   NVARCHAR(10)   = 'en-Us'  
 ,@enableDebugInfo CHAR(1)     = '0'  
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
            SELECT 'DriverStatistics_Get' AS '@procName'  
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
  IF OBJECT_ID('tempdb..#tripsDS') IS NOT NULL BEGIN DROP TABLE #tripsDS END  
  CREATE TABLE #tripsDS ([driverGuid] UNIQUEIDENTIFIER, [fleetGuid] UNIQUEIDENTIFIER, [tripGuid] UNIQUEIDENTIFIER,[startDateTime] DATETIME,[endDateTime] DATETIME,[isStarted] BIT,[isCompleted] BIT)   
    
   INSERT INTO #tripsDS ([driverGuid],[fleetGuid], [tripGuid],[startDateTime],[isStarted] ,[isCompleted],[endDateTime])   
   SELECT D.[guid] AS [driverGuid],T.[fleetGuid],T.[guid], [startDateTime], [isStarted] ,[isCompleted],    
   (SELECT top 1 endDateTime from [dbo].[TripStops] (NOLOCK) WHERE [tripGuid] = T.[Guid] AND [isDeleted] = 0 ORDER BY [endDateTime] DESC) AS [endDateTime]      
    FROM [dbo].[Trip] T (NOLOCK)   
    INNER JOIN [dbo].[Fleet] F (NOLOCK) ON T.[fleetGuid]=F.[guid]   
    INNER JOIN [dbo].[Driver] D (NOLOCK) ON D.[fleetGuid]=F.[guid]    
    WHERE D.[guid] = @guid AND T.[isDeleted] = 0 AND F.[isDeleted]=0 AND D.[isDeleted]=0 order by T.[startDateTime] asc    
  
  ;WITH   
  CTE_TripStatus  
  AS (   
    select *,CASE WHEN ([isStarted]=1 AND [isCompleted]=1)  
      THEN 'Completed'  
     ELSE CASE WHEN ([isStarted]=1 AND [isCompleted]=0)  
      THEN 'On Duty'  
      ELSE CASE WHEN [endDateTime] < @dt
			THEN 'Overdue' 
      ELSE 'Upcoming'  
      END  
      END END AS [status] from #tripsDS        
  )   
  ,CTE_AlertCount  
  AS ( SELECT D.[guid] AS [driverGuid]   
      , COUNT(A.[guid]) AS [totalAlerts]  
    FROM [dbo].[IOTConnectAlert] A (NOLOCK)   
    INNER JOIN [dbo].[Device] E (NOLOCK) ON A.[DeviceGuid] = E.[guid] AND E.[isDeleted] = 0   
    INNER JOIN [dbo].[Fleet] F (NOLOCK) ON E.[fleetGuid]=F.[guid] AND F.[isDeleted]=0  
    INNER JOIN [dbo].[Driver] D (NOLOCK) ON D.[fleetGuid]=F.[guid] AND D.[isDeleted]=0  
    WHERE D.[guid] = @guid  AND CONVERT(Date,A.[eventDate]) = CONVERT(DATE,@dt)   
    GROUP BY D.[guid]  
  )  
  SELECT D.[guid],D.[driverId],D.[fleetGuid] ,Z.[tripGuid]      
    ,ISNULL(Z.[Status],'') as [driverStatus]  
    , ISNULL(TC.[underTripCount],0) + ISNULL(TC.[scheduleTripCount],0)+ ISNULL(TC.[completeTripCount],0) AS [totalTripCount]  
    , ISNULL(TC.[underTripCount],0) AS [totalUnderTripCount]  
    , ISNULL(TC.[scheduleTripCount],0) AS [totalScheduledTripCount]   
    , ISNULL(TC.[completeTripCount],0) AS [totalCompletedTripCount]   
    , ISNULL(A.[totalAlerts],0) AS [totalAlerts]  
    ,ISNULL(D.[harshBraking],0) as [harshBreakingCount]  
    ,ISNULL(D.[overSpeed],0) as [overSpeedCount]  
    ,ROUND(CONVERT(DECIMAL(18,2),CONVERT(DECIMAL(18,7),ISNULL(D.[idleTime],0))/60),2) as [idleTimeHours]  
  FROM [dbo].[Driver] D (NOLOCK)     
  LEFT JOIN CTE_TripStatus Z ON D.[guid] = Z.[driverGuid]   
  LEFT JOIN (SELECT S.[driverGuid]  
      , SUM(CASE WHEN [status] = 'On Duty' THEN 1 ELSE 0 END) AS [underTripCount]  
      , SUM(CASE WHEN [status] = 'Upcoming' THEN 1 ELSE 0 END) AS [scheduleTripCount]  
      , SUM(CASE WHEN [status] = 'Completed' THEN 1 ELSE 0 END) AS [completeTripCount]  
     FROM CTE_TripStatus S   
     GROUP BY S.[driverGuid]) TC ON D.[guid] = TC.[driverGuid]    
  LEFT JOIN CTE_AlertCount A ON D.[guid] = A.[driverGuid]    
  WHERE D.[guid]=@guid AND D.[isDeleted]=0  
    
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
