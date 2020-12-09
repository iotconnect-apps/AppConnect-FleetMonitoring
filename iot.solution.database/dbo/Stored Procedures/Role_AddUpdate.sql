/*******************************************************************
DECLARE @output INT = 0
	,@fieldName	nvarchar(255)
	,@newid UNIQUEIDENTIFIER

EXEC [dbo].[Role_AddUpdate]
	@guid					= '895019CF-1D3E-420C-828F-8971253E5784'
	,@companyGuid			= '895019CF-1D3E-420C-828F-8971253E5784'
	,@name					= 'Role Test'
	,@description			= 'Role Test Desc2'
	,@isAdminRole			= 0
	,@invokingUser			= '200EDCFA-8FF1-4837-91B1-7D5F967F5129'
	,@version				= 'v1'
	,@output				= @output		OUTPUT
	,@fieldName				= @fieldName	OUTPUT
	,@newid					= @newid		OUTPUT

SELECT @output status, @fieldName fieldname,@newid newid

001	SGH-97	22-01-2020	[Nishit Khakhi]	Added Initial Version to Add Update Role
*******************************************************************/
create PROCEDURE [dbo].[Role_AddUpdate]
(	@companyGuid		UNIQUEIDENTIFIER
	,@guid				UNIQUEIDENTIFIER	= NULL
	,@name	 			NVARCHAR(100)		= NULL
	,@description		NVARCHAR(500)		= NULL
	,@isAdminRole		BIT					= 0
	,@invokingUser		UNIQUEIDENTIFIER
	,@version			NVARCHAR(10)
	,@output			SMALLINT			OUTPUT
	,@fieldName			NVARCHAR(100)		OUTPUT
	,@newid				UNIQUEIDENTIFIER   	OUTPUT
	,@culture			NVARCHAR(10)		= 'en-Us'
	,@enableDebugInfo	 CHAR(1)			= '0'
)
AS
BEGIN
	SET NOCOUNT ON
	DECLARE @dt DATETIME = GETUTCDATE()
    IF (@enableDebugInfo = 1)
	BEGIN
        DECLARE @Param XML
        SELECT @Param =
        (
            SELECT 'Role_AddUpdate' AS '@procName'
			, CONVERT(nvarchar(MAX),@companyGuid) AS '@companyGuid'
			, CONVERT(nvarchar(MAX),@guid) AS '@guid'
			, @name AS '@name'
			, @description AS '@description'
			, CONVERT(nvarchar(MAX),@isAdminRole) AS '@isAdminRole'
            , CONVERT(nvarchar(MAX),@invokingUser) AS '@invokingUser'
            , CONVERT(nvarchar(MAX),@version) AS '@version'
            , CONVERT(nvarchar(MAX),@output) AS '@output'
            , CONVERT(nvarchar(MAX),@fieldName) AS '@fieldName'
			FOR XML PATH('Params')
	    )
	    INSERT INTO DebugInfo(data, dt) VALUES(Convert(nvarchar(MAX), @Param), @dt)
    END

	SET @output = 1
	SET @fieldName = 'Success'
	
	BEGIN TRY
		IF @guid IS NOT NULL
		BEGIN
			SET @newid = @guid
		END
		ELSE IF EXISTS (SELECT TOP 1 1 FROM [Role] (NOLOCK) WHERE companyguid = @companyguid AND [isdeleted]=0 AND [name]=@name)
		BEGIN
			SELECT TOP 1 @newid = [guid] FROM [Role] (NOLOCK) WHERE companyguid = @companyguid AND [isdeleted]=0 AND [name]=@name
		END
		ELSE
		BEGIN
			SET @newid = ISNULL((SELECT TOP 1 [guid] FROM [Role] (NOLOCK) WHERE companyguid = @companyguid AND [isdeleted]=1 AND [name]=@name),NEWID())
		END
		
		BEGIN TRAN
			IF EXISTS (SELECT TOP 1 1 FROM [Role] (NOLOCK) WHERE companyguid = @companyguid AND [isdeleted]=1 AND [name]=@name)
			BEGIN
				UPDATE [Role]
				SET [isDeleted] = 0
					,[description] = ISNULL(@description,[description])
					,[updatedBy] = @invokingUser
					,[updatedDate] = @dt
					,[guid]=@newid
				WHERE companyguid = @companyguid AND [isdeleted]=1 AND [name]=@name
			END
			ELSE IF @guid IS NOT NULL
			BEGIN
				INSERT INTO [dbo].[Role]
			           ([guid]
			           ,[companyGuid]
			           ,[name]
					   ,[description]
					   ,[isAdminRole]
			           ,[isActive]
					   ,[isDeleted]
			           ,[createdDate]
			           ,[createdBy]
			           ,[updatedDate]
			           ,[updatedBy]
						)
			     VALUES
			           (@newid
			           ,@companyGuid
			           ,@name
			           ,@description
			           ,@isAdminRole
					   ,1
			           ,0
			           ,@dt
			           ,@invokingUser				   
					   ,@dt
					   ,@invokingUser				   
				       );
			END
			ELSE IF NOT EXISTS (SELECT TOP 1 1 FROM [Role] (NOLOCK) WHERE [guid] = @newid AND [isdeleted]=0 and companyguid = @companyguid AND [name]=@name)
			BEGIN	
				INSERT INTO [dbo].[Role]
			           ([guid]
			           ,[companyGuid]
			           ,[name]
					   ,[description]
					   ,[isAdminRole]
			           ,[isActive]
					   ,[isDeleted]
			           ,[createdDate]
			           ,[createdBy]
			           ,[updatedDate]
			           ,[updatedBy]
						)
			     VALUES
			           (@newid
			           ,@companyGuid
			           ,@name
			           ,@description
			           ,@isAdminRole
					   ,1
			           ,0
			           ,@dt
			           ,@invokingUser				   
					   ,@dt
					   ,@invokingUser				   
				       );
			END
			ELSE
			BEGIN
				UPDATE [dbo].[Role]
				SET	[name] = ISNULL(@name,[name])
					,[description] = ISNULL(@description,[description])
					,[isAdminRole] = @isAdminRole
					,[updatedDate] = @dt
					,[updatedBy] = @invokingUser
				WHERE [guid] = @newid AND [isDeleted] = 0
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