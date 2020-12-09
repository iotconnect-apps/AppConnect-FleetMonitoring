CREATE TABLE [dbo].[DeviceMaintenance]
(	[guid]         UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [companyGuid]  UNIQUEIDENTIFIER NOT NULL,
    [entityGuid]   UNIQUEIDENTIFIER NOT NULL,
    [deviceGuid]   UNIQUEIDENTIFIER NOT NULL,
    [description]  NVARCHAR (1000)  NULL,
    [createdDate]	DATETIME        NULL,
    [startDateTime]	DATETIME 		NULL,
	[endDateTime]	DATETIME 		NULL,
    [isDeleted] BIT Default (0) NOT NULL,
    [isCompleted] BIT Default (0) NOT NULL,
    [completedDate] DATETIME NULL
)

