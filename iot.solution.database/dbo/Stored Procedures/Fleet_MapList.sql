/*******************************************************************
DECLARE @count INT
     	,@output INT = 0
		,@fieldName	VARCHAR(255)

EXEC [dbo].[Fleet_MapList]
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
CREATE PROCEDURE [dbo].[Fleet_MapList]
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
            SELECT 'Fleet_MapList' AS '@procName'
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
	
		IF OBJECT_ID('tempdb..#temp_FleetMapList') IS NOT NULL DROP TABLE #temp_FleetMapList
	
		CREATE TABLE #temp_FleetMapList
		(	[guid]						UNIQUEIDENTIFIER 			
			,[fleetId]	nvarchar(100) 	
			,[isStarted] BIT
			,[uniqueId] nvarchar(100)
			,[tripGuid]	UNIQUEIDENTIFIER
			,[tripId]	nvarchar(100) 
			,[latitude] nvarchar(50) 
				,[longitude] nvarchar(50) 			
				,[sourceLatitude] nvarchar(50) 
				,[sourceLongitude] nvarchar(50) 
				,[destinationLatitude] nvarchar(50) 
				,[destinationLongitude] nvarchar(50) 
				,[radius] int 
				,[totalMiles] int
				,[fleetTypeName]	nvarchar(200)
				,[materialTypeName]	nvarchar(200)
				,[driverGuid]	UNIQUEIDENTIFIER
				,[driverId] nvarchar(200)
				,[status]	nvarchar(200)
								
			,[rowNum]					INT
		)

		IF LEN(ISNULL(@orderby, '')) = 0
		SET @orderby = 'fleetId asc'

		DECLARE @Sql nvarchar(MAX) = ''

		SET @Sql = '
		
		SELECT
			*
			,ROW_NUMBER() OVER (ORDER BY '+@orderby+') AS rowNum
		FROM
		(
			SELECT DISTINCT 
			L.[guid]
			,L.[fleetId]
			,ISNULL(T.[isStarted],0) as [isStarted]
			,DV.[uniqueId] 
			,T.[guid] AS [tripGuid]
			,T.[tripId] AS [tripId]
			,L.[latitude]				
			,L.[longitude]
				,T.[sourceLatitude]  
				,T.[sourceLongitude]  
				,T.[destinationLatitude]  
				,T.[destinationLongitude]  
				,L.[radius] 
				,ISNULL(T.[totalMiles],0) As [totalMiles]
				,FT.[name] as [fleetTypeName]
				,MT.[name] as [materialTypeName]
				,D.[guid] as [driverGuid]
				,D.[driverId]          
				,'''' as [status] 
							
			FROM [dbo].[Fleet] AS L WITH (NOLOCK) 
			INNER JOIN [dbo].[Device] AS DV WITH (NOLOCK) ON DV.[fleetGuid]=L.[Guid] AND DV.[isDeleted]=0	
			INNER JOIN [dbo].[FleetType] AS FT WITH (NOLOCK) ON L.[typeGuid]=FT.[Guid] AND FT.[isDeleted]=0 
			INNER JOIN [dbo].[FleetMaterialType] AS MT WITH (NOLOCK) ON L.[materialTypeGuid]=MT.[Guid] AND MT.[isDeleted]=0 		
			LEFT JOIN [dbo].[Driver] AS D WITH (NOLOCK) ON D.[companyGuid]=L.[companyGuid] AND D.[fleetGuid]=L.[Guid] AND D.[isDeleted]=0
			LEFT JOIN [dbo].[Trip] AS T WITH (NOLOCK) ON T.[companyGuid]=L.[companyGuid] AND T.[fleetGuid]=L.[Guid] AND T.[isDeleted]=0 
			
			 WHERE L.[companyGuid]=@companyGuid AND L.[isDeleted]=0  AND ISNULL(T.[isCompleted],0)=0 '
			+ CASE WHEN @search IS NULL THEN '' ELSE
			' AND (T.[tripId] LIKE ''%' + @search + '%'' OR D.[driverId] LIKE ''%' + @search + '%'' OR L.[fleetId] LIKE ''%' + @search + '%''		 
			  OR FT.[name] LIKE ''%' + @search + '%'' 
			   OR MT.[name] LIKE ''%' + @search + '%'' 		
			   OR DV.[uniqueId] LIKE ''%' + @search + '%'' 
			)'
			 END +
		' )  data '
		
		INSERT INTO #temp_FleetMapList
		EXEC sp_executesql 
			  @Sql
			, N'@orderby VARCHAR(100), @companyGuid UNIQUEIDENTIFIER  '
			, @orderby		= @orderby			
			, @companyGuid	= @companyGuid			
		
		SET @count = @@ROWCOUNT
		--SELECT * from #temp_FleetMapList 
		;WITH CTE_Trips
		AS (	
			SELECT  E.[guid],D.[guid] as [tripGuid], D.[startDateTime] 
			,(SELECT top 1 [endDateTime] from [dbo].[TripStops] (NOLOCK) WHERE [tripGuid] = D.[Guid] AND [isDeleted] = 0 ORDER BY [endDateTime] DESC) AS [endDateTime] 
			,D.[isStarted],D.[isCompleted],'' AS [status] 
				FROM dbo.[Trip] D (NOLOCK)
				INNER JOIN dbo.[Fleet] E ON D.[fleetGuid] = E.[guid] AND E.[isDeleted] = 0 				
				WHERE D.[companyGuid] = @companyGuid AND D.[isDeleted] = 0 			
		)
		, CTE_Maintenance
		AS (	
			SELECT DM.[deviceGuid] AS [fleetGuid]
					, DM.[guid] AS [guid]
					,DM.[startDateTime],DM.[endDateTime]
					 , DM.[isCompleted]
				FROM dbo.[DeviceMaintenance] DM (NOLOCK) 
				INNER JOIN [dbo].[Fleet] E ON DM.[DeviceGuid] = E.[guid] AND E.[isDeleted] = 0
				WHERE DM.[companyGuid] = @companyGuid 
				AND DM.[IsDeleted]=0 
			)
		UPDATE E 
		SET [status]=ISNULL((SELECT TOP 1 CASE WHEN (T.[isStarted]=1 AND T.[isCompleted]=0)    
									THEN 'Trip Running'        
									ELSE CASE WHEN T.[isCompleted]=1         
									THEN NULL
									ELSE CASE WHEN T.[endDateTime] < @currentDate
									THEN NULL
									ELSE 'Upcoming'        
									END        
									END END FROM CTE_Trips T WHERE T.[tripGuid]=E.[tripGuid]  ORDER BY T.[isStarted],T.[startDateTime] DESC),
									ISNULL((SELECT TOP 1 CASE WHEN (@currentDate>= CTM.[startDateTime]  AND @currentDate <=CTM.[endDateTime]) AND ISNULL(CTM.[isCompleted],0)=0
									 THEN CAST('Maintenance'  AS NVARCHAR(200)) 					 
									 END
									 FROM CTE_Maintenance CTM  
									WHERE CTM.[fleetGuid]=E.[guid] ORDER BY CTM.[startDateTime] desc
								),''))
		FROM #temp_FleetMapList E	
		--LEFT JOIN CTE_Trips CTR ON E.[guid] = CTR.[guid] --AND ISNULL(CTR.[isStarted],0) = 1
		--LEFT JOIN CTE_Maintenance CTM ON E.[guid] = CTM.[fleetGuid] 

		DELETE FROM #temp_FleetMapList WHERE [status] IN('') 
				
		
		SET @count = ISNULL((SELECT COUNT(1) from #temp_FleetMapList),0)
		--IF(@pageSize <> -1 AND @pageNumber <> -1)
		--	BEGIN
		--		SELECT 
		--			L.[guid]
		--			,L.[fleetId]
		--			,L.[isStarted]
		--			,L.[uniqueId]
		--			,L.[tripGuid]
		--		,L.[tripId]
		--		,L.[latitude] 
		--		,L.[longitude] 
		--		,L.[sourceLatitude]  
		--		,L.[sourceLongitude]  
		--		,L.[destinationLatitude]  
		--		,L.[destinationLongitude]  
		--		,L.[radius] 
		--		,L.[totalMiles]
		--		,L.[fleetTypeName]
		--		,L.[materialTypeName]
		--		,L.[driverGuid]
		--		,L.[driverId]
		--		,L.[status]
		--		,L.[rowNum] 
		--		FROM #temp_FleetMapList L
		--		WHERE rowNum BETWEEN ((@pageNumber - 1) * @pageSize) + 1 AND (@pageSize * @pageNumber)			
		--	END
		--ELSE
		--	BEGIN
				SELECT 
				L.[guid]
					,L.[fleetId]
					,L.[uniqueId]
					,L.[isStarted] 
					,L.[tripGuid]
				,L.[tripId]
				,L.[latitude] 
				,L.[longitude] 
				,L.[sourceLatitude]  
				,L.[sourceLongitude]  
				,L.[destinationLatitude]  
				,L.[destinationLongitude]  
				,L.[radius] 
				,L.[totalMiles]
				,L.[fleetTypeName]
				,L.[materialTypeName]
				,L.[driverGuid]
				,L.[driverId]
				,L.[status]
				,L.[rowNum] 	
				FROM #temp_FleetMapList L
			--END
	   
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
