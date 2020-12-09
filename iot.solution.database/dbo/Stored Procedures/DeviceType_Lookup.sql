/*******************************************************************
DECLARE 
     @output INT = 0
	,@fieldName				nvarchar(255)
EXEC [dbo].[DeviceType_Lookup]		
	@companyGuid		= 'DC4B1A8B-38D8-4431-83D0-933DE2DD4324'	
	,@entityGuid		= 'D7ABEA1B-5682-42D7-908F-4574615619F4'	
	,@invokingUser  	= '7D31E738-5E24-4EA2-AAEF-47BB0F3CCD41'
	,@version			= 'v1'
	,@output			= @output		OUTPUT
	,@fieldName			= @fieldName	OUTPUT	
               
SELECT @output status,  @fieldName AS fieldName 

001	SAM-138 06-07-2020 [Nishit Khakhi]	Added Initial Version to Lookup Device Type
*******************************************************************/

CREATE PROCEDURE [dbo].[DeviceType_Lookup]
(	 @companyGuid		UNIQUEIDENTIFIER
	,@entityGuid		UNIQUEIDENTIFIER	= NULL
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
	
	IF (@enableDebugInfo = 1)
	BEGIN
        DECLARE @Param XML
        SELECT @Param =
        (
            SELECT 'DeviceType_Lookup' AS '@procName'			
			, CONVERT(nvarchar(MAX),@companyGuid) AS '@companyGuid'
			, CONVERT(nvarchar(MAX),@entityGuid) AS '@entityGuid'
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

    IF NOT EXISTS (SELECT TOP 1 1 FROM [dbo].[DeviceType] (NOLOCK) WHERE companyGuid=@companyGuid AND [entityGuid] = ISNULL(@entityGuid,[entityGuid]) AND [isDeleted]=0)
	BEGIN
		Set @output = -3
		SET @fieldName = 'DeviceTypeNotFound'
		RETURN;
	END
  
    BEGIN TRY
		SELECT [guid]
			  ,[name]
			  ,[templateGuid]
		FROM [dbo].[DeviceType] (NOLOCK) 
		WHERE [companyguid]=@companyGuid AND [entityGuid] = ISNULL(@entityGuid,[entityGuid]) AND [isDeleted] = 0 
		ORDER BY [name]

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