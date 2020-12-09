/*******************************************************************
DECLARE @count INT
     ,@output INT = 0
	,@fieldName				nvarchar(255)
EXEC [dbo].[Device_Get]
	 @guid				= '3141C24F-81E5-48D2-BD55-2488117C0FDC'	
	,@companyGuid		= 'DC4B1A8B-38D8-4431-83D0-933DE2DD4324'	
	,@invokingUser  	= '7D31E738-5E24-4EA2-AAEF-47BB0F3CCD41'
	,@version			= 'v1'
	,@output			= @output		OUTPUT
	,@fieldName			= @fieldName	OUTPUT	
               
 SELECT @output status,  @fieldName AS fieldName    
 
 001	sgh-1	06-12-2019 [Nishit Khakhi]	Added Initial Version to Get Device Information
 002	SAM-138	06/07/2020 [Nishit Khakhi]	Updated to return DeviceType, Attributes details, Sensor fields
*******************************************************************/

CREATE PROCEDURE [dbo].[Device_Get]
(	 
	 @guid				UNIQUEIDENTIFIER	
	,@companyGuid		UNIQUEIDENTIFIER
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
            SELECT 'Device_Get' AS '@procName'
			, CONVERT(nvarchar(MAX),@guid) AS '@guid'			
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

    IF NOT EXISTS (SELECT TOP 1 1 FROM [dbo].[Device] (NOLOCK) WHERE [companyGuid]=@companyGuid AND [guid]=@guid AND [isDeleted]=0)
	BEGIN
		Set @output = -3
		SET @fieldName = 'DeviceNotFound'
		RETURN;
	END
  
    BEGIN TRY
		SELECT D.[guid]
				,D.[companyGuid]
				,D.[entityGuid]
				,EP.[guid] AS [parentEntityGuid]
				,D.[templateGuid]
				--, KT.[kitCode] AS [kitCode]
				,D.[parentDeviceGuid]
				,D.[typeGuid]
				,D.[uniqueId]
				,D.[name]
				--,D.[note]
				--,D.[tag]
				--,D.[image]
				,D.[specification]
				,D.[description]
				,D.[isProvisioned]
				,D.[isActive]
				,D.[fleetGuid]
				
				--,D.[createdDate]
				--,D.[createdBy]
				--,D.[updatedDate]
				--,D.[updatedBy]
				, F.[fleetId] As [fleetId] 
		FROM [dbo].[Device] D (NOLOCK)
		INNER JOIN [dbo].[Entity] E (NOLOCK) ON D.[entityGuid] = E.[guid] AND E.[isDeleted] = 0
		LEFT JOIN [dbo].[Entity] EP (NOLOCK) ON E.[parentEntityGuid] = EP.[guid] AND EP.[isDeleted] = 0
		LEFT JOIN [dbo].[Fleet] F With (NOLOCK) ON D.[fleetGuid] = F.[guid] and F.[isDeleted]=0 
		LEFT JOIN [dbo].[DeviceType] DT (NOLOCK) ON D.[typeGuid] = DT.[guid] AND DT.[isDeleted] = 0 
		WHERE D.[companyGuid]=@companyGuid AND D.[guid]=@guid AND D.[isDeleted]=0

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