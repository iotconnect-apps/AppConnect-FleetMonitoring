SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[IotConnect_ManageUser]
(              
	 @companyGuid		UNIQUEIDENTIFIER 
	,@action			VARCHAR(20)		
	,@enableDebugInfo	CHAR(1)			= '0'
	,@UserXml			XML 
)AS              
BEGIN
	SET NOCOUNT ON

	IF (@enableDebugInfo = 1)
	BEGIN
		DECLARE @Param XML
		
		SELECT @Param = 
        (
            SELECT 'IotConnect_ManageUser' AS '@procName'
			, CONVERT(nvarchar(MAX), @action) AS '@action'
			, CONVERT(nvarchar(MAX), @companyGuid) AS '@companyGuid'
			, CONVERT(nvarchar(MAX), @UserXml) AS '@UserXml'
			FOR XML PATH('Params')
	    )

		INSERT INTO DebugInfo (data, dt)
		VALUES(Convert(nvarchar(MAX), @Param), GETUTCDATE())
	END
	BEGIN TRY
		
		IF OBJECT_ID('tempdb..#tempUser') IS NOT NULL DROP TABLE #tempUser
		print('guid[1]')
		SELECT
			  b.value('guid[1]', 'UNIQUEIDENTIFIER') guid
			, b.exist('guid[1]') hasGuid

			, b.value('userId[1]', 'NVARCHAR(100)') userId
			, b.exist('userId[1]') hasUserId

			, b.value('firstName[1]', 'NVARCHAR(100)') firstName
			, b.exist('firstName[1]') hasFirstName

			, b.value('lastName[1]', 'NVARCHAR(100)') lastName
			, b.exist('lastName[1]') hasLastName

			, b.value('entityGuid[1]', 'UNIQUEIDENTIFIER') entityGuid
			, b.exist('entityGuid[1]') hasEntityGuid			

			, b.value('isActive[1]', 'BIT') isActive
			, b.exist('isActive[1]') hasIsActive

			, b.value('isDeleted[1]', 'BIT') isDeleted
			, b.exist('isDeleted[1]') hasIsDeleted
			
	

	INTO #tempUser
	FROM @UserXml.nodes('/items/item') a(b)


	
	select * From #tempUser
		BEGIN TRAN

		IF(@action = 'insert') 
		BEGIN
			;WITH ExistingUser AS
			(
				SELECT [guid]
				FROM dbo.[User] d (NOLOCK)
			)

			INSERT INTO dbo.[User]
				([guid], [firstName], [lastName], [entityGuid], [companyGuid], [isActive], [isDeleted],userid)
			SELECT DISTINCT [guid], firstName, lastName, entityGuid, @companyGuid, isActive, isDeleted,userid
			FROM #tempUser te
			WHERE NOT EXISTS ( SELECT 1 FROM ExistingUser ee WHERE te.[guid] = ee.[guid] )
		END
			
		IF(@action = 'update') 
		BEGIN
			UPDATE d
					SET [userid]	= CASE WHEN te.hasUserId = 1 THEN te.userId ELSE d.userId END,
					 [firstName] = CASE WHEN te.hasFirstName = 1 THEN te.firstName ELSE d.firstName END
						,[lastName]	= CASE WHEN te.hasLastName = 1 THEN te.lastName ELSE d.lastName END
						,[entityGuid] = CASE WHEN te.hasEntityguid = 1 THEN te.entityguid ELSE d.[entityGuid] END				
						
					FROM dbo.[User] d (NOLOCK)
			INNER JOIN #tempUser te
			ON te.guid = d.[guid]    
			WHERE te.hasGuid = 1
		END  
			
		IF(@action = 'delete') 
		BEGIN
			UPDATE e
					SET [isDeleted] = te.isDeleted
					FROM dbo.[User] AS e (NOLOCK)
			INNER JOIN #tempUser te
			ON te.[guid] = e.[guid]
			WHERE te.hasGuid = 1
		END
		
		IF(@action = 'status') 
		BEGIN
			UPDATE e
					SET [isActive] = te.isActive
					FROM dbo.[User] AS e (NOLOCK)
			INNER JOIN #tempUser te
			ON te.[guid] = e.[guid]
			WHERE te.hasGuid = 1
		END
		COMMIT
	END TRY              
	BEGIN CATCH              
	               
		DECLARE @errorReturnMessage nvarchar(MAX)        
	        
		SELECT
		@errorReturnMessage =        
			ISNULL(@errorReturnMessage, '') + SPACE(1) +        
			'ErrorNumber:' + ISNULL(CAST(ERROR_NUMBER() AS nvarchar), '') +        
			'ErrorSeverity:' + ISNULL(CAST(ERROR_SEVERITY() AS nvarchar), '') +        
			'ErrorState:' + ISNULL(CAST(ERROR_STATE() AS nvarchar), '') +        
			'ErrorLine:' + ISNULL(CAST(ERROR_LINE() AS nvarchar), '') +        
			'ErrorProcedure:' + ISNULL(CAST(ERROR_PROCEDURE() AS nvarchar), '') +        
			'ErrorMessage:' + ISNULL(CAST(ERROR_MESSAGE() AS nvarchar(max)), '')   
			
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


GO