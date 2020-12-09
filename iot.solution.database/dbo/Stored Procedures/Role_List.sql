
/*******************************************************************
DECLARE @count INT
     ,@output INT = 0
	,@fieldName				nvarchar(255)	
EXEC [dbo].[Role_List]	
	 @companyguid	= '895019CF-1D3E-420C-828F-8971253E5784'
	,@search		= NULL
	,@pagesize		= 30
	,@pageNumber	= 1	
	,@orderby		= 'name desc'
	,@count			= @count		OUTPUT	
	,@invokinguser  = 'E05A4DA0-A8C5-4A4D-886D-F61EC802B5FD'              
	,@version		= 'v1'              
	,@output		= @output		OUTPUT
	,@fieldname		= @fieldName	OUTPUT	

SELECT @count count, @output status, @fieldName fieldName

001	SGH-1 11-12-2019 [Nishit Khakhi]	Added Initial Version to List Role
*******************************************************************/
CREATE PROCEDURE [dbo].[Role_List]
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
	,@culture				nvarchar(10)		= 'en-Us'	
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
            SELECT 'Role_List' AS '@procName' 
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
		
		DECLARE @entityGuid UNIQUEIDENTIFIER 

		SELECT @entityGuid = [entityGuid] 
		FROM [dbo].[User] (NOLOCK) 
		WHERE companyguid = @companyguid 
		AND [guid] = @invokinguser
		AND [isdeleted]=0

		IF ISNULL(@entityGuid, '00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000' BEGIN
			Set @output = -3
			SET @fieldname = 'UserNotFound'
			RETURN;
		END        
		      
		
		IF OBJECT_ID('tempdb..#temp_Role') IS NOT NULL DROP TABLE #temp_Role
		
		CREATE TABLE #temp_Role
		(
			[guid]					UNIQUEIDENTIFIER
			,[companyguid]			UNIQUEIDENTIFIER
			,[name]					nvarchar(100)
			,[description]			nvarchar(500)		 
			,[isAdminRole]			BIT
			,[isactive]				BIT
			,[createdby]			UNIQUEIDENTIFIER
			,[createdDate]			DATETIME
			,[updatedBy]			UNIQUEIDENTIFIER
			,[updatedDate]			DATETIME
			,[row_num]				INT
		)
			
		IF LEN(ISNULL(@orderby, '')) = 0
		SET @orderby = 'name asc'

		DECLARE @Sql nvarchar(MAX) = ''

		SET @Sql = '
		
		SELECT 
			*
			,ROW_NUMBER() OVER (ORDER BY '+@orderby+') AS row_num
		FROM
		(
			SELECT   
			r.[guid], 
			r.[companyGuid], 
			r.[name], 
			r.[description], 
			r.[isAdminRole], 
			r.[isActive], 
			r.[createdBy], 
			r.[createdDate], 
			r.[updatedBy],
			r.[updatedDate]
			FROM [dbo].[Role] AS r WITH (NOLOCK)
			WHERE r.[companyGuid] = @companyguid AND r.[isdeleted] = 0'								
			+ CASE WHEN @search IS NULL THEN '' ELSE
			' AND (r.name LIKE ''%' + @search + '%'' or r.description like ''%' + @search +'%'') '
			 END +
		') data '
		

		INSERT INTO #temp_Role
		EXEC sp_executesql
			  @Sql
			, N'@orderby nvarchar(100), @companyguid UNIQUEIDENTIFIER, @invokinguser UNIQUEIDENTIFIER, @entityGuid UNIQUEIDENTIFIER'
			, @orderby			= @orderby			
			, @companyguid		= @companyguid
		    , @invokinguser		= @invokinguser
			, @entityGuid		= @entityGuid
		    
		SET @count = @@ROWCOUNT
		
		--PRINT @Sql

		SELECT 
			[guid]					
			,[companyguid] AS [companyGuid]
			,[name]	AS [name]
			,[description] AS [description]
			,[isAdminRole] AS [isAdminRole]
			,[isactive]	AS [isActive]	
			,[createdby] AS [createdBy]
			,[createdDate] AS [createdDate]
			,[updatedBy] AS [updatedBy]
			,[updatedDate] AS [updatedDate]
		FROM #temp_Role
		WHERE row_num BETWEEN ((@pagenumber - 1) * @pagesize) + 1 AND (@pagesize * @pagenumber)			
	   
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