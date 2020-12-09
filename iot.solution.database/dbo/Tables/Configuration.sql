CREATE TABLE [dbo].[Configuration] (
    [guid]        UNIQUEIDENTIFIER NOT NULL,
    [configKey]   NVARCHAR (100)   NOT NULL,
    [value]       NVARCHAR (500)   NULL,
    [isDeleted]   BIT              DEFAULT ((0)) NOT NULL,
    [createdDate] DATETIME         NULL,
    [createdBy]   UNIQUEIDENTIFIER NULL,
    [updatedDate] DATETIME         NULL,
    [updatedBy]   UNIQUEIDENTIFIER NULL,
    PRIMARY KEY CLUSTERED ([guid] ASC)
);

