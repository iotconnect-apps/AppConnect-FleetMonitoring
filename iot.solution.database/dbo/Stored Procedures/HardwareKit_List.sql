/*******************************************************************
DECLARE @count INT
     ,@output INT = 0
	,@fieldName				nvarchar(255)	
EXEC [dbo].[HardwareKit_List]	
	 @companyguid	= '415C8959-5BFC-4203-A493-F89458AE7736'
	,@isAssigned	= 0
	,@search		= NULL
	,@pagesize		= 30
	,@pageNumber	= 1	
	,@orderby		= 'kitCode desc'
	,@count			= @count		OUTPUT	
	,@invokinguser  = 'E05A4DA0-A8C5-4A4D-886D-F61EC802B5FD'              
	,@version		= 'v1'              
	,@output		= @output		OUTPUT
	,@fieldname		= @fieldName	OUTPUT	

SELECT @count count, @output status, @fieldName fieldName

001	SGH-97 13-01-2020 [Nishit Khakhi]	Added Initial Version to List Hardware Kit
002	SGH-97 16-01-2020 [Nishit Khakhi]	Added param isAssigned to get data for whiltelist and assigned screen
003 SAQM-108 20/04/2020 [Nishit Khakhi] Updated to remove redundant device count
*******************************************************************/
CREATE PROCEDURE [dbo].[HardwareKit_List]
(	@companyguid			UNIQUEIDENTIFIER	= NULL	
	,@search				nvarchar(100)		= NULL	
	,@isAssigned			BIT					= 0
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
            SELECT 'HardwareKit_List' AS '@procName' 
            , CONVERT(nvarchar(MAX),@companyguid) AS '@companyguid' 
			, @search AS '@search' 
			, CONVERT(nvarchar(MAX),@isAssigned) AS '@isAssigned' 
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
		
		IF OBJECT_ID('tempdb..#temp_hardwareKit') IS NOT NULL DROP TABLE #temp_hardwareKit
		
		CREATE TABLE #temp_hardwareKit
		(	[guid]				UNIQUEIDENTIFIER,
			[kitTypeGuid]		UNIQUEIDENTIFIER,
			[kitTypeName]		NVARCHAR(200)	,
			[kitCode]			NVARCHAR(50)	,
			[companyGuid]		UNIQUEIDENTIFIER,
			[entityGuid]		UNIQUEIDENTIFIER,
			[companyName]		NVARCHAR(100)	,
			[uniqueId]			NVARCHAR(500)	,
			[name]				NVARCHAR(500)	,
			[note]				NVARCHAR(1000)	,
			[tag]				NVARCHAR(50)	,
			[tagGuid]			UNIQUEIDENTIFIER,
			[attributeName]		NVARCHAR(100)	,
			[isProvisioned]		BIT				,
			[isActive]			BIT				,
			[isDeleted]			BIT				,
			[createdDate]		DATETIME		,
			[createdBy]			UNIQUEIDENTIFIER,
			[updatedDate]		DATETIME		,
			[updatedBy]			UNIQUEIDENTIFIER,
			[row_num]			INT
		)
			
		IF LEN(ISNULL(@orderby, '')) = 0
		SET @orderby = 'kitCode asc'

		DECLARE @Sql nvarchar(MAX) = ''

		SET @Sql = ' SELECT *,ROW_NUMBER() OVER (ORDER BY '+@orderby+') AS row_num
					FROM
					(	
				SELECT   
				h.[guid]
				, kt.[guid] AS [kitTypeGuid]
				, kt.[name] AS [kitTypeName]  
				, h.[kitCode]
				, h.[companyGuid]
				, h.[entityGuid]
				, ISNULL(c.[name],'''') AS [companyName]
				, h.[uniqueId]
				, h.[name]
				, h.[note]
				, KTA.[tag]
				, KTA.[guid] AS [tagGuid]
				, KTA.[localName] AS [AttributeName]
				, D.[isProvisioned]
				, h.[isActive]
				, h.[isDeleted]
				, h.[createdDate]
				, h.[createdBy]
				, h.[updatedDate]
				, h.[updatedBy]
			FROM [dbo].[HardwareKit] AS h WITH (NOLOCK)
			INNER JOIN [dbo].[KitType] AS kt WITH (NOLOCK) ON kt.[guid] = h.[kitTypeGuid] AND kt.[isDeleted] = 0
			Left JOIN [dbo].[Company] c WITH (NOLOCK) ON h.[companyGuid] = c.[guid] AND c.[isDeleted] = 0
			LEFT JOIN [dbo].[Device] D  WITH (NOLOCK) ON h.[uniqueId] = D.[uniqueId] AND c.[Guid] = D.[companyGuid] ANd D.[isDeleted] = 0
			LEFT JOIN [dbo].[KitTypeAttribute] KTA (NOLOCK) ON H.[tagGuid] = KTA.[guid]
			WHERE h.[isdeleted] = 0 '								
			+ CASE WHEN ISNULL(@isAssigned,0) = 0 THEN ' AND h.companyguid IS NULL ' ELSE ' AND h.companyguid IS NOT NULL ' END
			+ CASE WHEN @companyguid IS NULL THEN '' ELSE ' AND h.companyguid = @companyguid ' END
			+ CASE WHEN @search IS NULL THEN 
				'' 
				ELSE 
				' AND (kt.name LIKE ''%' + @search + '%''
				  OR h.[kitCode] LIKE ''%' + @search + '%''
				  OR c.[name] LIKE ''%' + @search + '%'') 
				' 
			END +
		') data '
		
		INSERT INTO #temp_hardwareKit
		EXEC sp_executesql
			  @Sql
			, N'@orderby nvarchar(100), @companyguid UNIQUEIDENTIFIER, @invokinguser UNIQUEIDENTIFIER'
			, @orderby			= @orderby			
			, @companyguid		= @companyguid
		    , @invokinguser		= @invokinguser
		    
		SET @count = @@ROWCOUNT
		
		--PRINT @Sql
		IF @pagenumber = -1
		BEGIN
			SELECT 
				[guid]
				, [kitTypeGuid]
				, [kitTypeName]  
				, [kitCode]
				, [companyGuid]
				, [entityGuid]
				, [companyName]
				, [uniqueId]
				, [name]
				, [note]
				, [tag]
				, [tagGuid]
				, [AttributeName]
				, [isProvisioned]
				, [isActive]
				, [isDeleted]
				, [createdDate]
				, [createdBy]
				, [updatedDate]
				, [updatedBy]
			FROM #temp_hardwareKit
		END
		ELSE
			BEGIN
				SELECT 
					[guid]
					, [kitTypeGuid]
					, [kitTypeName]  
					, [kitCode]
					, [companyGuid]
					, [entityGuid]
					, [companyName]
					, [uniqueId]
					, [name]
					, [note]
					, [tag]
					, [tagGuid]
					, [AttributeName]
					, [isProvisioned]
					, [isActive]
					, [isDeleted]
					, [createdDate]
					, [createdBy]
					, [updatedDate]
					, [updatedBy]
				FROM #temp_hardwareKit
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