CREATE TABLE [dbo].[Role] (
    [guid]        UNIQUEIDENTIFIER NOT NULL,
    [companyGuid] UNIQUEIDENTIFIER NOT NULL,
    [name]        NVARCHAR (100)   NOT NULL,
    [description] NVARCHAR (500)   NULL,
    [isAdminRole] BIT              CONSTRAINT [DF__Role__isAdminRol__44FF419A] DEFAULT ((0)) NOT NULL,
    [isActive]    BIT              CONSTRAINT [DF__Role__isactive__45F365D3] DEFAULT ((1)) NOT NULL,
    [isDeleted]   BIT              CONSTRAINT [DF__Role__isdeleted__46E78A0C] DEFAULT ((0)) NOT NULL,
    [createdDate] DATETIME         NULL,
    [createdBy]   UNIQUEIDENTIFIER NULL,
    [updatedDate] DATETIME         NULL,
    [updatedBy]   UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK__Role__497F6CB433C166E2] PRIMARY KEY CLUSTERED ([guid] ASC)
);

