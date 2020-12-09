SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

Create PROCEDURE [dbo].[IotConnect_ManageRole]
(
	@companyGuid		UNIQUEIDENTIFIER,
	@action				VARCHAR(20),
	@enableDebugInfo	CHAR(1)	= '0',
	@RoleXml			XML
)
AS
BEGIN
	SET NOCOUNT ON

	IF (@enableDebugInfo = 1)
	BEGIN
		DECLARE @Param XML
		SELECT @Param = 
        (
            SELECT 'IotConnect_ManageRole' AS '@procName'
			, CONVERT(nvarchar(MAX), @action) AS '@action'
			, CONVERT(nvarchar(MAX), @companyGuid) AS '@companyGuid'
			, CONVERT(nvarchar(MAX), @RoleXml) AS '@RoleXml'
			FOR XML PATH('Params')
	    )
		INSERT INTO DebugInfo
			(data, dt)
		VALUES(Convert(nvarchar(MAX), @Param), GETUTCDATE())
	END
	BEGIN TRY
		
		IF OBJECT_ID('tempdb..#tempRole') IS NOT NULL DROP TABLE #tempRole
		
		SELECT
			  b.value('guid[1]', 'UNIQUEIDENTIFIER') guid
			, b.exist('guid[1]') hasGuid


			, b.value('name[1]', 'NVARCHAR(100)') name
			, b.exist('name[1]') hasName

			, b.value('description[1]', 'NVARCHAR(100)') description
			, b.exist('description[1]') hasDescription

			, b.value('isActive[1]', 'BIT') isActive
			, b.exist('isActive[1]') hasIsActive

			, b.value('isDeleted[1]', 'BIT') isDeleted
			, b.exist('isDeleted[1]') hasIsDeleted

	INTO #tempRole
	FROM @RoleXml.nodes('/items/item') a(b)

		BEGIN TRAN

			IF(@action = 'insert') 
			BEGIN
				;WITH ExistingRole AS
				(
					SELECT guid
					FROM [Role] d (NOLOCK)
				)

				INSERT INTO [Role]
					([guid], [companyGuid], [name], [description], [isActive], [isDeleted])
				SELECT DISTINCT guid, @companyGuid, name, description, isActive, isDeleted
				FROM #tempRole te
				WHERE NOT EXISTS ( SELECT 1 FROM ExistingRole ee WHERE te.guid = ee.guid )
			END
			
			IF(@action = 'update') 
			BEGIN
				UPDATE d
						SET  [name]	= CASE WHEN te.hasname = 1 THEN te.name ELSE d.name END
							,[description]	= CASE WHEN te.hasDescription = 1 THEN te.description ELSE d.description END			
						FROM [Role] d (NOLOCK)
				INNER JOIN #tempRole te
				ON te.guid = d.[guid]    
				WHERE te.hasGuid = 1
			END  
			
			IF(@action = 'delete') 
			BEGIN
				UPDATE e
						SET [isDeleted] = te.isDeleted
						FROM [Role] AS e (NOLOCK)
				INNER JOIN #tempRole te
				ON te.guid = e.[guid]
				WHERE te.hasGuid = 1
			END
			
			IF(@action = 'status')
			BEGIN
				UPDATE e
						SET [isActive] = te.isActive
						FROM [Role] AS e (NOLOCK)
				INNER JOIN #tempRole te
				ON te.guid = e.[guid]
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