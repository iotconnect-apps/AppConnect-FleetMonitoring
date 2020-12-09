
/*******************************************************************
DECLARE @count INT
     ,@output INT = 0
	,@fieldName				nvarchar(255)	

EXEC [dbo].[User_Search]	
	 @companyGuid	= '895019CF-1D3E-420C-828F-8971253E5784'
	,@search		= NULL
	,@userGuids		= NULL
	,@roleGuids		= NULL
	,@pagesize		= 30
	,@pageNumber	= 1	
	,@orderby		= 'name desc'
	,@count			= @count		OUTPUT	
	,@invokinguser  = 'C1596B8C-7065-4D63-BFD0-4B835B93DFF2'              
	,@version		= 'v1'              
	,@output		= @output		OUTPUT
	,@fieldname		= @fieldName	OUTPUT	

SELECT @count count, @output status, @fieldName fieldName

001	sgh-1 26-11-2019 [Nishit Khakhi]	Added Initial Version to Get User List 
*******************************************************************/
CREATE PROCEDURE [dbo].[User_Search]
(	 
	@companyGuid			UNIQUEIDENTIFIER
	,@search				nvarchar(100)		= NULL	
	,@userGuids				XML					= NULL	
	,@roleGuids				XML					= NULL	
	,@status				BIT					= NULL
	,@pagesize				INT
	,@pagenumber			INT
	,@count					INT					OUTPUT
	,@orderby				nvarchar(100)		= NULL
	,@invokinguser			UNIQUEIDENTIFIER
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
            SELECT 'User_Search' AS '@procName' 
            , CONVERT(nvarchar(MAX),@companyGuid) AS '@companyGuid' 
			, CONVERT(nvarchar(MAX),@search) AS '@search' 
			, CONVERT(nvarchar(MAX),@userGuids) AS '@userGuids' 
			, CONVERT(nvarchar(MAX),@roleGuids) AS '@roleGuids' 
			, CONVERT(nvarchar(MAX),@status) AS '@status' 
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
		
		IF NOT EXISTS (SELECT TOP 1 1 FROM [dbo].[User] (NOLOCK) WHERE [companyGuid] = @companyGuid AND [isDeleted] = 0)
		BEGIN
			SET @output = -3
			SET @fieldname = 'UserNotFound'

			RETURN;
		END 
		
		--IF (@userGuids IS NULL AND @roleGuids IS NULL)
		--BEGIN
		--	SET @output = -3
		--	SET @fieldname = 'UserNotFound'

		--	RETURN;
		--END        
		
		IF OBJECT_ID('tempdb..#temp_user') IS NOT NULL DROP TABLE #temp_user
		
		CREATE TABLE #temp_user
		(
			[guid]					UNIQUEIDENTIFIER
			,[userid]				nvarchar(100)
			,[companyGuid]			UNIQUEIDENTIFIER
			,[firstname]			nvarchar(50)
			,[lastname]				nvarchar(50)
			,[name]					nvarchar(150)
			,[contactNo]			nvarchar(25)
			,[timezoneGuid]			UNIQUEIDENTIFIER
			,[entityName]		nvarchar(100)
			,[isactive]				BIT
			,[isverified]			BIT
			,[createdby]			UNIQUEIDENTIFIER
			,[roleName]				nvarchar(100)	
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
			,u.[email] AS [userId]							
			,u.[companyGuid]			
			,u.[firstName]			
			,u.[lastName]			
			,(u.[firstName] + '' '' + u.[lastName]) AS name
			,u.[contactNo]
			,u.[timezoneGuid]
			,e.[name] as [entityName]
			,u.[isactive]
			,CASE WHEN u.[password] IS NULL THEN 0 ELSE 1 END AS [isverified]
			,u.[createdby]
			,r.[name] AS roleName
			FROM [dbo].[User] AS u WITH (NOLOCK)
			 LEFT JOIN [dbo].[entity] AS e (NOLOCK) ON u.[entityGuid] = e.guid'	
			+ CASE WHEN @userGuids IS NULL THEN '' 
			ELSE ' INNER JOIN @userguids.nodes(''//userguids/userguid'') e(x) ON TRY_CONVERT(UNIQUEIDENTIFIER, x.value(''(.)[1]'',''nvarchar(50)'')) = U.[guid]'
			END + 
			CASE WHEN @roleguids IS NULL THEN 			
				' LEFT OUTER JOIN [dbo].[RoleUser] AS ru WITH (NOLOCK)
						ON u.[guid] = ru.[userGuid]
				   LEFT OUTER JOIN [dbo].[Role] AS r WITH(NOLOCK)
						ON ru.roleGuid = r.[guid] AND r.[companyGuid] = @companyGuid' 
			ELSE ' INNER JOIN [dbo].[RoleUser] AS ru WITH (NOLOCK) 						
						ON u.[guid] = ru.[userGuid]
				   INNER JOIN [dbo].[Role] AS r WITH(NOLOCK)
						ON ru.roleGuid = r.[guid] AND r.[companyGuid] = @companyGuid			
				   INNER JOIN @roleGuids.nodes(''//roleguids/roleguid'') rx(y) 
						ON TRY_CONVERT(UNIQUEIDENTIFIER, y.value(''(.)[1]'',''nvarchar(50)'')) = r.[guid]'
			END + 
			' WHERE u.companyGuid = @companyGuid AND u.[isdeleted] = 0'
			+ CASE WHEN @status IS NULL THEN '' 
			ELSE 'AND u.[isactive] = @status' 
			END
			+ CASE WHEN @search IS NULL THEN '' 
			ELSE 
			' AND (u.firstname LIKE ''%' + @search + '%'' OR u.lastname LIKE ''%' + @search + '%''
			  OR (u.firstname + '' '' + u.lastname) LIKE ''%' + @search + '%''
			  OR u.email LIKE ''%' + @search + '%'' OR u.contactno LIKE ''%' + @search + '%''
			  OR r.[name] LIKE ''%' + @search + '%'')'
			END + 
		') data'
		
		INSERT INTO #temp_user
		EXEC sp_executesql
			  @Sql
			, N'@orderby nvarchar(100), @companyGuid UNIQUEIDENTIFIER, @userGuids XML, @roleGuids XML, @status BIT'
			, @orderby			= @orderby			
			, @companyGuid		= @companyGuid
		    , @userGuids		= @userGuids
			, @roleGuids		= @roleGuids
			, @status			= @status
		    
		SET @count = @@ROWCOUNT
		
		--PRINT @Sql

		SELECT 
			[guid]					
			,[userid] AS userId
			,[companyGuid] AS [companyGuid]
			,[firstname] AS [firstName]		
			,[lastname]	AS [lastName]		
			,[name]		AS [name]
			,[contactNo] AS [contactNo]
			,[timezoneGuid]	AS [timezoneGuid]
			,[entityName] AS [entityName]
			,[isactive]	AS [isActive]	
			,[isverified] AS [isVerified]	
			,[createdby] AS [createdBy]
			,[roleName] 
		FROM #temp_user
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