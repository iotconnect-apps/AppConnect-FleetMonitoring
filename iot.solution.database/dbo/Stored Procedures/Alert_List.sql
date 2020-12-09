
/*******************************************************************
DECLARE @count INT
     ,@output INT = 0
	,@fieldName				nvarchar(255)	

EXEC [dbo].[Alert_List]		 
	@deviceGuid	= 'FA973382-0321-4701-A03E-CDDEAEC9F68B'
	,@pagesize		= 30
	,@pageNumber	= 1	
	,@orderby		= NULL
	,@count			= @count		OUTPUT	
	,@invokinguser  = '7D31E738-5E24-4EA2-AAEF-47BB0F3CCD41'              
	,@version		= 'v1'              
	,@output		= @output		OUTPUT
	,@fieldname		= @fieldName	OUTPUT	

SELECT @count count, @output status, @fieldName fieldName

001	SAQM-1	20-03-2020	[Nishit Khakhi]	Added Initial Version to List Alerts
002	SAM-138	13/07/2020	[Nishit Khakhi]	Updated to add parentEntityGuid param
*******************************************************************/
CREATE PROCEDURE [dbo].[Alert_List]
(	@companyGuid		UNIQUEIDENTIFIER	= NULL
	,@parentEntityGuid	UNIQUEIDENTIFIER	= NULL
	,@entityGuid		UNIQUEIDENTIFIER	= NULL
	,@deviceGuid		UNIQUEIDENTIFIER	= NULL
	,@currentDate Datetime =NULL
	,@search			VARCHAR(100)		= NULL
	,@pageSize			INT
	,@pageNumber		INT
	,@count				INT OUTPUT
	,@orderby			VARCHAR(100)		= NULL	
	,@invokingUser		UNIQUEIDENTIFIER
	,@version			VARCHAR(10)
	,@output			SMALLINT			OUTPUT
	,@fieldName			VARCHAR(255)		OUTPUT
	,@culture			VARCHAR(10)			= 'en-Us'
	,@enableDebugInfo	CHAR(1)				= '0'
)
AS
BEGIN
    SET NOCOUNT ON

    IF (@enableDebugInfo = 1)
	BEGIN
        DECLARE @Param XML
        SELECT @Param =
        (
            SELECT 'Alert_List' AS '@procName'				
		    	, CONVERT(VARCHAR(MAX),@companyGuid) AS '@companyGuid'
				, CONVERT(VARCHAR(MAX),@entityGuid) AS '@entityGuid'
				, CONVERT(VARCHAR(MAX),@parentEntityGuid) AS '@parentEntityGuid'
            	, CONVERT(VARCHAR(MAX),@deviceGuid) AS '@deviceGuid'
				, CONVERT(VARCHAR(MAX),@search) AS '@search'
				, CONVERT(VARCHAR(MAX),@pageSize) AS '@pageSize'
				, CONVERT(VARCHAR(MAX),@pageNumber) AS '@pageNumber'
				, CONVERT(VARCHAR(MAX),@orderby) AS '@orderby'
            	, CONVERT(VARCHAR(MAX),@version) AS '@version'
            	, CONVERT(VARCHAR(MAX),@invokingUser) AS '@invokingUser'
            FOR XML PATH('Params')
	    )
	    INSERT INTO DebugInfo(data, dt) VALUES(Convert(VARCHAR(MAX), @Param), GETUTCDATE())
    END
     DECLARE @dt DATETIME = GETUTCDATE()
	 DECLARE @poutput	SMALLINT,@pfieldname	nvarchar(100)

	IF(@poutput!=1)
		BEGIN
			SET @output = @poutput
			SET @fieldName = @pfieldName
			RETURN;
		END		
 BEGIN TRY

		SELECT
		 @output = 1
		,@count = -1

		IF OBJECT_ID('tempdb..#temp_alerts') IS NOT NULL DROP TABLE #temp_alerts

		CREATE TABLE #temp_alerts
		(
			[guid]					UNIQUEIDENTIFIER
			,[message]				NVARCHAR(500)
			,[eventDate]			DATETIME
			,[ruleName]				NVARCHAR(200)
			,[uniqueId]				NVARCHAR(500)
			,[severity]				NVARCHAR(200)
			,[entityName]			NVARCHAR(500)
			,[subEntityName]		NVARCHAR(500)
			,[deviceName]			NVARCHAR(500)
			,[rowNum]				INT
		)

		IF LEN(ISNULL(@orderby, '')) = 0
		SET @orderby = 'uniqueId asc'

		DECLARE @Sql nvarchar(MAX) = ''

		SET @Sql = '	SELECT
							*
							,ROW_NUMBER() OVER (ORDER BY '+@orderby+') AS rowNum
						FROM
						(	SELECT
								I.[guid]		
								, I.[message]		
								, I.[eventDate]
								,I.[ruleName]
								, E.[uniqueId]	
								, I.[severity]		
								, F.[fleetId] AS [entityName]
								, G.[name] AS [subEntityName]
								, E.[name] AS deviceName
							FROM dbo.[IOTConnectAlert] I WITH (NOLOCK)
							INNER JOIN dbo.[Device] E WITH (NOLOCK) ON I.[deviceGuid] = E.[guid] AND E.[isDeleted] = 0
							INNER JOIN dbo.[Entity] G WITH (NOLOCK) ON I.[entityGuid] = G.[guid] AND G.[isDeleted] = 0
							LEFT JOIN [dbo].[Fleet] F WITH (NOLOCK) ON E.[fleetGuid] = F.[guid] AND F.[isDeleted] = 0 
							WHERE I.[companyGuid] = ISNULL(@companyGuid,I.[companyGuid]) '						
						+CASE WHEN @entityGuid IS NULL THEN '' ELSE ' AND E.[fleetGuid] = @entityGuid ' END +
						+CASE WHEN @deviceGuid IS NULL THEN '' ELSE ' AND I.[deviceGuid] = @deviceGuid ' END +
--						AND (G.[parentEntityGuid] = ISNULL(@entityGuid,G.[parentEntityGuid]) OR I.[entityGuid] = ISNULL(@entityGuid,I.[entityGuid]))
						+CASE WHEN @currentDate IS NULL THEN '' ELSE ' AND CONVERT(Date,I.[eventDate]) = CONVERT(DATE,@currentDate) ' END 
						+ CASE WHEN @search IS NULL THEN '' ELSE ' AND (E.name LIKE ''%' + @search + '%''
							  OR E.[uniqueId] LIKE ''%' + @search + '%'' 
							  OR G.[name] LIKE ''%' + @search + '%'' 
							  OR F.[fleetId] LIKE ''%' + @search + '%'')'
						 END +
					') data'

		INSERT INTO #temp_alerts
		EXEC sp_executesql
			  @Sql
			, N'@orderby VARCHAR(100), @invokingUser UNIQUEIDENTIFIER, @companyGuid UNIQUEIDENTIFIER, @entityGuid UNIQUEIDENTIFIER,@currentDate DATETIME, @deviceGuid UNIQUEIDENTIFIER, @parentEntityGuid UNIQUEIDENTIFIER '
			, @orderby			= @orderby			
			, @invokingUser		= @invokingUser
			, @companyGuid		= @companyGuid
			, @entityGuid		= @entityGuid
			, @deviceGuid		= @deviceGuid
			, @parentEntityGuid	= @parentEntityGuid
			,@currentDate=@currentDate
		SET @count = @@ROWCOUNT

		IF(@pageSize <> -1 AND @pageNumber <> -1)
			BEGIN
				SELECT [guid]		
						,[message]		
						,[eventDate]
						,[ruleName]
						,[uniqueId]	
						,[severity]	
						,[entityName]
						,[subEntityName]
						,[deviceName]
				FROM #temp_alerts
				WHERE rowNum BETWEEN ((@pageNumber - 1) * @pageSize) + 1 AND (@pageSize * @pageNumber)
			END
		ELSE
			BEGIN
				SELECT
					[guid]		
						,[message]		
						,[eventDate]
						,[ruleName]
						,[uniqueId]	
						,[severity]	
						,[entityName]
						,[subEntityName]
						,[deviceName]
				FROM #temp_alerts
			END

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
