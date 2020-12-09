/*******************************************************************
DECLARE @count INT
     ,@output INT = 0
	,@fieldName					nvarchar(255)
	,@syncDate	DATETIME
EXEC [dbo].[Chart_UtilizationByFleetType]	
	@companyGuid = 'FB75BA99-7AB0-4275-9661-18D00F2C4A3E'
	,@parentEntityGuid = 'FB75BA99-7AB0-4275-9661-18D00F2C4A3E'	
	,@invokinguser  = 'E05A4DA0-A8C5-4A4D-886D-F61EC802B5FD'              
	,@version		= 'v1'              
	,@output		= @output		OUTPUT
	,@fieldname		= @fieldName	OUTPUT
	,@syncDate		= @syncDate		OUTPUT

SELECT @output status, @fieldName fieldName, @syncDate syncDate


*******************************************************************/
CREATE PROCEDURE [dbo].[Chart_UtilizationByFleetType]
(	@companyGuid		UNIQUEIDENTIFIER	
	,@parentEntityGuid	UNIQUEIDENTIFIER	= NULL
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

    IF (@enabledebuginfo = 1)
	BEGIN
        DECLARE @Param XML 
        SELECT @Param = 
        (
            SELECT 'Chart_UtilizationByFleetType' AS '@procName' 
            , CONVERT(nvarchar(MAX),@companyGuid) AS '@companyGuid' 
			, CONVERT(nvarchar(MAX),@version) AS '@version' 			
			, CONVERT(nvarchar(MAX),@parentEntityGuid) AS '@parentEntityGuid' 
            , CONVERT(nvarchar(MAX),@invokinguser) AS '@invokinguser' 
            FOR XML PATH('Params')
	    ) 
	    INSERT INTO DebugInfo(data, dt) VALUES(Convert(nvarchar(MAX), @Param), GETUTCDATE())
    END                    
    
      BEGIN TRY  
		DECLARE @dt DATETIME = GETUTCDATE(), @endDate DATETIME
		IF OBJECT_ID ('tempdb..#idsFT') IS NOT NULL DROP TABLE #idsFT
		IF OBJECT_ID('tempdb..#tripsFT') IS NOT NULL BEGIN DROP TABLE #tripsFT END
		
		SELECT E.[companyGuid] as [companyGuid],E.[uniqueId] as [uniqueId], E.[guid] as [guid], G.[typeGuid] AS [typeGuid], DT.[name] AS [name]
		INTO #idsFT
		FROM [dbo].[Device] E (NOLOCK) 		
		INNER JOIN [dbo].[Fleet] G WITH (NOLOCK) ON E.[fleetGuid] = G.[guid] AND G.[isDeleted] = 0 	
		INNER JOIN [dbo].[FleetType] DT (NOLOCK) ON G.[typeGuid] = DT.[guid] AND DT.[isDeleted] = 0	
		WHERE E.[companyGuid] = @companyGuid AND E.isDeleted = 0 
	
		CREATE TABLE #tripsFT ([companyGuid] UNIQUEIDENTIFIER, [fleetGuid] UNIQUEIDENTIFIER, [typeGuid] UNIQUEIDENTIFIER,[startDateTime] DATETIME,[endDateTime] DATETIME,[isStarted] BIT,[isCompleted] BIT) 		
			INSERT INTO #tripsFT ([companyGuid],[fleetGuid], [typeGuid],[startDateTime],[endDateTime],[isStarted] ,[isCompleted]) 
			SELECT distinct T.[companyGuid],T.[fleetGuid],G.[typeGuid], T.[startDateTime],			
			(SELECT top 1 endDateTime from [dbo].[TripStops] (NOLOCK) WHERE [tripGuid] = T.[Guid] AND [isDeleted] = 0 ORDER BY [endDateTime] DESC) AS [endDateTime] 			
				,[isStarted] ,[isCompleted] 
				FROM [dbo].[Trip] T (NOLOCK) 
				INNER JOIN [dbo].[Fleet] G WITH (NOLOCK) ON T.[fleetGuid] = G.[guid] AND G.[isDeleted] = 0 	
				INNER JOIN [dbo].[FleetType] DT (NOLOCK) ON G.[typeGuid] = DT.[guid] AND DT.[isDeleted] = 0	
				WHERE T.[companyGuid] = @companyGuid AND T.[isDeleted] = 0 
			
		;WITH 
		CTE_TripStatus
		AS (	
				SELECT [companyGuid],DM.[typeGuid] 
				,CASE WHEN  (DM.[isStarted]=1 AND DM.[isCompleted]=0)--(@dt >= DM.[startDateTime] AND @dt <= DM.[endDateTime]) OR
					 THEN 'InTransit'
					 ELSE 'Other'
					 END
					 AS [status] 
				FROM #tripsFT DM (NOLOCK) 
				WHERE [companyGuid] = @companyGuid
							
		)		
		
		SELECT [name], CASE WHEN [totalCount] > 0 THEN ISNULL(([utilizedCount] * 100 / [totalCount]),0)
					ELSE 0
					END AS [utilizationPer],CASE WHEN (CASE WHEN [totalCount] > 0 THEN ISNULL(([utilizedCount] * 100 / [totalCount]),0)
					ELSE 0
					END) >=50 THEN 'green' ELSE 'red' END as [Color] 
		FROM (
		SELECT I.[name], COUNT(I.[guid]) AS [totalCount], SUM(CASE WHEN DAU.[status] = 'InTransit' THEN 1 ELSE 0 END) AS [utilizedCount]
		FROM #idsFT I 
		LEFT JOIN CTE_TripStatus DAU ON I.[companyGuid]=DAU.[companyGuid] AND I.[typeGuid]=DAU.[typeGuid] 
		GROUP BY I.[name]
		) A
		 
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