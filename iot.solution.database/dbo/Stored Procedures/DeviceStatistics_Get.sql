/*******************************************************************
DECLARE @output INT = 0
		,@fieldName				nvarchar(255)
		,@syncDate	DATETIME
EXEC [dbo].[DeviceStatistics_Get]
	 @entityGuid		= 'FA973382-0321-4701-A03E-CDDEAEC9F68B'	
	,@guid				= 'FA973382-0321-4701-A03E-CDDEAEC9F68B'	
	,@invokingUser  	= '7D31E738-5E24-4EA2-AAEF-47BB0F3CCD41'
	,@version			= 'v1'
	,@output			= @output		OUTPUT
	,@fieldName			= @fieldName	OUTPUT	
    ,@syncDate		= @syncDate		OUTPUT          
 SELECT @output status,  @fieldName AS fieldName, @syncDate syncDate    
 
 001	SMAR-4 11-05-2020 [Nishit Khakhi]	Added Initial Version to Get Device Statistics
*******************************************************************/
CREATE PROCEDURE [dbo].[DeviceStatistics_Get]
(	 @entityGuid		UNIQUEIDENTIFIER	= NULL
	,@guid				UNIQUEIDENTIFIER	= NULL
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
            SELECT 'DeviceStatistics_Get' AS '@procName'
			, CONVERT(nvarchar(50),@entityGuid) AS '@entityGuid'
			, CONVERT(nvarchar(50),@guid) AS '@guid'	
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
		DECLARE @uniqueId NVARCHAR(500)

		SELECT TOP 1 @uniqueId = [uniqueId] FROM dbo.[Device] (NOLOCK) WHERE [guid] = @guid AND [isDeleted] = 0

		;WITH CTE_data
		AS (
			SELECT [uniqueId],[localName],[attributeValue],ROW_NUMBER() OVER (PARTITION BY [uniqueId],[localName] ORDER BY [createdDate] DESC) [no]
			FROM IOTConnect.AttributeValue (NOLOCK)
			WHERE [uniqueId] = @uniqueId AND [localName] = 'doorstatus'
		)

		SELECT TOP 1 E.[uniqueId]
			, EM.[startDateTime]
			, CASE WHEN CD.[attributeValue] = 1 THEN 'Open' Else 'Close' END AS [doorStatus]
			--, CASE WHEN [startDate] IS NOT NULL
			--	 AND [startDate] > GETUTCDATE() 
			--	 THEN 
			--		ISNULL(DATEDIFF(DD
			--		,GETUTCDATE()
			--		,[startDate]),0)
			--  ELSE 0
			--  END AS [day]
			--, CASE WHEN [startDate] IS NOT NULL 
			--	AND [startDate] > GETUTCDATE() 
			--	THEN 
			--		ISNULL(DATEDIFF(HH
			--		,GETUTCDATE()
			--		,[startDate])%24,0) 
			--  ELSE 0
			--  END AS [hour]
			--, CASE WHEN [startDate] IS NOT NULL 
			--	AND [startDate] > GETUTCDATE() 
			--	THEN 
			--		ISNULL(DATEDIFF(MINUTE
			--			,GETUTCDATE()
			--			,[startDate])%60,0) 
			--  ELSE 0
			--  END AS [minute]
		FROM [dbo].[Device] E (NOLOCK) 
		LEFT JOIN dbo.[DeviceMaintenance] EM (NOLOCK) ON E.[guid] = EM.[deviceGuid] AND EM.[isDeleted] = 0 
		LEFT JOIN CTE_data CD ON E.[uniqueId] = CD.[uniqueId]
		WHERE (E.[entityGuid] = @entityGuid OR E.[guid] = @guid) AND E.[isDeleted]=0 AND [startDateTime] >= @currentDate
		ORDER BY [startDateTime] ASC


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