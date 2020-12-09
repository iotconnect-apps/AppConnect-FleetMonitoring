/*******************************************************************
DECLARE @count INT
     	,@output INT = 0
		,@fieldName	VARCHAR(255)

EXEC [dbo].[DeviceType_List]
	@companyGuid  = 'C1596B8C-7065-4D63-BFD0-4B835B93DFF2'
	,@search		= 'manf'
	,@pageSize		= 10
	,@pageNumber	= 1
	,@orderby		= NULL
	,@count			= @count OUTPUT
	,@invokingUser  = 'C1596B8C-7065-4D63-BFD0-4B835B93DFF2'
	,@version		= 'v1'
	,@output		= @output	OUTPUT
	,@fieldName		= @fieldName	OUTPUT

SELECT @count count, @output status, @fieldName fieldName

001	SAM-138	01-07-2020 [Nishit Khakhi]	Added Initial Version to get List of Device Type
*******************************************************************/
CREATE PROCEDURE [dbo].[DeviceType_List]
(	@companyGuid		UNIQUEIDENTIFIER
	,@search			VARCHAR(100)		= NULL
	,@pageSize			INT
	,@pageNumber		INT
	,@orderby			VARCHAR(100)		= NULL
	,@invokingUser		UNIQUEIDENTIFIER
	,@version			VARCHAR(10)
	,@culture			VARCHAR(10)			= 'en-Us'
	,@output			SMALLINT			OUTPUT
	,@fieldName			VARCHAR(255)		OUTPUT
	,@count				INT OUTPUT
	,@enableDebugInfo		CHAR(1)			= '0'
)
AS
BEGIN
    SET NOCOUNT ON

    IF (@enableDebugInfo = 1)
	BEGIN
        DECLARE @Param XML
        SELECT @Param =
        (
            SELECT 'DeviceType_List' AS '@procName'
				, CONVERT(VARCHAR(MAX),@companyGuid) AS '@companyGuid'
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
    
    BEGIN TRY

		SELECT @output = 1
			  ,@count = -1

		IF OBJECT_ID('tempdb..#temp_DeviceType') IS NOT NULL DROP TABLE #temp_DeviceType

		CREATE TABLE #temp_DeviceType
		(	[companyGuid]		UNIQUEIDENTIFIER 
			,[guid]				UNIQUEIDENTIFIER 
			,[name]				NVARCHAR (200)   
			,[make]				NVARCHAR (200)   
			,[model]			NVARCHAR (200) 
			,[manufacturer]		NVARCHAR (200) 
			,[entityGuid]		UNIQUEIDENTIFIER 
			,[templateGuid]		UNIQUEIDENTIFIER
			,[description]		NVARCHAR (500)
			,[isActive]			BIT
			,[rowNum]			INT
		)

		IF LEN(ISNULL(@orderby, '')) = 0
		SET @orderby = 'name asc'

		DECLARE @Sql nvarchar(MAX) = ''

		SET @Sql = '
		SELECT
			*
			,ROW_NUMBER() OVER (ORDER BY '+@orderby+') AS rowNum
		FROM
		( SELECT
			DT.[companyGuid]	
			,DT.[guid]
			,DT.[name]				
			,DT.[make]				
			,DT.[model]			
			,DT.[manufacturer]		
			,DT.[entityGuid]		
			,DT.[templateGuid]		
			,DT.[description]		
			,DT.[isActive]			
			FROM [dbo].[DeviceType] DT WITH (NOLOCK) 
			WHERE DT.[companyGuid] = @companyGuid AND DT.[isDeleted]=0 '
			+ CASE WHEN @search IS NULL THEN '' ELSE
			' AND (DT.[name] LIKE ''%' + @search + '%''
			  OR DT.[make] LIKE ''%' + @search + '%'' 
			  OR DT.[model] LIKE ''%' + @search + '%'' 
			  OR DT.[manufacturer] LIKE ''%' + @search + '%'' 
			)'
			 END +
		' )  data '
		
		INSERT INTO #temp_DeviceType
		EXEC sp_executesql 
			  @Sql
			, N'@orderby VARCHAR(100), @companyGuid UNIQUEIDENTIFIER '
			, @orderby		= @orderby			
			, @companyGuid  = @companyGuid
		SET @count = @@ROWCOUNT

		IF(@pageSize <> -1 AND @pageNumber <> -1)
			BEGIN
				SELECT 
					DT.[companyGuid]
					,DT.[guid]
					,DT.[name]				
					,DT.[make]				
					,DT.[model]			
					,DT.[manufacturer]		
					,DT.[entityGuid]		
					,DT.[templateGuid]		
					,DT.[description]		
					,DT.[isActive]					
				FROM #temp_DeviceType DT
				WHERE rowNum BETWEEN ((@pageNumber - 1) * @pageSize) + 1 AND (@pageSize * @pageNumber)			
			END
		ELSE
			BEGIN
				SELECT 
					DT.[companyGuid]
					,DT.[guid]
					,DT.[name]				
					,DT.[make]				
					,DT.[model]			
					,DT.[manufacturer]		
					,DT.[entityGuid]		
					,DT.[templateGuid]		
					,DT.[description]		
					,DT.[isActive]					
				FROM #temp_DeviceType DT
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