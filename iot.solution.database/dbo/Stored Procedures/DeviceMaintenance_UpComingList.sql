/*******************************************************************
DECLARE @count INT
     	,@output INT = 0
		,@fieldName	VARCHAR(255)

EXEC [dbo].[DeviceMaintenance_UpComingList]
	 @guid	= 'FA973382-0321-4701-A03E-CDDEAEC9F68B'
	,@currentDate	= '2020-05-21 06:47:56.890'
	,@count			= @count OUTPUT
	,@invokingUser  = 'C1596B8C-7065-4D63-BFD0-4B835B93DFF2'
	,@version		= 'v1'
	,@output		= @output	OUTPUT
	,@fieldName		= @fieldName	OUTPUT

SELECT @count count, @output status, @fieldName fieldName

001	SR-1	28-04-2020 [Nishit Khakhi]	Added Initial Version to List Upcoming Device Maintenance
*******************************************************************/
CREATE PROCEDURE [dbo].[DeviceMaintenance_UpComingList]
(	@companyGuid		UNIQUEIDENTIFIER	= NULL
	,@entityGuid		UNIQUEIDENTIFIER	= NULL
	,@guid				UNIQUEIDENTIFIER	= NULL
	,@currentDate		DATETIME			= NULL
	,@invokingUser		UNIQUEIDENTIFIER	= NULL
	,@version			VARCHAR(10)
	,@culture			VARCHAR(10)			= 'en-Us'
	,@output			SMALLINT			OUTPUT
	,@fieldName			VARCHAR(255)		OUTPUT
	,@count				INT OUTPUT
	,@enableDebugInfo	CHAR(1)			= '0'
)
AS
BEGIN
    SET NOCOUNT ON

    IF (@enableDebugInfo = 1)
	BEGIN
        DECLARE @Param XML
        SELECT @Param =
        (
            SELECT 'DeviceMaintenance_UpComingList' AS '@procName'
            	, CONVERT(VARCHAR(MAX),@companyGuid) AS '@companyGuid'
				, CONVERT(VARCHAR(MAX),@entityGuid) AS '@entityGuid'
            	, CONVERT(VARCHAR(MAX),@guid) AS '@guid'
				, CONVERT(VARCHAR(50),@currentDate) as '@currentDate'
				, CONVERT(VARCHAR(MAX),@version) AS '@version'
            	, CONVERT(VARCHAR(MAX),@invokingUser) AS '@invokingUser'
            FOR XML PATH('Params')
	    )
	    INSERT INTO DebugInfo(data, dt) VALUES(Convert(VARCHAR(MAX), @Param), GETDATE())
    END
    
    BEGIN TRY

		SELECT TOP 10
				EM.[guid]
				, EM.[companyGuid]
				, EM.[entityGuid]
				, EM.[deviceGuid]				
				, G.[fleetId] AS [deviceName]
				, EM.[description]
				, EM.[startDateTime]
				, EM.[endDateTime]
			FROM [dbo].[DeviceMaintenance] EM WITH (NOLOCK) 			
			INNER JOIN [dbo].[Fleet] G WITH (NOLOCK) ON EM.[deviceGuid] = G.[guid] AND G.[isDeleted] = 0
			WHERE EM.[isDeleted] = 0 
			AND EM.[companyGuid]= ISNULL(@companyGuid,EM.[companyGuid])			
			AND EM.[deviceGuid]= ISNULL(@guid,EM.[deviceGuid])
			AND CONVERT(DATETIME,[startDateTime]) >= CONVERT(DATETIME,@currentDate)
			AND ISNULL(EM.[isCompleted],0)=0 
			ORDER BY [startDateTime] ASC

			SET @count = @@ROWCOUNT

        SET @output = 1
		SET @fieldName = 'Success'
	END TRY	
	BEGIN CATCH	
		DECLARE @errorReturnMessage VARCHAR(MAX)

		SET @output = 0

		SELECT @errorReturnMessage = 
			ISNULL(@errorReturnMessage, '') +  SPACE(1)   + 
			'ErrorNumber:'  + ISNULL(CAST(ERROR_NUMBER() as VARCHAR), '')  + 
			'ErrorSeverity:'  + ISNULL(CAST(ERROR_SEVERITY() as VARCHAR), '') + 
			'ErrorState:'  + ISNULL(CAST(ERROR_STATE() as VARCHAR), '') + 
			'ErrorLine:'  + ISNULL(CAST(ERROR_LINE () as VARCHAR), '') + 
			'ErrorProcedure:'  + ISNULL(CAST(ERROR_PROCEDURE() as VARCHAR), '') + 
			'ErrorMessage:'  + ISNULL(CAST(ERROR_MESSAGE() as VARCHAR(max)), '')
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