/*******************************************************************
DECLARE @count INT
     	,@output INT = 0
		,@fieldName	VARCHAR(255)

EXEC [dbo].[DeviceMaintenance_List]
	 @companyGuid	= 'FA882D18-DA0E-4E02-BE1F-F8CBC452A2B1'
	,@parentEntityGuid	= '54BF6C12-B107-4622-A199-0F0F9B075B13'
	--,@entityGuid	= '0EA95508-333B-4C06-BD9D-EB38109316EB'
	--,@deviceGuid	= 'D48F1109-4CFD-4622-9D71-81280F40CA5D'
	,@currentDate	= '2020-07-14 06:47:56.890'
	,@pageSize		= 10
	,@pageNumber	= 1
	,@orderby		= NULL
	,@count			= @count OUTPUT
	,@invokingUser  = 'C1596B8C-7065-4D63-BFD0-4B835B93DFF2'
	,@version		= 'v1'
	,@output		= @output	OUTPUT
	,@fieldName		= @fieldName	OUTPUT

SELECT @count count, @output status, @fieldName fieldName

001	SAM-138 07-07-2020 [Nishit Khakhi]	Added Initial Version to List Device Maintenance
*******************************************************************/
CREATE PROCEDURE [dbo].[DeviceMaintenance_List]
(	@companyGuid		UNIQUEIDENTIFIER
	,@parentEntityGuid	UNIQUEIDENTIFIER	= NULL
	,@entityGuid		UNIQUEIDENTIFIER	= NULL
	,@deviceGuid		UNIQUEIDENTIFIER	= NULL
	,@currentDate		DATETIME			= NULL
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
            SELECT 'DeviceMaintenance_List' AS '@procName'
            	, CONVERT(VARCHAR(MAX),@companyGuid) AS '@companyGuid'
				, CONVERT(VARCHAR(MAX),@parentEntityGuid) AS '@parentEntityGuid'
				, CONVERT(VARCHAR(MAX),@entityGuid) AS '@entityGuid'
				, CONVERT(VARCHAR(MAX),@deviceGuid) AS '@deviceGuid'
            	, CONVERT(VARCHAR(MAX),@search) AS '@search'
				, CONVERT(VARCHAR(50),@currentDate) as '@currentDate'
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

		IF OBJECT_ID('tempdb..#temp_DeviceMaintenance') IS NOT NULL DROP TABLE #temp_DeviceMaintenance

		CREATE TABLE #temp_DeviceMaintenance
		(	[guid]				UNIQUEIDENTIFIER
			,[companyGuid]		UNIQUEIDENTIFIER
			,[entityGuid]		UNIQUEIDENTIFIER
			,[DeviceGuid]		UNIQUEIDENTIFIER
			,[deviceName]		NVARCHAR(500)				
			,[description]		NVARCHAR(1000)
			,[status]			NVARCHAR(50)
			,[startDateTime]	DATETIME
			,[endDateTime]		DATETIME
			,[miles] INT
			,[createdDate]		DATETIME
			,[isCompleted] BIT
			,[completedDate] DATETIME
			,[rowNum]			INT
		)

		IF LEN(ISNULL(@orderby, '')) = 0
		SET @orderby = '[DeviceName] asc'

		DECLARE @Sql nvarchar(MAX) = ''

		SET @Sql = '
		SELECT
			*
			,ROW_NUMBER() OVER (ORDER BY '+@orderby+') AS rowNum
		FROM
		( SELECT
			EM.[guid]
			, EM.[companyGuid]
			, EM.[entityGuid]
			, EM.[DeviceGuid]		
			, G.[fleetId] AS [deviceName]			
			, EM.[description]
			, CASE WHEN (@currentDate >= [startDateTime] AND @currentDate <= [endDateTime]) AND ISNULL(EM.[isCompleted],0) =0
				THEN ''Under Maintenance''
				ELSE CASE WHEN ([startDateTime] < @currentDate AND [endDateTime] < @currentDate) OR ISNULL(EM.[isCompleted],0) =1
				THEN ''Completed''
				ELSE ''Scheduled''
				END
			END AS [status]
			, EM.[startDateTime] AS [startDateTime]
			, EM.[endDateTime] AS [endDateTime]
			,G.[totalMiles] AS [miles]
						, EM.[createdDate] 
						,EM.[isCompleted]
						,EM.[completedDate]
			FROM [dbo].[DeviceMaintenance] EM WITH (NOLOCK) 		
			INNER JOIN [dbo].[Fleet] G WITH (NOLOCK) ON EM.[deviceGuid] = G.[guid] AND G.[isDeleted] = 0 	
			
			WHERE EM.[companyGuid]=@companyGuid AND EM.[isDeleted] = 0 '
			
			+ CASE WHEN @entityGuid IS NULL THEN '' ELSE ' AND EM.[entityGuid] = @entityGuid ' END +
			+ CASE WHEN @deviceGuid IS NULL THEN '' ELSE ' AND EM.[deviceGuid] = @deviceGuid ' END +
			+ CASE WHEN @search IS NULL THEN '' ELSE
			' AND (G.[fleetId] LIKE ''%' + @search + '%''		
			  OR EM.[description] LIKE ''%' + @search + '%'' 
			 
			 )'
			 END +
		' )  data '
		
		INSERT INTO #temp_DeviceMaintenance
		EXEC sp_executesql 
			  @Sql
			, N'@orderby VARCHAR(100), @companyGuid UNIQUEIDENTIFIER, @entityGuid UNIQUEIDENTIFIER, @deviceGuid UNIQUEIDENTIFIER, @currentDate DATETIME '
			, @orderby		= @orderby			
			, @companyGuid	= @companyGuid	
			, @entityGuid	= @entityGuid			
			, @deviceGuid	= @deviceGuid
			, @currentDate  = @currentDate
		SET @count = @@ROWCOUNT

		IF(@pageSize <> -1 AND @pageNumber <> -1)
			BEGIN
				SELECT 
					EM.[guid]
					, EM.[companyGuid]
					, EM.[entityGuid]
					, EM.[DeviceGuid]
					, EM.[deviceName]					
					, EM.[description]
					, EM.[status]
					, EM.[startDateTime]
					, EM.[endDateTime]
					,EM.[miles]
					, EM.[createdDate]
					,EM.[isCompleted]
						,EM.[completedDate]
				FROM #temp_DeviceMaintenance EM
				WHERE rowNum BETWEEN ((@pageNumber - 1) * @pageSize) + 1 AND (@pageSize * @pageNumber)			
			END
		ELSE
			BEGIN
				SELECT 
					EM.[guid]
					, EM.[companyGuid]
					, EM.[entityGuid]
					, EM.[DeviceGuid]
					, EM.[deviceName]					
					, EM.[description]
					, EM.[status]
					, EM.[startDateTime]
					, EM.[endDateTime]	
					,EM.[miles]
					, EM.[createdDate]	
					,EM.[isCompleted]
						,EM.[completedDate]
				FROM #temp_DeviceMaintenance EM
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