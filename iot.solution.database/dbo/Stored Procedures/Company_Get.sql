
/*******************************************************************
DECLARE @output INT = 0
	,@fieldName				nvarchar(255)	

EXEC [dbo].[Company_Get]	
	@guid			= 'BE9A0C84-E880-4123-BA47-AAB147A30D57'
	,@invokinguser  = '200EDCFA-8FF1-4837-91B1-7D5F967F5129'   
	,@version		= 'v1'              
	,@output		= @output		OUTPUT
	,@fieldname		= @fieldName	OUTPUT	

SELECT @output status, @fieldName fieldName

001	sgh-1	26-11-2019 [Nishit Khakhi]	Added Initial Version to Get Company Information
002	SGH-97	28-01-2020 [Nishit Khakhi]	Updated to add State, City and Postal Code
*******************************************************************/
CREATE PROCEDURE [dbo].[Company_Get]
(	 
	@companyGuid		UNIQUEIDENTIFIER
	,@invokingUser		UNIQUEIDENTIFIER
	,@version			NVARCHAR(10)
	,@output			SMALLINT		  OUTPUT
	,@fieldName			NVARCHAR(255)	  OUTPUT	
	,@culture			NVARCHAR(10)	  = 'en-Us'
	,@enableDebugInfo	CHAR(1)			  = '0'
)
AS
BEGIN
    SET NOCOUNT ON
	DECLARE @orderBy VARCHAR(10)
    IF (@enableDebugInfo = 1)
	BEGIN
        DECLARE @Param XML
        SELECT @Param =
        (
            SELECT 'Company_Get' AS '@procName'
			, CONVERT(nvarchar(MAX),@companyGuid) AS '@companyGuid'
            , CONVERT(nvarchar(MAX),@invokingUser) AS '@invokingUser'
			, CONVERT(nvarchar(MAX),@version) AS '@version'
			, CONVERT(nvarchar(MAX),@output) AS '@output'
            , CONVERT(nvarchar(MAX),@fieldName) AS '@fieldName'
            FOR XML PATH('Params')
	    )
	    INSERT INTO DebugInfo(data, dt) VALUES(Convert(nvarchar(MAX), @Param), GETUTCDATE())
    END
    Set @output = 1
    SET @fieldName = 'Success'
   
	IF NOT EXISTS (SELECT TOP 1 1 FROM [dbo].[Company] (NOLOCK) WHERE [guid]=@companyGuid AND [isDeleted]=0 )
	BEGIN
		Set @output = -3
		SET @fieldname = 'CompanyNotExist'
		RETURN;
	END
	IF NOT EXISTS (SELECT TOP 1 1 FROM [dbo].[AdminUser] (NOLOCK) WHERE [guid]=@invokingUser AND [isDeleted]=0 AND isActive = 1 )
	BEGIN
		Set @output = -3
		SET @fieldname = 'UserNotExist'
		RETURN;
	END
 
    BEGIN TRY
		
		SELECT 
		c.*
			,U.email as userId
			,U.firstName
			,U.lastName
		FROM
			[dbo].[Company] (NOLOCK) as c
			LEFT JOIN [User] U ON C.[adminUserGuid] = U.Guid AND U.isDeleted = 0
		WHERE
			c.[guid] = @companyGuid
			AND c.[isDeleted] = 0

	END TRY
	BEGIN CATCH
		DECLARE @errorReturnMessage nvarchar(MAX)

		SET @output = 0

		SELECT @errorReturnMessage =
			ISNULL(@errorReturnMessage, '') +  SPACE(1)   +
			'ErrorNumber:'  + ISNULL(CAST(ERROR_NUMBER() as nvarchar), '')  +
			'ErrorSeverity:'  + ISNULL(CAST(ERROR_SEVERITY() as nvarchar), '') +
			'ErrorState:'  + ISNULL(CAST(ERROR_STATE() as nvarchar), '') +
			'ErrorLine:'  + ISNULL(CAST(ERROR_LINE () as nvarchar), '') +
			'ErrorProcedure:'  + ISNULL(CAST(ERROR_PROCEDURE() as nvarchar), '') +
			'ErrorMessage:'  + ISNULL(CAST(ERROR_MESSAGE() as nvarchar(max)), '')
		RAISERROR (@errorReturnMessage, 11, 1)

		IF (XACT_STATE()) = -1
		BEGIN
			ROLLBACK TRANSACTION
		END
		IF (XACT_STATE()) = 1
		BEGIN
			ROLLBACK TRANSACTION
		END
		RAISERROR (@errorReturnMessage, 11, 1)
	END CATCH
END