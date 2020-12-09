/*******************************************************************
DECLARE @output INT = 0
	,@fieldName				nvarchar(255)	

EXEC [dbo].[Entity_Delete]		 
	@companyguid	= '870D412C-3100-4147-B202-8D523621574C'
	,@guid			= '870D412C-3100-4147-B202-8D523621574C'
	,@invokinguser	= '200EDCFA-8FF1-4837-91B1-7D5F967F5129'
	,@version		= 'v1'                         
	,@output		= @output									OUTPUT
	,@fieldname		= @fieldName								OUTPUT	

SELECT @output status, @fieldName fieldName

001	sgh-1 05-12-2019 [Nishit Khakhi]	Added Initial Version to Delete Entity
*******************************************************************/
CREATE PROCEDURE [dbo].[Entity_Delete]
(	 
	@companyguid		UNIQUEIDENTIFIER
	,@guid				UNIQUEIDENTIFIER
	,@invokinguser		UNIQUEIDENTIFIER
	,@version			nvarchar(10)              
	,@output			SMALLINT			OUTPUT
	,@fieldname			nvarchar(255)		OUTPUT
	,@culture			nvarchar(10)		= 'en-Us'	
	,@enabledebuginfo	CHAR(1)				= '0'
)
AS
BEGIN
    SET NOCOUNT ON

    IF (@enabledebuginfo = 1)
	BEGIN
        DECLARE @Param XML 
        SELECT @Param = 
        (
            SELECT 'Entity_Delete' AS '@procName'             
			, CONVERT(nvarchar(MAX),@companyguid) AS '@companyguid'
			, CONVERT(nvarchar(MAX),@guid) AS '@guid'
			, CONVERT(nvarchar(MAX),@invokinguser) AS '@invokinguser'
			, CONVERT(nvarchar(MAX),@version) AS '@version'
            FOR XML PATH('Params')
	    ) 
	    INSERT INTO DebugInfo(data, dt) VALUES(Convert(nvarchar(MAX), @Param), GETUTCDATE())
    END                    
    
    BEGIN TRY            
        SET @output = 1
		SET @fieldname = 'Success'
		
		IF NOT EXISTS (SELECT TOP 1 1 FROM [dbo].[Entity] (NOLOCK) WHERE [companyGuid] = @companyguid AND [guid] = @guid AND [isdeleted] = 0)
		BEGIN
			SET @output = -3
			SET @fieldname = 'EntityNotFound'
			RETURN;
		END

		BEGIN TRAN
			UPDATE [dbo].[Entity]
			SET [isdeleted] = 1
				, isactive = 0
				, updatedby = @invokinguser 
				, updateddate = GETUTCDATE()
			WHERE [companyGuid] = @companyguid AND [guid] = @guid AND [isdeleted] = 0

		COMMIT
			
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