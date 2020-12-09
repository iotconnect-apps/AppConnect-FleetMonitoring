     
/*******************************************************************               
begin tran
DECLARE 
	@UserXml XML =N'<items>
  <item>
    <roleguid>3E08A469-03B8-432D-BD5F-8EB5306C07E9</roleguid>
    <guid>9EF33CFB-85E0-4385-855A-422E881CB054</guid>
    <userid>surendra-1234@yopmail.com</userid>
    <firstname>Surendrasinh</firstname>
    <lastname>Rathod</lastname>
    <entityGuid>A4138F4D-51A6-440A-9319-97AD8E5C31E4</entityGuid>
    <contactNo>91-9016824518</contactNo>
    <timezoneGuid>8ECBA46E-5951-475A-BEB4-C242A575EF19</timezoneGuid>
    <isActive>true</isActive>
    <isDeleted>0</isDeleted>
  </item>
</items>'

EXEC [dbo].[IotConnect_ManageUser]
	  @companyGuid	= '895019cf-1d3e-420c-828f-8971253e5784'
     ,@UserXml		= @UserXml
	 ,@action		= 'update'

rollback
 
001	sgh-1 21-11-2019 [Nishit Khakhi]	Manage multiple Entity

 *******************************************************************/            
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
		
		SELECT
			  b.value('guid[1]', 'UNIQUEIDENTIFIER') guid
			, b.exist('guid[1]') hasGuid
			, b.value('roleguid[1]', 'UNIQUEIDENTIFIER') roleGuid
			, b.exist('roleguid[1]') hasRoleGuid
			
			, b.value('userid[1]', 'NVARCHAR(100)') userId
			
			, b.value('firstname[1]', 'NVARCHAR(100)') firstName
			, b.exist('firstname[1]') hasFirstName

			, b.value('lastname[1]', 'NVARCHAR(100)') lastName
			, b.exist('lastname[1]') hasLastName

			, b.value('entityGuid[1]', 'UNIQUEIDENTIFIER') entityGuid
			, b.exist('entityGuid[1]') hasEntityGuid			

			, b.value('timezoneGuid[1]', 'UNIQUEIDENTIFIER') timeZoneGuid
			, b.exist('timezoneGuid[1]') hasTimeZoneGuid
			, b.value('contactNo[1]', 'NVARCHAR(25)') contactNo
			, b.exist('contactNo[1]') hasContactNo
			, b.value('isActive[1]', 'BIT') isActive
			, b.value('isDeleted[1]', 'BIT') isDeleted
			
	INTO #tempUser
	FROM @UserXml.nodes('/items/item') a(b)

		BEGIN TRAN

		IF(@action = 'insert') 
		BEGIN
		If not exists (select 1 from dbo.[User] where companyGuid=@companyGuid)
			begin
			;WITH ExistingUser AS
			(
				SELECT [guid]
				FROM dbo.[User] d (NOLOCK)
			)

			INSERT INTO dbo.[User]
				([guid],[email], [firstName], [lastName], [entityGuid], [companyGuid], [isActive], [isDeleted],[createdDate],[timeZoneGuid], [contactNo], [roleGuid])
			SELECT DISTINCT [guid], [userId], firstName, lastName, entityGuid, @companyGuid, isActive, isDeleted, GETUTCDATE(), [timeZoneGuid], [contactNo], [roleGuid]
			FROM #tempUser te
			WHERE NOT EXISTS ( SELECT 1 FROM ExistingUser ee WHERE te.[guid] = ee.[guid] )
			end
		END
			
		IF(@action = 'update') 
		BEGIN
			UPDATE d
			SET  [firstName] = CASE WHEN te.hasFirstName = 1 THEN te.firstName ELSE d.firstName END
				,[lastName]	= CASE WHEN te.hasLastName = 1 THEN te.lastName ELSE d.lastName END
				,[entityGuid] = CASE WHEN te.hasEntityguid = 1 THEN te.entityguid ELSE d.[entityGuid] END				
				,[roleGuid] = CASE WHEN te.hasRoleGuid = 1 THEN te.roleGuid ELSE d.[roleGuid] END				
				,[contactNo] = CASE WHEN te.hasContactNo = 1 THEN te.contactNo ELSE d.[contactNo] END				
				,[timeZoneGuid] = CASE WHEN te.hasTimeZoneGuid = 1 THEN te.timeZoneGuid ELSE d.[timeZoneGuid] END				
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