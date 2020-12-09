/****************************
DECLARE @output INT = 0
		,@fieldName	nvarchar(255)
		,@syncDate	DATETIME
EXEC [dbo].[Chart_EnergyConsumptionByFleet]
	 @guid				= '8E76A334-CB96-4EDE-901E-5A0B982BEAA2'	
	,@invokingUser  	= '7D31E738-5E24-4EA2-AAEF-47BB0F3CCD41'
	,@version			= 'v1'
	,@output			= @output		OUTPUT
	,@fieldName			= @fieldName	OUTPUT
	,@syncDate		= @syncDate		OUTPUT
               
 SELECT @output status,  @fieldName AS fieldName, @syncDate syncDate  
*******************************/
CREATE PROCEDURE [dbo].[Chart_EnergyConsumptionByFleet]
(	@guid				UNIQUEIDENTIFIER	
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
            SELECT 'Chart_EnergyConsumptionByFleet' AS '@procName' 
            , CONVERT(nvarchar(MAX),@guid) AS '@guid' 			
			, CONVERT(nvarchar(MAX),@version) AS '@version' 
            , CONVERT(nvarchar(MAX),@invokinguser) AS '@invokinguser' 
            FOR XML PATH('Params')
	    ) 
	    INSERT INTO DebugInfo(data, dt) VALUES(Convert(nvarchar(MAX), @Param), GETUTCDATE())
    END                    
    
  BEGIN TRY  
		DECLARE @dt DATETIME = GETUTCDATE(), @endDate DATETIME
		IF OBJECT_ID ('tempdb..#ids') IS NOT NULL DROP TABLE #ids
	
		IF OBJECT_ID('tempdb..#EnergyConsumption') IS NOT NULL BEGIN DROP TABLE #EnergyConsumption END
		IF OBJECT_ID('tempdb..#finalTable') IS NOT NULL BEGIN DROP TABLE #finalTable END
		
		CREATE TABLE #EnergyConsumption ([name] NVARCHAR(500),[tripGuid] UNIQUEIDENTIFIER, [EnergyConsumption] BIGINT,[MileageAverage] DECIMAL(18,2)) 
		
		SELECT TOP 5  E.[guid],T.[guid] AS [tripGuid],ISNULL(T.[tripId],'') AS [tripId],ISNULL(T.[coveredMiles],0) AS [totalMiles]
		INTO #ids
		FROM [dbo].[Device] E (NOLOCK) 
		INNER JOIN [dbo].[Fleet] F (NOLOCK) ON F.[Guid]=E.[fleetGuid] 
		INNER JOIN [dbo].[Trip] T (NOLOCK) ON E.[fleetGuid]=T.[fleetGuid] 
		WHERE F.[guid] = @guid  AND T.[isDeleted]=0 AND E.[isDeleted] = 0 AND F.[isDeleted]=0
		 AND T.[isStarted]=1 
		ORDER BY T.[actualStartDateTime] desc

							
			INSERT INTO #EnergyConsumption([name],[tripGuid], [EnergyConsumption],[MileageAverage])
			SELECT DISTINCT [name],[tripGuid],[value] AS [EnergyConsumption],CASE WHEN [value]>0 THEN CONVERT(DECIMAL(18,2),[totalMiles]/[value]) ELSE 0 END AS [MileageAverage] 
			FROM ( 
				SELECT DISTINCT I.[guid] AS [name],I.[tripGuid],I.[totalMiles],SUM([sum]) AS [value] 
				FROM #ids I 
				LEFT JOIN [dbo].[TelemetrySummary_Hourwise] T (NOLOCK) ON T.[deviceGuid] = I.[guid] AND T.[tripGuid]=I.[tripGuid]
				WHERE [attribute] = 'can_currentin' 
				GROUP BY I.[guid],I.[tripGuid],I.[totalMiles]
				) [data]
			
			UPDATE F 
		SET [name]=E.[tripId] 
		FROM #EnergyConsumption F
		LEFT JOIN #ids E ON E.[guid] = F.[name] AND E.[tripGuid]=F.[tripGuid] 
			
		SELECT [name], ISNULL([EnergyConsumption],0) AS [EnergyConsumption],[MileageAverage]  
		FROM #EnergyConsumption
			
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