/*******************************************************************
DECLARE @output INT = 0
	,@fieldName				nvarchar(255)	

EXEC [dbo].[Trip_Delete]		 
	@companyGuid			= '74B126BE-1139-4135-8766-3E56A0125D09'
	,@guid					= '12408268-EDFE-4B21-8C7C-374B5AC0E66A'
	,@invokinguser			= 'C1596B8C-7065-4D63-BFD0-4B835B93DFF2'
	,@version				= 'v1'                         
	,@output				= @output									OUTPUT
	,@fieldname				= @fieldName								OUTPUT	

SELECT @output status, @fieldName fieldName
*******************************************************************/
CREATE PROCEDURE [dbo].[Trip_Delete]
(	
     @companyguid UNIQUEIDENTIFIER 
	,@guid			UNIQUEIDENTIFIER 		 
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
            SELECT 'Trip_Delete' AS '@procName'             
            	, CONVERT(nvarchar(MAX),@companyguid) AS '@companyguid' 
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
		IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.[Trip] (NOLOCK) WHERE [guid]= @guid and [companyguid] = @companyguid AND [isdeleted]=0)
		BEGIN
			SET @output = -2
			SET @fieldname = 'TripNotFound'
			RETURN;
		END
			
		IF EXISTS (SELECT TOP 1 1 FROM dbo.[Trip] (NOLOCK) WHERE [isStarted]= 1 AND [isCompleted]=0 and [guid]=@guid)
		BEGIN
			SET @output = -1
			SET @fieldname = 'OnGoingTripExists'						
			RETURN;
		END
		
	BEGIN TRAN	
			UPDATE dbo.[TripStops]
			SET  [isDeleted]=1
				,[updatedDate]	= @dt
				,[updatedBy]	= @invokingUser			
			WHERE
				[tripGuid] = @guid
				AND [isDeleted] = 0

			UPDATE dbo.[Trip]
			SET  [isDeleted]=1
				,[updatedDate]	= @dt
				,[updatedBy]	= @invokingUser			
			WHERE
				[guid] = @guid
				AND [companyGuid] = @companyGuid
				AND [isDeleted] = 0
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