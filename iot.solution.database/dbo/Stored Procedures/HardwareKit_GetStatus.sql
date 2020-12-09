
/*******************************************************************
DECLARE @output INT = 0
	,@fieldName				nvarchar(255)	

EXEC [dbo].[HardwareKit_GetStatus]	
	@kitCode		= 'Elevator Kit 007'
	,@uniqueId		= 'ElevatorKit007'
	,@invokinguser  = '200EDCFA-8FF1-4837-91B1-7D5F967F5129'   
	,@version		= 'v1'              
	,@output		= @output		OUTPUT
	,@fieldname		= @fieldName	OUTPUT	

SELECT @output status, @fieldName fieldName

001	sgh-97 28-01-2020 [Nishit Khakhi]	Added Initial Version to Get Device Status
*******************************************************************/
CREATE PROCEDURE [dbo].[HardwareKit_GetStatus]
(	@kitCode			NVARCHAR(50) 
	,@uniqueId			NVARCHAR(MAX) 
	,@invokingUser		UNIQUEIDENTIFIER	= NULL
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
            SELECT 'HardwareKit_GetStatus' AS '@procName'
			, @kitCode AS '@kitCode'
			, @uniqueId AS '@uniqueId'
            , CONVERT(nvarchar(MAX),@invokingUser) AS '@invokingUser'
			, CONVERT(nvarchar(MAX),@version) AS '@version'
			, CONVERT(nvarchar(MAX),@output) AS '@output'
            , CONVERT(nvarchar(MAX),@fieldName) AS '@fieldName'
            FOR XML PATH('Params')
	    )
	    INSERT INTO DebugInfo(data, dt) VALUES(Convert(nvarchar(MAX), @Param), GETUTCDATE())
    END
    
	BEGIN TRY
		IF OBJECT_ID ('tempdb..#ids') IS NOT NULL DROP TABLE #ids

		SELECT [value] as [uniqueId]
		INTO #ids
		FROM string_split(@uniqueId,',')

		IF EXISTS (SELECT TOP 1 1
				   FROM #ids 
				   WHERE [uniqueId] NOT IN (SELECT [uniqueId] FROM [dbo].[HardwareKit] (NOLOCK) AS KT WHERE KT.[kitCode] = @kitCode AND KT.[isDeleted] = 0)
				   )
		BEGIN
			SET @output = 1
			SET @fieldName = 'Failed'
			RETURN;
		END		
		ELSE
		BEGIN
			SET @output = 1
			SET @fieldName = 'Success'	

			SELECT KD.* , KD.[guid] AS TemplateGuid, KT.[name] AS [templateName]
			FROM [dbo].[HardwareKit] (NOLOCK) KD
				JOIN [KitType] (NOLOCK) KT ON KT.[guid] = KD.[kitTypeGuid]
			WHERE KD.[kitCode] = @kitCode AND KD.[isActive] = 1 AND KD.[isDeleted] = 0
		END

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