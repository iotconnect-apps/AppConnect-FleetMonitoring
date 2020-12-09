CREATE TABLE [dbo].[DeviceType] (
    [guid]        UNIQUEIDENTIFIER NOT NULL,
	[companyGuid] UNIQUEIDENTIFIER NOT NULL,
    [name]        NVARCHAR (200)   NOT NULL,
	[description] NVARCHAR (500)   NULL,
	[make]		  NVARCHAR (200)   NOT NULL,
	[model]		  NVARCHAR (200)   NOT NULL,
	[manufacturer]  NVARCHAR (200) NOT NULL,
	[entityGuid]	UNIQUEIDENTIFIER NOT NULL,
	[templateGuid]  UNIQUEIDENTIFIER NOT NULL,
    [isActive]    BIT              NOT NULL,
    [isDeleted]   BIT              NOT NULL,
    [createdDate] DATETIME         NULL,
    [createdBy]   UNIQUEIDENTIFIER NULL,
    [updatedDate] DATETIME         NULL,
    [updatedBy]   UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_Device_Guid] PRIMARY KEY CLUSTERED ([guid] ASC)
);

