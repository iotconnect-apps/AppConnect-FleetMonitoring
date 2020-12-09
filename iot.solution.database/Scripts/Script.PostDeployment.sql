DECLARE @dt DATETIME = GETUTCDATE()
IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.[configuration] WHERE [configKey] = 'db-version')
BEGIN
	INSERT [dbo].[Configuration] ([guid], [configKey], [value], [isDeleted], [createdDate], [createdBy], [updatedDate], [updatedBy]) VALUES (N'cf45da4c-1b49-49f5-a5c3-8bc29c1999ea', N'db-version', N'1', 0, CAST(N'2020-04-08T13:16:53.940' AS DateTime), NULL, CAST(N'2020-04-08T13:16:53.940' AS DateTime), NULL)
END

IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.[configuration] WHERE [configKey] = 'telemetry-last-exectime')
BEGIN
	INSERT [dbo].[Configuration] ([guid], [configKey], [value], [isDeleted], [createdDate], [createdBy], [updatedDate], [updatedBy]) VALUES (N'465970b2-8bc3-435f-af97-8ca26f2bf383', N'telemetry-last-exectime', N'2020-04-25 12:08:02.380', 0, CAST(N'2020-04-25T06:41:01.030' AS DateTime), NULL, CAST(N'2020-04-25T06:41:01.030' AS DateTime), NULL)
END


DECLARE @DBVersion FLOAT  = 0
SELECT @DBVersion = CONVERT(FLOAT,[value]) FROM dbo.[configuration] WHERE [configKey] = 'db-version'

IF @DBVersion < 1 
BEGIN
INSERT INTO [dbo].[AdminUser] ([guid],[email],[companyGuid],[firstName],[lastName],[password],[isActive],[isDeleted],[createdDate]) VALUES (NEWID(),'admin@fleetmonitoring.com','AB469212-2488-49AD-BC94-B3A3F45590D2','Fleet Monitoring','admin','Softweb#123',1,0,GETUTCDATE())


INSERT INTO [dbo].[KitType]
           ([guid]
           ,[companyGuid]
           ,[name]
           ,[code]
           ,[tag]
           ,[isActive]
           ,[isDeleted]
           ,[createdDate]
           ,[createdBy]
           ,[updatedDate]
           ,[updatedBy])
     VALUES
           ('309B8B60-D31E-4712-A4CB-DE78D2DED948'
           ,null
           ,'FMDefault'
           ,'FMDefault'
           ,''
           ,1
           ,0
           ,GETUTCDATE()
           ,NEWID()
           ,GETUTCDATE()
           ,NEWID())

  INSERT [dbo].[KitTypeAttribute] ([guid], [parentTemplateAttributeGuid], [templateGuid], [localName], [code], [tag], [description])
   VALUES (newid(), NULL, N'309B8B60-D31E-4712-A4CB-DE78D2DED948', N'can_vehicle_speed', N'can_vehicle_speed', NULL, N'can_vehicle_speed'),
   (newid(), NULL, N'309B8B60-D31E-4712-A4CB-DE78D2DED948', N'gps_lat', N'gps_lat', NULL, N'gps_lat'),
   (newid(), NULL, N'309B8B60-D31E-4712-A4CB-DE78D2DED948', N'gps_lng', N'gps_lng', NULL, N'gps_lng'),
   (newid(), NULL, N'309B8B60-D31E-4712-A4CB-DE78D2DED948', N'can_currentin', N'can_currentin', NULL, N'can_currentin'),
   (newid(), NULL, N'309B8B60-D31E-4712-A4CB-DE78D2DED948', N'can_fuel_level', N'can_fuel_level', NULL, N'can_fuel_level'),
   (newid(), NULL, N'309B8B60-D31E-4712-A4CB-DE78D2DED948', N'can_tyrepressure', N'can_tyrepressure', NULL, N'can_tyrepressure'),
   (newid(), NULL, N'309B8B60-D31E-4712-A4CB-DE78D2DED948', N'can_enginetemp', N'can_enginetemp', NULL, N'can_enginetemp'),
    (newid(), NULL, N'309B8B60-D31E-4712-A4CB-DE78D2DED948', N'can_engine_rpm', N'can_engine_rpm', NULL, N'can_engine_rpm'),
    (newid(), NULL, N'309B8B60-D31E-4712-A4CB-DE78D2DED948', N'gps_time', N'gps_time', NULL, N'gps_time'),
    (newid(), NULL, N'309B8B60-D31E-4712-A4CB-DE78D2DED948', N'gps_altitude', N'gps_altitude', NULL, N'gps_altitude'),
    (newid(), NULL, N'309B8B60-D31E-4712-A4CB-DE78D2DED948', N'gps_num_sats', N'gps_num_sats', NULL, N'gps_num_sats'),
    (newid(), NULL, N'309B8B60-D31E-4712-A4CB-DE78D2DED948', N'vehicle_ign_sense', N'vehicle_ign_sense', NULL, N'vehicle_ign_sense'),
    (newid(), NULL, N'309B8B60-D31E-4712-A4CB-DE78D2DED948', N'gateway_uptime', N'gateway_uptime', NULL, N'gateway_uptime'),
    (newid(), NULL, N'309B8B60-D31E-4712-A4CB-DE78D2DED948', N'can_odometer', N'can_odometer', NULL, N'can_odometer'),
    (newid(), NULL, N'309B8B60-D31E-4712-A4CB-DE78D2DED948', N'can_hours_operation', N'can_hours_operation', NULL, N'can_hours_operation'),
    (newid(), NULL, N'309B8B60-D31E-4712-A4CB-DE78D2DED948', N'can_engine_rpm_total', N'can_engine_rpm_total', NULL, N'can_engine_rpm_total'),
    (newid(), NULL, N'309B8B60-D31E-4712-A4CB-DE78D2DED948', N'can_distance_to_service', N'can_distance_to_service', NULL, N'can_distance_to_service'),
    (newid(), NULL, N'309B8B60-D31E-4712-A4CB-DE78D2DED948', N'can_diagnostic_error_mesg', N'can_diagnostic_error_mesg', NULL, N'can_diagnostic_error_mesg')


INSERT INTO [dbo].[FleetType] ([guid],[name],[description],[isActive],[isDeleted],[createdDate],[createdBy],[updatedDate],[updatedBy])
     VALUES           (newid()           ,'Truck'           ,'Truck'           ,1           ,0           ,GETUTCDATE()           ,NEWID()           ,GETUTCDATE()           ,NEWID()),
		   (newid()           ,'Car'           ,'Car'           ,1           ,0           ,GETUTCDATE()           ,NEWID()           ,GETUTCDATE()           ,NEWID()),
           (newid()           ,'AirCraft '           ,'AirCraft'           ,1           ,0           ,GETUTCDATE()           ,NEWID()           ,GETUTCDATE()           ,NEWID()),
           (newid()           ,'Vans '           ,'Vans'           ,1           ,0           ,GETUTCDATE()           ,NEWID()           ,GETUTCDATE()           ,NEWID())

INSERT INTO [dbo].[FleetMaterialType]           ([guid]           ,[name]           ,[description]           ,[isActive]           ,[isDeleted]           ,[createdDate]           ,[createdBy]           ,[updatedDate]           ,[updatedBy])
     VALUES           (NEWID()           ,'Goods'           ,'Goods'           ,1           ,0           ,GETUTCDATE()           ,NewID()           ,GETUTCDate()           ,NewID()),
	 (NEWID()           ,'Liquid'           ,'Liquid'           ,1           ,0           ,GETUTCDATE()           ,NewID()           ,GETUTCDate()           ,NewID()),
	 (NEWID()           ,'Plastic'           ,'Plastic'           ,1           ,0           ,GETUTCDATE()           ,NewID()           ,GETUTCDate()           ,NewID()),
	 (NEWID()           ,'Natural Gas'           ,'Natural Gas'           ,1           ,0           ,GETUTCDATE()           ,NewID()           ,GETUTCDate()           ,NewID()),
	  (NEWID()           ,'Coal'           ,'Coal'           ,1           ,0           ,GETUTCDATE()           ,NewID()           ,GETUTCDate()           ,NewID()),
	  (NEWID()           ,'Other'           ,'Other'           ,1           ,0           ,GETUTCDATE()           ,NewID()           ,GETUTCDate()           ,NewID())


UPDATE [dbo].[Configuration]
SET [value]  = '1'
WHERE [configKey] = 'db-version'

END
