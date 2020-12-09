CREATE TABLE [dbo].[DeviceAttributeUtilization]
(
	[guid] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY
	, [companyGuid]	UNIQUEIDENTIFIER NOT NULL
	, [deviceGuid]	UNIQUEIDENTIFIER NOT NULL
	, [attrbGuid] UNIQUEIDENTIFIER NOT NULL
	, [totalCount]	BIGINT NOT NULL
	, [utilizedCount] BIGINT NOT NULL
	, [createdDate] DATE NOT NULL
)
