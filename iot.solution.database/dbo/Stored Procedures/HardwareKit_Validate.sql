
/*******************************************************************
DECLARE @output INT = 0
	,@fieldName				nvarchar(255)	

EXEC [dbo].[HardwareKit_Validate]	
	@data			= '<HardwareKits><KitTypeGuid>A0EFC81-A862-49C4-BD78-0B2307ACEC95</KitTypeGuid><companyGuid>3ac359d3-2d8e-45a2-a315-d1e6fdb5cdd2</companyGuid><HardwareKit><KitCode>kitbyNikunj</KitCode><UniqueId>testdevice1</UniqueId><Name>devicenametest</Name><Note>testnote</Note><Tag>demo</Tag><IsProvisioned>True</IsProvisioned></HardwareKit></HardwareKits>'
	,@isEdit		= 0
	,@invokinguser  = '200EDCFA-8FF1-4837-91B1-7D5F967F5129'   
	,@version		= 'v1'              
	,@output		= @output		OUTPUT
	,@fieldname		= @fieldName	OUTPUT	

SELECT @output status, @fieldName fieldName

001	SG-18 17-02-2020 [Nishit Khakhi]	Added Initial Version to Validate Kit
*******************************************************************/
CREATE PROCEDURE [dbo].[HardwareKit_Validate]
(	@data				XML
	,@isEdit			BIT					= 0
	,@invokingUser		UNIQUEIDENTIFIER	= NULL
	,@version			NVARCHAR(10)
	,@output			SMALLINT		  OUTPUT
	,@fieldName			NVARCHAR(255)	  OUTPUT	
	,@culture			NVARCHAR(10)	  = 'en-Us'
	,@enableDebugInfo	CHAR(1)			  = '0'
)
AS
BEGIN
    SET NOCOUNT ON
	DECLARE @orderBy VARCHAR(10)
    IF (@enableDebugInfo = 1)
	BEGIN
        DECLARE @Param XML
        SELECT @Param =
        (
            SELECT 'HardwareKit_Validate' AS '@procName'
			, CONVERT(nvarchar(MAX),@data) AS '@data'
			, CONVERT(nvarchar(MAX),@isEdit) AS '@isEdit'
            , CONVERT(nvarchar(MAX),@invokingUser) AS '@invokingUser'
			, CONVERT(nvarchar(MAX),@version) AS '@version'
			, CONVERT(nvarchar(MAX),@output) AS '@output'
            , CONVERT(nvarchar(MAX),@fieldName) AS '@fieldName'
            FOR XML PATH('Params')
	    )
	    INSERT INTO DebugInfo(data, dt) VALUES(Convert(nvarchar(MAX), @Param), GETUTCDATE())
    END
    
	BEGIN TRY
		DECLARE @kitTypeGuid UNIQUEIDENTIFIER
		SELECT TOP 1 @kitTypeGuid = TRY_CONVERT(UNIQUEIDENTIFIER, x.y.value('KitTypeGuid[1]', 'NVARCHAR(50)')) 
		FROM @data.nodes('/KitVerifyRequest') x(y) 
		IF OBJECT_ID ('tempdb..#hardwareKit_data') IS NOT NULL DROP TABLE #hardwareKit_data 
	
		CREATE TABLE #hardwareKit_data
		(	[kitTypeGuid]	UNIQUEIDENTIFIER
			,[kitCode]		NVARCHAR(50)
			,[uniqueId]			NVARCHAR(500)
			,[name]				NVARCHAR(500)
			,[note]				NVARCHAR(1000)
			,[tag]				NVARCHAR(50)
			,[isProvisioned]	BIT
			,[attributeName]	NVARCHAR(100)
			,[errorMessage]	NVARCHAR(300)
		)

		INSERT INTO #hardwareKit_data
		SELECT DISTINCT 
				@kitTypeGuid AS 'KitTypeGuid',
				x.R.query('KitCode').value('.', 'NVARCHAR(50)') AS 'KitCode'
				, x.R.query('UniqueId').value('.', 'NVARCHAR(500)') AS [uniqueId]
				, x.R.query('Name').value('.', 'NVARCHAR(500)') AS [name]
				, x.R.query('Note').value('.', 'NVARCHAR(1000)') AS [note]
				, x.R.query('Tag').value('.', 'NVARCHAR(50)') AS [tag]
				, 0 AS [isProvisioned]
				, x.R.query('AttributeName').value('.', 'NVARCHAR(50)') AS [attributeName]
				, NULL AS [errorMessage]
			FROM @data.nodes('/KitVerifyRequest/HardwareKits/HardwareKitRequest') as x(R)
		
		UPDATE H
		SET [errorMessage] = 'Invalid Kit Type'
		FROM #hardwareKit_data H
		LEFT JOIN [dbo].[KitType] K ON H.[kitTypeGuid] = K.[guid]
		WHERE K.[guid] IS NULL
		
		IF (@isedit = 0)
		BEGIN 
			UPDATE dd
			SET	[errorMessage] = 'UniqueId Already Exists' 					 
			FROM #hardwareKit_data dd
			INNER JOIN [dbo].[HardwareKit] H (NOLOCK)
				ON dd.[uniqueId] = H.uniqueid
				AND H.isDeleted=0 

			UPDATE dd
			SET	[errorMessage] = 'Kit Code Already Exists' 					 
			FROM #hardwareKit_data dd
			INNER JOIN [dbo].[HardwareKit] H (NOLOCK)
				ON dd.[kitCode] = H.[kitCode]
				AND H.isDeleted=0 
		END

		--UPDATE dd
		--SET	[errorMessage] = 'Invalid Attribute Name' 					 
		--FROM #hardwareKit_data dd
		--LEFT JOIN KitTypeAttribute K (NOLOCK) 
		--	ON dd.[attributeName] = K.[localName]
		--	AND dd.[tag] = K.[tag]
		--WHERE K.[guid] IS NULL
		
		SELECT H.[kitCode]
				,H.[kitTypeGuid]
				,H.[uniqueId]			
				,H.[name]				
				,H.[note]				
				,H.[tag]				
				,H.[isProvisioned]	
				,H.[errorMessage] AS [hardwareKitError]	
		FROM #hardwareKit_data H

		IF EXISTS (SELECT TOP 1 1 FROM #hardwareKit_data WHERE [errorMessage] IS NOT NULL)
		BEGIN
			SET @output = -1
			SET @fieldName = 'Failed'	
		END
		
		SET @output = 1
		SET @fieldName = 'Success'	

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