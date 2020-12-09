/*******************************************************************              
DECLARE @output INT = 0              
,@fieldName nvarchar(255)              
,@syncDate DATETIME              
EXEC [dbo].[TripStatistics_Get]              
@guid    = '4798ac03-4e96-4ea2-ad10-35dcf2cc7666'              
,@currentDate = '2020-10-01 12:35:36.000'              
,@invokingUser   = '7D31E738-5E24-4EA2-AAEF-47BB0F3CCD41'              
,@version   = 'v1'              
,@output   = @output  OUTPUT              
,@fieldName   = @fieldName OUTPUT              
,@syncDate  = @syncDate  OUTPUT              
                             
SELECT @output status,  @fieldName AS fieldName, @syncDate syncDate                  
               
              
*******************************************************************/   
CREATE PROCEDURE [dbo].[TripStatistics_Get]                  
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
        SELECT 'TripStatistics_Get' AS '@procName'                  
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
IF OBJECT_ID('tempdb..#tempTrips') IS NOT NULL BEGIN DROP TABLE #tempTrips END                  
CREATE TABLE #tempTrips ([guid] UNIQUEIDENTIFIER,[driverGuid] UNIQUEIDENTIFIER, [fleetGuid] UNIQUEIDENTIFIER, [startDateTime] DATETIME,[endDateTime] DATETIME)                   
                    
INSERT INTO #tempTrips ([guid],[driverGuid],[fleetGuid], [startDateTime],[endDateTime])                   
SELECT @guid,D.[guid] AS [driverGuid],T.[fleetGuid], [startDateTime],                     
(SELECT top 1 endDateTime from [dbo].[TripStops] (NOLOCK) WHERE [tripGuid] = @guid AND [isDeleted] = 0 ORDER BY [endDateTime] DESC) AS [endDateTime]                      
FROM [dbo].[Trip] T (NOLOCK)                   
INNER JOIN [dbo].[Fleet] F (NOLOCK) ON T.[fleetGuid]=F.[guid] AND F.[isDeleted]=0                  
INNER JOIN [dbo].[Driver] D (NOLOCK) ON D.[fleetGuid]=F.[guid]  AND D.[isDeleted]=0                  
WHERE T.[guid] = @guid AND T.[isDeleted] = 0                     
                  
;WITH                   
CTE_TripStatus                  
AS (                   
SELECT T.[guid]                
	,CASE WHEN (T.[isStarted]=1 AND T.[isCompleted]=0)                  
		THEN 'In Transit'                  
	   ELSE CASE WHEN T.[isCompleted]=1                     
		THEN 'Completed'                  
		ELSE CASE WHEN (SELECT top 1 endDateTime from [dbo].[TripStops] (NOLOCK) WHERE [tripGuid] = @guid AND [isDeleted] = 0 ORDER BY [endDateTime] DESC) < @dt
			THEN 'Overdue' 
			ELSE 'Upcoming'        
			END        
			END END AS [status]                   
	FROM [dbo].[Trip] T (NOLOCK)                   
	WHERE [guid] = @guid                    
)                   
                
SELECT T.[guid],T.[tripId],T.[sourceLocation],T.[sourceLatitude]      
,T.[sourceLongitude],T.[destinationLocation],T.[destinationLatitude],T.[destinationLongitude],T.[weight],T.[startDateTime]      
,L.[endDateTime],T.[totalMiles],D.[guid] as [driverGuid],D.[firstName]+' '+D.[lastName] as [driverName],D.[driverId],D.[email],D.[contactNo],D.[image] as [driverImage]      
,F.[guid] as [fleetGuid],F.[fleetId],F.[speedLimit],M.[name] as [materialType],DV.[guid] as deviceGuid,DV.[uniqueId]            
,ISNULL(Z.[Status],'') as [tripStatus]                  
,T.[harshBraking]                
,T.[aggressiveAcceleration]              
,T.[overSpeed]              
,T.[idleTime]    
,FT.[name] as [fleetType]    
FROM [dbo].[Trip] T (NOLOCK)                 
INNER JOIN [dbo].[Fleet] F (NOLOCK) ON F.[guid]=T.[fleetGuid] AND F.[isDeleted]=0                  
INNER JOIN [dbo].[FleetMaterialType] M (NOLOCK) ON M.[guid]=T.[materialTypeGuid] AND M.[isDeleted]=0     
INNER JOIN [dbo].[Driver] D (NOLOCK) ON D.[fleetGuid]=T.[fleetGuid] AND D.[isDeleted]=0        
LEFT JOIN [dbo].[FleetType] FT (NOLOCK) ON FT.[guid]=F.[typeGuid] AND FT.[isDeleted]=0    
LEFT JOIN [dbo].[Device] DV (NOLOCK) ON DV.[fleetGuid]=T.[fleetGuid] AND D.[isDeleted]=0             
LEFT JOIN #tempTrips L ON T.[guid] = L.[guid]                  
LEFT JOIN CTE_TripStatus Z ON T.[guid] = Z.[guid]                   
WHERE T.[guid]=@guid AND T.[isDeleted]=0                  
                    
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