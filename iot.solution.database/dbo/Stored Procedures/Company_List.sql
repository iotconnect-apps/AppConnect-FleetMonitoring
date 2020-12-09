/*******************************************************************
DECLARE @count INT
     ,@output INT = 0
	,@fieldName				nvarchar(255)	

EXEC [dbo].[Company_List]		 
	@pagesize		= 30
	,@pageNumber	= 1	
	,@orderby		= NULL
	,@count			= @count		OUTPUT	
	,@invokinguser  = '7D31E738-5E24-4EA2-AAEF-47BB0F3CCD41'              
	,@version		= 'v1'              
	,@output		= @output		OUTPUT
	,@fieldname		= @fieldName	OUTPUT	

SELECT @count count, @output status, @fieldName fieldName

001	sgh-1	26-11-2019 [Nishit Khakhi]	Added Initial Version to List Information of Company
002	SGH-97	28-01-2020 [Nishit Khakhi]	Updated to add State, City and Postal Code
*******************************************************************/
CREATE PROCEDURE [dbo].[Company_List]
(	@search				VARCHAR(100)		= NULL
	,@pageSize				INT
	,@pageNumber			INT
	,@count					INT OUTPUT
	,@orderby				VARCHAR(100)		= NULL	
	,@invokingUser			UNIQUEIDENTIFIER
	,@version				VARCHAR(10)
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
            SELECT 'Company_List' AS '@procName'				
		    	, CONVERT(VARCHAR(MAX),@search) AS '@search'
				, CONVERT(VARCHAR(MAX),@pageSize) AS '@pageSize'
				, CONVERT(VARCHAR(MAX),@pageNumber) AS '@pageNumber'
				, CONVERT(VARCHAR(MAX),@orderby) AS '@orderby'
            	, CONVERT(VARCHAR(MAX),@version) AS '@version'
            	, CONVERT(VARCHAR(MAX),@invokingUser) AS '@invokingUser'
            FOR XML PATH('Params')
	    )
	    INSERT INTO DebugInfo(data, dt) VALUES(Convert(VARCHAR(MAX), @Param), GETDATE())
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

		IF OBJECT_ID('tempdb..#temp_company') IS NOT NULL DROP TABLE #temp_company

		CREATE TABLE #temp_company
		(
			[guid]					UNIQUEIDENTIFIER
			,[name]					NVARCHAR(100)
			,[cpId]					NVARCHAR(200)
			,[contactNo]			NVARCHAR(25)
			,[address]				NVARCHAR(500)
			,[countryGuid]			UNIQUEIDENTIFIER
			,[stateGuid]			UNIQUEIDENTIFIER
			,[timezoneGuid]			UNIQUEIDENTIFIER		
			,[city]					NVARCHAR(250)
			,[postalCode]			NVARCHAR(250)
			,[image]				NVARCHAR(250)
			,[adminUserGuid]		UNIQUEIDENTIFIER
			,[entityGuid]		UNIQUEIDENTIFIER
			,[isactive]				BIT
			,[userEmail]			NVARCHAR(200)			
			,[firstName]			NVARCHAR(100)	
			,[lastName]				NVARCHAR(100)
			,[rowNum]				INT
		)

		IF LEN(ISNULL(@orderby, '')) = 0
		SET @orderby = 'name asc'

		DECLARE @Sql nvarchar(MAX) = ''

		SET @Sql = '
		SELECT
			*
			,ROW_NUMBER() OVER (ORDER BY '+@orderby+') AS rowNum
		FROM
		(
			SELECT
			c.[guid]
			,c.[name]
			,c.[cpid]
			,c.[contactNo]
			,c.[address]
			,c.[countryGuid]
			,c.[stateGuid]
			,c.[timezoneGuid]
			,c.[city]
			,c.[postalCode]
			,c.[image]
			,c.[adminUserGuid]
			,c.[entityGuid]
			,c.[isactive]
			,u.email as userEmail
			,u.firstName			
			,u.lastName
			FROM dbo.Company as c WITH (NOLOCK)	
			LEFT JOIN [User] U (NOLOCK) ON C.[adminUserGuid] = U.Guid AND U.isDeleted = 0
			 WHERE c.[isDeleted]=0'
			+ CASE WHEN @search IS NULL THEN '' ELSE ' AND (c.name LIKE ''%' + @search + '%'' OR c.cpid LIKE ''%' + @search + '%''
			)'
			 END +
		') data'

		INSERT INTO #temp_company
		EXEC sp_executesql
			  @Sql
			, N'@orderby VARCHAR(100), @invokingUser UNIQUEIDENTIFIER'
			, @orderby			= @orderby			
			, @invokingUser		= @invokingUser

		SET @count = @@ROWCOUNT

		IF(@pageSize <> -1 AND @pageNumber <> -1)
			BEGIN
				SELECT
					[guid]	
					,[name]
					,[cpId]
					,[contactNo]
					,[address]
					,[countryGuid]
					,[stateGuid]
					,[timezoneGuid]
					,[city]
					,[postalCode]
					,[image]
					,[adminUserGuid]
					,[entityGuid]
					,[isactive]			
					,[userEmail] as userId
					,firstName
					,lastName
				FROM #temp_company
				WHERE rowNum BETWEEN ((@pageNumber - 1) * @pageSize) + 1 AND (@pageSize * @pageNumber)
			END
		ELSE
			BEGIN
				SELECT
					[guid]	
					,[name]
					,[cpId]
					,[contactNo]
					,[address]
					,[countryGuid]
					,[stateGuid]
					,[timezoneGuid]
					,[city]
					,[postalCode]
					,[image]
					,[adminUserGuid]
					,[entityGuid]
					,[isactive]
					,[userEmail] as userId
					,firstName
					,lastName
				FROM #temp_company
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