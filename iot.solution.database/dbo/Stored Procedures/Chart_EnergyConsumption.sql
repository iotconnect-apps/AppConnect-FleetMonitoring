/*******************************************************************
DECLARE @count INT
     ,@output INT = 0
	,@fieldName					nvarchar(255)	
	,@syncDate	DATETIME
EXEC [dbo].[Chart_EnergyConsumption]
	@guid	= '8DD0600B-A6A0-4437-8AF3-7891039AB95D'	
	,@frequency		='M'				
	,@invokinguser  = 'E05A4DA0-A8C5-4A4D-886D-F61EC802B5FD'              
	,@version		= 'v1'              
	,@output		= @output		OUTPUT
	,@fieldname		= @fieldName	OUTPUT	
	,@syncDate		= @syncDate		OUTPUT
SELECT @output status, @fieldName fieldName, @syncDate syncDate


*******************************************************************/
CREATE PROCEDURE [dbo].[Chart_EnergyConsumption]
(	@guid				UNIQUEIDENTIFIER	
	,@frequency			CHAR(1)		='M'		
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
            SELECT 'Chart_EnergyConsumption' AS '@procName' 
            , CONVERT(nvarchar(MAX),@guid) AS '@guid' 			
			, CONVERT(nvarchar(MAX),@version) AS '@version' 
            , CONVERT(nvarchar(MAX),@invokinguser) AS '@invokinguser' 
            FOR XML PATH('Params')
	    ) 
	    INSERT INTO DebugInfo(data, dt) VALUES(Convert(nvarchar(MAX), @Param), GETUTCDATE())
    END                    
    
    BEGIN TRY  
		DECLARE @dt DATETIME = GETUTCDATE(), @endDate DATETIME
		IF OBJECT_ID ('tempdb..#ids') IS NOT NULL DROP TABLE #ids
		IF OBJECT_ID ('tempdb..#months') IS NOT NULL BEGIN DROP TABLE #months END
		IF OBJECT_ID('tempdb..#Utilization') IS NOT NULL BEGIN DROP TABLE #Utilization END
		IF OBJECT_ID('tempdb..#finalTable') IS NOT NULL BEGIN DROP TABLE #finalTable END
		CREATE TABLE [#months] ([date] DATE)
		CREATE TABLE #Utilization ([date] DATE, [Year] INT, [Month] INT, [name] NVARCHAR(20), [totalCount] BIGINT, [totalUtilization] BIGINT) 
		CREATE TABLE #finalTable ([date] DATE, [Year] INT, [Month] INT, [name] NVARCHAR(20), [totalCount] BIGINT, [totalUtilization] BIGINT) 

		SELECT E.[uniqueId] as [uniqueId], E.[guid] as [guid] 
		INTO #ids
		FROM [dbo].[Device] E (NOLOCK) 
		INNER JOIN [dbo].[Fleet] F (NOLOCK) ON F.[Guid]=E.[fleetGuid] 
		INNER JOIN [dbo].[Trip] T (NOLOCK) ON E.[fleetGuid]=T.[fleetGuid] 
		WHERE E.[companyGuid] = @guid  AND T.[isDeleted]=0 AND E.[isDeleted] = 0 AND F.[isDeleted]=0
		AND T.[isStarted]=1 

		IF @frequency = 'M'
		BEGIN
			SET @endDate = DATEADD(YEAR,-1,@dt)
			
			INSERT INTO [#months]
			SELECT CONVERT(DATE, DATEADD(Month, (T.i - 11), @dt)) AS [Date]
			FROM (VALUES (11), (10), (9), (8), (7), (6), (5), (4), (3), (2), (1), (0)) AS T(i)

			INSERT INTO #Utilization([Year],[Month],[totalCount],[totalUtilization])
			SELECT DATEPART(YY,[date]) AS [Year], DATEPART(MM,[date]) AS [Month],0 AS [totalCount],SUM([sum]) AS [value] 
			FROM #ids I 
			LEFT JOIN [dbo].[TelemetrySummary_Hourwise]  T (NOLOCK) ON T.[deviceGuid] = I.[guid]
			WHERE CONVERT(Date,[date]) BETWEEN CONVERT(DATE,@endDate) AND CONVERT(DATE,@dt)
			GROUP BY DATEPART(YY,[date]), DATEPART(MM,[date]) 
			
			SELECT SUBSTRING(DATENAME(MONTH, M.[date]), 1, 3) + '-' + FORMAT(M.[date],'yy') AS [name]
				, ISNULL([totalUtilization],0) AS [EnergyConsumption]
			FROM [#months] M
			LEFT OUTER JOIN #Utilization R ON R.[Month] = DATEPART(MM, M.[date]) AND R.[Year] = DATEPART(YY, M.[date]) 
			ORDER BY  M.[date]
		END

							
			
			
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