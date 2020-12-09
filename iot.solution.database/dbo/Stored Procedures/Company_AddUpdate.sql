
/*******************************************************************
begin tran
DECLARE @output INT = 0 
	,@fieldname	nvarchar(255) 
	
EXEC [dbo].[Company_AddUpdate]	
	 @name	= 'Long CPID Test 2'    
	,@cpid	= 'RkQ1NjNGNkEtRjVFNy00NDdGLUFERjctNjIxMDc1MUNDQTczLWFjY2Vzc0tFWS03aXR1dGxhcTV0'    
	,@address = 'Address 1'
	,@countryGuid='BE89F822-F858-46F8-AD78-5EA63BC67B33' 
	,@timezoneGuid='31C6A008-B76F-423C-8F3F-D330CF1F5AF0'
	,@contactNo='1-9845685452'
	,@firstName		='Imramn'
	,@lastName		='Bhadelia' 
	,@userId		='imran.softweb+555@gmail.com'
	,@entityGuid ='BE89F822-F858-46F8-AD78-5EA63BC67B33' 
	,@invokinguser	= '3963F124-A2B7-48CE-A9EB-EB1C205EE44F'
	,@companyGuid			= 'F74434BC-1623-4524-85ED-83A54E00C5DC'
	,@userGuid				= 'F74434BC-1623-4524-85ED-83A54E00C5DC'
	,@roleGuid				= 'F74434BC-1623-4524-85ED-83A54E00C5DC'
	,@version				= 'v1'              
	,@output				= @output		OUTPUT
	,@fieldname				= @fieldname	OUTPUT	

SELECT @output status, @fieldname fieldname
rollback

select * FROM DebugInfo order by 1 desc

001	sgh-1	26-11-2019 [Nishit Khakhi]	Added Initial Version to Add Company
002	SGH-97	22-01-2020 [Nishit Khakhi]	Updated to combine code of Update
003	SGH-97	28-01-2020 [Nishit Khakhi]	Updated to add State, City and Postal Code
*******************************************************************/

CREATE PROCEDURE [dbo].[Company_AddUpdate]
	@name					nvarchar(100)
	,@cpid					nvarchar(200)		
	,@address				nvarchar(500)		= NULL		
	,@countryGuid			UNIQUEIDENTIFIER	= NULL
	,@stateGuid				UNIQUEIDENTIFIER	= NULL
	,@city					nvarchar(50)		= NULL	
	,@postalCode			nvarchar(30)		= NULL	
	,@timezoneGuid			UNIQUEIDENTIFIER	= NULL
	,@contactNo				nvarchar(25)		= NULL	
	,@firstName				nvarchar(50)		= NULL
	,@lastName				nvarchar(50)		= NULL
	,@userId				nvarchar(100)		= NULL
	,@invokinguser			UNIQUEIDENTIFIER		
	,@version				nvarchar(10)    
	,@companyGuid			UNIQUEIDENTIFIER
	,@userGuid				UNIQUEIDENTIFIER
	,@entityGuid		UNIQUEIDENTIFIER
	,@roleGuid			    UNIQUEIDENTIFIER
	,@output				SMALLINT			OUTPUT
	,@fieldname				nvarchar(100)		OUTPUT
	,@culture				nvarchar(10)		= 'en-Us'
	,@enabledebuginfo		CHAR(1)				= '0'
	
AS
BEGIN
	SET NOCOUNT ON
	DECLARE @dt DATETIME = GETUTCDATE()				
    IF (@enabledebuginfo = 1)
	BEGIN
        DECLARE @Param XML 
        SELECT @Param = 
        (
            SELECT 'Company_AddUpdate' AS '@procName'             
            , CONVERT(nvarchar(MAX),@name) AS '@name' 
			, CONVERT(nvarchar(MAX),@cpid) AS '@cpid'			
			, CONVERT(nvarchar(MAX),@address) AS '@address'	
			, CONVERT(nvarchar(MAX),@stateGuid) AS '@stateGuid'									
			, CONVERT(nvarchar(MAX),@countryGuid) AS '@countryGuid' 
			, CONVERT(nvarchar(MAX),@timezoneGuid) AS '@timezoneGuid' 
			, @city AS '@city' 			
			, @postalCode AS '@postalCode' 			
			, @contactNo AS '@contactNo' 			
			, CONVERT(nvarchar(MAX),@firstName) AS '@firstName' 
			, CONVERT(nvarchar(MAX),@lastName) AS '@lastName' 
			, CONVERT(nvarchar(MAX),@userId) AS '@userId' 
			, CONVERT(nvarchar(MAX),@companyGuid) AS '@companyGuid' 
			, CONVERT(nvarchar(MAX),@userGuid) AS '@userGuid' 
			, CONVERT(nvarchar(MAX),@entityGuid) AS '@entityGuid' 
			, CONVERT(nvarchar(MAX),@roleGuid) AS '@roleGuid' 
            , CONVERT(nvarchar(MAX),@invokinguser) AS '@invokinguser'            
            , CONVERT(nvarchar(MAX),@version) AS '@version' 
            , CONVERT(nvarchar(MAX),@output) AS '@output' 
            , CONVERT(nvarchar(MAX),@fieldname) AS '@fieldname'   
            FOR XML PATH('Params')
	    ) 
	    INSERT INTO DebugInfo(data, dt) VALUES(Convert(nvarchar(MAX), @Param), @dt)
    END       
	
	SET @output = 1
	SET @fieldname = 'Success'

	BEGIN TRY
		IF EXISTS(SELECT TOP 1 1 FROM [Company] (NOLOCK) WHERE ([name] = @name Or [guid] = @companyGuid) AND [isdeleted] = 0)
		BEGIN
			SET @output = -3
			SET @fieldname = 'CompanyAlreadyExists'			
			RETURN;
		END

		IF EXISTS(SELECT TOP 1 1 FROM [Company] (NOLOCK) WHERE [cpid] = @cpid AND [isdeleted] = 0)
		BEGIN
			SET @output = -3
			SET @fieldname = 'CompanyCpIdAlreadyExists'			
			RETURN;
		END

		IF EXISTS(SELECT TOP 1 1 FROM [User] (NOLOCK) WHERE [email] = @userId AND [isdeleted] = 0)
		BEGIN
			SET @output = -3
			SET @fieldname = 'EmailAlreadyExists'			
			RETURN;
		END
		
	BEGIN TRAN	
		IF NOT EXISTS(SELECT TOP 1 1 FROM [dbo].[Company] (NOLOCK) where [guid] = @companyGuid AND [isdeleted] = 0)
		BEGIN	
			INSERT INTO [dbo].[Company]([guid]
			,[name]
			,[cpid]
			,[isactive]
			,[isdeleted]
			,[createddate]
			,[createdby]
			,[updateddate]
			,[updatedby]
			,[contactNo]		
			,[address]
			,[countryGuid]
			,[timezoneGuid]
			,[entityGuid]
			,[stateGuid]
			,[city]
			,[postalCode]
			)
			VALUES(
			@companyGuid
			,@name
			,@cpid
			,1
			,0
			,@dt
			,@invokinguser
			,@dt
			,@invokinguser
			,@contactNo		
			,@address						
			,@countryGuid
			,@timezoneGuid
			,@entityGuid
			,@stateGuid
			,@city
			,@postalCode
			)
		END
		ELSE
		BEGIN
			UPDATE [dbo].[Company] 
			SET [name] = @name
				,[contactNo] = @contactNo				
				,[address] = @address								
				,[countryGuid] = @countryGuid
				,[stateGuid] = @stateGuid
				,[city] = @city
				,[postalCode] = @postalCode
				,[timezoneGuid] = @timezoneGuid 
				,[adminUserGuid] = @userGuid
				,[entityGuid] = @entityGuid  
				,[updatedby] = @invokinguser
				,[updateddate] = @dt				
			WHERE [guid] = @userGuid AND isDeleted = 0 
		END

		IF NOT EXISTS(SELECT TOP 1 1 FROM [dbo].[User] (NOLOCK) where [guid] = @userGuid AND companyGuid = @companyGuid AND [isdeleted] = 0)
		BEGIN
			INSERT INTO [User]
			([guid]
			,[email]
			,[companyguid]
			,[firstname]
			,[lastname]
			,[timezoneguid]
			,[imagename]
			,[contactno]
			,[isactive]	
			,[isDeleted]	
			,[createddate]
			,[createdby]
			,[updateddate]
			,[updatedby]
			,[entityGuid]
			,[roleGuid]
			)
			VALUES
			(@userGuid
			,@userId
			,@companyGuid
			,@firstName
			,@lastName
			,@timezoneGuid
			,NULL
			,NULL
			,1
			,0
			,@dt
			,@invokinguser
			,@dt
			,@invokinguser
			,@entityGuid
			,@roleGuid
			)
		END
		
		IF NOT EXISTS(SELECT TOP 1 1 FROM [dbo].[Role] (NOLOCK) where [guid] = @roleGuid AND companyGuid = @companyGuid AND [isdeleted] = 0)
		BEGIN
			INSERT INTO [Role](
			[guid]
			,[companyguid]
			,[name]
			,[isactive]
			,[isdeleted]
			,[createddate]
			,[createdby]
			,[updateddate]
			,[updatedby]
			,[description]
			,[isAdminRole]
			)
			VALUES(@roleGuid
				,@companyGuid
				,'Admin'
				,1
				,0
				,@dt
				,@invokinguser
				,@dt
				,@invokinguser
				,NULL
				,0
			)
		END
		
		---- For Role Module Permission	
		IF NOT EXISTS(SELECT TOP 1 1 FROM [dbo].[RoleModulePermission] (NOLOCK) where [roleGuid] = @roleGuid AND [companyGuid] = @companyGuid)
		BEGIN 
			Insert Into dbo.[RoleModulePermission]
			([guid],[roleGuid],[moduleGuid],[Permission],[companyguid],[createddate],[createdBy])		
			SELECT 
				NEWID(),@roleGuid,m.[guid],m.permission,@companyGuid,@dt,@invokinguser
			FROM dbo.[Module] (NOLOCK) m 
		END
	  
	COMMIT TRAN	

	END TRY 	
	BEGIN CATCH
	SET @output = 0
	DECLARE @errorReturnMessage nvarchar(MAX)

	SELECT
		@errorReturnMessage = ISNULL(@errorReturnMessage, ' ') + SPACE(1) +
		'ErrorNumber:' + ISNULL(CAST(ERROR_NUMBER() AS nvarchar), ' ') +
		'ErrorSeverity:' + ISNULL(CAST(ERROR_SEVERITY() AS nvarchar), ' ') +
		'ErrorState:' + ISNULL(CAST(ERROR_STATE() AS nvarchar), ' ') +
		'ErrorLine:' + ISNULL(CAST(ERROR_LINE() AS nvarchar), ' ') +
		'ErrorProcedure:' + ISNULL(CAST(ERROR_PROCEDURE() AS nvarchar), ' ') +
		'ErrorMessage:' + ISNULL(CAST(ERROR_MESSAGE() AS nvarchar(MAX)), ' ')

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