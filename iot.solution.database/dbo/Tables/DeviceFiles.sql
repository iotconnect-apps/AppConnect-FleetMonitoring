CREATE TABLE [dbo].[DeviceFiles] (
    [guid]        UNIQUEIDENTIFIER NOT NULL,
    [deviceGuid]  UNIQUEIDENTIFIER NOT NULL,
    [filePath]    NVARCHAR (500)   NOT NULL,
    [description] NVARCHAR (200)   NULL,
    [isDeleted]   BIT              NOT NULL,
    [createdDate] DATETIME         NOT NULL,
    [createdBy]   UNIQUEIDENTIFIER NOT NULL,
    [updatedDate] DATETIME         NULL,
    [updatedBy]   UNIQUEIDENTIFIER NULL,
    [type] NCHAR(1) NULL, 
    CONSTRAINT [PK__DeviceFiles__GUID] PRIMARY KEY CLUSTERED ([guid] ASC)
);

