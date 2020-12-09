
/*******************************************************************
DECLARE @output INT = 0
	,@fieldName	nvarchar(255)
	,@newid UNIQUEIDENTIFIER

EXEC [dbo].[Device_UpdateImage]
	@companyGuid		= '895019CF-1D3E-420C-828F-8971253E5784'
	,@guid				= '5330D171-DA1E-4554-A868-87D222CD5D35'
	,@image				= 'Device 123.jpg'
	,@invokingUser		= '200EDCFA-8FF1-4837-91B1-7D5F967F5129'
	,@version			= 'v1'
	,@output			= @output		OUTPUT
	,@fieldName			= @fieldName	OUTPUT
	,@newid				= @newid		OUTPUT

SELECT @output status, @fieldName fieldname,@newid newid

001	SGH-1 05-12-2019 [Nishit Khakhi]	Added Initial Version to Update Device Image
*******************************************************************/

create PROCEDURE [dbo].[Device_UpdateImage]
(
	@companyGuid		UNIQUEIDENTIFIER
	,@guid				UNIQUEIDENTIFIER
	,@image				NVARCHAR(50)		= NULL
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
            SELECT 'Device_UpdateImage' AS '@procName'
			, CONVERT(nvarchar(MAX),@companyGuid) AS '@companyGuid'
			, CONVERT(nvarchar(MAX),@guid) AS '@guid'
			, @image AS '@image'
			, CONVERT(nvarchar(MAX),@invokingUser) AS '@invokingUser'
            , CONVERT(nvarchar(MAX),@version) AS '@version'
            , CONVERT(nvarchar(MAX),@output) AS '@output'
            , CONVERT(nvarchar(MAX),@fieldName) AS '@fieldName'
			, CONVERT(nvarchar(MAX),@newid) AS '@newid'
            	FOR XML PATH('Params')
	    )
	    INSERT INTO DebugInfo(data, dt) VALUES(Convert(nvarchar(MAX), @Param), @dt)
    END

	SET @output = 1
	SET @fieldName = 'Success'
	
	BEGIN TRY
		IF NOT EXISTS (SELECT TOP 1 1 FROM [dbo].[Device] (NOLOCK) WHERE [guid] = @guid AND [companyGuid]=@companyGuid AND [isDeleted]=0)
		BEGIN
			Set @output = -3
			SET @fieldname = 'DeviceNotFound'
			RETURN;
		END  
		
		BEGIN TRAN
			UPDATE [dbo].[Device]
		    SET	[image] = @image
				,[updatedDate] = @dt
		        ,[updatedBy] = @invokingUser
			WHERE [guid] = @guid AND [companyGuid] = @companyGuid AND [isDeleted] = 0
			   
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