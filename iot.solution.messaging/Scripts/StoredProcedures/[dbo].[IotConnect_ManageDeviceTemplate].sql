SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

Create PROCEDURE [dbo].[IotConnect_ManageDeviceTemplate]
(
	@companyGuid		UNIQUEIDENTIFIER,
	@action				VARCHAR(20),
	@enableDebugInfo	CHAR(1)	= '0',
	@DeviceTemplateXml	XML
)
AS
BEGIN
	SET NOCOUNT ON

	IF (@enableDebugInfo = 1)
	BEGIN
		DECLARE @Param XML
		SELECT @Param = 
        (
            SELECT 'DeviceTemplate_ManageRef' AS '@procName'
			, CONVERT(nvarchar(MAX), @action) AS '@action'
			, CONVERT(nvarchar(MAX), @companyGuid) AS '@companyGuid'
			, CONVERT(nvarchar(MAX), @DeviceTemplateXml) AS '@DeviceTemplateXml'
			FOR XML PATH('Params')
	    )
		INSERT INTO DebugInfo (data, dt) VALUES(Convert(nvarchar(MAX), @Param), GETUTCDATE())
	END
	BEGIN TRY
		
		IF OBJECT_ID('tempdb..#tempDeviceTemplate') IS NOT NULL DROP TABLE #tempDeviceTemplate
		
		SELECT
			  b.value('guid[1]', 'UNIQUEIDENTIFIER') guid
			, b.exist('guid[1]') hasGuid

			, b.value('code[1]', 'NVARCHAR(10)') code
			, b.exist('code[1]') hasCode

			, b.value('name[1]', 'NVARCHAR(100)') name
			, b.exist('name[1]') hasName

			, b.value('firmwareGuid[1]', 'UNIQUEIDENTIFIER') firmwareGuid
			, b.exist('firmwareGuid[1]') hasFirmwareGuid			

			, b.value('isDeleted[1]', 'BIT') isDeleted
			, b.exist('isDeleted[1]') hasIsDeleted

			, b.value('isEdgeSupport[1]', 'BIT') isEdgeSupport
			, b.exist('isEdgeSupport[1]') hasIsEdgeSupport

			, b.value('isType2Support[1]', 'BIT') isType2Support
			, b.exist('isType2Support[1]') hasIsType2Support

			, b.value('tag[1]', 'NVARCHAR(50)') tag
			, b.exist('tag[1]') hasTag

	INTO #tempDeviceTemplate
	FROM @DeviceTemplateXml.nodes('/items/item') a(b)

		BEGIN TRAN

			IF(@action = 'insert')
			BEGIN
				;WITH ExistingDeviceTemplate AS
				(
					SELECT guid FROM [DeviceTemplate] d (NOLOCK)
				)

				INSERT INTO [DeviceTemplate] ([guid], [companyGuid], [code], [name], [firmwareGuid], [isDeleted], [isEdgeSupport], [isType2Support], [tag])
				SELECT DISTINCT guid, @companyGuid, code, name, firmwareGuid, isDeleted, isEdgeSupport, (CASE WHEN tag IS NOT NULL THEN 1 ELSE 0 END), LOWER(tag)
				FROM #tempDeviceTemplate te
				WHERE NOT EXISTS ( SELECT 1 FROM ExistingDeviceTemplate ee WHERE te.guid = ee.guid )
			END
			
			IF(@action = 'update') 
			BEGIN
				UPDATE d
					SET  [name]				= CASE WHEN te.hasName = 1 THEN te.name ELSE d.name END
						,[firmwareGuid]		= CASE WHEN te.hasFirmwareGuid = 1 THEN te.firmwareGuid ELSE d.firmwareGuid END
				FROM [DeviceTemplate] d (NOLOCK)
				INNER JOIN #tempDeviceTemplate te
					ON te.guid = d.[guid]
				WHERE te.hasGuid = 1
			END
			
			IF(@action = 'delete')
			BEGIN
				UPDATE e
					SET [isDeleted] = te.isDeleted
				FROM [DeviceTemplate] AS e (NOLOCK)
				INNER JOIN #tempDeviceTemplate te
					ON te.guid = e.[guid]
				WHERE te.hasGuid = 1
			END

		COMMIT
	END TRY
	BEGIN CATCH
	               
		DECLARE @errorReturnMessage nvarchar(MAX)
	        
		SELECT @errorReturnMessage = 
			ISNULL(@errorReturnMessage, '') + SPACE(1) + 
			'ErrorNumber:' + ISNULL(CAST(ERROR_NUMBER() AS nvarchar), '') + 
			'ErrorSeverity:' + ISNULL(CAST(ERROR_SEVERITY() AS nvarchar), '') + 
			'ErrorState:' + ISNULL(CAST(ERROR_STATE() AS nvarchar), '') + 
			'ErrorLine:' + ISNULL(CAST(ERROR_LINE() AS nvarchar), '') + 
			'ErrorProcedure:' + ISNULL(CAST(ERROR_PROCEDURE() AS nvarchar), '') + 
			'ErrorMessage:' + ISNULL(CAST(ERROR_MESSAGE() AS nvarchar(max)), '')
			
		RAISERROR (@errorReturnMessage, 11, 1)

		IF (XACT_STATE()) = -1 BEGIN
			ROLLBACK TRANSACTION
		END
		IF (XACT_STATE()) = 1 BEGIN
			ROLLBACK TRANSACTION
		END
	END CATCH
END

GO