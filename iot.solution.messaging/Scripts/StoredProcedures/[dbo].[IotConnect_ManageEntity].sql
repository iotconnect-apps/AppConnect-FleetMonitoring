SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[IotConnect_ManageEntity]
(
	@companyGuid		UNIQUEIDENTIFIER,
	@action				nVARCHAR(20),
	@enableDebugInfo	CHAR(1)	= '0',
	@EntityXml			XML
)
AS
BEGIN
	SET NOCOUNT ON

	IF (@enableDebugInfo = 1)
	BEGIN
		DECLARE @Param XML
		SELECT @Param = 
        (
            SELECT 'IotConnect_ManageEntity' AS '@procName'
			, CONVERT(nvarchar(MAX), @action) AS '@action'
			, CONVERT(nvarchar(MAX), @companyGuid) AS '@companyGuid'
			, CONVERT(nvarchar(MAX), @EntityXml) AS '@EntityXml'
			FOR XML PATH('Params')
	    )
		INSERT INTO DebugInfo
			(data, dt)
		VALUES(Convert(nvarchar(MAX), @Param), GETUTCDATE())
	END

	BEGIN TRY
		
		IF OBJECT_ID('tempdb..#tempEntity') IS NOT NULL DROP TABLE #tempEntity
		
		SELECT
			b.value('guid[1]', 'UNIQUEIDENTIFIER') guid
			, b.exist('guid[1]') hasGuid
			
			, b.value('name[1]', 'NVARCHAR(100)') name
			, b.exist('name[1]') hasName

			, b.value('address[1]', 'NVARCHAR(100)') address
			, b.exist('address[1]') hasaddress

			, b.value('city[1]', 'NVARCHAR(100)') city
			, b.exist('city[1]') hascity

			, b.value('isDeleted[1]', 'BIT') isDeleted
			, b.exist('isDeleted[1]') hasIsDeleted

			, b.value('state[1]', 'UNIQUEIDENTIFIER') state
			, b.exist('state[1]') hasstate

			, b.value('country[1]', 'UNIQUEIDENTIFIER') country
			, b.exist('country[1]') hascountry
			
			, b.value('pGuid[1]', 'UNIQUEIDENTIFIER') pGuid
			, b.exist('pGuid[1]') hasPGuid		
			
	INTO #tempEntity
	FROM @EntityXml.nodes('/items/item') a(b)

		BEGIN TRAN

			IF(@action = 'insert') 
			BEGIN
				;WITH
					ExistingEntity
					AS
					(
						SELECT [guid]
						FROM dbo.[Entity] d (NOLOCK)
					)

				INSERT INTO dbo.[Entity] ([guid], [name], [companyGuid], [isDeleted],[address],city,stateGuid,countryGuid,[parentEntityGuid])
				SELECT DISTINCT [guid], [name], @companyGuid, isDeleted,[address],[city],[state],[country],pGuid
				FROM #tempEntity te
				WHERE NOT EXISTS ( SELECT 1 FROM ExistingEntity ee WHERE te.[guid] = ee.[guid] )
			END
			
			IF(@action = 'update') 
			BEGIN
				UPDATE d
				SET  [name]	= CASE WHEN te.hasName = 1 THEN te.[name] ELSE d.[name] END
				,[parentEntityGuid]	= CASE WHEN te.hasPGuid = 1 THEN te.pguid ELSE d.parentEntityGuid END				
				FROM dbo.[Entity] d (NOLOCK)
				INNER JOIN #tempEntity te
				ON te.[guid] = d.[guid]    
				WHERE te.hasGuid = 1
			END  
			
			IF(@action = 'delete') 
			BEGIN
				UPDATE e
				SET [isDeleted] = te.isDeleted
				FROM dbo.[Entity] AS e (NOLOCK)
				INNER JOIN #tempEntity te
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