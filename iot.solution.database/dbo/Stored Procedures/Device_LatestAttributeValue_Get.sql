/*******************************************************************
DECLARE @output INT = 0
		,@fieldName				nvarchar(255)
		,@syncDate	DATETIME
EXEC [dbo].[Device_LatestAttributeValue_Get]
	 @guid				= '2D442AEA-E58B-4E8E-B09B-5602E1AA545A'	
	,@invokingUser  	= '7D31E738-5E24-4EA2-AAEF-47BB0F3CCD41'
	,@version			= 'v1'
	,@output			= @output		OUTPUT
	,@fieldName			= @fieldName	OUTPUT
	,@syncDate		= @syncDate		OUTPUT
               
 SELECT @output status,  @fieldName AS fieldName, @syncDate syncDate    
 
 001	SAQM-1 02-04-2020 [Nishit Khakhi]	Added Initial Version to Get Device Attribute Latest Value
*******************************************************************/

CREATE PROCEDURE [dbo].[Device_LatestAttributeValue_Get]
(	 @guid				UNIQUEIDENTIFIER	
	,@isAdmin			BIT					= 0
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
            SELECT 'Device_LatestAttributeValue_Get' AS '@procName'
			, CONVERT(nvarchar(MAX),@guid) AS '@guid'			
			, CoNVERT(NVARCHAR(2),@isAdmin) AS '@isAdmin'
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

		IF OBJECT_ID('tempdb..#result') IS NOT NULL BEGIN DROP TABLE #result END
		
		CREATE TABLE #result([key] NVARCHAR(1000), [description] NVARCHAR(500), [value] DECIMAL(18,2))

		DECLARE	@uniqueId NVARCHAR(500)

		SELECT TOP 1 @uniqueId = [uniqueId] FROM dbo.[Device] (NOLOCK) WHERE [guid] = @guid AND [isDeleted] = 0
		SET @syncDate = (SELECT TOP 1 [createdDate] FROM [IOTConnect].[AttributeValue] WHERE [uniqueId] = @uniqueId ORDER BY [createdDate] DESC)
		INSERT INTO #result([key],[description])
		SELECT DISTINCT [code],[description]
		FROM dbo.[KitTypeAttribute]

		;WITH CTE_data
		AS (
			SELECT [uniqueId],[localName],[attributeValue],ROW_NUMBER() OVER (PARTITION BY [uniqueId],[localName] ORDER BY [createdDate] DESC) [no]
			FROM IOTConnect.AttributeValue (NOLOCK)
			WHERE [uniqueId] = @uniqueId
		)
		UPDATE R
		SET [value] = CONVERT(DECIMAL(18,7),attributeValue)
		FROM #result R
		LEFT JOIN CTE_data C ON R.[key] = C.[localName] AND C.[uniqueId] = @uniqueId
		WHERE C.[no] = 1

		SELECT @uniqueId AS [uniqueId], [key], [description], ISNULL(ROUND([value],2),0) AS [value] 
		FROM #result
		--WHERE ISNULL([value],0) > 0

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