/*******************************************************************
DECLARE @count INT
     ,@output INT = 0
	,@fieldName					nvarchar(255)
	,@syncDate	DATETIME
EXEC [dbo].[Chart_UtilizationByCompany]	
	@companyGuid = 'FB75BA99-7AB0-4275-9661-18D00F2C4A3E'
	,@frequency = 'm'
	,@invokinguser  = 'E05A4DA0-A8C5-4A4D-886D-F61EC802B5FD'              
	,@version		= 'v1'              
	,@output		= @output		OUTPUT
	,@fieldname		= @fieldName	OUTPUT
	,@syncDate		= @syncDate		OUTPUT

SELECT @output status, @fieldName fieldName, @syncDate syncDate

001	SAM-138 13-07-2020 [Nishit Khakhi]	Added Initial Version to represent Utilization by Company
*******************************************************************/
CREATE PROCEDURE [dbo].[Chart_UtilizationByCompany]
(	@companyGuid		UNIQUEIDENTIFIER		
	,@frequency			CHAR(1)				
	,@invokinguser		UNIQUEIDENTIFIER	= NULL
	,@version			nvarchar(10)              
	,@output			SMALLINT			OUTPUT
	,@fieldname			nvarchar(255)		OUTPUT
	,@syncDate			DATETIME			OUTPUT
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
            SELECT 'Chart_UtilizationByCompany' AS '@procName' 
            , CONVERT(nvarchar(MAX),@companyGuid) AS '@companyGuid' 
			, CONVERT(nvarchar(MAX),@version) AS '@version' 
            , CONVERT(nvarchar(MAX),@invokinguser) AS '@invokinguser' 
            FOR XML PATH('Params')
	    ) 
	    INSERT INTO DebugInfo(data, dt) VALUES(Convert(nvarchar(MAX), @Param), GETUTCDATE())
    END                    
    
    BEGIN TRY  
		DECLARE @dt DATETIME = GETUTCDATE(), @endDate DATETIME
		IF OBJECT_ID ('tempdb..#ids') IS NOT NULL DROP TABLE #ids
	
		SELECT E.[guid] as [guid]
		INTO #ids
		FROM [dbo].[Device] E (NOLOCK) 
		WHERE E.[companyGuid] = @companyGuid AND E.isDeleted = 0 AND E.isActive = 1

		SET @endDate = @dt

		IF @frequency = 'W'
		BEGIN
			SET @dt = DATEADD(DAY,-7,@dt)
		END
		ELSE IF @frequency = 'M'
		BEGIN
			SET @dt = DATEADD(MONTH,-1,@dt)
		END
		ELSE
		BEGIN
			SET @dt = DATEADD(YEAR,-1,@dt)
		END
		
		SELECT CASE WHEN [totalCount] > 0 THEN ISNULL(([utilizedCount] * 100 / [totalCount]),0)
					ELSE 0
					END AS [utilizationPer]
		FROM (
		SELECT SUM(DAU.[totalCount]) AS [totalCount], SUM(DAU.[utilizedCount]) AS [utilizedCount]
		FROM #ids I 
		LEFT JOIN [dbo].[DeviceAttributeUtilization] DAU (NOLOCK) ON I.[guid] = DAU.[deviceGuid]
			AND CONVERT(Date,DAU.[createdDate]) BETWEEN CONVERT(DATE,@dt) AND CONVERT(DATE,@endDate)
		) A

		SET @output = 1
		SET @fieldname = 'Success'
		
		SET @syncDate = (SELECT TOP 1 CONVERT(DATETIME,[value]) FROM dbo.[Configuration] (NOLOCK) WHERE [configKey] = 'telemetry-last-exectime')
              
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