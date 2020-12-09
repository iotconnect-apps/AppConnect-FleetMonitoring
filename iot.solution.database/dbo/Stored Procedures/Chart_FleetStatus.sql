/*******************************************************************
DECLARE @count INT
     	,@output INT = 0
		,@fieldName	VARCHAR(255)

EXEC [dbo].[Chart_FleetStatus]
	 @guid				= 'DC4B1A8B-38D8-4431-83D0-933DE2DD4324'
	,@currentDate	= '2020-07-02 06:47:56.890'
	,@invokingUser  	= '7D31E738-5E24-4EA2-AAEF-47BB0F3CCD41'
	,@version			= 'v1'
	,@output			= @output		OUTPUT
	,@fieldName			= @fieldName	OUTPUT
	,@syncDate		= @syncDate		OUTPUT
               
 SELECT @output status,  @fieldName AS fieldName, @syncDate syncDate   


*******************************************************************/
CREATE PROCEDURE [dbo].[Chart_FleetStatus]
(   @guid				UNIQUEIDENTIFIER	
	,@currentDate		DATETIME			= NULL		
	,@invokinguser		UNIQUEIDENTIFIER	= NULL
	,@version			nvarchar(10)              
	,@output			SMALLINT			OUTPUT
	,@fieldname			nvarchar(255)		OUTPUT
	,@syncDate			DATETIME			OUTPUT
	,@culture			nvarchar(10)		= 'en-Us'	
	,@enabledebuginfo	CHAR(1)				= '0'
)
AS
BEGIN
    SET NOCOUNT ON

    IF (@enableDebugInfo = 1)
	BEGIN
       DECLARE @Param XML 
        SELECT @Param = 
        (
            SELECT 'Chart_EnergyConsumption' AS '@procName' 
            , CONVERT(nvarchar(MAX),@guid) AS '@guid' 			
			, CONVERT(nvarchar(MAX),@version) AS '@version' 
            , CONVERT(nvarchar(MAX),@invokinguser) AS '@invokinguser' 
            FOR XML PATH('Params')
	    ) 
	    INSERT INTO DebugInfo(data, dt) VALUES(Convert(nvarchar(MAX), @Param), GETUTCDATE())
    END
    
    BEGIN TRY  
		DECLARE @dt DATETIME = ISNULL(@currentDate,GETUTCDATE()), @endDate DATETIME
		IF OBJECT_ID('tempdb..#tripsFleetStatus') IS NOT NULL BEGIN DROP TABLE #tripsFleetStatus END
		IF OBJECT_ID ('tempdb..#idsFleetStatus') IS NOT NULL DROP TABLE #idsFleetStatus
		CREATE TABLE #tripsFleetStatus ([companyGuid] UNIQUEIDENTIFIER, [fleetGuid] UNIQUEIDENTIFIER, [startDateTime] DATETIME,[endDateTime] DATETIME) 
		
			INSERT INTO #tripsFleetStatus ([companyGuid],[fleetGuid], [startDateTime],[endDateTime]) 
			SELECT distinct T.[companyGuid],T.[fleetGuid], T.[actualStartDateTime],			
			(SELECT top 1 endDateTime from [dbo].[TripStops] (NOLOCK) WHERE [tripGuid] = T.[Guid] AND [isDeleted] = 0 ORDER BY [endDateTime] DESC) AS [endDateTime] 			
				FROM [dbo].[Fleet] F (NOLOCK) 
		INNER JOIN [dbo].[Trip] T (NOLOCK) ON F.[Guid]=T.[fleetGuid] 
		WHERE T.[companyGuid] = @guid AND  T.[isDeleted]=0  AND F.[isDeleted]=0
		AND T.[isStarted]=1  

		--	SELECT TOP 8 C.[guid],C.[FleetId] 			
		--		, SUM(CASE WHEN ISNULL(DATEDIFF(hour,T.[actualStartDateTime],@dt),0)<0 THEN 0 ELSE CONVERT(DECIMAL(18,2),ISNULL(DATEDIFF(hour,T.[actualStartDateTime],ISNULL(T.[completedDate],@dt)),0)) END) AS [activeCount]
		--		, SUM(CASE WHEN ISNULL(TS.[haltCount],0)<0 THEN 0 ELSE ISNULL(TS.[haltCount],0) END) AS [haltCount]
		--		, SUM(CASE WHEN ISNULL(TS.[idleCount],0)<0 THEN 0 ELSE ISNULL(TS.[idleCount],0) END) AS [idleCount]
		--FROM [dbo].[Fleet] C (NOLOCK) 	
		--	LEFT JOIN [dbo].[Trip] T (NOLOCK) ON C.[Guid]=T.[fleetGuid] 
		--	LEFT JOIN (SELECT distinct _TS.[fleetGuid]						
		--				--, SUM(ISNULL(DATEDIFF(minute,HS.[haltEndDateTime],HS.[haltStartDateTime]),0)) AS [haltCount]
		--				, ROUND(CONVERT(DECIMAL(18,2),SUM(CONVERT(DECIMAL(18,2),ISNULL(DATEDIFF(minute,HS.[haltEndDateTime],HS.[haltStartDateTime]),0))/60)),2) AS [haltCount]
		--				,ROUND(CONVERT(DECIMAL(18,2),SUM(CONVERT(DECIMAL(18,2),ISNULL(DATEDIFF(minute,I.[idleEndDateTime],I.[idleStartDateTime]),0))/60)),2) as [idleCount]
		--			FROM #tripsFleetStatus _TS 
		--			INNER JOIN [dbo].[FleetHaltStatus] HS (NOLOCK) on _TS.[fleetGuid]=HS.[fleetGuid] 
		--		INNER JOIN [dbo].[FleetIdleStatus] I (NOLOCK) on _TS.[fleetGuid]=I.[fleetGuid] 
		--			GROUP BY _TS.[fleetGuid]) TS ON C.[guid]=TS.[fleetGuid]
		--WHERE C.[companyGuid]=@guid AND C.[isDeleted]=0
		--GROUP BY C.[guid],C.[fleetId]
		SELECT DISTINCT TOP 8  C.[guid],C.[FleetId] 			
				,  CONVERT(DECIMAL(18,2),ISNULL((SELECT TOP 1 CONVERT(DECIMAL(18,2),ISNULL(DATEDIFF(minute,T.[actualStartDateTime],ISNULL(T.[completedDate],@dt)),0))/60 FROM [dbo].[Trip] T (NOLOCK) 
					WHERE T.[fleetGuid] = C.[Guid] AND T.[isStarted]=1 Order by T.[actualStartDateTime] desc 
				),0)) AS [activeCount]	
				, CONVERT(DECIMAL(18,2),TS.[haltCount]) AS [haltCount]
				,  CONVERT(DECIMAL(18,2),TS.[idleCount]) AS [idleCount]
		FROM [dbo].[Fleet] C (NOLOCK) 				
			INNER JOIN (SELECT distinct F.[Guid] AS [fleetGuid]
						--, SUM(ISNULL(DATEDIFF(minute,HS.[haltEndDateTime],HS.[haltStartDateTime]),0)) AS [haltCount]
						, SUM(CONVERT(DECIMAL(18,2),ISNULL(DATEDIFF(minute,HS.[haltEndDateTime],HS.[haltStartDateTime]),0)))/60 AS [haltCount]
						,SUM(CONVERT(DECIMAL(18,2),ISNULL(DATEDIFF(minute,I.[idleEndDateTime],I.[idleStartDateTime]),0)))/60 as [idleCount]
					FROM [dbo].[Fleet] F (NOLOCK)  
					LEFT JOIN [dbo].[FleetHaltStatus] HS (NOLOCK) on F.[guid]=HS.[fleetGuid] 
				LEFT JOIN [dbo].[FleetIdleStatus] I (NOLOCK) on F.[guid]=I.[fleetGuid] 
					GROUP BY F.[Guid],F.[fleetId]) TS ON C.[guid]=TS.[fleetGuid]
					WHERE C.[companyGuid]=@guid AND C.[isDeleted]=0
			
		SET @output = 1
		SET @fieldname = 'Success'  
		SET @syncDate = (SELECT TOP 1 CONVERT(DATETIME,[value]) FROM dbo.[Configuration] (NOLOCK) WHERE [configKey] = 'telemetry-last-exectime')
                           
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
