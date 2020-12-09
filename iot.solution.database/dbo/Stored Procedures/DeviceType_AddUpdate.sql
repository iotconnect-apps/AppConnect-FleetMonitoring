/*******************************************************************
DECLARE @output INT = 0
	,@fieldName	nvarchar(255)
	,@newid	UNIQUEIDENTIFIER
EXEC [dbo].[DeviceType_AddUpdate]
	@guid	= '200EDCFA-8FF1-4837-91B1-7D5F967F5129'
	,@companyGuid	= '200EDCFA-8FF1-4837-91B1-7D5F967F5129'
	,@name			= 'Test Device Type Name'
	,@make			= 'Test Device Type Make'
	,@model			= 'Test Device Type Model'
	,@manufacturer  = 'Test Device Type Manuf'
	,@entityGuid	= '200EDCFA-8FF1-4837-91B1-7D5F967F5129'
	,@templateGuid  = '200EDCFA-8FF1-4837-91B1-7D5F967F5129'
	,@description	= 'Test Device Type Desc'
	,@invokingUser	= '200EDCFA-8FF1-4837-91B1-7D5F967F5129'
	,@version		= 'v1'
	,@output		= @output		OUTPUT
	,@fieldName		= @fieldName	OUTPUT
	,@newId			= @newId		OUTPUT
SELECT @output status, @fieldName fieldname, @newId	newId	

001	SAR-	01-07-2020	[Nishit Khakhi]	Added Initial Version to Add Device Type
*******************************************************************/

CREATE PROCEDURE [dbo].[DeviceType_AddUpdate]
(	@companyGuid	UNIQUEIDENTIFIER 
	,@name			NVARCHAR (200)   
	,@make			NVARCHAR (200)   
	,@model			NVARCHAR (200) 
	,@manufacturer  NVARCHAR (200) 
	,@entityGuid	UNIQUEIDENTIFIER 
	,@templateGuid  UNIQUEIDENTIFIER
	,@description	NVARCHAR (500)		= NULL
	,@guid			UNIQUEIDENTIFIER	= NULL
	,@invokingUser	UNIQUEIDENTIFIER
	,@version		NVARCHAR(10)
	,@output		SMALLINT			OUTPUT
	,@fieldName		NVARCHAR(100)		OUTPUT
	,@newId			UNIQUEIDENTIFIER	OUTPUT
	,@culture		NVARCHAR(10)		= 'en-Us'
	,@enableDebugInfo	 CHAR(1)		= '0'
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
            SELECT 'DeviceType_AddUpdate' AS '@procName'
			, CONVERT(nvarchar(MAX),@companyGuid) AS '@companyGuid'
			, @name AS '@name'
			, @make AS '@make'
			, @model AS '@model'
			, @manufacturer AS '@manufacturer'
			, @description AS '@description'
			, CONVERT(nvarchar(MAX),@entityGuid) AS '@entityGuid'
			, CONVERT(nvarchar(MAX),@templateGuid) AS '@templateGuid'
			, CONVERT(nvarchar(MAX),@guid) AS '@guid'
			, CONVERT(nvarchar(MAX),@invokingUser) AS '@invokingUser'
            , CONVERT(nvarchar(MAX),@version) AS '@version'
            , CONVERT(nvarchar(MAX),@output) AS '@output'
            , CONVERT(nvarchar(MAX),@fieldName) AS '@fieldName'
			FOR XML PATH('Params')
	    )
	    INSERT INTO DebugInfo(data, dt) VALUES(Convert(nvarchar(MAX), @Param), @dt)
    END

	BEGIN TRY
	
	
	BEGIN TRAN
		IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.[DeviceType] (NOLOCK) WHERE [guid] = @guid And companyGuid=@companyGuid AND [isDeleted] = 0)
		BEGIN
			SET @newId = NEWID()
			INSERT INTO [dbo].[DeviceType]
	           ( [guid]  
				,[companyGuid]
				,[name]        
				,[description] 
				,[make]		  
				,[model]		  
				,[manufacturer] 
				,[entityGuid]	
				,[templateGuid] 
				,[isActive]
				,[isDeleted]
				,[createdDate]
				,[createdBy]
				,[updatedDate]
				,[updatedBy]
				)
	     VALUES (
	           @newId
			   ,@companyGuid
	           ,@name
			   ,@description
	           ,@make
	           ,@model
			   ,@manufacturer
			   ,@entityGuid
			   ,@templateGuid
			   ,1
	           ,0
	           ,@dt
	           ,@invokingUser				   
			   ,@dt
			   ,@invokingUser				   
		)
		END
		ELSE
		BEGIN
			SET @newId = (SELECT TOP 1 [guid] FROM dbo.[DeviceType] (NOLOCK) WHERE [guid] = @guid And companyGuid=@companyGuid AND [isDeleted] = 0)
			UPDATE [dbo].[DeviceType]
			SET [name] = @name
				,[description] = @description
				,[make] = @make
				,[model] = @model		  
				,[manufacturer] = @manufacturer
				,[entityGuid] = @entityGuid	
				,[templateGuid] = @templateGuid
			WHERE [guid] = @newId AND [name] = @name AND [isDeleted] = 0
		END
	COMMIT TRAN
		
	SET @output = 1
	SET @fieldName = 'Success'
	
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