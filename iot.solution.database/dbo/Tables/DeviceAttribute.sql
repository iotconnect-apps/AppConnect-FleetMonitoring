CREATE TABLE [dbo].[DeviceAttribute]
(
	[guid]				UNIQUEIDENTIFIER NOT NULL PRIMARY KEY
	, [companyGuid]		UNIQUEIDENTIFIER NOT NULL 
	, [deviceGuid]		UNIQUEIDENTIFIER NOT NULL
	, [attrGuid]		UNIQUEIDENTIFIER NOT NULL
	, [attrName]		NVARCHAR(100) NOT NULL
	, [displayName]		NVARCHAR(100) NULL
	,[isActive]         BIT              DEFAULT ((1)) NOT NULL
    ,[isDeleted]        BIT              DEFAULT ((0)) NOT NULL
    ,[createdDate]      DATETIME         NULL
    ,[createdBy]        UNIQUEIDENTIFIER NULL
    ,[updatedDate]      DATETIME         NULL
    ,[updatedBy]        UNIQUEIDENTIFIER NULL
)
