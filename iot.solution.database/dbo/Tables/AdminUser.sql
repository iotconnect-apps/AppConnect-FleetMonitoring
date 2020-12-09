CREATE TABLE [dbo].[AdminUser] (
    [guid]        UNIQUEIDENTIFIER NOT NULL,
    [email]       NVARCHAR (100)   NOT NULL,
    [companyGuid] UNIQUEIDENTIFIER NOT NULL,
    [firstName]   NVARCHAR (50)    NOT NULL,
    [lastName]    NVARCHAR (50)    NOT NULL,
    [password]    NVARCHAR (500)   NOT NULL,
    [contactNo]   NVARCHAR (25)    NULL,
    [isActive]    BIT              NOT NULL,
    [isDeleted]   BIT              NOT NULL,
    [createdDate] DATETIME         NULL,
    [createdBy]   UNIQUEIDENTIFIER NULL,
    [updatedDate] DATETIME         NULL,
    [updatedBy]   UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_AdminUser] PRIMARY KEY CLUSTERED ([guid] ASC)
);

