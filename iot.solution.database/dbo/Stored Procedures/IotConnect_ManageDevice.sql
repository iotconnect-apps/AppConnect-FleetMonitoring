     
/*******************************************************************               
begin tran
DECLARE 
	@UserXml XML =N'<items><item><guid>0EB61811-06BB-4621-88D3-026B91A55534</guid><uniqueId>OldOne</uniqueId><entityGuid>93C45621-0B05-4922-A3F1-4C40CA991681</entityGuid><oldEntityGuid>93C45621-0B05-4922-A3F1-4C40CA991681</oldEntityGuid><deviceTemplateGuid>938B3485-9970-4A8A-9C25-600FF8503DBB</deviceTemplateGuid><firmwareupgradeguid /><displayName>Teset2</displayName><note>test note</note><isActive>1</isActive><isDeleted>0</isDeleted><isAcquired>0</isAcquired><parentDeviceGuid>5A376AB3-0794-4960-A96E-84C0C0629124</parentDeviceGuid></item></items><items><item><guid>5A376AB3-0794-4960-A96E-84C0C0629124</guid><uniqueId>NewOne</uniqueId><entityGuid>93C45621-0B05-4922-A3F1-4C40CA991681</entityGuid><oldEntityGuid>93C45621-0B05-4922-A3F1-4C40CA991681</oldEntityGuid><deviceTemplateGuid>938B3485-9970-4A8A-9C25-600FF8503DBB</deviceTemplateGuid><firmwareupgradeguid /><displayName>Test1</displayName><note>test note55</note><isActive>1</isActive><isDeleted>0</isDeleted><isAcquired>0</isAcquired><parentDeviceGuid /></item></items>'

EXEC [dbo].[IotConnect_ManageDevice]
	  @companyGuid	= '895019cf-1d3e-420c-828f-8971253e5784'
     ,@DeviceXml	= @UserXml
	 ,@action		= 'insert'

rollback
 
001	sgh-1 27-12-2019 [Nishit Khakhi]	Manage Device
 *******************************************************************/            
CREATE PROCEDURE [dbo].[IotConnect_ManageDevice]
(    @companyGuid		UNIQUEIDENTIFIER
	,@action			VARCHAR(20)
	,@enableDebugInfo	CHAR(1)	= '0'
	,@DeviceXml		XML
)AS              
BEGIN
    SET NOCOUNT ON

    IF (@enableDebugInfo = 1)
	BEGIN
        DECLARE @Param XML 
        SELECT @Param = 
        (
            SELECT 'IotConnect_ManageDevice' AS '@procName'
			, CONVERT(nvarchar(MAX), @action) AS '@action'
			, CONVERT(nvarchar(MAX), @companyGuid) AS '@companyGuid'
			, CONVERT(nvarchar(MAX), @DeviceXml) AS '@DeviceXml'
			FOR XML PATH('Params')
	    ) 
	    INSERT INTO DebugInfo(data, dt) VALUES(Convert(nvarchar(MAX), @Param), GETUTCDATE())
    END         
	BEGIN TRY

		BEGIN TRAN
		IF OBJECT_ID('tempdb..#tempDevice') IS NOT NULL DROP TABLE #tempDevice
		
		SELECT 
			 b.value('guid[1]', 'UNIQUEIDENTIFIER') [guid]
			,b.exist('guid[1]') hasGuid

			,b.value('uniqueId[1]', 'NVARCHAR(200)') uniqueId
			,b.exist('uniqueId[1]') hasUniqueId
			
			,b.value('entityGuid[1]', 'UNIQUEIDENTIFIER') entityGuid
			,b.exist('entityGuid[1]') hasentityGuid

			,b.value('gatewayGuid[1]', 'UNIQUEIDENTIFIER') gatewayGuid
			,b.exist('gatewayGuid[1]') hasgatewayGuid

			,b.value('isActive[1]', 'BIT') isActive
			,b.value('isDeleted[1]', 'BIT') isDeleted
			
			,b.value('isConnected[1]', 'BIT') isConnected
			,b.exist('isAcquired[1]') hasIsConnected
			
			,b.value('tag[1]', 'NVARCHAR(50)') tag
			,b.exist('tag[1]') hasTag

			,b.value('displayName[1]', 'NVARCHAR(500)') [name]
			,b.exist('displayName[1]') hasname

			,b.value('note[1]', 'NVARCHAR(1000)') note
			,b.exist('note[1]') hasnote

			,b.value('(parentDeviceGuid/text())[empty(.)=false()][1]', 'UNIQUEIDENTIFIER') parentDeviceGuid
			,b.exist('parentDeviceGuid[1]') hasParentDeviceGuid
			
			,b.value('isHardDelete[1]', 'BIT') isHardDelete
			,b.exist('isHardDelete[1]') hasIsHardDelete

			,b.value('deviceTemplateGuid[1]', 'UNIQUEIDENTIFIER') deviceTemplateGuid
			,b.exist('deviceTemplateGuid[1]') hasDeviceTemplateGuid
			,b.value('isAcquired[1]', 'BIT') isAcquired
			
		INTO #tempDevice
		FROM @DeviceXml.nodes('/items/item') a(b)

		--IF(@action = 'insert')
		--BEGIN
		--	;WITH ExistingDevice AS
		--	(
		--		SELECT [guid] FROM dbo.[Device] d (NOLOCK) WHERE d.companyGuid = @companyGuid
		--	)
	
		--	INSERT INTO dbo.[Device]([guid], [uniqueId], [name], [companyGuid], [entityGuid], [isActive], [isDeleted], [templateGuid], [isProvisioned], [tag], [parentDeviceGuid],[createdDate],[note])
		--	SELECT DISTINCT [guid], uniqueId, ISNULL([name],''), @companyGuid, entityGuid, isActive, isDeleted, deviceTemplateGuid, IsNULL(isConnected,0), LOWER(tag), parentDeviceGuid, GETUTCDATE(),ISNULL([note],'')
		--	FROM #tempDevice td
		--	WHERE NOT EXISTS ( SELECT 1 FROM ExistingDevice eu WHERE td.[guid] = eu.[guid] )
	
		--END

		IF(@action = 'update')
		BEGIN
			UPDATE d
				SET  [uniqueId]			= CASE WHEN td.hasUniqueId = 1 THEN td.uniqueId ELSE d.[uniqueId] END
					,[entityGuid]		= CASE WHEN td.hasentityGuid = 1 THEN td.entityGuid ELSE d.[entityGuid] END
					,[templateGuid]		= CASE WHEN td.hasdevicetemplateGuid = 1 THEN td.deviceTemplateGuid ELSE d.templateGuid END
					,[isProvisioned]	= CASE WHEN td.hasIsConnected = 1 THEN td.isAcquired ELSE d.isProvisioned END
					,[note]				= CASE WHEN td.hasnote = 1 THEN td.note ELSE d.note END
					,[name]				= CASE WHEN td.hasname = 1 THEN td.[name] ELSE d.[name] END
			FROM dbo.[Device] d (NOLOCK)
			INNER JOIN #tempDevice td
				ON td.[guid] = d.[guid]
			WHERE td.hasGuid = 1
		END

		IF(@action = 'delete')
		BEGIN
			DECLARE @isHardDelete BIT = 0;
			SET @isHardDelete = (select top 1 td.isHardDelete from #tempDevice td where td.hasGuid = 1 and td.hasIsHardDelete = 1)
			IF @isHardDelete IS NULL
			BEGIN
				SET @isHardDelete = 0
			END

			IF(@isHardDelete = 1)
			BEGIN
				DELETE FROM dbo.[Device] 
				WHERE [guid] IN (select td.[guid] from #tempDevice td where td.hasGuid = 1)
			END
			ELSE IF(@isHardDelete = 0)
			BEGIN
				UPDATE d
					SET [isDeleted] = td.isDeleted
				FROM dbo.[Device] d (NOLOCK)
				INNER JOIN #tempDevice td ON td.[guid] = d.[guid]
				WHERE td.hasGuid = 1
			END
		END
		
		IF(@action = 'status')
		BEGIN
			UPDATE d
				SET [isActive] = td.isActive
			FROM dbo.[Device] d (NOLOCK)
			INNER JOIN #tempDevice td ON td.[guid] = d.[guid]
			WHERE td.hasGuid = 1
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