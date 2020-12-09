/*******************************************************************               
BEGIN tran
select * FROM ref.Role where guid = '10DDA92E-D959-4208-A48B-D715E6A8B1DD'
DECLARE @RoleXml XML =N'<roles>
						  <role>
							<guid>10dda92e-d959-4208-a48b-d715e6a8b1dd</guid>    
						  </role>
						</roles>'

EXEC [dbo].[IotConnect_ManageRole]
	  @companyGuid	= 'C51D63B4-5B6F-461A-B7FC-F55EF734F779'
     ,@RoleXml		= @RoleXml
	 ,@action		= 'update'

ROLLBACK TRAN

select * FROM dbo.Role where guid = '10DDA92E-D959-4208-A48B-D715E6A8B1DD'

001	sgh-1 25-11-2019 [Nishit Khakhi]	Manage multiple Role
 *******************************************************************/
CREATE PROCEDURE [dbo].[IotConnect_ManageRole]
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

			IF(@action = 'insert' OR @action = 'update') 
			BEGIN
			/*	;WITH ExistingRole AS
				(
					SELECT [guid]
					FROM dbo.[Role] d (NOLOCK)
				)

				INSERT INTO dbo.[Role]
					([guid], [companyGuid], [name], [description], [isActive], [isDeleted],[createdDate])
				SELECT DISTINCT [guid], @companyGuid, [name], [description], isActive, isDeleted, GETUTCDATE()
				FROM #tempRole te
				WHERE NOT EXISTS ( SELECT 1 FROM ExistingRole ee WHERE te.[guid] = ee.[guid] )
			END
			
			IF(@action = 'update') 
			BEGIN
				UPDATE d
				SET  [name]	= CASE WHEN te.hasname = 1 THEN te.name ELSE d.name END
					,[description]	= CASE WHEN te.hasDescription = 1 THEN te.description ELSE d.description END			
				FROM dbo.[Role] d (NOLOCK)
				INNER JOIN #tempRole te
				ON te.[guid] = d.[guid]    
				WHERE te.hasGuid = 1
			END  */

			MERGE dbo.[Role] AS target  
			USING #tempRole AS source ON (target.[name] = source.[name] OR target.[guid] = source.[guid]) and target.companyGuid=@companyGuid 
			WHEN MATCHED THEN
				UPDATE SET [name] = source.[name],[IsDeleted]=0,target.[guid] = source.[guid],
						[description] = source.[description]
			WHEN NOT MATCHED THEN  
				INSERT ([guid], [companyGuid], [name], [description], [isActive], [isDeleted],[createdDate])  
				VALUES (source.[guid], @companyGuid, source.[name], source.[description], source.[isActive], source.[isDeleted], GETUTCDATE())  ;
						
			END
			
			IF(@action = 'delete') 
			BEGIN
				UPDATE e
				SET [isDeleted] = te.isDeleted
				FROM dbo.[Role] AS e (NOLOCK)
				INNER JOIN #tempRole te
				ON te.guid = e.[guid]
				WHERE te.hasGuid = 1
			END
			
			IF(@action = 'status')
			BEGIN
				UPDATE e
				SET [isActive] = te.isActive
				FROM dbo.[Role] AS e (NOLOCK)
				INNER JOIN #tempRole te
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