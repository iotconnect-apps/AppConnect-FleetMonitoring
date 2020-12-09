CREATE TABLE [IOTConnect].[AttributeValue] (
    [guid]               UNIQUEIDENTIFIER NOT NULL,
    [companyGuid]        UNIQUEIDENTIFIER NULL,
    [localName]          NVARCHAR (100)   NULL,
    [uniqueId]           NVARCHAR (200)   NULL,
    [tag]                NVARCHAR (200)   NULL,
    [attributeValue]     NVARCHAR (1000)  NULL,
    [createdDate]        DATETIME         NULL,
    [sdkUpdatedDate]     DATETIME         NULL,
    [gatewayUpdatedDate] DATETIME         NULL,
    [deviceUpdatedDate]  DATETIME         NULL,
    [aggregateType]      INT              DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([guid] ASC)
);

