
CREATE PROCEDURE [dbo].[IotConnect_ManageDeviceTemplate]
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
			, b.value('name[1]', 'NVARCHAR(100)') name
			, b.exist('name[1]') hasName
			, b.value('tag[1]', 'NVARCHAR(50)') tag
			, b.value('isDeleted[1]', 'BIT') isDeleted
			
	INTO #tempDeviceTemplate
	FROM @DeviceTemplateXml.nodes('/items/item') a(b)

		BEGIN TRAN
				IF NOT EXISTS(SELECT 1 from dbo.[KitType] d (NOLOCK) INNER JOIN #tempDeviceTemplate te ON te.[guid] = d.[guid] WHERE te.hasGuid = 1) AND @action='update'
				BEGIN
					 SET @action='insert'
				END 
			IF(@action = 'insert')
			BEGIN
				;WITH ExistingDeviceTemplate AS
				(
					SELECT [name], [companyGuid] FROM [dbo].[KitType] k (NOLOCK)
				)

				INSERT INTO [dbo].[KitType] ([guid], [companyGuid], [code], [name], [isDeleted], [tag], [isActive])
				SELECT DISTINCT [guid], @companyGuid,[code], [name], [isDeleted], LOWER([tag]), 1
				FROM #tempDeviceTemplate te
				WHERE NOT EXISTS ( SELECT 1 FROM ExistingDeviceTemplate ee WHERE te.[guid] = ee.[companyGuid] AND te.[name] = ee.[name])
			END
			
			IF(@action = 'update') 
			BEGIN
				UPDATE d
					SET  [name]	= CASE WHEN te.hasName = 1 THEN te.[name] ELSE d.[name] END
				FROM [KitType] d (NOLOCK)
				INNER JOIN #tempDeviceTemplate te
					ON te.[guid] = d.[guid]
						AND d.[name] = te.[name]
				WHERE te.hasGuid = 1
			END
			
			IF(@action = 'delete')
			BEGIN
				UPDATE e
					SET [isDeleted] = te.isDeleted
				FROM [KitType] AS e (NOLOCK)
				INNER JOIN #tempDeviceTemplate te
					ON te.[guid] = e.[guid]
						AND e.[name] = te.[name]
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