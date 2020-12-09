/*******************************************************************
DECLARE @count INT
     	,@output INT = 0
		,@fieldName	VARCHAR(255)

EXEC [dbo].[Device_List]
	 @companyGuid	= '007D434D-1C8E-40B9-A2EA-A7263F02DC0E'
	,@fleetGuid	= '49908008-6653-46FE-920F-AAEFED6C7DA6'
	,@entityGuid	= '205837F2-E30B-4C3B-A617-8C28F704AC38'
	,@isParent		= 1		
	,@pageSize		= 10
	,@pageNumber	= 1
	,@orderby		= NULL
	,@count			= @count OUTPUT
	,@invokingUser  = 'C1596B8C-7065-4D63-BFD0-4B835B93DFF2'
	,@version		= 'v1'
	,@output		= @output	OUTPUT
	,@fieldName		= @fieldName	OUTPUT

SELECT @count count, @output status, @fieldName fieldName

001	SAM-138 07-07-2019 [Nishit Khakhi]	Added Initial Version to List Device
*******************************************************************/
CREATE PROCEDURE [dbo].[Device_List]
(	@companyGuid		UNIQUEIDENTIFIER
	,@fleetGuid	UNIQUEIDENTIFIER	= NULL
	,@entityGuid		UNIQUEIDENTIFIER	= NULL
	,@isParent			BIT					= 0
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
            SELECT 'Device_List' AS '@procName'
            	, CONVERT(VARCHAR(MAX),@companyGuid) AS '@companyGuid'
				, CONVERT(VARCHAR(MAX),@fleetGuid) AS '@fleetGuid'
				, CONVERT(VARCHAR(MAX),@entityGuid) AS '@entityGuid'
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

		SELECT
		 @output = 1
		,@count = -1

		IF OBJECT_ID('tempdb..#temp_Device') IS NOT NULL DROP TABLE #temp_Device

		CREATE TABLE #temp_Device
		(	[guid]				UNIQUEIDENTIFIER
			,[companyGuid]		UNIQUEIDENTIFIER
			,[entityGuid]		UNIQUEIDENTIFIER
			,[entityName]		NVARCHAR(500)
			,[subEntityName]	NVARCHAR(500)
			,[templateGuid]		UNIQUEIDENTIFIER
			,[parentDeviceGuid]	UNIQUEIDENTIFIER
			,[typeGuid]			UNIQUEIDENTIFIER
			,[fleetId]			NVARCHAR(100)
			,[uniqueId]			NVARCHAR(500)
			,[name]				NVARCHAR(500)
			,[description]		NVARCHAR(1000)
			--,[specification]	NVARCHAR(1000)
			--,[note]				NVARCHAR(1000)
			--,[tag]				NVARCHAR(50)
			,[image]			NVARCHAR(200)
			,[isProvisioned]	BIT
			,[isActive]			BIT
			,[count]			BIGINT
			,[totalAlert]		BIGINT			
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
			D.[guid]
			, D.[companyGuid]
			, D.[entityGuid]
			, ISNULL(EP.[name],G.[name]) AS [entityName]
			, G.[name] AS [subEntityName]
			, D.[templateGuid]
			, D.[parentDeviceGuid]
			, D.[typeGuid]
			, F.[fleetId] As [fleetId]
			, D.[uniqueId]
			, D.[name]
			, D.[description]
			--, D.[specification]
			--, D.[note]
			--, D.[tag]
			, '''' AS [image]
			, D.[isProvisioned]
			, D.[isActive]
			, 0 AS [count]	
			, 0 AS [totalAlert]
			
			FROM [dbo].[Device] D WITH (NOLOCK) 
			INNER JOIN [dbo].[Entity] G WITH (NOLOCK) ON D.[entityGuid] = G.[guid] AND G.[isDeleted] = 0
			LEFT JOIN [dbo].[Entity] EP WITH (NOLOCK) ON G.[parentEntityGuid] = EP.[guid] AND EP.[isDeleted] = 0 
			LEFT JOIN [dbo].[Fleet] F With (NOLOCK) ON D.[fleetGuid] = F.[guid] and F.[isDeleted]=0 
			 WHERE D.[companyGuid]=@companyGuid AND D.[isDeleted]=0 '
			--+ CASE WHEN @isParent = 0 THEN ' AND parentDeviceGuid IS NOT NULL ' ELSE
			--' AND parentDeviceGuid IS NULL ' END +
			+ CASE WHEN @fleetGuid IS NULL THEN '' ELSE ' AND D.[fleetGuid] = @fleetGuid ' END +
			+CASE WHEN @entityGuid IS NULL THEN '' ELSE ' AND D.[entityGuid] = @entityGuid ' END +
				+ CASE WHEN @search IS NULL THEN '' ELSE
			' AND (D.name LIKE ''%' + @search + '%''
			  OR D.[uniqueId] LIKE ''%' + @search + '%'' 
			  OR G.[name] LIKE ''%' + @search + '%'' 
			  OR F.[fleetId] LIKE ''%' + @search + '%'' 
			  OR EP.[name] LIKE ''%' + @search + '%'' 
			)'
			 END +
		' )  data '
		
		INSERT INTO #temp_Device
		EXEC sp_executesql 
			  @Sql
			, N'@orderby VARCHAR(100), @companyGuid UNIQUEIDENTIFIER, @entityGuid UNIQUEIDENTIFIER, @fleetGuid UNIQUEIDENTIFIER '
			, @orderby		= @orderby			
			, @companyGuid	= @companyGuid
			, @entityGuid = @entityGuid
			, @fleetGuid	= @fleetGuid
			
		SET @count = @@ROWCOUNT

		IF @isParent = 1
		BEGIN
			;WITH CTEDATA
			AS 
			(	SELECT d.[parentDeviceGuid],COUNT(*) [totalDevice] FROM [Device] D
				INNER JOIN #temp_Device t ON D.[parentDeviceGuid] = t.[Guid]
				GROUP BY d.[parentDeviceGuid]
			)
			UPDATE t
			SET [count] = c.[totalDevice]
			FROM #temp_Device t INNER JOIN CTEDATA c ON t.[guid] = c.[parentDeviceGuid]

		END

		;WITH CTEDATA
			AS 
			(	SELECT t.[Guid],COUNT(A.[guid]) [totalAlert] FROM [IOTConnectAlert] A (NOLOCK)
				INNER JOIN #temp_Device t ON A.[deviceGuid] = t.[Guid]
				GROUP BY t.[Guid]
			)
		, CTE_ImageData
		AS (	SELECT D.[guid], DF.[filePath], ROW_NUMBER() OVER (PARTITION BY DF.[deviceGuid], DF.[type] ORDER BY DF.[createdDate] DESC) AS [no]
				FROM #temp_Device D
				INNER JOIN dbo.[DeviceFiles] DF (NOLOCK) ON D.[guid] = DF.[deviceGuid] AND DF.[isDeleted] = 0 AND DF.[type] = 'I'
		)
		
		UPDATE t
			SET [totalAlert] = ISNULL(c.[totalAlert],0)
				, [image] = I.[filePath]
				
		FROM #temp_Device t 
			LEFT JOIN CTEDATA c ON t.[guid] = c.[Guid]
			LEFT JOIN CTE_ImageData I ON t.[guid] = I.[guid] AND I.[no] = 1
			

		IF(@pageSize <> -1 AND @pageNumber <> -1)
			BEGIN
				SELECT 
					D.[guid]
					, D.[companyGuid]
					, D.[entityGuid]
					, D.[entityName]
					, D.[subEntityName]
					, D.[templateGuid]
					, D.[parentDeviceGuid]
					, D.[typeGuid]
					, D.[fleetId]
					, D.[uniqueId]
					, D.[name]
					, D.[description]
					--, D.[specification]
					--, D.[note]
					--, D.[tag]
					, D.[image]
					, D.[isProvisioned]
					, D.[isActive]	
					, D.[count]		
					, D.[totalAlert]
					
				FROM #temp_Device D
				WHERE rowNum BETWEEN ((@pageNumber - 1) * @pageSize) + 1 AND (@pageSize * @pageNumber)			
			END
		ELSE
			BEGIN
				SELECT 
					D.[guid]
					, D.[companyGuid]
					, D.[entityGuid]
					, D.[entityName]
					, D.[subEntityName]
					, D.[templateGuid]
					, D.[parentDeviceGuid]
					, D.[typeGuid]
					, D.[fleetId]
					, D.[uniqueId]
					, D.[name]
					, D.[description]
					--, D.[specification]
					--, D.[note]
					--, D.[tag]
					, D.[image]
					, D.[isProvisioned]
					, D.[isActive]		
					, D.[count]		
					, D.[totalAlert]
					
				FROM #temp_Device D
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
