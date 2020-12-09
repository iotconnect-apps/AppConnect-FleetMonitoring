
CREATE PROCEDURE [IoTConnect].[TelemetryAttributeValue_Bulkadd] 
	 @companyguid			UNIQUEIDENTIFIER	
	,@attributeValues		XML	
	,@output				SMALLINT				OUTPUT    
	,@fieldname				nvarchar(100)			OUTPUT   
	,@culture				nvarchar(10)			= 'en-Us'
	,@version				NVARCHAR(10)			= NULL
	,@enabledebuginfo		CHAR(1)					= '0'
	
AS
BEGIN
	SET NOCOUNT ON   
	
	IF (@enabledebuginfo = 1)
	BEGIN
        DECLARE @Param XML 
        SELECT @Param = 
        (
            SELECT 'AttributeValue_Bulkadd' AS '@procName'             
            , CONVERT(nvarchar(MAX),@companyguid) AS '@companyguid' 			
			, CONVERT(nvarchar(MAX),@attributeValues) AS '@attributeValues'
            --, CONVERT(nvarchar(MAX),@invokinguser) AS '@invokinguser'
            , CONVERT(nvarchar(MAX),@version) AS '@version' 
            , CONVERT(nvarchar(MAX),@output) AS '@output' 
            , CONVERT(nvarchar(MAX),@fieldname) AS '@fieldname'   
            FOR XML PATH('Params')
	    ) 
	    INSERT INTO dbo.DebugInfo(data, dt) VALUES(Convert(nvarchar(MAX), @Param), GETUTCDATE())
    END       

	DECLARE @dt DATETIME = GETUTCDATE()	

	SET @output = 1
	SET @fieldname = 'Success'

	BEGIN TRY
		IF OBJECT_ID('tempdb..#tmpTelemetry') IS NOT NULL DROP TABLE #tmpTelemetry

		--DECLARE @attr AS TABLE (guid UNIQUEIDENTIFIER, aggregateType INT,matchAttribute NVARCHAR(200) )
		CREATE TABLE #tmpTelemetry
		(
			 guid				UNIQUEIDENTIFIER
			,sdkDate			DATETIME
			,gatewayDate		DATETIME
			,deviceDate			DATETIME
			,attributeGuid		UNIQUEIDENTIFIER NULL
			,deviceGuid			UNIQUEIDENTIFIER NULL
			,aggregateType		INT
			,uniqueId			NVARCHAR(200)	NULL
			,parent				NVARCHAR(100)	NULL
			,LocalName			NVARCHAR(100)	NULL
			,value				nvarchar(2000)
			,tag				NVARCHAR(200)	NULL
			--,matchAttribute		NVARCHAR(200)   NULL	
		)

		IF ISNULL(@version, '') = '2.0'  BEGIN
			INSERT INTO #tmpTelemetry
			SELECT 
				NEWID() guid
				,TRY_CONVERT(DATETIME,a.value('(sdkDate)[1]','datetime'))  sdkDate
				,TRY_CONVERT(DATETIME,a.value('(gatewayDate)[1]','datetime')) gatewayDate
				,TRY_CONVERT(DATETIME,a.value('(deviceDate)[1]','datetime')) deviceDate
				,TRY_CONVERT(UNIQUEIDENTIFIER,a.value('(attributeGuid)[1]','nvarchar(50)')) attributeGuid
				,TRY_CONVERT(UNIQUEIDENTIFIER,a.value('(deviceGuid)[1]','nvarchar(50)')) deviceGuid
				,TRY_CONVERT(INT,IsNULL(a.value('(aggregateType)[1]','nvarchar(50)'),0)) aggregateType
				,a.value('(uniqueId)[1]','nvarchar(200)') uniqueId
				,a.value('(parent)[1]','nvarchar(100)') parent			
				,a.value('(localName)[1]','nvarchar(100)') LocalName
				,a.value('(value)[1]','nvarchar(2000)') value
				,a.value('(tag)[1]','nvarchar(200)') tag
				--,NULL matchAttribute
			FROM @attributeValues.nodes('//attributes/attrbute') xmldata(a) 
			ORDER BY 6, 5, 7

		END
		ELSE BEGIN
			INSERT INTO #tmpTelemetry
			SELECT 
				 NEWID() guid
				,TRY_CONVERT(DATETIME,a.value('(sdkDate)[1]','datetime'))  sdkDate
				,TRY_CONVERT(DATETIME,a.value('(gatewayDate)[1]','datetime')) gatewayDate
				,TRY_CONVERT(DATETIME,a.value('(deviceDate)[1]','datetime')) deviceDate
				,TRY_CONVERT(UNIQUEIDENTIFIER,a.value('(attributeGuid)[1]','nvarchar(50)')) attributeGuid
				,TRY_CONVERT(UNIQUEIDENTIFIER,a.value('(deviceGuid)[1]','nvarchar(50)')) deviceGuid
				,TRY_CONVERT(INT,IsNULL(a.value('(aggregateType)[1]','nvarchar(50)'),0)) aggregateType		
				,NULL uniqueId	
				,NULL parent		
				,NULL LocalName			
				,a.value('(value)[1]','nvarchar(2000)') value
				,NULL AS tag
				--,NULL matchAttribute
			
			FROM @attributeValues.nodes('//attributes/attribute') xmldata(a) 
			ORDER BY 6, 5, 7
		END	
		
		 
	  

		BEGIN TRAN
				INSERT INTO [IoTConnect].[AttributeValue]
					(guid, companyGuid, localName, uniqueId, tag,attributeValue, createdDate, sdkUpdatedDate, gatewayUpdatedDate, deviceUpdatedDate,aggregateType)
				SELECT 
					NEWID()
					,@companyguid
					,tt.LocalName
					,tt.uniqueId
					,tt.tag
					,tt.value
					,@dt
					,tt.sdkDate
					,tt.gatewayDate
					,tt.deviceDate
					,tt.aggregateType
				FROM #tmpTelemetry tt 			 

		COMMIT TRAN	
	 
	END TRY 	
	BEGIN CATCH
	SET @output = 0
	DECLARE @errorReturnMessage nvarchar(MAX)

	SELECT
		@errorReturnMessage = ISNULL(@errorReturnMessage, ' ') + SPACE(1) +
		'ErrorNumber:' + ISNULL(CAST(ERROR_NUMBER() AS nvarchar), ' ') +
		'ErrorSeverity:' + ISNULL(CAST(ERROR_SEVERITY() AS nvarchar), ' ') +
		'ErrorState:' + ISNULL(CAST(ERROR_STATE() AS nvarchar), ' ') +
		'ErrorLine:' + ISNULL(CAST(ERROR_LINE() AS nvarchar), ' ') +
		'ErrorProcedure:' + ISNULL(CAST(ERROR_PROCEDURE() AS nvarchar), ' ') +
		'ErrorMessage:' + ISNULL(CAST(ERROR_MESSAGE() AS nvarchar(MAX)), ' ')

	RAISERROR (@errorReturnMessage
	, 11
	, 1
	)

	IF (XACT_STATE()) = -1 BEGIN
		ROLLBACK TRANSACTION
	END
	IF (XACT_STATE()) = 1 BEGIN
		ROLLBACK TRANSACTION
	END
	END CATCH
END
