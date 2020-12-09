/*******************************************************************
DECLARE @count INT
     ,@output INT = 0
	,@fieldName					nvarchar(255)
	,@syncDate	DATETIME
EXEC [dbo].[Chart_Utilization]	
	@entityGuid	= '0F6A29B2-71C0-4997-B42F-1FF078EDD24B'
	,@guid	= '0F6A29B2-71C0-4997-B42F-1FF078EDD24B'
	,@frequency = 'M'
	,@invokinguser  = 'E05A4DA0-A8C5-4A4D-886D-F61EC802B5FD'              
	,@version		= 'v1'              
	,@output		= @output		OUTPUT
	,@fieldname		= @fieldName	OUTPUT
	,@syncDate		= @syncDate		OUTPUT

SELECT @output status, @fieldName fieldName, @syncDate syncDate

001	SAM-138 16-07-2020 [Nishit Khakhi]	Added Initial Version to represent Utilization By Device
*******************************************************************/
CREATE PROCEDURE [dbo].[Chart_Utilization]
(	@entityGuid			UNIQUEIDENTIFIER	=NULL
	,@guid				UNIQUEIDENTIFIER	= NULL
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
            SELECT 'Chart_Utilization' AS '@procName' 
			, CONVERT(nvarchar(MAX),@entityGuid) AS '@entityGuid' 
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
		IF OBJECT_ID('tempdb..#weekdays') IS NOT NULL BEGIN DROP TABLE #weekdays END
		IF OBJECT_ID('tempdb..#Utilization') IS NOT NULL BEGIN DROP TABLE #Utilization END
		IF OBJECT_ID('tempdb..#finalTable') IS NOT NULL BEGIN DROP TABLE #finalTable END
		IF OBJECT_ID ('tempdb..#months') IS NOT NULL BEGIN DROP TABLE #months END
		CREATE TABLE [#months] ([date] DATE)
		CREATE TABLE #weekdays ([weekDay] NVARCHAR(20))
		CREATE TABLE #Utilization ([date] DATE, [Year] INT, [Month] INT, [name] NVARCHAR(20), [totalCount] BIGINT, [totalUtilization] BIGINT) 
		CREATE TABLE #finalTable ([date] DATE, [Year] INT, [Month] INT, [name] NVARCHAR(20), [totalCount] BIGINT, [totalUtilization] BIGINT) 

		SELECT E.[uniqueId] as [uniqueId], E.[guid] as [guid]
		INTO #ids
		FROM [dbo].[Device] E (NOLOCK)
		INNER JOIN [dbo].[Entity] En (NOLOCK) ON E.[entityGuid] = En.[guid] AND En.[isDeleted] = 0
		WHERE (E.[guid] = @guid OR En.[parentEntityGuid] = @entityGuid) AND E.isDeleted = 0 AND E.isActive = 1

		IF @frequency = 'D'
		BEGIN
			SET @endDate = @dt
			INSERT INTO #weekdays values ('00:00'),('03:00'),('06:00'),('09:00'),('12:00'),('15:00'),('18:00'),('21:00')

			INSERT INTO #Utilization([name], [totalCount], [totalUtilization])
			SELECT CASE WHEN LEN(CONVERT(NVARCHAR(2),([Hour]*3))) < 2 THEN 
							'0' + CONVERT(NVARCHAR(2),([Hour]*3)) + ':00'  
						ELSE CONVERT(NVARCHAR(2),([Hour]*3)) + ':00'  
						END 
					AS [HH],[totalCount],[utilizedCount] AS [totalUtilization]
			FROM ( 
				SELECT DATEPART(HOUR,[createdDate])/3 AS [Hour],SUM([totalCount]) AS [totalCount],SUM([utilizedCount]) AS [utilizedCount]
				FROM #ids I 
				LEFT JOIN [dbo].[DeviceAttributeUtilization] T (NOLOCK) ON T.[deviceGuid] = I.[guid]
				WHERE CONVERT(Date,[createdDate]) = CONVERT(DATE,@dt) 
				GROUP BY DATEPART(HOUR,[createdDate])/3
				) [data]
			
		INSERT INTO #finalTable([name])
		SELECT [weekDay]
		FROM #weekDays

		UPDATE F
		SET [totalCount] = E.[totalCount]
			, [totalUtilization] = E.[totalUtilization]
		FROM #finalTable F
		LEFT JOIN #Utilization E ON E.[name] = F.[name]
		
		SELECT [name], CASE WHEN [totalCount] > 0 THEN ISNULL(([totalUtilization] * 100 / [totalCount]),0)
					ELSE 0
					END AS [utilizationPer]
		FROM #finalTable
		ORDER BY 
				CASE [name] 
						WHEN '00:00' THEN 1
						WHEN '03:00' THEN 2
						WHEN '06:00' THEN 3
						WHEN '09:00' THEN 4
						WHEN '12:00' THEN 5
						WHEN '15:00' THEN 6
						WHEN '18:00' THEN 7
						WHEN '21:00' THEN 8
						ELSE 9
				END 

		END
		ELSE IF @frequency = 'W'
		BEGIN
			SET @endDate = DATEADD(DAY,-7,@dt)
			
			INSERT INTO [#months]
			SELECT CONVERT(DATE, DATEADD(DAY, (T.i - 6), @dt)) AS [Date]
			FROM (VALUES (6), (5), (4), (3), (2), (1), (0)) AS T(i)

			INSERT INTO #Utilization([date], [totalCount], [totalUtilization])
			SELECT CONVERT(DATE,[createdDate]) AS [Day],SUM([totalCount]) AS [totalCount],SUM([utilizedCount]) AS [utilizedCount]
				FROM #ids I 
				LEFT JOIN [dbo].[DeviceAttributeUtilization] T (NOLOCK) ON T.[deviceGuid] = I.[guid]
				WHERE CONVERT(Date,[createdDate]) BETWEEN CONVERT(DATE,@endDate) AND CONVERT(DATE,@dt)
				GROUP BY CONVERT(DATE,[createdDate])

			SELECT CONCAT(DATENAME(day, M.[date]), ' - ', FORMAT( M.[date], 'ddd')) AS [name]
				, CASE WHEN [totalCount] > 0 THEN ISNULL(([totalUtilization] * 100 / [totalCount]),0)
					ELSE 0
					END AS [utilizationPer]
			FROM [#months] M
			LEFT OUTER JOIN #Utilization R ON R.[date] = M.[date]
			ORDER BY  M.[date]
		END
		ELSE
		BEGIN
			SET @endDate = DATEADD(YEAR,-1,@dt)
			
			INSERT INTO [#months]
			SELECT CONVERT(DATE, DATEADD(Month, (T.i - 11), @dt)) AS [Date]
			FROM (VALUES (11), (10), (9), (8), (7), (6), (5), (4), (3), (2), (1), (0)) AS T(i)

			INSERT INTO #Utilization([Year],[Month],[totalCount],[totalUtilization])
			SELECT DATEPART(YY,[createdDate]) AS [Year], DATEPART(MM,[createdDate]) AS [Month],SUM([totalCount]) AS [totalCount],SUM([utilizedCount]) AS [utilizedCount]
			FROM #ids I 
			LEFT JOIN [dbo].[DeviceAttributeUtilization] T (NOLOCK) ON T.[deviceGuid] = I.[guid]
			WHERE CONVERT(Date,[createdDate]) BETWEEN CONVERT(DATE,@endDate) AND CONVERT(DATE,@dt)
			GROUP BY DATEPART(YY,[createdDate]), DATEPART(MM,[createdDate]) 
			
			SELECT SUBSTRING(DATENAME(MONTH, M.[date]), 1, 3) + '-' + FORMAT(M.[date],'yy') AS [name]
				, CASE WHEN [totalCount] > 0 THEN ISNULL(([totalUtilization] * 100 / [totalCount]),0)
					ELSE 0
					END AS [utilizationPer]
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