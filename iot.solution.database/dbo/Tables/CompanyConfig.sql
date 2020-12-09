CREATE TABLE [dbo].[CompanyConfig] (
    [guid]              UNIQUEIDENTIFIER NOT NULL,
    [companyGuid]       UNIQUEIDENTIFIER NOT NULL,
    [configurationGuid] UNIQUEIDENTIFIER NOT NULL,
    [value]             NVARCHAR (MAX)   NULL,
    [isDeleted]         BIT              DEFAULT ((0)) NOT NULL,
    [createdDate]       DATETIME         NULL,
    [createdBy]         UNIQUEIDENTIFIER NULL,
    [updatedDate]       DATETIME         NULL,
    [updatedBy]         UNIQUEIDENTIFIER NULL,
    PRIMARY KEY CLUSTERED ([guid] ASC)
);

