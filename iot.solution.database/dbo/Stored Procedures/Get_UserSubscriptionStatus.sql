/*******************************************************************
DECLARE @output INT = 0
	,@fieldName				nvarchar(255)	

EXEC [dbo].[Get_UserSubscriptionStatus]		 
	 @guid					= '5426C420-3931-42E6-94FA-DF2BC038EE8C'
	,@invokinguser			= '5426C420-3931-42E6-94FA-DF2BC038EE8C'
	,@version				= 'v1'                         
	,@output				= @output									OUTPUT
	,@fieldname				= @fieldName								OUTPUT	

SELECT @output status, @fieldName fieldName
*******************************************************************/
CREATE PROCEDURE [dbo].[Get_UserSubscriptionStatus]
(	
	 @guid			UNIQUEIDENTIFIER 		 
	,@invokingUser	UNIQUEIDENTIFIER
	,@version		nvarchar(10)
	,@output		SMALLINT			OUTPUT    
	,@fieldName		nvarchar(100)		OUTPUT   
	,@culture		nvarchar(10)		= 'en-Us'
	,@enableDebugInfo	CHAR(1)			= '0'
)	
AS
BEGIN

	SET @enableDebugInfo = 1
	SET NOCOUNT ON
	DECLARE @dt DATETIME = GETUTCDATE()
    IF (@enableDebugInfo = 1)
	BEGIN
        DECLARE @Param XML 
        SELECT @Param = 
        (
            SELECT 'Get_UserSubscriptionStatus' AS '@procName'             
            	, CONVERT(nvarchar(MAX),@guid) AS '@guid' 
				, CONVERT(nvarchar(MAX),@invokingUser) AS '@invokingUser'
            	, CONVERT(nvarchar(MAX),@version) AS '@version' 
            	, CONVERT(nvarchar(MAX),@output) AS '@output' 
            	, @fieldName AS '@fieldName'   
            FOR XML PATH('Params')
	    ) 
	    INSERT INTO DebugInfo(data, dt) VALUES(Convert(nvarchar(MAX), @Param), @dt)
    END       
	
	DECLARE @poutput		SMALLINT
			,@pFieldName	nvarchar(100)
    
	  IF(@poutput!=1)
      BEGIN
        SET @output = @poutput
        SET @fieldName = @pfieldName
        RETURN;
      END


	SET @output = 1
	SET @fieldName = 'Success'

	BEGIN TRY
		IF EXISTS (SELECT TOP 1 1 FROM dbo.[User] (NOLOCK) WHERE [guid]= @guid AND [isdeleted]=0 and [subscriptionEndDate]<@dt)
		BEGIN
			SET @output = 0
			SET @fieldname = 'SubscriptionExpired'						
			RETURN;
		END
		ELSE
		BEGIN
			SET @output = 1
			SET @fieldname = 'Success'
			RETURN;
		END
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