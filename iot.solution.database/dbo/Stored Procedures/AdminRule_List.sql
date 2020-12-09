/*******************************************************************
DECLARE @count INT
		,@output INT = 0
		,@fieldName				nvarchar(255)	

EXEC [dbo].[AdminRule_List]		 
	@search		= NULL
	,@invokinguser  = '7D31E738-5E24-4EA2-AAEF-47BB0F3CCD41'              
	,@version		= 'v1'              
	,@count			= @count		OUTPUT	
	,@output		= @output		OUTPUT
	,@fieldname		= @fieldName	OUTPUT	

SELECT @count count, @output status, @fieldName fieldName

001	SG-18	06-03-2020 [Nishit Khakhi]	Added Initial Version to List Admin Rule
002	SAQM-1	24-03-2020 [Nishit Khakhi]	Updated to add isAdmin functionality

*******************************************************************/
CREATE PROCEDURE [dbo].[AdminRule_List]
(	@invokingUser			UNIQUEIDENTIFIER	= NULL
	,@search				nvarchar(100)		= NULL	
	,@isAdmin				BIT					= 0
	,@version				VARCHAR(10)
	,@count					INT					OUTPUT
	,@output				SMALLINT			OUTPUT
	,@fieldName				VARCHAR(255)		OUTPUT
	,@culture				VARCHAR(10)			= 'en-Us'
	,@enableDebugInfo		CHAR(1)				= '0'
)
AS
BEGIN
    SET NOCOUNT ON

    IF (@enableDebugInfo = 1)
	BEGIN
        DECLARE @Param XML
        SELECT @Param =
        (
            SELECT 'AdminRule_List' AS '@procName'		
				, CONVERT(nvarchar(MAX),@search) AS '@search' 		
				, CONVERT(nvarchar(2),@isAdmin) AS '@isAdmin' 	
				, CONVERT(VARCHAR(MAX),@version) AS '@version'
            	, CONVERT(VARCHAR(MAX),@invokingUser) AS '@invokingUser'
            FOR XML PATH('Params')
	    )
	    INSERT INTO DebugInfo(data, dt) VALUES(Convert(VARCHAR(MAX), @Param), GETUTCDATE())
    END
    DECLARE @dt DATETIME = GETUTCDATE()
   
   BEGIN TRY
		IF OBJECT_ID('tempdb..#temp_adminRule') IS NOT NULL DROP TABLE #temp_adminRule
		
		CREATE TABLE #temp_adminRule
		(
			[guid]					UNIQUEIDENTIFIER	
			,[templateGuid]			UNIQUEIDENTIFIER	
			,[kitTypeName]			NVARCHAR(200)	
			,[ruleType]				SMALLINT DEFAULT (1)
			,[name]					NVARCHAR(100)		
			,[attributeGuid]		NVARCHAR(1000)					
			,[conditionText]		NVARCHAR(1000)	
			,[conditionValue]		NVARCHAR(1000)	
			,[severityLevelGuid]	UNIQUEIDENTIFIER	
			,[notificationType]		BIGINT
			,[commandText]			NVARCHAR(500)
			,[commandName]			NVARCHAR(100)
			,[commandValue]			NVARCHAR(100)
			,[description]			NVARCHAR(1000)
			,[localName]			NVARCHAR(100)
			,[tag]					NVARCHAR(50)
			,[isActive]				BIT DEFAULT (1)		
			,[createdDate]			DATETIME			
			,[createdBy]			UNIQUEIDENTIFIER	
		)
			
		DECLARE @Sql nvarchar(MAX) = ''

		SET @Sql = '
		
		SELECT   
			u.[guid]						
			,u.[templateGuid]	
			,k.[name] AS [kitTypeName]			
			,u.[ruleType]			
			,u.[name]					
			,u.[attributeGuid]						
			,u.[conditionText]			
			,u.[conditionValue]					
			,u.[severityLevelGuid]				
			,u.[notificationType]				
			,u.[commandText]		
			,ISNULL(KTC.[name],'''') AS [commandName]
			,u.[commandValue]	
			,u.[description]
			,KTA.[localName]
			,KTA.[tag]
			,u.[isActive]	
			,u.[createdDate]		
			,u.[createdBy]		
			FROM [dbo].[AdminRule] AS u WITH (NOLOCK)
			INNER JOIN [dbo].[KitType] AS k WITH (NOLOCK) ON u.[templateGuid] = k.[guid] AND k.[isDeleted] = 0
			LEFT JOIN [dbo].[KitTypeAttribute] AS KTA WITH (NOLOCK) ON u.[attributeGuid] = CONVERT(NVARCHAR(50),KTA.[guid])
			LEFT JOIN [dbo].[KitTypeCommand] AS KTC WITH (NOLOCK) ON TRY_CONVERT(UNIQUEIDENTIFIER,u.[commandText]) = KTC.[guid]
			WHERE u.[isdeleted] = 0 '	
			+ CASE WHEN @isAdmin = 1 THEN '' ELSE
			' AND u.[isActive] = 1 '
			END 
			+ CASE WHEN @search IS NULL THEN '' ELSE
			' AND u.name LIKE ''%' + @search + '%'''
			END 

		INSERT INTO #temp_adminRule
		EXEC sp_executesql
			  @Sql
			, N'@invokinguser UNIQUEIDENTIFIER'
			, @invokinguser		= @invokinguser
			
		SET @count = @@ROWCOUNT

		SELECT [guid]
			,[templateGuid]
			,[kitTypeName]
			,[ruleType]
			,[name]
			,[attributeGuid]
			,[conditionText]
			,[conditionValue]
			,[severityLevelGuid]
			,[notificationType]
			,[commandText]
			,[commandName]
			,[commandValue]
			,[description]
			,[localName]
			,[tag]
			,[isActive]
			,[createdDate]
			,[createdBy]
		FROM #temp_adminRule
		
        SET @output = 1
		SET @fieldName = 'Success'
	END TRY
	BEGIN CATCH
		DECLARE @errorReturnMessage VARCHAR(MAX)

		SET @output = 0

		SELECT @errorReturnMessage =
			ISNULL(@errorReturnMessage, '') +  SPACE(1)   +
			'ErrorNumber:'  + ISNULL(CAST(ERROR_NUMBER() as VARCHAR), '')  +
			'ErrorSeverity:'  + ISNULL(CAST(ERROR_SEVERITY() as VARCHAR), '') +
			'ErrorState:'  + ISNULL(CAST(ERROR_STATE() as VARCHAR), '') +
			'ErrorLine:'  + ISNULL(CAST(ERROR_LINE () as VARCHAR), '') +
			'ErrorProcedure:'  + ISNULL(CAST(ERROR_PROCEDURE() as VARCHAR), '') +
			'ErrorMessage:'  + ISNULL(CAST(ERROR_MESSAGE() as VARCHAR(max)), '')
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