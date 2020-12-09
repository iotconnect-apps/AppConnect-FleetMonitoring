CREATE TABLE [dbo].[KitType] (
    [guid]        UNIQUEIDENTIFIER NOT NULL,
    [companyGuid] UNIQUEIDENTIFIER NULL,
    [name]        NVARCHAR (200)   NOT NULL,
    [code]        NVARCHAR (50)    NULL,
    [tag]         NVARCHAR (50)    NULL,
    [isActive]    BIT              CONSTRAINT [DF__KitType__isActiv__6E565CE8] DEFAULT ((1)) NOT NULL,
    [isDeleted]   BIT              CONSTRAINT [DF__KitType__isDelet__6F4A8121] DEFAULT ((0)) NOT NULL,
    [createdDate] DATETIME         NULL,
    [createdBy]   UNIQUEIDENTIFIER NULL,
    [updatedDate] DATETIME         NULL,
    [updatedBy]   UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK__KitType__497F6CB4F9806AB2] PRIMARY KEY CLUSTERED ([guid] ASC)
);

