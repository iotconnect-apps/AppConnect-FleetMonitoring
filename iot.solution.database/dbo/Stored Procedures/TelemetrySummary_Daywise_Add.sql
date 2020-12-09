/*******************************************************************
EXEC [dbo].[TelemetrySummary_Daywise_Add]	
	
001	SAQM-1 08-04-2020 [Nishit Khakhi]	Added Initial Version to Add Telemetry Summary Day wise for Day
*******************************************************************/
CREATE PROCEDURE [dbo].[TelemetrySummary_Daywise_Add]
AS
BEGIN
	SET NOCOUNT ON
	
	BEGIN TRY
	DECLARE @dt DATETIME = GETUTCDATE(), @lastExecDate DATETIME
	SELECT 
		TOP 1 @lastExecDate = CONVERT(DATETIME,[value]) 
	FROM [dbo].[Configuration] 
	WHERE [configKey] = 'telemetry-last-exectime' AND [isDeleted] = 0

	BEGIN TRAN	
		IF OBJECT_ID('tempdb..#temp_data') IS NOT NULL DROP TABLE #temp_data
	
		CREATE TABLE #temp_data
		(	[companyGuid]	UNIQUEIDENTIFIER NOT NULL
			, [deviceGuid]	UNIQUEIDENTIFIER NOT NULL
			, [attrbGuid] UNIQUEIDENTIFIER NOT NULL
			, [totalCount]	BIGINT NOT NULL
			, [utilizedCount] BIGINT NOT NULL
			, [createdDate] DATE NOT NULL
		)

		DECLARE @Sql nvarchar(MAX) = ''

		SET @Sql = '
		
			;WITH CTE_DATA
		AS (
		SELECT A.[companyGuid],
			D.[guid] AS [deviceGuid],
			DA.[guid] AS [attrbGuid],
			D.[sensorCondition],
			D.[sensorValue],
			A.[attributeValue],
			CONVERT(DATE,A.[createdDate]) [createdDate]
		FROM IOTConnect.[AttributeValue] A (NOLOCK) 
		INNER JOIN dbo.[device] D (NOLOCK) ON A.[uniqueId] = D.[uniqueId] AND D.[isDeleted] = 0
		INNER JOIN dbo.[DeviceAttribute] DA (NOLOCK) ON D.[guid] = DA.[deviceGuid] AND A.[localName] = DA.[attrName] AND DA.[isDeleted] = 0
		WHERE CONVERT(DATE,A.[createdDate]) BETWEEN CONVERT(DATE,@lastExecDate) AND CONVERT(DATE,@dt)
		)
		Insert INTO #temp_data
		SELECT 
			[companyGuid]
			, [deviceGuid]
			, [attrbGuid]
			, COUNT(1) AS [totalCount]
			, SUM(	CASE WHEN [sensorCondition] = ''='' AND [sensorValue] = [attributeValue] THEN 1
						ELSE CASE WHEN [sensorCondition] = ''!='' AND [sensorValue] <> [attributeValue] THEN 1
						ELSE CASE WHEN [sensorCondition] = ''>'' AND [sensorValue] > [attributeValue] THEN 1
						ELSE CASE WHEN [sensorCondition] = ''>='' AND [sensorValue] >= [attributeValue] THEN 1
						ELSE CASE WHEN [sensorCondition] = ''<'' AND [sensorValue] < [attributeValue] THEN 1
						ELSE CASE WHEN [sensorCondition] = ''<='' AND [sensorValue] <= [attributeValue] THEN 1
						ELSE 0
					END 
					END 
					END 
					END 
					END 
					END
			) AS [utilizedCount]
			, [createdDate] 		
		FROM CTE_DATA 
		GROUP BY [companyGuid], [deviceGuid], [attrbGuid], [createdDate]
		'		
		
		EXEC sp_executesql 
			  @Sql
			, N' @lastExecDate DATETIME, @dt DATETIME '
			, @dt = @dt
			, @lastExecDate = @lastExecDate
		INSERT INTO dbo.[DeviceAttributeUtilization]
		select NEWID(), [companyGuid], [deviceGuid], [attrbGuid], [totalCount], [utilizedCount], [createdDate]
		FROM #temp_data
		
		UPDATE C 
		SET [value] = convert(nvarchar(50),@dt,121)
		FROM [dbo].[Configuration] C 
		WHERE [configKey] = 'telemetry-last-exectime' AND [isDeleted] = 0

	COMMIT TRAN	

	END TRY	
	BEGIN CATCH
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