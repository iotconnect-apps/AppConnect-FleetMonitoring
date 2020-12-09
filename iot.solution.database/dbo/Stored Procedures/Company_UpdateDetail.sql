
CREATE PROCEDURE [dbo].[Company_UpdateDetail]
	@companyGuid			UNIQUEIDENTIFIER
	,@name					nvarchar(100)
	,@contactNo				nvarchar(25)		= NULL	
	,@address				nvarchar(500)		= NULL		
	,@countryGuid			UNIQUEIDENTIFIER	= NULL
	,@stateGuid				UNIQUEIDENTIFIER	= NULL
	,@city					nvarchar(50)		= NULL	
	,@postalCode			nvarchar(30)		= NULL	
	,@timezoneGuid			UNIQUEIDENTIFIER	= NULL
	,@invokinguser			UNIQUEIDENTIFIER		
	,@version				nvarchar(10)    
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
			, CONVERT(nvarchar(MAX),@address) AS '@address'	
			, CONVERT(nvarchar(MAX),@stateGuid) AS '@stateGuid'									
			, CONVERT(nvarchar(MAX),@countryGuid) AS '@countryGuid' 
			, CONVERT(nvarchar(MAX),@timezoneGuid) AS '@timezoneGuid' 
			, @city AS '@city' 			
			, @postalCode AS '@postalCode' 			
			, @contactNo AS '@contactNo' 			
			, CONVERT(nvarchar(MAX),@companyGuid) AS '@companyGuid' 
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
		
	BEGIN TRAN	
		
		UPDATE [dbo].[Company] 
			SET [name] = @name
				,[contactNo] = @contactNo				
				,[address] = @address								
				,[countryGuid] = @countryGuid
				,[stateGuid] = @stateGuid
				,[city] = @city
				,[postalCode] = @postalCode
				,[timezoneGuid] = @timezoneGuid 
				,[updatedby] = @invokinguser
				,[updateddate] = @dt				
			WHERE [guid] = @companyGuid AND isDeleted = 0 
	  
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