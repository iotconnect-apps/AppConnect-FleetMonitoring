SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[IotConnect_ManageDevice]
(              
	 @companyGuid		UNIQUEIDENTIFIER
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
            SELECT 'Device_ManageRef' AS '@procName'
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
		
		DECLARE @dtAttr DATETIME = GETDATE();

		SELECT 
			 b.value('guid[1]', 'UNIQUEIDENTIFIER') guid
			,b.exist('guid[1]') hasGuid

			,b.value('uniqueId[1]', 'NVARCHAR(200)') uniqueId
			,b.exist('uniqueId[1]') hasUniqueId
			
			,b.value('entityGuid[1]', 'UNIQUEIDENTIFIER') entityGuid
			,b.exist('entityGuid[1]') hasentityGuid

			,b.value('companyGuid[1]', 'UNIQUEIDENTIFIER') companyGuid
			,b.exist('companyGuid[1]') hasCompanyGuid
			
			,b.value('isActive[1]', 'BIT') isActive
			,b.exist('isActive[1]') hasIsActive

			,b.value('isDeleted[1]', 'BIT') isDeleted
			,b.exist('isDeleted[1]') hasIsDeleted

			,b.value('isConnected[1]', 'BIT') isConnected
			,b.exist('isConnected[1]') hasIsConnected
			
			,b.value('tag[1]', 'NVARCHAR(50)') tag
			,b.exist('tag[1]') hasTag

			,b.value('name[1]', 'NVARCHAR(500)') name
			,b.exist('name[1]') hasname

			,b.value('note[1]', 'NVARCHAR(1000)') note
			,b.exist('note[1]') hasnote

			,b.value('parentDeviceGuid[1]', 'UNIQUEIDENTIFIER') parentDeviceGuid
			,b.exist('parentDeviceGuid[1]') hasParentDeviceGuid
			
			,b.value('isHardDelete[1]', 'BIT') isHardDelete
			,b.exist('isHardDelete[1]') hasIsHardDelete

			,b.value('deviceTemplateGuid[1]', 'UNIQUEIDENTIFIER') deviceTemplateGuid
			,b.exist('deviceTemplateGuid[1]') hasDeviceTemplateGuid

			
			,b.value('isAcquired[1]', 'BIT') isAcquired
			,b.exist('isAcquired[1]') hasisAcquired

			
		INTO #tempDevice
		FROM @DeviceXml.nodes('/items/item') a(b)
		
		--select * FROM #tempDevice
		--SELECT rd.* FROM ref.Device rd
		--INNER JOIN #tempDevice td
		--	ON rd.guid = td.guid

		IF(@action = 'insert')
		BEGIN
		
			;WITH ExistingDevice AS
			(
				SELECT guid FROM [Device] d (NOLOCK) WHERE d.companyGuid = @companyGuid
			)
	
			INSERT INTO [device]([guid], [uniqueId], [companyGuid],  [isActive], [isDeleted],   [tag],entityGuid,name,note,deviceTemplateGuid,parentDeviceGuid,isConnected,isAcquired)
			SELECT DISTINCT guid, uniqueId, companyGuid,  isActive, isDeleted,   LOWER(tag)	,entityGuid,name,note,deviceTemplateGuid,parentDeviceGuid,isConnected,isAcquired		FROM #tempDevice td
			WHERE NOT EXISTS ( SELECT 1 FROM ExistingDevice eu WHERE td.guid = eu.guid )

		

		END

		IF(@action = 'update')
		BEGIN
			UPDATE d
				SET  [uniqueId]				= CASE WHEN td.hasUniqueId = 1 THEN td.uniqueId ELSE d.[uniqueId] END
					,[entityGuid]			= CASE WHEN td.hasEntityguid = 1 THEN td.entityguid ELSE d.entityGuid END
					,[deviceTemplateGuid]	= CASE WHEN td.hasDeviceTemplateGuid = 1 THEN td.deviceTemplateGuid ELSE d.deviceTemplateGuid END
					,isAcquired				= CASE WHEN td.hasIsAcquired = 1 THEN td.isAcquired ELSE d.isAcquired END
					,isConnected			= CASE WHEN td.hasIsConnected = 0 THEN td.isConnected ELSE d.isConnected END
			FROM [Device] d (NOLOCK)
			INNER JOIN #tempDevice td
				ON td.guid = d.[guid]
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
				DELETE FROM [device] 
				WHERE guid in (select td.guid from #tempDevice td where td.hasGuid = 1)
							
			END
			ELSE IF(@isHardDelete = 0)
			BEGIN
				UPDATE d
					SET [isDeleted] = td.isDeleted
				FROM [Device] d (NOLOCK)
				INNER JOIN #tempDevice td
					ON td.guid = d.[guid]
				WHERE td.hasGuid = 1
			END
		END
		
		IF(@action = 'bulkdelete')
		BEGIN
			UPDATE d
			SET [isDeleted] = td.isDeleted
				
			FROM [Device] d (NOLOCK)
			INNER JOIN #tempDevice td ON td.[parentDeviceGuid] = d.[guid]
			WHERE td.hasGuid = 1

			UPDATE d
			SET [isDeleted] = td.isDeleted
			FROM [Device] d (NOLOCK)
			INNER JOIN #tempDevice td ON td.[guid] = d.[guid]
			WHERE td.hasGuid = 1
		END

		IF(@action = 'status')
		BEGIN
			UPDATE d
				SET [isActive] = td.isActive
			FROM [Device] d (NOLOCK)
			INNER JOIN #tempDevice td
				ON td.guid = d.[guid]
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

GO
