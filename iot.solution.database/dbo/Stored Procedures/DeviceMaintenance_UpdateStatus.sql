/*******************************************************************
BEGIN TRAN

DECLARE @output INT = 0	
	,@fieldname nvarchar(100)
   
EXEC [dbo].[DeviceMaintenance_UpdateStatus]	 
	@guid			= '651B311A-40D7-4304-B3C3-7C2DC4E8F505'	
	,@companyGuid	= '651B311A-40D7-4304-B3C3-7C2DC4E8F505'	
	,@startDateTime		= '06:00:00'
	,@endDateTime		= '06:00:00'
	,@invokinguser 	= 'FF221908-486A-4E5A-843A-C68EB413F6EA'
	,@output		= @output OUTPUT		
	,@version		= 'v1'
	,@fieldname		= @fieldname OUTPUT

SELECT @output status, @fieldname fieldName

ROLLBACK TRAN

001	SR-4 28-04-2020 [Nishit Khakhi]	Added Initial Version to Update Device Maintenance Status
*******************************************************************/
CREATE PROCEDURE [dbo].[DeviceMaintenance_UpdateStatus]
		@guid					UNIQUEIDENTIFIER
		,@companyGuid			UNIQUEIDENTIFIER	
		,@startDateTime			DATETIME				= NULL				
		,@endDateTime			DATETIME				= NULL
		,@description			NVARCHAR(1000)		= NULL
		,@invokinguser			UNIQUEIDENTIFIER	= NULL
		,@version				nvarchar(10)    
		,@output				SMALLINT			OUTPUT    
		,@fieldname				nvarchar(100)		OUTPUT   
		,@culture				nvarchar(10)		= 'en-Us'
		,@enabledebuginfo		CHAR(1)				= '0'
AS
BEGIN
	SET NOCOUNT ON

    IF (@enabledebuginfo = 1)
	BEGIN
        DECLARE @Param XML 
        SELECT @Param = 
        (
            SELECT 'DeviceMaintenance_UpdateStatus' AS '@procName' 
            , CONVERT(nvarchar(MAX),@guid) AS '@guid' 
			, CONVERT(nvarchar(MAX),@companyGuid) AS '@companyGuid' 		
            , @description AS '@description' 
			, CONVERT(nvarchar(100),@startDateTime) AS '@startDateTime'
			, CONVERT(nvarchar(100),@endDateTime) AS '@endDateTime'
            , CONVERT(nvarchar(MAX),@invokinguser) AS '@invokinguser' 
            , CONVERT(nvarchar(MAX),@version) AS '@version' 
            , CONVERT(nvarchar(MAX),@output) AS '@output' 
            , CONVERT(nvarchar(MAX),@fieldname) AS '@fieldname'   
            FOR XML PATH('Params')
	    ) 
	    INSERT INTO DebugInfo(data, dt) VALUES(Convert(nvarchar(MAX), @Param), GETUTCDATE())
    END       
	
	DECLARE @dt DATETIME = GETUTCDATE()

	SET @output = 1
	SET @fieldname = 'Success'

	BEGIN TRY		
		IF NOT EXISTS (SELECT TOP 1 1 FROM [dbo].[DeviceMaintenance] (NOLOCK) WHERE [guid] = @guid AND [companyGuid] = @companyGuid AND [isDeleted] = 0)
		BEGIN
			SET @output = -2
			SET @fieldname = 'DeviceMaintenanceNotFound'
			RETURN;
		END    
		
		BEGIN TRAN

			UPDATE [dbo].[DeviceMaintenance] 
			SET [description] = ISNULL(@description,[description])
				,[startDateTime] = ISNULL(@startDateTime,[startDateTime])
				,[endDateTime] = ISNULL(@endDateTime,[endDateTime])
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