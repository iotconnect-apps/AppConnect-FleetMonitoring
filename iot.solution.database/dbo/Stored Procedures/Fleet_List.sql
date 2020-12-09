/*******************************************************************
DECLARE @count INT
     	,@output INT = 0
		,@fieldName	VARCHAR(255)

EXEC [dbo].[Fleet_List]
	 @companyGuid	= '007D434D-1C8E-40B9-A2EA-A7263F02DC0E'	
	,@currentDate	= '2020-07-16 06:47:56.890'
	,@pageSize		= 10
	,@pageNumber	= 1
	,@orderby		= NULL
	,@count			= @count OUTPUT
	,@invokingUser  = 'C1596B8C-7065-4D63-BFD0-4B835B93DFF2'
	,@version		= 'v1'
	,@output		= @output	OUTPUT
	,@fieldName		= @fieldName	OUTPUT

SELECT @count count, @output status, @fieldName fieldName


*******************************************************************/
CREATE   PROCEDURE [dbo].[Fleet_List]
(   @companyGuid		UNIQUEIDENTIFIER	
	,@search			VARCHAR(100)		= NULL
	,@currentDate		DATETIME			= NULL
	,@pageSize			INT
	,@pageNumber		INT
	,@orderby			VARCHAR(100)		= NULL
	,@invokingUser		UNIQUEIDENTIFIER
	,@version			VARCHAR(10)
	,@culture			VARCHAR(10)			= 'en-Us'
	,@output			SMALLINT			OUTPUT
	,@fieldName			VARCHAR(255)		OUTPUT
	,@count				INT					OUTPUT
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
            SELECT 'Fleet_List' AS '@procName'
            	, CONVERT(VARCHAR(MAX),@companyGuid) AS '@companyGuid'				
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

		SET	@output = 1
		SET @count = -1
		SET @currentDate = ISNULL(@currentDate,GETUTCDATE())
		IF OBJECT_ID('tempdb..#temp_Fleet') IS NOT NULL DROP TABLE #temp_Fleet
	
		CREATE TABLE #temp_Fleet
		(	[guid]						UNIQUEIDENTIFIER 			
			,[fleetId]	nvarchar(100) 
			,[companyGuid] UNIQUEIDENTIFIER 
				,[registrationNo] nvarchar(100) 
				,[loadingCapacity] nvarchar(100) 
				,[typeGuid] UNIQUEIDENTIFIER
				,[materialTypeGuid] UNIQUEIDENTIFIER				
				,[image] nvarchar(250)
				,[speedLimit]	nvarchar(250)
				,[latitude] nvarchar(50) 
				,[longitude] nvarchar(50) 
				,[radius] int 
				,[totalMiles] int
				,[fleetTypeName]	nvarchar(200)
				,[materialTypeName]	nvarchar(200)
				,[templateName] nvarchar(max)
				,[status]	nvarchar(200)
			,[isActive]					BIT
			,[totalDevices]				BIGINT			
			,[rowNum]					INT
		)

		IF LEN(ISNULL(@orderby, '')) = 0
		SET @orderby = 'registrationNo asc'

		DECLARE @Sql nvarchar(MAX) = ''
		SET @orderby = REPLACE(@orderby,'loadingCapacity','CONVERT(INT,loadingCapacity)')
		SET @Sql = '
		
		SELECT
			*
			,ROW_NUMBER() OVER (ORDER BY '+@orderby+') AS rowNum
		FROM
		(
			SELECT  
			L.[guid]
			,L.[fleetId]

			,L.[companyGuid]
				,L.[registrationNo] 
				,L.[loadingCapacity] 
				,L.[typeGuid] 
				,L.[materialTypeGuid] 			
				,L.[image] 
				,L.[speedLimit]
				,L.[latitude] 
				,L.[longitude] 
				,L.[radius] 
				,L.[totalMiles]
				,FT.[name] as [fleetTypeName]
				,MT.[name] as [materialTypeName]
				,ISNULL(STUFF((SELECT distinct '', '' + KT.[name] 
         from dbo.[Device] (NOLOCK) t1 
		 INNER JOIN dbo.[KitType] (NOLOCK) KT ON t1.[templateGuid]=KT.[Guid] AND KT.[isDeleted]=0 
		 WHERE t1.[fleetGuid] = L.[guid] AND t1.[isDeleted]=0	
            FOR XML PATH(''''), TYPE
            ).value(''.'', ''NVARCHAR(MAX)'') 
        ,1,2,''''),'''') as [templateName]
				,'''' as [status] 
			, L.[isActive]	
			, 0 AS [totalDevices] 			
			FROM [dbo].[Fleet] AS L WITH (NOLOCK) 
			
			INNER JOIN [dbo].[FleetType] AS FT WITH (NOLOCK) ON L.[typeGuid]=FT.[Guid] AND FT.[isDeleted]=0 
			INNER JOIN [dbo].[FleetMaterialType] AS MT WITH (NOLOCK) ON L.[materialTypeGuid]=MT.[Guid] AND MT.[isDeleted]=0 
			
			 WHERE L.[companyGuid]=@companyGuid AND L.[isDeleted]=0 '
			+ CASE WHEN @search IS NULL THEN '' ELSE
			' AND (L.[fleetId] LIKE ''%' + @search + '%''
			  OR L.[registrationNo] LIKE ''%' + @search + '%'' 
			  OR FT.[name] LIKE ''%' + @search + '%'' 
			   OR MT.[name] LIKE ''%' + @search + '%'' 
			   -- OR L.[templateName] LIKE ''%' + @search + '%'' 
			)'
			 END +
		' )  data '
		
		INSERT INTO #temp_Fleet
		EXEC sp_executesql 
			  @Sql
			, N'@orderby VARCHAR(100), @companyGuid UNIQUEIDENTIFIER  '
			, @orderby		= @orderby
			, @companyGuid	= @companyGuid			
		
		SET @count = @@ROWCOUNT
		--select * from #temp_Fleet
		;WITH CTE_TotalDevice
		AS (	SELECT E.[guid] , COUNT(1) AS [totalDevices]
				FROM dbo.[Device] D (NOLOCK)
				INNER JOIN dbo.[Fleet] E ON D.[fleetGuid] = E.[guid] AND E.[isDeleted] = 0 				
				WHERE D.[companyGuid] = @companyGuid AND D.[isDeleted] = 0
				GROUP BY E.[guid] 
		)
		--,		CTE_Template
		--AS (	
		--	SELECT  distinct E.[guid] ,  STUFF((SELECT distinct ', ' + cast(D.[templateGuid] as nvarchar(100))
  --       from dbo.[Device] (NOLOCK) t1
  --       where t1.[fleetGuid] = E.[guid] 							
  --          FOR XML PATH(''), TYPE
  --          ).value('.', 'NVARCHAR(MAX)') 
  --      ,1,2,'') AS [templateGuid] 
		--		FROM dbo.[Device] D (NOLOCK)
		--		INNER JOIN dbo.[Fleet] E ON D.[fleetGuid] = E.[guid] AND E.[isDeleted] = 0 				
		--		WHERE D.[companyGuid] = @companyGuid AND D.[isDeleted] = 0 				
		--)		
		UPDATE E
		SET [totalDevices] = ISNULL(CTD.[totalDevices],0)
			--, [templateGuid] = ISNULL(CT.[templateGuid],CAST(0x0 AS UNIQUEIDENTIFIER))			
		FROM #temp_Fleet E
		INNER JOIN CTE_TotalDevice CTD ON E.[guid] = CTD.[guid]
		--LEFT JOIN CTE_Template CT ON E.[guid] = CT.[guid]
		
		;WITH CTE_Trips
		AS (	
			SELECT  E.[guid], D.[startDateTime] 
			,(SELECT top 1 [endDateTime] from [dbo].[TripStops] (NOLOCK) WHERE [tripGuid] = D.[Guid] AND [isDeleted] = 0 ORDER BY [endDateTime] DESC) AS [endDateTime] 
			,D.[isStarted],D.[isCompleted]
				FROM dbo.[Trip] D (NOLOCK)
				INNER JOIN dbo.[Fleet] E ON D.[fleetGuid] = E.[guid] AND E.[isDeleted] = 0 				
				WHERE D.[companyGuid] = @companyGuid AND D.[isDeleted] = 0 			
		)
		, CTE_Maintenance
		AS (	
			SELECT DM.[deviceGuid] AS [fleetGuid]
					, DM.[guid] AS [guid]
					,DM.[startDateTime],DM.[endDateTime]
					 ,DM.[isCompleted] 
				FROM dbo.[DeviceMaintenance] DM (NOLOCK) 
				INNER JOIN [dbo].[Fleet] E ON DM.[DeviceGuid] = E.[guid] AND E.[isDeleted] = 0
				WHERE DM.[companyGuid] = @companyGuid 
				AND DM.[IsDeleted]=0 
			)
			--SELECT * FROM CTE_Trips
			
		UPDATE E 
		SET [status]=ISNULL((SELECT TOP 1  CASE WHEN (CTR.[isStarted]=1 AND CTR.[isCompleted]=0)
					 THEN 	CAST('On Duty' AS NVARCHAR(200)) 
					  ELSE CASE WHEN (CTR.[isCompleted]=1) 
						THEN CAST('Trip Completed' AS NVARCHAR(200)) 		
					 ELSE CASE WHEN CTR.[endDateTime] < @currentDate
						THEN 'Overdue' 	
					 ELSE CASE WHEN @currentDate >=CTR.[startDateTime] 
						THEN 'Upcoming' 												 
					 END END END END FROM CTE_Trips CTR 
		WHERE CTR.[guid]=E.[guid] ORDER BY CTR.[startDateTime] DESC
		),ISNULL((SELECT TOP 1 CASE WHEN (@currentDate>= CTM.[startDateTime]  AND @currentDate <=CTM.[endDateTime]) AND ISNULL(CTM.[isCompleted],0)=0 
					 THEN 'Maintenance'					 
					 ELSE CASE WHEN (CTM.[startDateTime] < @currentDate AND CTM.[endDateTime] < @currentDate) OR ISNULL(CTM.[isCompleted],0)=1 
					 THEN 'Maintenance Completed' END
					 END FROM CTE_Maintenance CTM  
		WHERE CTM.[fleetGuid]=E.[guid] ORDER BY CTM.[startDateTime] DESC
		),'Unassigned'  					  
					  ))
		FROM #temp_Fleet E				
		--LEFT JOIN CTE_Trips CTR ON E.[guid] = CTR.[guid]
		--LEFT JOIN CTE_Maintenance CTM ON E.[guid] = CTM.[fleetGuid] 
		--UPDATE E 
		--SET [status]=ISNULL((SELECT TOP 1 CASE WHEN (CTR.[isCompleted]=1) 
		--			 THEN 'Trip Completed'		
		--			  ELSE CASE WHEN (CTR.[isStarted]=1 AND CTR.[isCompleted]=0) 
		--			 THEN 'Trip Running' END						 
		--			 END FROM CTE_Trips CTR 
		--WHERE CTR.[guid]=E.[guid] ORDER BY CTR.[isStarted] DESC
		--),ISNULL((SELECT TOP 1 CASE WHEN @currentDate>= CTM.[startDateTime]  AND @currentDate <=CTM.[endDateTime] 
		--			 THEN 'Maintenance'					 
		--			 END
		--			 FROM CTE_Maintenance CTM  
		--WHERE CTM.[fleetGuid]=E.[guid] 
		--),'Upcoming'))
		--FROM #temp_FleetMapList E	
		--LEFT JOIN CTE_Trips CTR ON E.[guid] = CTR.[guid]
		--LEFT JOIN CTE_Maintenance CTM ON E.[guid] = CTM.[guid] 

		IF(@pageSize <> -1 AND @pageNumber <> -1)
			BEGIN
				SELECT 
					L.[guid]
					,L.[fleetId]
					,L.[companyGuid]
				,L.[registrationNo] 
				,L.[loadingCapacity] 
				,L.[typeGuid] 
				,L.[materialTypeGuid] 				
				,L.[image] 
				,L.[speedLimit]
				,L.[latitude] 
				,L.[longitude] 
				,L.[radius] 
				,L.[totalMiles]
				,L.[fleetTypeName]
				,L.[materialTypeName]
				,L.[templateName]
				,L.[status]
					, L.[isActive]
					, L.[totalDevices]
					
				FROM #temp_Fleet L
				WHERE rowNum BETWEEN ((@pageNumber - 1) * @pageSize) + 1 AND (@pageSize * @pageNumber)			
			END
		ELSE
			BEGIN
				SELECT 
				L.[guid]
					,L.[fleetId]
					,L.[companyGuid]
				,L.[registrationNo] 
				,L.[loadingCapacity] 
				,L.[typeGuid] 
				,L.[materialTypeGuid] 				
				,L.[image] 
				,L.[speedLimit] 
				,L.[latitude] 
				,L.[longitude] 
				,L.[radius] 
				,L.[totalMiles]
				,L.[fleetTypeName]
				,L.[materialTypeName]
				,L.[templateName]
				,L.[status]
					, L.[isActive]
					, L.[totalDevices]
					
				FROM #temp_Fleet L
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
