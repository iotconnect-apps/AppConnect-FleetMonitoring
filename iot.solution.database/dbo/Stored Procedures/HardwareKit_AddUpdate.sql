
/*******************************************************************
DECLARE @output INT = 0
	,@fieldName	nvarchar(255)
	
EXEC [dbo].[HardwareKit_AddUpdate]
	@data			= '<HardwareKits><KitTypeGuid>3ac359d3-2d8e-45a2-a315-d1e6fdb5cdd2</KitTypeGuid><companyGuid>3ac359d3-2d8e-45a2-a315-d1e6fdb5cdd2</companyGuid><HardwareKit><KitCode>kitbyNikunj</KitCode><UniqueId>testdevice1</UniqueId><Name>devicenametest</Name><Note>testnote</Note><Tag>demo</Tag><IsProvisioned>True</IsProvisioned></HardwareKit></HardwareKits>'
	,@invokingUser	= '200EDCFA-8FF1-4837-91B1-7D5F967F5129'
	,@version		= 'v1'
	,@output		= @output		OUTPUT
	,@fieldName		= @fieldName	OUTPUT
	
SELECT @output status, @fieldName fieldname

001	SGH-18	11-02-2020	[Nishit Khakhi]	Added Initial Version to Add Update Hardware Kit

*******************************************************************/

CREATE PROCEDURE [dbo].[HardwareKit_AddUpdate]
(	@data				XML,
	 @isedit			BIT					= 0
	,@invokingUser		UNIQUEIDENTIFIER	= NULL
	,@version			NVARCHAR(10)
	,@output			SMALLINT			OUTPUT
	,@fieldName			NVARCHAR(100)		OUTPUT
	,@culture			NVARCHAR(10)		= 'en-Us'
	,@enableDebugInfo	CHAR(1)				= '0'
)
AS
BEGIN
	SET NOCOUNT ON
	DECLARE @dt DATETIME = GETUTCDATE()
    IF (@enableDebugInfo = 1)
	BEGIN
        DECLARE @Param XML
        SELECT @Param =
        (
            SELECT 'HardwareKit_AddUpdate' AS '@procName'
			, CONVERT(nvarchar(MAX),@data) AS '@data'
            , CONVERT(nvarchar(MAX),@invokingUser) AS '@invokingUser'
            , CONVERT(nvarchar(MAX),@version) AS '@version'
            , CONVERT(nvarchar(MAX),@output) AS '@output'
            , CONVERT(nvarchar(MAX),@fieldName) AS '@fieldName'
			FOR XML PATH('Params')
	    )
	    INSERT INTO DebugInfo(data, dt) VALUES(Convert(nvarchar(MAX), @Param), @dt)
    END

	SET @output = 1
	SET @fieldName = 'Success'
	
	BEGIN TRY
		DECLARE @kitTypeGuid UNIQUEIDENTIFIER, @companyGuid UNIQUEIDENTIFIER
		
		SELECT TOP 1 @kitTypeGuid = TRY_CONVERT(UNIQUEIDENTIFIER, x.y.value('KitTypeGuid[1]', 'NVARCHAR(50)')) 
			, @companyGuid = TRY_CONVERT(UNIQUEIDENTIFIER, x.y.value('companyGuid[1]', 'NVARCHAR(50)')) 
		FROM @data.nodes('/KitVerifyRequest') x(y) 

		IF OBJECT_ID ('tempdb..#hardwareKit_data') IS NOT NULL DROP TABLE #hardwareKit_data 
		IF OBJECT_ID ('tempdb..#HardwareKit_InsertedData') IS NOT NULL DROP TABLE #HardwareKit_InsertedData 
		
		CREATE TABLE #HardwareKit_InsertedData
		(	[kitGuid]		UNIQUEIDENTIFIER
			,[kitTypeGuid]	UNIQUEIDENTIFIER
			,[kitCode]		NVARCHAR(50)
		)
		CREATE TABLE #hardwareKit_data
		(	[kitGuid]		UNIQUEIDENTIFIER
			,[kitTypeGuid]	UNIQUEIDENTIFIER
			,[kitCode]		NVARCHAR(50)
			,[uniqueId]			NVARCHAR(500)
			,[name]				NVARCHAR(500)
			,[note]				NVARCHAR(1000)
			,[tag]				NVARCHAR(50)
			,[isProvisioned]	BIT
			,[attributeName]	NVARCHAR(100)
		)

		INSERT INTO #hardwareKit_data
		SELECT ISNULL(H.[guid],NEWID()) AS [kitGuid], A.* FROM
		(	
			SELECT DISTINCT 
				@kitTypeGuid AS 'KitTypeGuid'
				, x.R.query('./KitCode').value('.', 'NVARCHAR(50)') AS 'KitCode'
				, x.R.query('./UniqueId').value('.', 'NVARCHAR(500)') AS [uniqueId]
				, x.R.query('./Name').value('.', 'NVARCHAR(500)') AS [name]
				, x.R.query('./Note').value('.', 'NVARCHAR(1000)') AS [note]
				, x.R.query('./Tag').value('.', 'NVARCHAR(50)') AS [tag]
				, 0 AS [isProvisioned]
				, x.R.query('./AttributeName').value('.', 'NVARCHAR(50)') AS [attributeName]
			FROM @data.nodes('/KitVerifyRequest/HardwareKits/HardwareKitRequest') as x(R)
		) A
		LEFT JOIN [dbo].[HardwareKit] H (NOLOCK) ON A.[kitCode] = H.[kitCode] AND H.[isDeleted] = 0
		

		BEGIN TRAN

			IF ( @isedit != 1 )
			BEGIN 
			INSERT INTO [dbo].[HardwareKit] ([guid]
				,[kitTypeGuid]
				,[kitCode]
				,[companyGuid]
				,[uniqueId]
				,[name]
				,[note]
				,[tagGuid]
				,[isProvisioned]
				,[isActive]
				,[isDeleted]
				,[createdDate]
				,[createdBy]
				,[updatedDate]
				,[updatedBy]
)
			OUTPUT inserted.[guid], inserted.[kitTypeGuid], inserted.[kitCode] INTO #HardwareKit_InsertedData
			SELECT TH.[kitGuid]
				, TH.[kitTypeGuid]
				, TH.[kitCode]
				, @companyGuid
				, TH.[uniqueId]
				, TH.[name]
				, TH.[note]
				, KTA.[Guid]
				, TH.[isProvisioned]
				, 1
				, 0
				,@dt
				,@invokingUser
				,@dt
				,@invokingUser
			FROM #hardwareKit_data TH
			LEFT JOIN [dbo].[HardwareKit] H (NOLOCK) ON TH.[kitCode] = H.[kitCode] AND H.isDeleted = 0
			LEFT JOIN [dbo].[KitTypeAttribute] KTA (NOLOCK) ON TH.[kitTypeGuid] = KTA.[templateGuid] AND TH.[attributeName] = KTA.[localName] AND KTA.[tag] = TH.[tag]
			WHERE H.[guid] IS NULL
			 
			END
			ELSE
			BEGIN
				INSERT INTO #HardwareKit_InsertedData
				SELECT h.[guid]		
					,h.[kitTypeGuid]	
					,h.[kitCode]		
				FROM [dbo].[HardwareKit] h (NOLOCK)
				INNER JOIN #hardwareKit_data hd ON h.[kitCode] = hd.[kitCode]
				WHERE h.[isDeleted] = 0
			END

			-- Update Existsing Device
			UPDATE KD
			SET [name] = DD.[name]
				,[note] = DD.[note]
				,[tagGuid] = KTA.[guid]
				,[isProvisioned] = DD.[isProvisioned]
			FROM [dbo].[HardwareKit] KD
			INNER JOIN #hardwareKit_data DD ON KD.[kitCode] = DD.[kitCode] AND KD.[uniqueId] = DD.[uniqueId] AND KD.[isDeleted] = 0
			LEFT JOIN [dbo].[KitTypeAttribute] KTA (NOLOCK) ON KD.[kitTypeGuid] = KTA.[templateGuid] AND DD.[attributeName] = KTA.[localName] AND KTA.[tag] = DD.[tag]
			
		COMMIT TRAN
		--SELECT * FROM #hardwareKit_data
		
		SELECT * FROM #HardwareKit_InsertedData
		
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