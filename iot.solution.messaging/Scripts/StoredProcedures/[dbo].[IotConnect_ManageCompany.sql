SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

Create PROCEDURE [dbo].[IotConnect_ManageCompany]
(              
	 @companyGuid		UNIQUEIDENTIFIER 	
	,@action			VARCHAR(20)		
	,@enableDebugInfo	CHAR(1)	= '0'
	,@ComapnyXml		XML			
)AS              
BEGIN
    SET NOCOUNT ON

    IF (@enableDebugInfo = 1)
	BEGIN
        DECLARE @Param XML 
        SELECT @Param = 
        (
            SELECT 'Company_ManageRef' AS '@procName'
			, CONVERT(nvarchar(MAX), @action) AS '@action'
			, CONVERT(nvarchar(MAX), @companyGuid) AS '@companyGuid'
			, CONVERT(nvarchar(MAX), @ComapnyXml) AS '@CompanyXml'
			FOR XML PATH('Params')
	    ) 
	    INSERT INTO DebugInfo(data, dt) VALUES(Convert(nvarchar(MAX), @Param), GETUTCDATE())
    END         
	BEGIN TRY
		
		IF OBJECT_ID('tempdb..#tempCompany') IS NOT NULL DROP TABLE #tempCompany
		
		SELECT 
			b.value('guid[1]', 'UNIQUEIDENTIFIER') guid
			,b.exist('guid[1]') hasGuid

			,b.value('name[1]', 'NVARCHAR(100)') name
			,b.exist('name[1]') hasName

			,b.value('cpId[1]', 'NVARCHAR(500)') cpId
			,b.exist('cpId[1]') hasCpId

			,b.value('entityGuid[1]', 'UNIQUEIDENTIFIER') entityGuid
			,b.exist('entityGuid[1]') hasEntityGuid			

			,b.value('isActive[1]', 'BIT') isActive
			,b.exist('isActive[1]') hasIsActive

			,b.value('isDeleted[1]', 'BIT') isDeleted
			,b.exist('isDeleted[1]') hasIsDeleted
			
		INTO #tempCompany
		FROM @ComapnyXml.nodes('/items/item') a(b)

		BEGIN TRAN

			IF(@action = 'insert') 
			
		
			BEGIN
				;WITH ExistingCompany AS
				(
					SELECT [guid] FROM dbo.[Company] d (NOLOCK)
				)
			
				INSERT INTO dbo.[Company]([guid], [cpId], [name], [entityGuid], [isActive], [isDeleted])
				SELECT DISTINCT guid, cpId, name, entityGuid, isActive, isDeleted
				FROM #tempCompany tc WHERE NOT EXISTS ( SELECT 1 FROM ExistingCompany eu WHERE tc.[guid] = eu.[guid] )		
				
			END
			
			IF(@action = 'update') 
			BEGIN
				UPDATE d
				SET  [cpId]				= CASE WHEN tc.hascpId = 1 THEN tc.cpId ELSE d.[cpId] END
					,[name]				= CASE WHEN tc.hasname = 1 THEN tc.name ELSE d.name END
					,[entityGuid]	= CASE WHEN tc.hasEntityguid = 1 THEN tc.entityguid ELSE d.[entityGuid] END
				FROM dbo.[Company] d (NOLOCK)    
				INNER JOIN #tempCompany tc
					ON tc.guid = d.[guid]    
				WHERE tc.hasGuid = 1
			END  
			
			IF(@action = 'delete') 
			BEGIN
				UPDATE c
					SET [isDeleted] = tc.isDeleted , [isActive] = 0
				FROM dbo.[Company] AS c (NOLOCK)
				INNER JOIN #tempCompany tc
					ON tc.[guid] = c.[guid]
				WHERE tc.hasGuid = 1

				UPDATE u
					SET [isDeleted] = tc.isDeleted , [isActive] = 0
				FROM dbo.[User] AS u (NOLOCK)
				INNER JOIN #tempCompany tc
					ON tc.[guid] = u.[companyGuid]
				WHERE tc.hasGuid = 1
			END
			
			IF(@action = 'status')
			BEGIN
				UPDATE c
					SET [isActive] = tc.isActive
				FROM dbo.[Company] AS c (NOLOCK)
				INNER JOIN #tempCompany tc
					ON tc.[guid] = c.[guid]
				WHERE tc.hasGuid = 1
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