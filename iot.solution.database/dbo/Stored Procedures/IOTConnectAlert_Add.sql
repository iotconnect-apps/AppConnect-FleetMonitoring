CREATE PROCEDURE [dbo].[IOTConnectAlert_Add]
(	
	@data XML
)
AS
BEGIN
    SET NOCOUNT ON
	
	INSERT INTO IOTConnectAlert
	SELECT DISTINCT NEWID() AS [guid]
	, CASE WHEN x.R.query('ruleName').value('.', 'NVARCHAR(50)')='HighSpeed' 
		THEN 'High Speed detected' 
		ELSE CASE WHEN x.R.query('ruleName').value('.', 'NVARCHAR(50)')='TyrePressure'
		THEN 'Tyre Pressure is low'
		ELSE CASE WHEN x.R.query('ruleName').value('.', 'NVARCHAR(50)')='FuelLevel'
		THEN 'Low Fuel Level'
		ELSE 
			x.R.query('message').value('.', 'NVARCHAR(500)') 
		END 
		END
		END AS 'message'
	, x.R.query('companyGuid').value('.', 'UNIQUEIDENTIFIER') AS 'companyGuid'
	, x.R.query('condition').value('.', 'NVARCHAR(1000)') AS 'condition'
	, x.R.query('deviceGuid').value('.', 'UNIQUEIDENTIFIER') AS 'deviceGuid'
	, x.R.query('entityGuid').value('.', 'UNIQUEIDENTIFIER') AS 'entityGuid'
	, x.R.query('eventDate').value('.', 'DATETIME') AS 'eventDate'
	, x.R.query('uniqueId').value('.', 'NVARCHAR(50)') AS 'uniqueId'
	, x.R.query('audience').value('.', 'NVARCHAR(2000)') AS 'audience'
	, x.R.query('eventId').value('.', 'NVARCHAR(50)') AS 'eventId'
	, x.R.query('refGuid').value('.', 'UNIQUEIDENTIFIER') AS 'refGuid'
	, x.R.query('severity').value('.', 'NVARCHAR(50)') AS 'severity'
	, x.R.query('ruleName').value('.', 'NVARCHAR(50)') AS 'ruleName'
	, x.R.query('data').value('.', 'NVARCHAR(2000)') AS 'data'
	FROM @data.nodes('/IOTAlertMessage') as x(R)

	IF OBJECT_ID ('tempdb..#AlertDevices') IS NOT NULL DROP TABLE #AlertDevices
	SELECT x.R.query('deviceGuid').value('.', 'UNIQUEIDENTIFIER') as [deviceGuid]
	INTO #AlertDevices 
	FROM @data.nodes('/IOTAlertMessage') as x(R)

	Update I SET [message] = ISNULL(F.[fleetId],'') + ' : '+ [message] 
	--SELECT [message],F.[fleetId] 
	FROM IOTConnectAlert I 
	INNER JOIN #AlertDevices X ON I.[deviceGuid]= X.[deviceGuid]
	INNER JOIN [dbo].[Device] D (NOLOCK) ON I.[deviceGuid]=D.[guid]
	 INNER JOIN [dbo].[Fleet] F (NOLOCK) ON D.[fleetGuid]=F.[guid]
	 WHERE [message] NOT LIKE '%'+ISNULL(F.[fleetId],'')+'%'

END