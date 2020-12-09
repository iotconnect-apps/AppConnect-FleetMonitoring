
/*******************************************************************
DECLARE @count INT
     ,@output INT = 0
	,@fieldName				nvarchar(255)	
EXEC [dbo].[User_List]	
	 @companyguid	= 'FA882D18-DA0E-4E02-BE1F-F8CBC452A2B1'
	,@search		= NULL
	,@pagesize		= 30
	,@pageNumber	= 1	
	,@orderby		= 'name desc'
	,@count			= @count		OUTPUT	
	,@invokinguser  = '027418A7-E0DF-47ED-908D-F91015E6EC6B'              
	,@version		= 'v1'              
	,@output		= @output		OUTPUT
	,@fieldname		= @fieldName	OUTPUT	

SELECT @count count, @output status, @fieldName fieldName

001	SAM-138 13-07-2020 [Nishit Khakhi]	Added Initial Version to List Users
*******************************************************************/
CREATE PROCEDURE [dbo].[User_List]
(	 
	@companyguid		UNIQUEIDENTIFIER
	,@search			NVARCHAR(100)		= NULL	
	,@parentEntityGuid	UNIQUEIDENTIFIER	= NULL
	,@entityGuid		UNIQUEIDENTIFIER	= NULL
	,@pagesize			INT
	,@pagenumber		INT
	,@count				INT					OUTPUT
	,@orderby			nvarchar(100)		= NULL
	,@invokinguser		UNIQUEIDENTIFIER
	,@version			nvarchar(10)              
	,@output			SMALLINT			OUTPUT
	,@fieldname			nvarchar(255)		OUTPUT
	,@culture			nvarchar(10)		= 'en-Us'	
	,@enabledebuginfo	CHAR(1)				= '0'
)
AS
BEGIN
    SET NOCOUNT ON

    IF (@enabledebuginfo = 1)
	BEGIN
        DECLARE @Param XML 
        SELECT @Param = 
        (
            SELECT 'User_List' AS '@procName' 
            , CONVERT(nvarchar(MAX),@companyguid) AS '@companyguid' 
			, CONVERT(nvarchar(MAX),@search) AS '@search' 
			, CONVERT(nvarchar(MAX),@parentEntityGuid) AS '@parentEntityGuid' 
			, CONVERT(nvarchar(MAX),@entityGuid) AS '@entityGuid' 
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
		
		DECLARE @userEntityGuid UNIQUEIDENTIFIER 

		SELECT @userEntityGuid = [entityGuid] 
		FROM [dbo].[User] (NOLOCK) 
		WHERE companyguid = @companyguid 
		AND [guid] = @invokinguser
		AND [isdeleted]=0

		IF ISNULL(@userEntityGuid, '00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000' BEGIN
			Set @output = -3
			SET @fieldname = 'UserNotFound'
			RETURN;
		END        
		   
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
			,[timezoneguid]			UNIQUEIDENTIFIER
			,[isactive]				BIT
			,[entityName]			NVARCHAR(500)
			--,[subEntityName]		NVARCHAR(500)
			,[roleName]				NVARCHAR(100)
			,[createdby]			UNIQUEIDENTIFIER
			,[entityGuid]			UNIQUEIDENTIFIER
			--,[subEntityGuid]		UNIQUEIDENTIFIER
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
			,u.[timezoneguid]
			,u.[isactive]	
			, E.[name] AS [entityName]
			--, E.[name] AS [subEntityName]
			,r.[name] AS [roleName]
			,u.[createdby]
			,E.[guid] AS [entityGuid]
			--,u.[entityGuid] AS [subEntityGuid]
			FROM [dbo].[User] AS u WITH (NOLOCK)
			LEFT JOIN [dbo].[entity] AS e WITH (NOLOCK) ON e.[guid] = u.[entityGuid] AND e.[isDeleted] = 0
			--LEFT JOIN [dbo].[Entity] EP WITH (NOLOCK) ON e.[parentEntityGuid] = EP.[guid] AND EP.[isDeleted] = 0 
			LEFT JOIN [dbo].[Role] r WITH (NOLOCK) ON u.[roleGuid] = r.[guid] AND r.[isDeleted] = 0
			WHERE u.companyguid = @companyguid AND u.[isdeleted] = 0'	
			+ CASE WHEN @parentEntityGuid IS NULL THEN '' ELSE ' AND E.[parentEntityGuid] = @parentEntityGuid ' END +
			+CASE WHEN @entityGuid IS NULL THEN '' ELSE ' AND u.[entityGuid] = @entityGuid ' END +
			+ CASE WHEN @search IS NULL THEN '' ELSE
			' AND (u.firstname LIKE ''%' + @search + '%'' OR u.lastname LIKE ''%' + @search + '%''
			  OR (u.firstname + '' '' + u.lastname) LIKE ''%' + @search + '%''
			  OR u.email LIKE ''%' + @search + '%''
			  OR r.[name] LIKE ''%' + @search + '%''
			  OR u.[contactno] LIKE ''%' + @search + '%''
			) '
			 END +
		') data '
		

		INSERT INTO #temp_user
		EXEC sp_executesql
			  @Sql
			, N'@orderby nvarchar(100), @companyguid UNIQUEIDENTIFIER, @invokinguser UNIQUEIDENTIFIER, @parentEntityGuid UNIQUEIDENTIFIER, @entityGuid UNIQUEIDENTIFIER '
			, @orderby			= @orderby			
			, @companyguid		= @companyguid
		    , @invokinguser		= @invokinguser
			, @entityGuid		= @entityGuid
			, @parentEntityGuid		= @parentEntityGuid
		    
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
			,[timezoneguid]	AS [timeZoneGuid]	
			,[isactive]	AS [isActive]	
			,[createdby] AS [createdBy]
			,[entityName] AS [entityName]
			--,[subEntityName] AS [subEntityName]
			,[roleName] AS [roleName]
			,[entityGuid] AS [entityGuid]
			--,[subEntityGuid] AS [subEntityGuid]
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
				,[timezoneguid]	AS [timeZoneGuid]	
				,[isactive]	AS [isActive]	
				,[createdby] AS [createdBy]
				,[entityName] AS [entityName]
				--,[subEntityName] AS [subEntityName]
				,[roleName] AS [roleName]
				,[entityGuid] AS [entityGuid]
				--,[subEntityGuid] AS [subEntityGuid]
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