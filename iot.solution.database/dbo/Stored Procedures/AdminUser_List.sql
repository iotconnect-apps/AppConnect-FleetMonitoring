
/*******************************************************************
DECLARE @count INT
     ,@output INT = 0
	,@fieldName				nvarchar(255)	
EXEC [dbo].[AdminUser_List]	
	 @companyguid	= '895019CF-1D3E-420C-828F-8971253E5784'
	,@search		= NULL
	,@pagesize		= 30
	,@pageNumber	= 1	
	,@orderby		= 'name desc'
	,@count			= @count		OUTPUT	
	,@version		= 'v1'              
	,@output		= @output		OUTPUT
	,@fieldname		= @fieldName	OUTPUT	

SELECT @count count, @output status, @fieldName fieldName

001	SGH-97 15-01-2020 [Nishit Khakhi]	Added Initial Version to List Admin Users
*******************************************************************/
CREATE PROCEDURE [dbo].[AdminUser_List]
(	@companyguid			UNIQUEIDENTIFIER
	,@search				nvarchar(100)		= NULL	
	,@pagesize				INT
	,@pagenumber			INT
	,@count					INT					OUTPUT
	,@orderby				nvarchar(100)		= NULL
	,@invokinguser			UNIQUEIDENTIFIER	= NULL
	,@version				nvarchar(10)              
	,@output				SMALLINT			OUTPUT
	,@fieldname				nvarchar(255)		OUTPUT
	,@culture				nvarchar(10)			= 'en-Us'	
	,@enabledebuginfo		CHAR(1)				= '0'
)
AS
BEGIN
    SET NOCOUNT ON

    IF (@enabledebuginfo = 1)
	BEGIN
        DECLARE @Param XML 
        SELECT @Param = 
        (
            SELECT 'AdminUser_List' AS '@procName' 
            , CONVERT(nvarchar(MAX),@companyguid) AS '@companyguid' 
			, CONVERT(nvarchar(MAX),@search) AS '@search' 
			, CONVERT(nvarchar(MAX),@pagesize) AS '@pagesize' 
			, CONVERT(nvarchar(MAX),@pagenumber) AS '@pagenumber' 
			, CONVERT(nvarchar(MAX),@orderby) AS '@orderby' 
            , CONVERT(nvarchar(MAX),@version) AS '@version' 
            , CONVERT(nvarchar(MAX),@invokinguser) AS '@invokinguser' 
            FOR XML PATH('Params')
	    ) 
	    INSERT INTO DebugInfo(data, dt) VALUES(Convert(nvarchar(MAX), @Param), GETDATE())
    END                    
    
    BEGIN TRY            
		
		IF OBJECT_ID('tempdb..#temp_user') IS NOT NULL DROP TABLE #temp_user
		
		CREATE TABLE #temp_user
		(
			[guid]					UNIQUEIDENTIFIER
			,[email]				NVARCHAR(100)
			,[companyguid]			UNIQUEIDENTIFIER
			,[firstname]			NVARCHAR(50)
			,[lastname]				NVARCHAR(50)
			,[name]					NVARCHAR(150)
			,[contactno]			NVARCHAR(25)		 
			,[isactive]				BIT
			,[createdby]			UNIQUEIDENTIFIER
			,[row_num]				INT
		)
			
		IF LEN(ISNULL(@orderby, '')) = 0
		SET @orderby = 'firstname asc'

		DECLARE @Sql nvarchar(MAX) = ''

		SET @Sql = '
		
		SELECT 
			*
			,ROW_NUMBER() OVER (ORDER BY '+@orderby+') AS row_num
		FROM
		(
			SELECT   
			u.[guid]					
			,u.[email] AS [email]								
			,u.[companyguid]			
			,u.[firstname]			
			,u.[lastname]			
			,(u.[firstname] + '' '' + u.[lastname]) AS name
			,u.[contactno]		  
			,u.[isactive]	
			,u.[createdby]
			FROM [dbo].[AdminUser] AS u WITH (NOLOCK)
			WHERE u.companyguid = @companyguid AND u.[isdeleted] = 0 '								
			+ CASE WHEN @search IS NULL THEN '' ELSE
			' AND (u.firstname LIKE ''%' + @search + '%'' OR u.lastname LIKE ''%' + @search + '%''
			  OR (u.firstname + '' '' + u.lastname) LIKE ''%' + @search + '%''
			  OR u.email LIKE ''%' + @search + '%''			  
			  OR u.contactNo LIKE ''%' + @search + '%''			  
			) '
			 END +
		') data '
		

		INSERT INTO #temp_user
		EXEC sp_executesql
			  @Sql
			, N'@orderby nvarchar(100), @companyguid UNIQUEIDENTIFIER, @invokinguser UNIQUEIDENTIFIER'
			, @orderby			= @orderby			
			, @companyguid		= @companyguid
		    , @invokinguser		= @invokinguser
			
		SET @count = @@ROWCOUNT
		
		--PRINT @Sql
		IF @pagesize = -1
		BEGIN
			SELECT 
			[guid]					
			,[email] AS [email]
			,[companyguid] AS [companyGuid]
			,[firstname] AS [firstName]		
			,[lastname]	AS [lastName]		
			,[name]	AS [name]
			,[contactno] AS [contactNo]			  
			,[isactive]	AS [isActive]	
			,[createdby] AS [createdBy]
			FROM #temp_user
		END
		ELSE
		BEGIN
			SELECT 
				[guid]					
				,[email] AS [email]
				,[companyguid] AS [companyGuid]
				,[firstname] AS [firstName]		
				,[lastname]	AS [lastName]		
				,[name]	AS [name]
				,[contactno] AS [contactNo]			  
				,[isactive]	AS [isActive]	
				,[createdby] AS [createdBy]
				FROM #temp_user
			WHERE row_num BETWEEN ((@pagenumber - 1) * @pagesize) + 1 AND (@pagesize * @pagenumber)			
		END

        SET @output = 1
		SET @fieldname = 'Success'   
              
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