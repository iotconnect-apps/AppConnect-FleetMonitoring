/*******************************************************************

EXEC [dbo].[TelemetrySummary_HourWise_Add]	
	

*******************************************************************/
CREATE PROCEDURE [dbo].[TelemetrySummary_HourWise_Add]
AS
BEGIN
	SET NOCOUNT ON
	
	

BEGIN TRY
	DECLARE @dt DATETIME = GETUTCDATE(), @lastExecDate DATETIME
	SELECT 
		TOP 1 @lastExecDate = CONVERT(DATETIME,[value]) 
	FROM [dbo].[Configuration] 
	WHERE [configKey] = 'telemetry-last-exectime' AND [isDeleted] = 0

	BEGIN TRAN		
		
		IF OBJECT_ID ('tempdb..#CTE_Attribute_CurrentSpeed') IS NOT NULL DROP TABLE #CTE_Attribute_CurrentSpeed
		IF OBJECT_ID ('tempdb..#CTE_Attribute_Enginerpm') IS NOT NULL DROP TABLE #CTE_Attribute_Enginerpm
		IF OBJECT_ID ('tempdb..#CTE_Attribute_EnginerpmNo') IS NOT NULL DROP TABLE #CTE_Attribute_EnginerpmNo
		IF OBJECT_ID ('tempdb..#CTE_Attribute_EnginerpmHalt') IS NOT NULL DROP TABLE #CTE_Attribute_EnginerpmHalt
		UPDATE [IOTConnect].[AttributeValue] SET [attributeValue]=REPLACE([attributeValue],',','') 

		
			SELECT Dr.[guid],T.[guid] as [tripGuid],A.[uniqueId], A.[localName], A.[createdDate],F.[speedLimit], ROUND(CONVERT(DECIMAL(18,7),[attributeValue]),2) AS [attributeValue], ROW_NUMBER() OVER (PARTITION BY Dr.[guid],T.[guid],A.[uniqueId],A.[localName] ORDER BY A.[createdDate] DESC) AS [no] 
			INTO #CTE_Attribute_CurrentSpeed
				FROM [IOTConnect].[AttributeValue] A (NOLOCK)
				INNER JOIN [dbo].[Device] D (NOLOCK) ON D.[uniqueId]=A.[uniqueId] AND D.[isDeleted]=0 
				INNER JOIN [dbo].[Fleet] F (NOLOCK) ON F.[guid]=D.[fleetGuid] AND F.[isDeleted]=0 
				INNER JOIN [dbo].[Driver] Dr (NOLOCK) ON Dr.[fleetGuid]=F.[guid] AND Dr.[isDeleted]=0 
				INNER JOIN [dbo].[Trip] T (NOLOCK) ON T.[fleetGuid]=F.[guid] AND T.[isDeleted]=0 
				WHERE A.[localName] = 'can_vehicle_speed' AND  CONVERT(DECIMAL(18,7),A.[attributeValue])>0 AND T.[isStarted]=1 AND A.[createdDate]>=T.[actualStartDateTime] 
						AND (CONVERT(DATE,A.[createdDate]) BETWEEN CONVERT(DATE,@lastExecDate) AND CONVERT(DATE,@dt))
			
				SELECT D.[guid],T.[guid] as [tripGuid],Dr.[guid] As [driverGuid],F.[companyGuid] as [companyGuid],F.[guid] as [fleetGuid],A.[uniqueId], A.[localName], A.[createdDate], 
			ROUND(CONVERT(DECIMAL(18,7),[attributeValue]),2) AS [attributeValue],
			 ROW_NUMBER() OVER (PARTITION BY D.[guid],T.[guid],A.[uniqueId],A.[localName] ORDER BY A.[createdDate] DESC) AS [no]
					INTO #CTE_Attribute_Enginerpm
				FROM [IOTConnect].[AttributeValue] A (NOLOCK)
				INNER JOIN [dbo].[Device] D (NOLOCK) ON D.[uniqueId]=A.[uniqueId] AND D.[isDeleted]=0 
				INNER JOIN [dbo].[Fleet] F (NOLOCK) ON F.[guid]=D.[fleetGuid] AND F.[isDeleted]=0 
				INNER JOIN [dbo].[Driver] Dr (NOLOCK) ON Dr.[fleetGuid]=F.[guid] AND Dr.[isDeleted]=0 
				INNER JOIN [dbo].[Trip] T (NOLOCK) ON T.[fleetGuid]=F.[guid] AND T.[isDeleted]=0 
				WHERE A.[localName] = 'can_engine_rpm'  AND T.[isStarted]=1 AND A.[createdDate]>=T.[actualStartDateTime] 			 
					AND (CONVERT(DATE,A.[createdDate]) BETWEEN CONVERT(DATE,@lastExecDate) AND CONVERT(DATE,@dt))
			
			-- Incident 
			--DECLARE @harshBraking INT =0 , @aggressiveAcceleration INT=0,@overSpeed INT =0
			;WITH CTR_HB AS (
			select  D.[guid],SUM(A.[sum]) as [sum]
			FROM dbo.[Driver] D (NOLOCK)
			INNER JOIN (SELECT curspeed.[guid],curspeed.[tripGuid],SUM(CASE WHEN curspeed.[attributeValue]< (prvspeed.[attributeValue]*0.50) THEN 1 ELSE 0 END) as [sum]
			FROM #CTE_Attribute_CurrentSpeed curspeed 
			LEFT OUTER JOIN #CTE_Attribute_CurrentSpeed prvspeed ON (curspeed.[guid]=prvspeed.[guid] AND curspeed.[tripGuid]=prvspeed.[tripGuid] AND curspeed.[no] = prvspeed.[no] + 1) 
			 GROUP BY curspeed.[guid],curspeed.[tripGuid]) A ON D.[guid]=A.[guid] 
			   GROUP BY D.[guid])
			UPDATE D 
			SET D.[harshBraking] = HB.[sum] 
			FROM dbo.[Driver] D (NOLOCK) 
			INNER JOIN CTR_HB HB ON D.[guid]=HB.[guid] 

			;WITH CTR_AA AS (
			 select  D.[guid],SUM(A.[sum]) as [sum]
			FROM dbo.[Driver] D (NOLOCK) 
			INNER JOIN (SELECT curspeed.[guid],curspeed.[tripGuid],SUM(CASE WHEN curspeed.[attributeValue]>= (prvspeed.[attributeValue]*0.50) THEN 1 ELSE 0 END) as [sum]
			FROM #CTE_Attribute_CurrentSpeed curspeed 
			LEFT OUTER JOIN #CTE_Attribute_CurrentSpeed prvspeed ON (curspeed.[guid]=prvspeed.[guid] AND curspeed.[tripGuid] = prvspeed.[tripGuid] AND curspeed.[no] = prvspeed.[no] + 1) 
			 GROUP BY curspeed.[guid],curspeed.[tripGuid]) A ON D.[guid]=A.[guid] 
			  GROUP BY D.[guid])
			 UPDATE D 
			SET D.[aggressiveAcceleration] = AA.[sum]
			FROM dbo.[Driver] D (NOLOCK) 
			INNER JOIN CTR_AA AA ON D.[guid]=AA.[guid] 

			  UPDATE D 
			SET D.[overSpeed] = A.[sum]
			FROM dbo.[Driver] D (NOLOCK)
			INNER JOIN (SELECT curspeed.[guid],SUM(CASE WHEN curspeed.[attributeValue]> curspeed.[speedLimit] THEN 1 ELSE 0 END) as [sum]
			FROM #CTE_Attribute_CurrentSpeed curspeed
			 GROUP BY curspeed.[guid]) A ON D.[guid]=A.[guid] 

		
		-- Incident For Trip
			;WITH CTR_THB AS (
			select  Tr.[guid],SUM(A.[sum]) as [sum]
			FROM dbo.[Trip] Tr (NOLOCK)
			INNER JOIN (SELECT curspeed.[guid],curspeed.[tripGuid],SUM(CASE WHEN curspeed.[attributeValue]< (prvspeed.[attributeValue]*0.50) THEN 1 ELSE 0 END) as [sum]
			FROM #CTE_Attribute_CurrentSpeed curspeed 
			LEFT OUTER JOIN #CTE_Attribute_CurrentSpeed prvspeed ON (curspeed.[guid]=prvspeed.[guid] AND curspeed.[tripGuid]=prvspeed.[tripGuid] AND curspeed.[no] = prvspeed.[no] + 1) 
			 GROUP BY curspeed.[guid],curspeed.[tripGuid]) A ON Tr.[guid]=A.[tripGuid] 
			   GROUP BY Tr.[guid])
			UPDATE Tr 
			SET Tr.[harshBraking] = THB.[sum] 
			FROM dbo.[Trip] Tr (NOLOCK) 
			INNER JOIN CTR_THB THB ON Tr.[guid]=THB.[guid] 

			;WITH CTR_TAA AS (
			 select  Tr.[guid],SUM(A.[sum]) as [sum]
			FROM dbo.[Trip] Tr (NOLOCK) 
			INNER JOIN (SELECT curspeed.[tripGuid],SUM(CASE WHEN curspeed.[attributeValue]>= (prvspeed.[attributeValue]*0.50) THEN 1 ELSE 0 END) as [sum]
			FROM #CTE_Attribute_CurrentSpeed curspeed 
			LEFT OUTER JOIN #CTE_Attribute_CurrentSpeed prvspeed ON (curspeed.[tripGuid] = prvspeed.[tripGuid] AND curspeed.[no] = prvspeed.[no] + 1) 
			 GROUP BY curspeed.[tripGuid]) A ON Tr.[guid]=A.[tripGuid] 
			  GROUP BY Tr.[guid])
			 UPDATE Tr 
			SET Tr.[aggressiveAcceleration] = TAA.[sum]
			FROM dbo.[Trip] Tr (NOLOCK) 
			INNER JOIN CTR_TAA TAA ON Tr.[guid]=TAA.[guid] 

			UPDATE Tr 
			SET Tr.[overSpeed] = A.[sum]
			FROM dbo.[Trip] tr (NOLOCK)
			INNER JOIN (SELECT curspeed.[tripGuid],SUM(CASE WHEN curspeed.[attributeValue]> curspeed.[speedLimit] THEN 1 ELSE 0 END) as [sum]
			FROM #CTE_Attribute_CurrentSpeed curspeed
			 GROUP BY curspeed.[tripGuid]) A ON Tr.[guid]=A.[tripGuid] 

		-- Engine RPM
		--Halt State

				Select temp.[guid],temp.[tripGuid],temp.[uniqueId],[idleno],[startno],[localName]
					INTO #CTE_Attribute_EnginerpmHalt
				FROM 				
					(SELECT cur.[guid],cur.[tripGuid],cur.[no] as [idleno],cur.[uniqueId],cur.[localName]
							, (SELECT TOP 1 [no] FROM #CTE_Attribute_Enginerpm I  WHERE [no]>cur.[no] AND [guid]=cur.[guid] 
							AND [uniqueid]=cur.[uniqueId] AND [attributeValue]>0 group by [guid],[tripGuid],[uniqueId],[localName],[no]) as [startno] 
							from #CTE_Attribute_Enginerpm cur WHERE cur.[attributeValue]<=0 group by cur.[guid],cur.[tripGuid],cur.[uniqueId],
							cur.[localName],cur.[no]
						) temp 
				order by temp.[guid],temp.[tripGuid]

				
						DELETE FROM [dbo].[FleetHaltStatus] WHERE (CONVERT(DATE,[lastUpdatedHaltDateTime]) BETWEEN CONVERT(DATE,@lastExecDate) AND CONVERT(DATE,@dt))
						INSERT INTO [dbo].[FleetHaltStatus](
						[guid]
						,[companyGuid]
						,[fleetGuid]
						,[tripGuid]
						,[driverGuid]
						,[haltStartDateTime]
						,[haltEndDateTime]		
						,[lastUpdatedHaltDateTime]
						)
						SELECT NEWID(),M.[companyGuid],M.[fleetGuid],M.[tripGuid],M.[driverGuid],Max(M.[createdDate]) as [haltStart],_M.[createdDate] as [haltEnd],@dt 
					 from #CTE_Attribute_EnginerpmHalt N 
					INNER JOIN #CTE_Attribute_Enginerpm M ON N.[guid]=M.[guid] AND N.[tripGuid]=M.[tripGuid] AND N.[uniqueId]=M.[uniqueId] AND N.[idleno]=M.[no]
					INNER JOIN #CTE_Attribute_Enginerpm _M ON N.[guid]=_M.[guid] AND N.[tripGuid]=M.[tripGuid] AND N.[uniqueId]=_M.[uniqueId] AND N.[startno]=_M.[no]
					GROUP by M.[companyGuid],M.[fleetGuid],M.[tripGuid],M.[driverGuid],_M.[createdDate] 

					;WITH CTR_HT AS (
					 select  D.[guid],SUM(A.[min]) as [min]
					FROM dbo.[Driver] D (NOLOCK) 
					INNER JOIN (
					 SELECT cur.[driverGuid] as [guid],cur.[tripGuid],SUM(ISNULL(DATEDIFF(minute,cur.[haltEndDateTime],cur.[haltStartDateTime]),0)) As [min]
						FROM [dbo].[FleetHaltStatus] cur (NOLOCK)
						GROUP BY cur.[driverGuid],cur.[tripGuid]) A ON D.[guid]=A.[guid] 
						 GROUP BY D.[guid]
					)
				 UPDATE D
			 SET [haltTime] = AA.[min]
			 FROM dbo.[Driver] D (NOLOCK) 
			 INNER JOIN CTR_HT AA ON D.[guid]=AA.[guid] 

					--Idle State
			Select temp.[guid],temp.[tripGuid],temp.[uniqueId],[idleno],[startno],[localName]
					INTO #CTE_Attribute_EnginerpmNo
				FROM 				
					(SELECT cur.[guid],cur.[tripGuid],cur.[no] as [idleno],cur.[uniqueId],cur.[localName]
							, (SELECT TOP 1 [no] FROM #CTE_Attribute_Enginerpm I  WHERE [no]>cur.[no] AND [guid]=cur.[guid] 
							AND [uniqueid]=cur.[uniqueId] AND [attributeValue]>1000 group by [guid],[tripGuid],[uniqueId],[localName],[no]) as [startno] 
							from #CTE_Attribute_Enginerpm cur WHERE cur.[attributeValue]<=1000 AND cur.[attributeValue]>0 group by cur.[guid],cur.[tripGuid],cur.[uniqueId],
							cur.[localName],cur.[no]
						) temp 
				order by temp.[guid],temp.[tripGuid]
						DELETE FROM [dbo].[FleetIdleStatus] WHERE (CONVERT(DATE,[lastUpdatedIdleDateTime]) BETWEEN CONVERT(DATE,@lastExecDate) AND CONVERT(DATE,@dt))
						INSERT INTO [dbo].[FleetIdleStatus](
						[guid]
						,[companyGuid]
						,[fleetGuid]
						,[tripGuid]
						,[driverGuid]
						,[idleStartDateTime]
						,[idleEndDateTime]		
						,[lastUpdatedIdleDateTime]
						)
					SELECT NEWID(),M.[companyGuid],M.[fleetGuid],M.[tripGuid],M.[driverGuid],Max(M.[createdDate]),_M.[createdDate],@dt  from #CTE_Attribute_EnginerpmNo N 
					INNER JOIN #CTE_Attribute_Enginerpm M ON N.[guid]=M.[guid] AND N.[tripGuid]=M.[tripGuid] AND N.[uniqueId]=M.[uniqueId] AND N.[idleno]=M.[no]
					INNER JOIN #CTE_Attribute_Enginerpm _M ON N.[guid]=_M.[guid] AND N.[tripGuid]=M.[tripGuid] AND N.[uniqueId]=_M.[uniqueId] AND N.[startno]=_M.[no] 
					GROUP by M.[companyGuid],M.[fleetGuid],M.[tripGuid],M.[driverGuid],_M.[createdDate] 
			

			 
			 ;WITH CTR_IS AS (
					 select  D.[guid],SUM(A.[min]) as [min]
					FROM dbo.[Driver] D (NOLOCK) 
					INNER JOIN (
					SELECT cur.[driverGuid] as [guid],cur.[tripGuid],SUM(ISNULL(DATEDIFF(minute,cur.[idleEndDateTime],cur.[idleStartDateTime]),0)) As [min]
					FROM [dbo].[FleetIdleStatus] cur (NOLOCK)
					GROUP BY cur.[driverGuid],cur.[tripGuid]) A ON D.[guid]=A.[guid]
					  GROUP BY D.[guid]
					)
				UPDATE D
			 SET [idleTime] = AA.[min] 
			 FROM dbo.[Driver] D (NOLOCK) 
			 INNER JOIN CTR_IS AA ON D.[guid]=AA.[guid] 

			 --Trip Idle State
			 ;WITH CTR_TIS AS (
					 select  Tr.[guid],SUM(A.[min]) as [min]
					FROM dbo.[Trip] Tr (NOLOCK) 
					INNER JOIN (
					SELECT cur.[tripGuid],SUM(ISNULL(DATEDIFF(minute,cur.[idleEndDateTime],cur.[idleStartDateTime]),0)) As [min]
					FROM [dbo].[FleetIdleStatus] cur (NOLOCK)
					GROUP BY cur.[tripGuid]) A ON Tr.[guid]=A.[tripGuid]
					  GROUP BY Tr.[guid]
					)
				UPDATE Tr
			 SET [idleTime] = TIS.[min] 
			 FROM dbo.[Trip] Tr (NOLOCK) 
			 INNER JOIN CTR_TIS TIS ON Tr.[guid]=TIS.[guid]
			 
			-- Fuel Utilization
		
		--DELETE FROM [dbo].[TelemetrySummary_Hourwise] WHERE (CONVERT(DATE,[date]) BETWEEN CONVERT(DATE,@lastExecDate) AND CONVERT(DATE,@dt))  
		INSERT INTO [dbo].[TelemetrySummary_Hourwise]([guid]
		,[deviceGuid]
		,[tripGuid]
		,[date]
		,[attribute]
		,[min]
		,[max]
		,[avg]
		,[latest]
		,[sum]
		)
		
		SELECT NEWID(), [guid], [tripGuid],DATEADD(HOUR,[HOUR],CAST([Date] AS smalldatetime)), [localName], 0, 0, 0, 0, ValueCount
		FROM (
		
		select D.[guid],T.[guid] AS [tripGuid],KA.[code] AS [localName], CONVERT(DATE,A.createdDate) [DATE], DATEPART(HOUR,A.createdDate) [Hour], SUM(CONVERT(DECimal(18, 2),attributeValue)) ValueCount
		FROM [IOTConnect].[AttributeValue] A (NOLOCK)
		INNER JOIN [dbo].[Device] D (NOLOCK) ON A.[uniqueId] = D.[uniqueId] AND D.[isDeleted] = 0
		INNER JOIN [dbo].[KitTypeAttribute] KA (NOLOCK) ON A.[localName] = KA.[localName] --AND D.[tag] = KA.[tag]
		INNER JOIN [dbo].[Fleet] F (NOLOCK) ON F.[Guid]=D.[fleetGuid] AND F.[isDeleted] = 0
		INNER JOIN [dbo].[Trip] T (NOLOCK) ON D.[fleetGuid]=F.[guid] AND T.[isDeleted] = 0
		WHERE ISNULL(T.[isStarted],0)=1 AND (CONVERT(DATE,A.[createdDate]) BETWEEN CONVERT(DATE,T.[actualStartDateTime]) AND CONVERT(DATE,ISNULL(T.[completedDate], @dt))) AND KA.[code] IN ('can_currentin')
		GROUP BY D.[guid],T.[guid],KA.[code], CONVERT(DATE,A.createdDate), DATEPART(HOUR,A.[createdDate])
		) A


	COMMIT TRAN	

	END TRY	
	BEGIN CATCH
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

